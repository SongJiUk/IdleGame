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

    protected virtual double baseHp { get; set; }
    public double BaseHp { get { return baseHp; } }
    protected virtual double baseDamage { get; set; }
    public double BaseDamage { get { return baseDamage; } }

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
    protected bool isPlayer = false;
    public bool IsPlayer
    {
        get { return isPlayer; }
    }
    //TODO : TEMP;
    protected float CriticalRate = 0;
    public Action OnTargetDead;
    public override bool Init()
    {
        if (!base.Init()) return false;
        if (animator == null) animator = GetComponent<Animator>();


        return true;
    }

    protected virtual void OnDisable()
    {
        if (target != null) target.OnTargetDead -= OnTargetDeadCallBack;
    }
    public virtual void InitStat()
    {

    }

    public virtual void OnDamage()
    {

    }

    public virtual void Dead()
    {
        isDead = true;
        OnTargetDead?.Invoke();
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
            isTargetLocked = false;
        }

    }

    public virtual void AnimatorChange(Define.CreatureState _state)
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

        if (target == null || !target.IsValid()) return;
        float dist = Vector3.Distance(transform.position, target.transform.position);
        if (dist > attackrange) return;

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


    protected void FindClosetTarget<T>(List<T> _targets) where T : Component
    {

        float minDistance = float.MaxValue;

        if (target != null)
            target.OnTargetDead -= OnTargetDeadCallBack;

        CreatureController closetTarget = null;

        foreach (var t in _targets)
        {
            if (t == null) continue;

            CreatureController cc = t as CreatureController;
            if (cc == null || cc.isDead) continue;

            float dist = Vector3.Distance(this.transform.position, t.transform.position);

            if (dist > detectrange) continue;

            if (dist < minDistance)
            {
                minDistance = dist;
                closetTarget = cc;
            }
        }

        target = closetTarget;

        if (target != null)
        {
            target.OnTargetDead += OnTargetDeadCallBack;
            isTargetLocked = true;
        }
        else isTargetLocked = false;
    }
    protected void OnTargetDeadCallBack()
    {
        ResetTarget();
        isAttack = false;
    }


    public virtual void GetDamage(double _dmg, CreatureController _attacker, bool _isCritical = false)
    {
        if (isDead) return;

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
