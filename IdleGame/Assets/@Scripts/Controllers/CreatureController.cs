using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour, ITickable
{
    bool isInit = false;
    protected Animator animator;
    protected virtual bool isDead { get; set; }
    public bool IsDead { get { return isDead; } }
    protected virtual double Hp { get; set; }
    protected virtual double MaxHp { get; set; }
    protected virtual float Attack { get; set; }

    protected CreatureController target;

    public virtual bool Init()
    {
        if (isInit) return false;

        isInit = true;
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


    public virtual void Tick(float _deltaTime)
    {

    }

    protected void FindClosetTarget<T>(HashSet<T> _targets) where T : Component
    {
        var targets = _targets;
        T closetTarget = null;
        //TODO : 찾는범위 알아서
        float maxDistance = 5f;

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
}
