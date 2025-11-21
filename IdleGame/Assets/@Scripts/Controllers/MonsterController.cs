using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{

    Vector3 startPos;
    #region 넉백 변수
    bool isKnockBack = false;
    Vector3 knockBackDir;
    float knockBackPower;
    float knockBackTime;
    float knockBackElapsed;

    #endregion
    public Action<MonsterController> OnMonsterInfoUpdate;
    public bool isBoss = false;
    SkillBase skillbase;
    Data.CreatureData data;
    CancellationTokenSource skillCTS;
    public override bool Init()
    {
        if (!base.Init()) return false;
       
        //TODO : 몬스터 처음나올때 초기화해주기
        //sAttack = Utils.Datas.levelData.Base_Attack;

        return true;
    }



    void OnEnable()
    {
        Managers.UpdateM.Register(this);

    }
    protected override void OnDisable()
    {
        skillCTS?.Cancel();
        Managers.UpdateM.UnRegister(this);

        base.OnDisable();
    }

    //TODO : 이걸 temp아이디를 넘겨받아서 여기서 하는게 맞을까 싶긴함
    public void SetInfo(Data.CreatureData _data)
    { 
        data = _data;
        isDead = false;
        isKnockBack = false;
        baseHp = _data.BaseHp;
        baseDamage = _data.BaseDamage;
        maxHp = Utils.Datas.stageData.HP((float)baseHp);
        hp = Utils.Datas.stageData.HP((float)baseHp);
        damage = Utils.Datas.stageData.Damage((float)baseDamage);

        Debug.Log($"HP : {hp}");
        Debug.Log($"Damage : {damage}");

        attackrange = data.AttackRange;
        detectrange = Mathf.Infinity;
        CriticalRate = 0f;
        isBoss = false;
        target = null;

        if (data.Type == ObjectType.Boss)
        {
            isBoss = true;
            skillCTS?.Cancel();
            skillCTS = new CancellationTokenSource();

            if (skillbase == null) skillbase = GetComponent<SkillBase>();
            SkillStart(skillCTS.Token).Forget();
        }
    }
    async UniTask SkillStart(CancellationToken _ct)
    {
        while (true)
        {
            try
            {
                await UniTask.WaitForSeconds(3.0f, cancellationToken: _ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (isDead) break;
            if (!isBoss) break;
            if (target == null) break;

            if (skillbase != null) skillbase.SetSkill(this);
            else break;
        }
        skillCTS.Dispose();
        skillCTS = null;
    }

    public override void Attack()
    {
        if (target == null || target.IsDead) return;

        Managers.ObjectM.Spawn<MeleeAttackController>(transform.position, 20001, this, target);
    }

    public override void Projectile()
    {
        if (target == null || target.IsDead) return;
        Managers.ObjectM.Spawn<RangeAttackController>(transform.position, 20002, this, target);

    }

    public void KnockBack(Vector3 _dir, float _power = 3f, float _duration = 0.3f)
    {
        isKnockBack = true;
        knockBackDir = _dir.normalized;
        knockBackPower = _power;
        knockBackTime = _duration;
        knockBackElapsed = 0f;
    }


    void UpdateKnockBack(float _deltaTime)
    {
        knockBackElapsed += _deltaTime;

        float t = knockBackElapsed / knockBackTime;
        float force = Mathf.Lerp(knockBackPower, 0f, t);

        transform.position += knockBackDir * force * _deltaTime;

        if (knockBackElapsed >= knockBackTime)
            isKnockBack = false;
    }

    public override void Tick(float _deltaTime)
    {
        if (Managers.StageM.stageState != StageState.Play && Managers.StageM.stageState != StageState.BossPlay) return;
        if (isDead) return;

        if (target == null || target.IsDead)
        {
            ResetTarget();

            FindClosetTarget(Managers.SpawnM.players);

            if(target ==null)
            {
                AnimatorChange(CreatureState.Idle);
                return;
            }
        }
        float targetDist = Vector3.Distance(transform.position, target.transform.position);

        if(targetDist > attackrange)
        {
            if(!isAttack)
            {
                AnimatorChange(Define.CreatureState.Move);
                transform.LookAt(target.transform.position);
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, _deltaTime);
            }
        }
        else
        {
            if(!isAttack)
            {
                isAttack = true;
                isTargetLocked = true;

                AnimatorChange(Define.CreatureState.Attack);
                transform.LookAt(target.transform);
                WaitForAttackDelay().Forget();
            }
        }
    }


    public override void GetDamage(double _dmg, CreatureController _attacker, bool _isCiritical = false)
    {
        if (isDead) return;

        base.GetDamage(_dmg, _attacker, _attacker.GetCritical());
        Managers.ObjectM.Spawn<ObjectController>(transform.position, 20000);
        OnMonsterInfoUpdate?.Invoke(this);
        if (hp <= 0)
        {
            hp = 0;
            
            Dead();
        }
    }


    public override void Dead()
    {
        base.Dead();
        if (isBoss)
        {
            Managers.ObjectM.DeSpawn(this);
            Managers.StageM.StateChange(StageState.Clear);
        }
        else
        {
            if(!Managers.StageM.isDead)
            {
                Managers.StageM.count++;
                Managers.GameM.mPlayer.KillCount++;
            }
            
            Managers.ObjectM.DeSpawn(this);
        }

        //TODO : 이것도 바꿔야됌
        //Managers.ObjectM.Spawn<CoinDirecting>(transform.position, );
        GameObject go = Managers.ResourceM.Instantiate("CoinDirecting", _pooling: true);
        CoinDirecting coinDriecting = go.GetComponent<CoinDirecting>();
        coinDriecting.Init(transform.position);

        var items = Managers.ItemM.GetDropItem();

        //TODO : 몬스터마다 아이템 개수 다르게
        for (int i = 0; i < items.Count; i++)
        {
            GameObject obj = Managers.ResourceM.Instantiate("DropItem", _pooling: true);
            DropItemController dc = obj.GetComponent<DropItemController>();
            dc.Init();
            dc.SetInfo(transform.position, items[i]);
        }
    }
    // async UniTask WaitForTime(float _time)
    // {

    // }

    //TODO : 필요하면 사용 
    public override UniTask ReturnObject(float _time)
    {
        return base.ReturnObject(_time);
    }
}
