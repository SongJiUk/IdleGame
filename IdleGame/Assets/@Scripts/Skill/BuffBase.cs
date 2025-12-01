using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffBase : IBuff
{

    protected CreatureController target;
    protected float duration;
    protected float leftTime;


    public BuffBase(float _duration)
    {
        this.duration = _duration;
        this.leftTime = _duration;
    }
    public void Update(float _deltaTime)
    {
        leftTime -= _deltaTime;
    }

    public bool isExpired()
    {
        return leftTime <= 0;
    }


    public abstract void Apply(CreatureController _target);
    public abstract void Remove(CreatureController _target);

}
