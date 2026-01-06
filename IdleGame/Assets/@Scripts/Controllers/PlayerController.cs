using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Data.Common;
using static Define;


public class PlayerController : CreatureController
{
    [SerializeField]
    List<GameObject> trails;
    public ParticleSystem provocation;

    #region Action
    public Action<PlayerController> OnPlayerDataUpdate;

    #endregion

    int killCount;
    public int KillCount
    {
        get => killCount;
        set
        {
            killCount = value;
        }
    }

    Vector3 startPos = Vector3.zero;
    string ownerName;
    public int Level;
    public int index;

    int mp;
    public int MP
    {
        get => mp;
        set
        {
            mp = value;
        }
    }

    public int maxMp;
    public int MaxMp
    {
        get => maxMp;
    }




    void OnEnable()
    {
        Managers.UpdateM.Register(this);
        ConnectEvent();
    }
    protected override void OnDisable()
    {
        Managers.UpdateM.UnRegister(this);
        UnConnectEvent();

        base.OnDisable();
    }

    public override bool Init()
    {
        if (!base.Init()) return false;
        return true;
    }
    public void SetInfo(Data.CreatureData _data)
    {
        DATA = _data;
        baseHp = _data.BaseHp;
        baseDamage = _data.BaseDamage;

        base.SetSkill();


        isPlayer = true;
        isDead = false;
        isAttacking = false;
        SpawnPos = transform.position;
        //TODO : 지우기
        SetStat();
        mp = 0;
        maxMp = DATA.MaxMp;
        attackrange = DATA.AttackRange;
        detectrange = 5f;
        ownerName = this.name;
        CriticalRate = 0.5f;
        target = null;
    }
    public void SetStat()
    {
        hp = Managers.PlayerM.GetHP(DATA.CharacterGrade, Managers.GameM.gameData.Characters_Data[DATA.Name]);
        maxHp = hp;
        damage = Managers.PlayerM.GetAttack(DATA.CharacterGrade, Managers.GameM.gameData.Characters_Data[DATA.Name]);

        if (animator != null)
        {
            animator.speed = 1.0f;
            animator.SetFloat("AttackSpeed", DATA.AttackSpeed);
        }
    }
    public override void InitStat()
    {
        damage = Utils.Datas.levelData.Damage();
        hp = Utils.Datas.levelData.HP();
    }

    public override void OnDamage()
    {

    }

    public override void Projectile()
    {
        if (!isAttacking || currentTarget == null || currentTarget.IsDead)
        {
            StopAttack();
            return;
        }
        Managers.ObjectM.Spawn<RangeAttackController>(transform.position, DATA.ProjectileDataID, this, target);
        GetMp(30);

    }

    public override void Attack()
    {
        if (!isAttacking) return;

        if (currentTarget == null || currentTarget.IsDead)
        {
            StopAttack();
            return;
        }

        MonsterController monsterTarget = target as MonsterController;
        if (monsterTarget == null) return;



        if (trails != null)
        {
            foreach (var trail in trails) trail.SetActive(true);
        }

        Managers.ObjectM.Spawn<MeleeAttackController>(transform.position, DATA.ProjectileDataID, this, monsterTarget);
        DelegateHolder.PlayerAttack(this, monsterTarget);
        TrailDisable().Forget();

        GetMp(5);
    }

    private void StopAttack()
    {
        isAttacking = false;
        AnimatorChange(CreatureState.Idle); // 타겟이 없으니 대기 상태로
    }


    public async UniTaskVoid TrailDisable()
    {
        await UniTask.WaitForSeconds(0.3f);
        if (trails != null)
        {
            for (int i = 0; i < trails.Count; i++)
            {
                trails[i].SetActive(false);
            }
        }
    }

    private void OnPlay(Define.StageState _state)
    {
        base.AnimatorChange(CreatureState.Idle);
        OnPlayerDataUpdate?.Invoke(this);
    }
    private void OnBoss()
    {
        if (isDead) return;

        base.AnimatorChange(Define.CreatureState.Idle);
        target = null;
        if (provocation != null) provocation.Play();
    }
    private void OnDead()
    {
        target = null;
    }

    private void OnClear()
    {
        if (isDead) return;
        //TODO : 환호 애니메이션으로 변경하기
        AnimatorChange(CreatureState.Idle);
    }

