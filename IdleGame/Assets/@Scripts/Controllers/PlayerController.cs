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
    GameObject trail;
    public ParticleSystem provocation;

    #region Action
    public Action OnPlayerDataUpdate;
    #endregion

    int killCount;
    public int KillCount
    {
        get => killCount;
        set
        {
            killCount = value;
            OnPlayerDataUpdate?.Invoke();
        }
    }


    Data.CreatureData data;
    void OnEnable() => Managers.UpdateM.Register(this);
    void OnDisable() => Managers.UpdateM.UnRegister(this);

    Vector3 startPos = Vector3.zero;
    
    string ownerName;
     
    public override bool Init()
    {
        if (!base.Init()) return false;
        Managers.StageM.readyEvent += OnReady;
        Managers.StageM.bossEvent += OnBoss;
        return true;
    }
    public void SetInfo(Data.CreatureData _data)
    {        data = _data;
        isAttack = false;
        SpawnPos = transform.position;
        hp = 100000;
        attackrange = data.AttackRange;
        detectrange = 5f;
        ownerName = this.name;
        CriticalRate = 0.5f;
    }

    public override void InitStat()
    {

    }

    public override void OnDamage()
    {

    }

    public override void OnDead()
    {

    }

    public override void Projectile()
    {
        if (target == null || target.IsDead) return;
        Managers.ObjectM.Spawn<RangeAttackController>(transform.position, 20000, this, target);
   
    }

    public override void Attack()
    {
        if (target == null || target.IsDead) return;

        if (trail != null) trail.SetActive(true);
        Managers.ObjectM.Spawn<MeleeAttackController>(transform.position, 20001, this, target);
        TrailDisable().Forget();
    }

    public async UniTaskVoid TrailDisable()
    {
        await UniTask.WaitForSeconds(1f);
        if (trail != null) trail.SetActive(false);
    }
    protected override void AnimatorChange(Define.CreatureState _state)
    {
        base.AnimatorChange(_state);
    }
    
    private void OnReady()
    {
        transform.position = SpawnPos;
    }
    private void OnBoss()
    {
        base.AnimatorChange(Define.CreatureState.Idle);
        provocation.Play();
    }

    public async UniTask KnockBack(float _power, float _durtaion)
    {
        float t = _durtaion;
        Vector3 force = transform.forward * -_power;
        force.y = 0f;

        while(t > 0f)
        {
            t -= Time.deltaTime;
            transform.position += force * Time.deltaTime;
            await UniTask.Yield();
        }
    }

    public override void Tick(float _deltaTime)
    {
        if (Managers.StageM.stageState == StageState.Play || Managers.StageM.stageState == StageState.BossPlay)
        {
            if (isDead) return;

            if (!isAttack)
                FindClosetTarget(Managers.ObjectM.mcSet);

            if (target == null || target.IsDead)
            {
                ResetTarget();
                GoBackToSpawn(_deltaTime);
                return;
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

                return;
            }

            if (!isAttack)
                StartAttack();

        }



    }

    public override void GetDamage(double _dmg, CreatureController _attacker, bool _isCritical = false)
    {
        
        base.GetDamage(_dmg, _attacker, _attacker.GetCritical());
        Debug.Log($"Hit Damage : {_dmg}");
        if(hp <= 0)
        {
            hp = 0;
            isDead = true;
            Managers.ObjectM.DeSpawn(this);
        }
    }

    
}
