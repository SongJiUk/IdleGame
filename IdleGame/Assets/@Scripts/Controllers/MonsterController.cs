using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public override bool Init()
    {
        if (!base.Init()) return false;
        attackrange = 2f;
        detectrange = Mathf.Infinity;
        CriticalRate = 0f;

        //TODO : 몬스터 처음나올때 초기화해주기
        //sAttack = Utils.Datas.levelData.Base_Attack;

        return true;
    }

    

    void OnEnable()
    {
        Managers.UpdateM.Register(this);

    }
    void OnDisable() => Managers.UpdateM.UnRegister(this);

    //TODO : 이걸 temp아이디를 넘겨받아서 여기서 하는게 맞을까 싶긴함
    public void SetInfo(int _tempId)
    {
        isDead = false;
        isKnockBack = false;
        Managers.DataM.CreatureDataDic.TryGetValue(_tempId, out var data);
        maxHp = data.BaseHp;
        hp = data.BaseHp;
        damage = data.BaseDamage;
        if (data.Type == ObjectType.Boss) isBoss = true;

        if (isBoss)
        {
            skillbase = GetComponent<SkillBase>();
            SkillStart().Forget();
        }

    }
    async UniTask SkillStart()
    {
        while (true)
        {
            if (isDead) break;
            await UniTask.WaitForSeconds(3.0f);
            if (isDead) break;

            if (skillbase != null) skillbase.SetSkill(this);
            else break;
        }
        
        
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

        if (Managers.StageM.stageState == StageState.Play || Managers.StageM.stageState == StageState.BossPlay)
        {
            if (isDead) return;


            if (!isAttack && !isTargetLocked)
                FindClosetTarget(Managers.ObjectM.pcSet);

            if (target == null || target.IsDead)
            {
                ResetTarget();
                AnimatorChange(Define.CreatureState.Idle);
                return;
            }

            float targetDist = Vector3.Distance(transform.position, target.transform.position);
            
            if (targetDist > attackrange)
            {
                AnimatorChange(Define.CreatureState.Move);
                transform.LookAt(target.transform.position);
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, _deltaTime);
            }


            if (targetDist <= attackrange && !isAttack)
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
        base.GetDamage(_dmg, _attacker, _attacker.GetCritical());
        Managers.ObjectM.Spawn<ObjectController>(transform.position, 20000);
        OnMonsterInfoUpdate?.Invoke(this);
        if (hp <= 0)
        {
            hp = 0;

            if (isBoss)
            {
                isDead = true;
                Managers.ObjectM.DeSpawn(this);
                Managers.StageM.StateChange(StageState.Clear);
            }
            else
            {

                isDead = true;
                Managers.StageM.count++;
                Managers.GameM.mPlayer.KillCount++;
                Managers.ObjectM.DeSpawn(this);


                //TODO : 이것도 바꿔야됌
                //Managers.ObjectM.Spawn<CoinDirecting>(transform.position, );
                GameObject go = Managers.ResourceM.Instantiate("CoinDirecting", _pooling: true);
                CoinDirecting coinDriecting = go.GetComponent<CoinDirecting>();
                coinDriecting.Init(transform.position);

                //TODO : 몬스터마다 아이템 개수 다르게
                for (int i = 0; i < 3; i++)
                {
                    GameObject obj = Managers.ResourceM.Instantiate("DropItem", _pooling: true);
                    DropItemController dc = obj.GetComponent<DropItemController>();
                    dc.Init();
                    dc.SetInfo(transform.position);
                }
            }
            
        }
    }

    async UniTask WaitForTime(float _time)
    {

    }

    //TODO : 필요하면 사용 
    public override UniTask ReturnObject(float _time)
    {
        return base.ReturnObject(_time);
    }
}
