using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffBase : IBuff
{

    protected CreatureController target;
    protected float duration;
    protected float leftTime;
    protected float ratio;
    protected float radius;
    protected float interval;
    protected float timeSinceLastTick = 0;


    public System.Type GetBuffType()
    {
        return this.GetType();
    }

    public BuffBase(float _duration)
    {
        leftTime = _duration;
    }
    public virtual void Update(float _deltaTime)
    {
        leftTime -= _deltaTime;
    }

    public bool isExpired()
    {
        return leftTime <= 0;
    }


    
    public abstract void Apply(CreatureController _target);
    public abstract void Remove(CreatureController _target);
    public abstract Define.BuffEffectType GetBuffTypes();
    public float GetRatio() => ratio;

}
