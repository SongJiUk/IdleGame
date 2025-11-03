using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterController : CreatureController
{

    void OnEnable() => Managers.UpdateM.Register(this);
    void OnDisable() => Managers.UpdateM.UnRegister(this);


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
        isDead = false;
        lifeTime = 10f;
        //TODO : 몬스터 처음나올때 초기화해주기
        //sAttack = Utils.Datas.levelData.Base_Attack;
        return true;
    }
    public override void SetInfo()
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
        lifeTime -= _deltaTime;
        if (lifeTime <= 0f)
        {
            Managers.ObjectM.DeSpawn(this);
            return;
        }
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


    void AnimatorChange(Define.CreatureState _state)
    {
        switch (_state)
        {
            case Define.CreatureState.Idle:
                animator.SetBool("isIdle", true);
                break;
            case Define.CreatureState.Move:
                animator.SetBool("isMove", true);
                break;
            case Define.CreatureState.Attack:
                animator.SetBool("isAttack", true);
                break;
            case Define.CreatureState.Hit:
                animator.SetBool("isHit", true);
                break;
            case Define.CreatureState.Dead:
                animator.SetBool("isDead", true);
                break;
        }
    }

}
