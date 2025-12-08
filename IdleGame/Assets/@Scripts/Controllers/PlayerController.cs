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
        isAttack = false;
        SpawnPos = transform.position;
        hp = Utils.Datas.levelData.HP((float)baseHp);
        maxHp = hp;
        damage = Utils.Datas.levelData.Damage((float)baseDamage);
        mp = 0;
        maxMp = DATA.MaxMp;
        attackrange = DATA.AttackRange;
        detectrange = 5f;

        ownerName = this.name;
        CriticalRate = 0.5f;
        target = null;
    }

    public override void InitStat()
    {
        damage = Utils.Datas.levelData.Damage((float)baseDamage);
        hp = Utils.Datas.levelData.HP((float)BaseHp);
    }

    public override void OnDamage()
    {

    }

    public override void Projectile()
    {
        if (target == null || target.IsDead) return;
        Managers.ObjectM.Spawn<RangeAttackController>(transform.position, DATA.ProjectileDataID, this, target);
        GetMp(30);

    }

    public override void Attack()
    {
        if (!isAttack) return;
        if (target == null || target.IsDead) return;

        if (trails != null)
        {
            for (int i = 0; i < trails.Count; i++)
            {
                trails[i].SetActive(true);
            }

        }

        Managers.ObjectM.Spawn<MeleeAttackController>(transform.position, DATA.ProjectileDataID, this, target);
        TrailDisable().Forget();
        GetMp(30);
    }

    public async UniTaskVoid TrailDisable()
    {
        await UniTask.WaitForSeconds(1f);
        if (trails != null)
        {
            for (int i = 0; i < trails.Count; i++)
            {
                trails[i].SetActive(false);
            }
        }
    }

    private void OnPlay()
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
        AnimatorChange(CreatureState.Idle);
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

        if (isUsingSkill) return;
        if (isDead) return;
        if (isAttack) return;

        if (target == null || target.IsDead)
        {
            ResetTarget();

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
            ResetTarget();
            GoBackToSpawn(_deltaTime);
            return;
        }

        if (targetDist > attackrange)
        {
            if (!isAttack)
                MoveToTarget(_deltaTime);
        }
        else
        {
            if (!isAttack)
            {
                StartAttack();
            }

        }
    }

    void ConnectEvent()
    {
        Managers.StageM.playEvent += OnPlay;
        Managers.StageM.bossEvent += OnBoss;
        Managers.StageM.clearEvent += OnClear;
        Managers.StageM.deadEvent += OnDead;
    }

    void UnConnectEvent()
    {
        Managers.StageM.playEvent -= OnPlay;
        Managers.StageM.bossEvent -= OnBoss;
        Managers.StageM.clearEvent -= OnClear;
        Managers.StageM.deadEvent -= OnDead;
    }

    public void GetMp(int _value)
    {
        //TODO : bool값 체크
        //if (isUsingSkill) return;

        mp += _value;
        if (mp >= MaxMp)
        {
            UsePlayerSkill();
        }
        OnPlayerDataUpdate?.Invoke(this);
    }

    void UsePlayerSkill()
    {
        CreatureController t = null;
        if (target != null)
        {
            t = target;
            if (skillController.UseSKill(_target: t))
            {
                mp = 0;
                isUsingSkill = true;
            }
            else isUsingSkill = false;
        }
        else
        {
            if (skillController.UseSKill())
            {
                mp = 0;
                isUsingSkill = true;
            }
            else isUsingSkill = false;
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
        GetMp(3);
        base.GetDamage(_dmg, _attacker, _attacker.GetCritical());
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
            Managers.StageM.StateChange(StageState.Dead);
        }
    }
}
