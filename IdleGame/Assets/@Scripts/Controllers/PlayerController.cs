using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerController : CreatureController
{

    void OnEnable() => Managers.UpdateM.Register(this);
    void OnDisable() => Managers.UpdateM.UnRegister(this);

    Vector3 startPos = Vector3.zero;
    string ownerName;
    public override bool Init()
    {
        if (!base.Init()) return false;
        isAttack = false;
        attack_range = 2f;
        detect_range = 5f;
        ownerName = this.name;
        return true;
    }

    public override void SetInfo()
    {

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

    public void Attack()
    {
        Managers.ObjectM.Spawn<ProjectileController>(transform.position, 20000, ownerName, target);
    }

    protected override void AnimatorChange(Define.CreatureState _state)
    {
        base.AnimatorChange(_state);
    }

    public override void Tick(float _deltaTime)
    {
        if (isDead) return;
        if(!isTargetLocked)
            FindClosetTarget(Managers.ObjectM.mcSet);
        
       
        if(target == null)
        {
            float targetPos = Vector3.Distance(transform.position, startPos);
            if(targetPos > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPos, _deltaTime);
                transform.LookAt(startPos);
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

            if(targetDist > detect_range)
            {
                target = null;
                AnimatorChange(Define.CreatureState.Idle);
                return;
            }

            if(targetDist > attack_range && !isAttack)
            {
                AnimatorChange(Define.CreatureState.Move);
                transform.LookAt(target.transform);
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, _deltaTime);
            }
            else if(targetDist <= attack_range && !isAttack)
            {
                isAttack = true;
                isTargetLocked = true;
                AnimatorChange(Define.CreatureState.Attack);
                InitAttack().Forget();
            }
        }
    }
}
