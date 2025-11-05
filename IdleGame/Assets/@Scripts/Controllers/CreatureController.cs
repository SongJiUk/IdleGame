using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class CreatureController : BaseController
{
    
    protected Animator animator;
    protected virtual bool isDead { get; set; }
    protected virtual bool isTargetLocked { get; set; }
    protected virtual bool isAttack { get; set; }
    public bool IsDead { get { return isDead; } }

    protected virtual double Hp { get; set; }
    protected virtual double MaxHp { get; set; }
    protected virtual float Damage { get; set; }
    protected virtual float attack_range { get; set; }
    protected virtual float detect_range { get; set; }

    protected CreatureController target;

    public override bool Init()
    {
        if (!base.Init()) return false;
        if (animator == null) animator = GetComponent<Animator>();


        return true;
    }

    public virtual void SetInfo()
    {

    }

    public virtual void InitStat()
    {

    }

    public virtual void OnDamage()
    {

    }

    public virtual void OnDead()
    {

    }


    public override void Tick(float _deltaTime)
    {
        base.Tick(_deltaTime);
    }

    protected async UniTask InitAttack()
    {
        // TODO : 이거도 캐릭터에 맞게 설정해야됌(공격 딜레이)
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }
        catch(Exception e)
        {
            Debug.LogError($"InitAttack Error {e.Message}");
        }
        finally
        {
            isAttack = false;
            isTargetLocked = false;
        }
        
    }

    protected virtual void AnimatorChange(Define.CreatureState _state)
    {
        int stateIndex = (int)_state;
        animator.SetInteger(Define.AnimState, stateIndex);
    }


    protected void FindClosetTarget<T>(HashSet<T> _targets) where T : Component
    {
        var targets = _targets;
        T closetTarget = null;
        //TODO : 찾는범위 알아서
        float maxDistance = 100f;

        foreach (var t in targets)
        {
            float targetDistance = Vector3.Distance(this.transform.position, t.transform.position);

            if (targetDistance < maxDistance)
            {
                closetTarget = t;
                maxDistance = targetDistance;
            }

            target = closetTarget as CreatureController;
            if (target != null) transform.LookAt(target.transform.position);
        }
    }

    public virtual void GetDamage(double _dmg)
    {
        Managers.ObjectM.ShowDamageFont(transform.position, _dmg, transform);
    }
}
