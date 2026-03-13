using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static Define;
using System.Threading;

public class CreatureController : BaseController
{
    public CreatureType Type = CreatureType.None;

    protected Animator animator;
    protected virtual bool isDead { get; set; }
    protected virtual bool isTargetLocked { get; set; }
    protected virtual bool isAttacking { get; set; }
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
    public virtual double Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    protected virtual double defense { get; set; }
    public virtual double Defense
    {
        get { return defense; }
        set { defense = value; }
    }

    protected virtual float speed { get; set; }
    public float Speed
    {
        get { return speed; }
        set { speed = value; }

    }
    protected virtual float attackRange { get; set; }
    public float AttackRange { get { return attackRange; } }
    protected virtual float detectRange { get; set; }
    public float DetectRange { get { return detectRange; } }
    protected CreatureController target;
    protected CreatureController currentTarget;
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
    public bool isUsingSkill = false;
    public List<GameObject> vfxs = new List<GameObject>();

    protected float searchDelayTimer = 0f;
    private const float SEARCH_DELAY = 0.5f;

    protected CancellationTokenSource attackCTS;

    protected Dictionary<BuffEffectType, float> activeBuffs = new Dictionary<BuffEffectType, float>();
    public override bool Init()
    {
        if (!base.Init()) return false;
        if (animator == null) animator = GetComponent<Animator>();
        if (buffController == null) buffController = GetComponent<BuffController>();
        if (skillController == null) skillController = GetComponent<SkillController>();


        return true;
    }

    protected virtual void OnDisable()
    {
        if (target != null) target.OnTargetDead -= OnTargetDeadCallBack;

        ClearAttachedVFXs();
        if (buffController != null) buffController.ClearAllBuffs();

    }
    public virtual void InitStat()
    {
        if (data.AttackSpeed <= 0)
        {
            Debug.LogWarning($"[Data Error] {name}의 공격속도가 0입니다. 기본값 1로 강제설정합니다.");
            data.AttackSpeed = 1f;
        }
    }

    public virtual void OnDamage()
    {

    }
    public virtual void Dead()
    {
        attackCTS?.Cancel();
        attackCTS = null;
        isAttacking = false;
        isDead = true;

        if (buffController != null) buffController.ClearAllBuffs();
        //if (skillController != null) skillController.ClearAllSkillsVFX();
        ClearAttachedVFXs();
        OnTargetDead?.Invoke();
    }

    public virtual void SetSkill()
    {
        this.Type = DATA.CreatureType;


        if (skillController != null)
        {
            skillController = GetComponent<SkillController>();
            List<SkillBase> initialSkills = SkillRegistry.CreateSkillsForCreature(this.Type);
            skillController.InitSkills(initialSkills);
        }
    }

    public void ClearAttachedVFXs()
    {
        if (vfxs == null) return;

        for (int i = vfxs.Count - 1; i >= 0; i--)
        {

            if (vfxs[i] != null)
            {
                Managers.ResourceM.Destroy(vfxs[i]);
            }
        }

        vfxs.Clear();
    }

    public virtual void Projectile() { }
    public virtual void Attack() { }
    public void Heal(float _amount)
    {
        if (IsDead) return;
        double healValue = MaxHP * _amount;
        hp += healValue;

        if (HP > MaxHP)
            hp = MaxHP;

        //Debug.Log($"[Heal] {name} 회복량: {healValue} (비율:{_amount}) 현재HP:{HP}/{MaxHP}");
    }

    public float GetCurrentPlayingClipDuration(Animator anim)
    {
        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);

        if (clipInfo.Length > 0)
        {
            return clipInfo[0].clip.length;
        }

