using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Data.Common;

public class PlayerController : CreatureController
{
    [SerializeField]
    GameObject trail;

    Data.CreatureData data;
    void OnEnable() => Managers.UpdateM.Register(this);
    void OnDisable() => Managers.UpdateM.UnRegister(this);

    Vector3 startPos = Vector3.zero;
    
    string ownerName;
     
    public override bool Init()
    {
        if (!base.Init()) return false;
        return true;
    }
    public void SetInfo(Data.CreatureData _data)
    {
        data = _data;
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
    
    public override void Tick(float _deltaTime)
    {
        if (isDead) return;

        if(!isAttack)
            FindClosetTarget(Managers.ObjectM.mcSet);

        if(target == null || target.IsDead)
        {
            ResetTarget();
            GoBackToSpawn(_deltaTime);
            return;
        }

        float targetDist = Vector3.Distance(transform.position, target.transform.position);

        if(targetDist > detectrange)
        {
            ResetTarget();
            GoBackToSpawn(_deltaTime);
            return;
        }

        if(targetDist > attackrange)
        {
            if (!isAttack)
                MoveToTarget(_deltaTime);

            return;
        }

        if (!isAttack)
            StartAttack();


        //if (!isTargetLocked)
        //{
        //    FindClosetTarget(Managers.ObjectM.mcSet);
            
        //    if(target != null)
        //    {
        //        float dist = Vector3.Distance(transform.position, target.transform.position);

        //        if (dist <= attackrange)
        //            isTargetLocked = true;
        //        else
        //            return;
        //    }
        //}
           

        //if (target == null)
        //{
        //    float targetPos = Vector3.Distance(transform.position, SpawnPos);
        //    if (targetPos > 0.1f)
        //    {
        //        transform.position = Vector3.MoveTowards(transform.position, SpawnPos, _deltaTime);
        //        transform.LookAt(SpawnPos);
        //        AnimatorChange(Define.CreatureState.Move);
        //    }
        //    else
        //    {
        //        AnimatorChange(Define.CreatureState.Idle);
        //    }
        //}
        //else
        //{
        //    if (target.IsDead)
        //    {
        //        target = null;
        //        isTargetLocked = false;
        //        isAttack = false;
        //        return;
        //    }

        //    float targetDist = Vector3.Distance(transform.position, target.transform.position);
        //    if(targetDist> detectrange)
        //    {
        //        target = null;
        //        isTargetLocked = false;
        //        isAttack = false;

        //        AnimatorChange(Define.CreatureState.Idle);
        //        transform.LookAt(SpawnPos);
        //        transform.position = Vector3.MoveTowards(transform.position, SpawnPos, _deltaTime);
        //        return;
        //    }
            
        //    if (targetDist <= detectrange && targetDist > attackrange && !isAttack)
        //    {
        //        AnimatorChange(Define.CreatureState.Move);
        //        transform.LookAt(target.transform);
        //        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, _deltaTime);
        //    }
        //    else if (targetDist <= attackrange && !isAttack)
        //    {
        //        if(target == null || target.IsDead)
        //        {
        //            target = null;
        //            isTargetLocked = false;
        //            isAttack = false;
        //            return;
        //        }

        //        isAttack = true;
        //        isTargetLocked = true;
        //        AnimatorChange(Define.CreatureState.Attack);
        //        transform.LookAt(target.transform);

        //        WaitForAttackDelay().Forget();
        //    }
        //}
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