    void OnDungeon(int _value)
    {
        AnimatorChange(CreatureState.Idle);
        isDead = false;
        target = null;
        transform.position = startPos;

    }

    void OnDungeonClear(int _value)
    {
        OnDead();
    }

    public async UniTask KnockBack(float _power, float _durtaion)
    {
        float t = _durtaion;
        Vector3 force = transform.forward * -_power;
        force.y = 0f;

        while (t > 0f)
        {
            t -= Time.deltaTime;
            transform.position += force * Time.deltaTime;
            await UniTask.Yield();
        }
    }


    public override void Tick(float _deltaTime)
    {
        if (Managers.StageM.stageState != StageState.Play && Managers.StageM.stageState != StageState.BossPlay) return;

        if (isUsingSkill || isDead || isAttacking) return;

        //if (searchDelayTimer > 0)
        //{
        //    searchDelayTimer -= _deltaTime;
        //    base.AnimatorChange(CreatureState.Idle);
        //    return;
        //}

        if (target == null || target.IsDead)
        {
            FindClosetTarget(Managers.ObjectM.mcList);
            if (target == null)
            {
                GoBackToSpawn(_deltaTime);
                return;
            }
        }

        float targetDist = Vector3.Distance(transform.position, target.transform.position);

        if (targetDist > detectrange)
        {
            //ResetTarget();
            GoBackToSpawn(_deltaTime);
            return;
        }

        if (targetDist > attackrange)
        {
            if (!isAttacking)
                MoveToTarget(_deltaTime);
        }
        else
        {
            if (!isAttacking)
            {
                StartAttack().Forget();
            }

        }
    }

    void ConnectEvent()
    {
        Managers.StageM.playEvent += OnPlay;
        Managers.StageM.bossEvent += OnBoss;
        Managers.StageM.clearEvent += OnClear;
        Managers.StageM.deadEvent += OnDead;
        Managers.StageM.dungeonEvent += OnDungeon;
        Managers.StageM.dungeonClearEvent += OnDungeonClear;
    }

    void UnConnectEvent()
    {
        Managers.StageM.playEvent -= OnPlay;
        Managers.StageM.bossEvent -= OnBoss;
        Managers.StageM.clearEvent -= OnClear;
        Managers.StageM.deadEvent -= OnDead;
        Managers.StageM.dungeonEvent -= OnDungeon;
        Managers.StageM.dungeonClearEvent -= OnDungeonClear;
    }
    protected override void OnAttackDelayEnd()
    {
        //TODO : 스킬쓸때 무적으로 만들까?
        if (mp >= MaxMp)
        {
            UsePlayerSkill();
        }
    }
    public void GetMp(int _value)
    {
        //TODO : bool값 체크
        //if (isUsingSkill) return;

        mp += _value;
        if (mp >= MaxMp)
        {
            mp = MaxMp;
        }
        OnPlayerDataUpdate?.Invoke(this);
    }

    void UsePlayerSkill()
    {
        if (skillController.UseSkill(_target: target))
        {
            mp = 0;
            isUsingSkill = true;
            isAttacking = false;

            OnPlayerDataUpdate?.Invoke(this);
        }
        else
        {
            isUsingSkill = false;
            target = null;
            AnimatorChange(CreatureState.Idle);
        }
    }

    public void SkillEnd()
    {
        isUsingSkill = false;
    }
    public override void GetDamage(double _dmg, CreatureController _attacker, bool _isCritical = false, bool _isSkill = false)
    {
        if (isDead) return;
        if (Managers.StageM.isDead) return;
        base.GetDamage(_dmg, _attacker, _attacker.GetCritical());
        DelegateHolder.PlayerHit(this);

        if (hp <= 0)
        {
            hp = 0;
            Dead();
        }
        OnPlayerDataUpdate?.Invoke(this);
    }

    public override void Dead()
    {
        base.Dead();
        isUsingSkill = false;
        AnimatorChange(CreatureState.Dead);
        Managers.SpawnM.players.Remove(this);
        if (Managers.SpawnM.players.Count <= 0)
        {
            if (Managers.StageM.isDungeon) Managers.StageM.StateChange(StageState.DungeonFail);
            else Managers.StageM.StateChange(StageState.Dead);
        }
    }
}
