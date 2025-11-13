using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static Define;

public class CreatureController : BaseController
{

    protected Animator animator;
    protected virtual bool isDead { get; set; }
    protected virtual bool isTargetLocked { get; set; }
    protected virtual bool isAttack { get; set; }
    protected virtual bool isCritical { get; set; }
    public bool IsDead { get { return isDead; } }

    protected virtual double hp { get; set; }
    public double HP { get { return hp; } }
    protected virtual double maxHp { get; set; }
    public double MaxHP { get { return maxHp; } }
    protected virtual double damage { get; set; }
    public double Damage { get { return damage; } }
    protected virtual float attackrange { get; set; }
    protected virtual float detectrange { get; set; }

    protected CreatureController target;
    protected Vector3 SpawnPos;
    //TODO : TEMP;
    protected float CriticalRate = 0;

    public override bool Init()
    {
        if (!base.Init()) return false;
        if (animator == null) animator = GetComponent<Animator>();


        return true;
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

    public virtual void Projectile() { }
    public virtual void Attack() { }

    protected async UniTask WaitForAttackDelay()
    {
        try
        {
            // TODO : 이거도 캐릭터에 맞게 설정해야됌(공격 딜레이)
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }
        catch (Exception e)
        {
            Debug.LogError($"InitAttack Error {e.Message}");
        }
        finally
        {
            isAttack = false;
        }

    }

    protected virtual void AnimatorChange(Define.CreatureState _state)
    {
        int stateIndex = (int)_state;
        animator.SetInteger(Define.AnimState, stateIndex);
    }

    public void GoBackToSpawn(float _deltaTime)
    {
        float dist = Vector3.Distance(transform.position, SpawnPos);

        if (dist > 0.1f)
        {
            AnimatorChange(Define.CreatureState.Move);
            transform.LookAt(SpawnPos);
            transform.position = Vector3.MoveTowards(transform.position, SpawnPos, _deltaTime);
        }
        else
            AnimatorChange(Define.CreatureState.Idle);
    }

    public void MoveToTarget(float _deltaTime)
    {
        AnimatorChange(Define.CreatureState.Move);
        transform.LookAt(target.transform);
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, _deltaTime);
    }

    public void StartAttack()
    {
        isAttack = true;
        AnimatorChange(Define.CreatureState.Attack);
        transform.LookAt(target.transform);

        WaitForAttackDelay().Forget();
    }

    public void ResetTarget()
    {
        target = null;
        isAttack = false;
        isTargetLocked = false;
    }


    protected void FindClosetTarget<T>(HashSet<T> _targets) where T : Component
    {
        CreatureController closetTarget = null;
        float minDistance = float.MaxValue;

        foreach (var t in _targets)
        {
            if (t == null) continue;

            CreatureController cc = t as CreatureController;
            if (cc != null && cc.isDead) continue;

            float targetDistance = Vector3.Distance(this.transform.position, t.transform.position);

            if (targetDistance <= detectrange && targetDistance < minDistance)
            {
                closetTarget = cc;
                minDistance = targetDistance;
            }
        }

        target = closetTarget;
    }

    public virtual void GetDamage(double _dmg, CreatureController _attacker,  bool _isCritical = false)
    {
        double finaldamage = _dmg;
        isCritical = _isCritical;
        if (isCritical)
        {
            //TODO : 수정 
            finaldamage = _dmg * 1.5f;
        }

        hp -= finaldamage;
        bool isMonster = false;
        if (_attacker as MonsterController) isMonster = true;

        Managers.ObjectM.ShowDamageFont(transform.position, finaldamage, isMonster, isCritical);

    }

    public virtual bool GetCritical()
    {
        if (UnityEngine.Random.value <= this.CriticalRate) return true;

        return false;
    }
}
