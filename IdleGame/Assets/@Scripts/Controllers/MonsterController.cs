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

    float lifeTime = 8f;
    #endregion


    public override bool Init()
    {
        if (!base.Init()) return false;
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

    public override void SetInfo()
    {
        isDead = false;
        isKnockBack = false;
        Hp = 10;

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
        
        FindClosetTarget(Managers.ObjectM.pcSet);
        transform.LookAt(target.transform);


        if (isKnockBack)
        {
            UpdateKnockBack(_deltaTime);
            return;
        }

        if (target.gameObject != null)
        {
            float targetPos = Vector3.Distance(transform.position, target.transform.position);
            transform.LookAt(target.transform);
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, _deltaTime);
        }
        else
        {
            if (target.IsDead)
                FindClosetTarget(Managers.ObjectM.pcSet);
        }
    }


    public override void GetDamage(double _dmg)
    {
        base.GetDamage(_dmg);
        Hp -= _dmg;
        Managers.ObjectM.Spawn<ObjectController>(transform.position, 20000);
        
        if(Hp <= 0)
        {
            Hp = 0;
            isDead = true;
            Managers.ObjectM.DeSpawn(this);
            return;
        }
    }

    //TODO : 필요하면 사용 
    public override UniTask ReturnObject(float _time)
    {
        return base.ReturnObject(_time);
    }

}
