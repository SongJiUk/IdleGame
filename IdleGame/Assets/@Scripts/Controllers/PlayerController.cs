using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Data.Common;

public class PlayerController : CreatureController
{
    Data.CreatureData data;
    void OnEnable() => Managers.UpdateM.Register(this);
    void OnDisable() => Managers.UpdateM.UnRegister(this);

    Vector3 startPos = Vector3.zero;
    protected Vector3 SpawnPos;
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

        attackrange = data.AttackRange;
        detectrange = 5f;
        ownerName = this.name;
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
        if (target == null) return;
        Managers.ObjectM.Spawn<RangeAttackController>(transform.position, 20000, ownerName, target);
    }

    public override void Attack()
    {
        GameObject go = Managers.ResourceM.Instantiate(Managers.DataM.ProjectileDataDic[20001].prefabName, _pooling: true);
        MeleeAttackController mac = go.GetComponent<MeleeAttackController>();
        mac.AttackInit(target as MonsterController, 10);

    }

    protected override void AnimatorChange(Define.CreatureState _state)
    {
        base.AnimatorChange(_state);
    }

    public override void Tick(float _deltaTime)
    {
        if (isDead) return;
        if (!isTargetLocked)
            FindClosetTarget(Managers.ObjectM.mcSet);


        if (target == null)
        {
            float targetPos = Vector3.Distance(transform.position, SpawnPos);
            if (targetPos > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, SpawnPos, _deltaTime);
                transform.LookAt(SpawnPos);
                AnimatorChange(Define.CreatureState.Move);
            }
            else
            {
                AnimatorChange(Define.CreatureState.Idle);
            }
        }
        else
        {
            if (target.IsDead)
            {
                target = null;
                return;
            }

            float targetDist = Vector3.Distance(transform.position, target.transform.position);

            if (targetDist > detectrange)
            {
                target = null;
                AnimatorChange(Define.CreatureState.Idle);
                return;
            }

            if (targetDist > attackrange && !isAttack)
            {
                AnimatorChange(Define.CreatureState.Move);
                transform.LookAt(target.transform);
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, _deltaTime);
            }
            else if (targetDist <= attackrange && !isAttack)
            {
                isAttack = true;
                isTargetLocked = true;
                AnimatorChange(Define.CreatureState.Attack);
                InitAttack().Forget();
            }
        }
    }
}
