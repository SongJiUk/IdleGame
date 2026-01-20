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
        CriticalRate = Managers.PlayerM.CriticalChance();
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
        Debug.Log($"<color=cyan>[Projectile Step 1] 이벤트 호출됨 - {gameObject.name}</color>");
        isAttacking = true;

        if (currentTarget == null || currentTarget.IsDead)
        {
            FindClosetTarget(Managers.ObjectM.mcList);
            currentTarget = target;
        }
       
        if ( currentTarget == null)
        {
            Debug.LogWarning($"[Projectile Step 1-Error] 주변에 타겟이 없음");
            StopAttack();
            return;
        }
        if (DATA == null) { Debug.LogError("DATA가 Null입니다!"); return; }

        if (DATA.ProjectileDataID == 0)
        {
            Debug.LogError($"{gameObject.name}의 Range ProjectileDataID가 0입니다!");
        }
        Debug.Log($"[Projectile Step 2] 발사 시도 - ID: {DATA.ProjectileDataID}, 타겟: {currentTarget.name}");
        transform.LookAt(currentTarget.transform);
        var go = Managers.ObjectM.Spawn<RangeAttackController>(transform.position, DATA.ProjectileDataID, this, currentTarget);

        if (go != null)
        {
            Debug.Log("<color=white>[Projectile Step 3-Success] 원거리 투사체 생성 완료!</color>");
            GetMp(30);
        }

    }

    public override void Attack()
    {
        // 지점 1: 이벤트 호출 확인
        Debug.Log($"[Step 1] Attack 이벤트 발생 - {gameObject.name}");
        isAttacking = true;

        if (currentTarget == null || currentTarget.IsDead)
        {
            FindClosetTarget(Managers.ObjectM.mcList);
            currentTarget = target;
        }

        if (currentTarget == null)
        {
            Debug.LogWarning("[Step 1-Error] 타겟 찾기 실패");
            StopAttack();
            return;
        }

        // 지점 2: 데이터 확인
        if (DATA == null) { Debug.LogError("DATA가 Null입니다!"); return; }

        // 지점 3: 스폰 직전
        Debug.Log($"[Step 2] 스폰 시도 - ID: {DATA.ProjectileDataID}, Target: {currentTarget.name}");


        Debug.Log("<color=yellow>[Step 3-Success] 공격 오브젝트 생성 완료!</color>");

        if (trails != null)
        {
            foreach (var trail in trails) trail.SetActive(true);
        }
        var go = Managers.ObjectM.Spawn<MeleeAttackController>(transform.position, DATA.ProjectileDataID, this, currentTarget);

        if (go != null)
        {
            DelegateHolder.PlayerAttack(this, currentTarget);
            GetMp(5);
        }

        TrailDisable().Forget();
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

    void OnReady()
    {
        base.AnimatorChange(CreatureState.Idle);
        target = null;
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

    void OnDungeonClear()
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
        Managers.StageM.readyEvent += OnReady;
        Managers.StageM.playEvent += OnPlay;
        Managers.StageM.bossEvent += OnBoss;
        Managers.StageM.clearEvent += OnClear;
        Managers.StageM.deadEvent += OnDead;
        Managers.StageM.dungeonEvent += OnDungeon;
        Managers.StageM.dungeonClearEvent += OnDungeonClear;
    }

    void UnConnectEvent()
    {
        Managers.StageM.readyEvent -= OnReady;
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
        isAttacking = false;
        if (skillController.UseSkill(_target: target))
        {
            mp = 0;
            isUsingSkill = true;

            HandleSkillEnd().Forget();
            OnPlayerDataUpdate?.Invoke(this);
        }
        else
        {
            isUsingSkill = false;
            target = null;
            AnimatorChange(CreatureState.Idle);
        }
    }

    private async UniTaskVoid HandleSkillEnd()
    {
        float duration = GetCurrentPlayingClipDuration(animator);
        if (duration <= 0) duration = 2f; 

        await UniTask.Delay(TimeSpan.FromSeconds(duration));

        isUsingSkill = false;
        isAttacking = false;

        AnimatorChange(CreatureState.Idle);

        Debug.Log($"[Skill] {gameObject.name} 스킬 종료 및 상태 복구 완료");
    }

    public void SkillEnd()
    {
        isUsingSkill = false;
    }
    public override void GetDamage(double _dmg, CreatureController _attacker, bool _isCritical = false, bool _isSkill = false)
    {
        if (isDead) return;
        if (Managers.StageM.isDead) return;
        if (isUsingSkill) return;

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
        if (Managers.CharacterM.AlivePlayers.Count <= 0)
        {
            if (Managers.StageM.isDungeon) Managers.StageM.StateChange(StageState.DungeonFail);
            else Managers.StageM.StateChange(StageState.Dead);
        }
    }
}
