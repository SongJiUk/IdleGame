using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static Define;

public class CreatureController : BaseController
{
    public CreatureType Type = CreatureType.None;

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
    public double Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    protected virtual double defence { get; set; }
    public double Defence
    {
        get { return defence; }
        set { defence = value; }
    }

    protected virtual float speed { get; set; }
    public float Speed
    {
        get { return speed; }
        set { speed = value; }

    }
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

    public BuffController buffController;
    public SkillController skillController;
    Data.CreatureData data;
    public Data.CreatureData DATA
    {
        get { return data; }
        set { data = value; }
    }

    public override bool Init()
    {
        if (!base.Init()) return false;
        if (animator == null) animator = GetComponent<Animator>();
        if (buffController == null) buffController = GetComponent<BuffController>();

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

    public void SetSkill()
    {
        this.Type = DATA.CreatureType;

        skillController = GetComponent<SkillController>();
        if (skillController != null)
        {
            List<SkillBase> initialSkills = SkillRegistry.CreateSkillsForCreature(this.Type);
            skillController.InitSkills(initialSkills);
        }
    }

    public virtual void Projectile() { }
    public virtual void Attack() { }
    public void Heal(float _amount)
    {

    }

    public float GetCurrentPlayingClipDuration(Animator anim)
    {
        // 0번 레이어(Base Layer)의 클립 정보를 가져옵니다.
        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);

        if (clipInfo.Length > 0)
        {
            // 현재 가중치(Weight)가 가장 높은 (또는 첫 번째) 클립의 길이를 반환합니다.
            // 블렌딩 중이라면 정확하지 않을 수 있습니다.
            return clipInfo[0].clip.length;
        }

        // 재생 중인 클립이 없을 경우
        return 0f;
    }
    protected async UniTask WaitForAttackDelay()
    {
        try
        {
            // TODO : 이거도 캐릭터에 맞게 설정해야됌(공격 딜레이)

            float time = GetCurrentPlayingClipDuration(animator);

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