        return 0f;
    }

    public virtual float GetAttackSpeed()
    {
        return data.AttackSpeed;
    }
    protected async UniTask WaitForAttackDelay(CancellationToken _token)
    {
        try
        {
            float safeSpeed = Mathf.Max(0.1f, GetAttackSpeed());
            float attackDuration = 1f / safeSpeed;
            await UniTask.Delay(TimeSpan.FromSeconds(attackDuration), cancellationToken: _token);

            isAttacking = false;
            isTargetLocked = false;
            currentTarget = null;
            OnAttackDelayEnd();
        }
        catch (OperationCanceledException)
        {
            isAttacking = false;
            isTargetLocked = false;
            currentTarget = null;
        }
        catch (Exception e)
        {
            isAttacking = false;
            Debug.LogError($"InitAttack Error {e.Message}");
        }
    }

    protected virtual void OnAttackDelayEnd() { }

    public virtual void AnimatorChange(Define.CreatureState _state)
    {
        if (animator == null) return;

        if (isDead && _state != CreatureState.Dead)
        {
            Debug.LogWarning($"{gameObject.name}이(가) 죽은 상태인데 {_state}로 전환하려고 함! 차단됨.");
            return;
        }


        int stateIndex = (int)_state;
        animator.SetInteger(Define.AnimState, stateIndex);
    }

    public void GoBackToSpawn(float _deltaTime)
    {
        float dist = Vector3.Distance(transform.position, SpawnPos);

        if (dist > 0.1f)
        {
            AnimatorChange(Define.CreatureState.Move);

            Vector3 dir = SpawnPos - transform.position;

            if (dir.sqrMagnitude > 0.05f)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }

            transform.position = Vector3.MoveTowards(transform.position, SpawnPos, _deltaTime);
        }
        else
        {
            AnimatorChange(Define.CreatureState.Idle);
        }
    }

    public void MoveToTarget(float _deltaTime)
    {
        if (target == null) return;

        AnimatorChange(Define.CreatureState.Move);
        Vector3 dir = target.transform.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude > 0.05f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, _deltaTime);
    }

    public virtual async UniTask StartAttack()
    {
        attackCTS?.Cancel();
        attackCTS = new CancellationTokenSource();

        if (target == null || !target.IsValid())
        {
            isAttacking = false;
            return;
        }
        currentTarget = target;
        isAttacking = true;

        AnimatorChange(Define.CreatureState.Attack);
        Vector3 lookPos = target.transform.position;
        lookPos.y = transform.position.y;

        transform.LookAt(lookPos);

        try
        {
            await WaitForAttackDelay(attackCTS.Token);

        }
        finally
        {
            isAttacking = false;
        }

    }

    public virtual void ResetTarget()
    {
        target = null;
        isTargetLocked = false;
    }

    protected void FindClosetTarget<T>(List<T> _targets) where T : Component
    {
        if (_targets == null) return;
        FindClosetTarget(_targets.ToArray());
    }

    protected void FindClosetTarget<T>(T[] _targets) where T : Component
    {

        if (_targets == null || _targets.Length == 0) return;

        float minDist = float.MaxValue;

        if (target != null)
            target.OnTargetDead -= OnTargetDeadCallBack;

        CreatureController closetTarget = null;

        for (int i = 0; i < _targets.Length; i++)
        {
            if (_targets[i] == null) continue;

            CreatureController cc = _targets[i] as CreatureController;
            if (cc == null || cc.isDead) continue;

            float dist = Vector3.Distance(this.transform.position, cc.transform.position);
            if (dist > detectRange) continue;

            if (dist < minDist)
            {
                minDist = dist;
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
    }


    public virtual void GetDamage(double _dmg, CreatureController _attacker, bool _isCritical = false, bool _isSkill = false)
    {
        if (isDead) return;

        double finalDamage = _dmg;
        isCritical = _isCritical;


        if (isCritical) finalDamage *= Managers.PlayerM.CriticalDamage() / 100f;


        //if (_isSkill)
        //{
        //    finalDamage = _dmg;
        //}
        double currentDefense = this.Defense;


        double reduction = 100.0 / (100.0 + (currentDefense > 0 ? currentDefense : 0));
        finalDamage *= reduction;
        if (finalDamage < 1) finalDamage = 1;

        //TODO : 피격데미지 확인하는곳
        //Debug.Log($"<color=red>[Buff_Step 4]</color> 최종 피격 데미지: {finalDamage} (적용 방어력: {currentDefense})");

        hp -= finalDamage;
        bool isMonster = false;
        if (_attacker as MonsterController) isMonster = true;

        Managers.SoundM.Play(Sound.Effect, "Hit");
        Managers.ObjectM.ShowDamageFont(transform.position, finalDamage, isMonster, isCritical, _isSkill);
    }

    public virtual bool GetCritical()
    {
        float rand = UnityEngine.Random.Range(0, 100f);
        if (rand <= this.CriticalRate) return true;

        return false;
    }

    public void ApplyStatBuff(BuffEffectType _type, float _amount, float _duration)
    {
        if (!activeBuffs.ContainsKey(_type)) activeBuffs[_type] = 0;

        activeBuffs[_type] += _amount;

        RemoveBuffAfterTime(_type, _amount, _duration).Forget();
    }

    private async UniTaskVoid RemoveBuffAfterTime(BuffEffectType _type, float _amount, float _duration)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_duration));

        if (activeBuffs.ContainsKey(_type))
        {
            activeBuffs[_type] -= _amount;
        }
    }

    public float GetBuffValue(BuffEffectType _type)
    {
        if (buffController == null) return 0f;

        return buffController.GetTotalRatio(_type);
    }
}
