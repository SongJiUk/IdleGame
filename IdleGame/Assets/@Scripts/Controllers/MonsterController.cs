using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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


    public override bool Init()
    {
        if (!base.Init()) return false;
        attackrange = 0.5f;
        SetInfo();

        //TODO : 몬스터 처음나올때 초기화해주기
        //sAttack = Utils.Datas.levelData.Base_Attack;
        return true;
    }

    void OnEnable()
    {
        Managers.UpdateM.Register(this);
        SetInfo();

    }
    void OnDisable() => Managers.UpdateM.UnRegister(this);

    public void SetInfo()
    {
        isDead = false;
        isKnockBack = false;
        Hp = 1000;

    }

    public override void Attack()
    {

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
        if (isDead) return;


        if (target == null)
        {
            FindClosetTarget(Managers.ObjectM.pcSet);

            if (target != null)
            {
                if (target.IsDead)
                {
                    FindClosetTarget(Managers.ObjectM.pcSet);
                }

                float targetDist = Vector3.Distance(transform.position, target.transform.position);

                if (targetDist > attackrange && !isAttack)
                {
                    AnimatorChange(Define.CreatureState.Move);
                    transform.LookAt(target.transform.position);
                    transform.position = Vector3.MoveTowards(transform.position, target.transform.position, _deltaTime);
                }
                else if (targetDist <= attackrange && !isAttack)
                {
                    isAttack = true;
                    AnimatorChange(Define.CreatureState.Attack);
                    InitAttack().Forget();
                }
            }
        }
    }


    public override void GetDamage(double _dmg)
    {

        Hp -= _dmg;
        Managers.ObjectM.Spawn<ObjectController>(transform.position, 20000);
        base.GetDamage(_dmg);

        if (Hp <= 0)
        {
            Hp = 0;
            isDead = true;
            Managers.ObjectM.DeSpawn(this);
            //TODO : 이것도 바꿔야됌
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

            return;
        }
    }

    //TODO : 필요하면 사용 
    public override UniTask ReturnObject(float _time)
    {
        return base.ReturnObject(_time);
    }

}
