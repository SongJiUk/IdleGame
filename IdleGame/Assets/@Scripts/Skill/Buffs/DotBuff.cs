using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotBuff : BuffBase
{
    CreatureController Owner;
    public DotBuff(float _duration, float _ratio, float _interval, CreatureController _owner) : base(_duration)
    {
        duration = _duration;
        ratio = _ratio;
        interval = _interval;
        Owner = _owner;
    }

    public override void Apply(CreatureController _target)
    {
        target = _target;
    }

    public override void Remove(CreatureController _target)
    {
        target = null;
    }

    public override void Update(float _deltaTime)
    {
        //남은 시간 감소
        base.Update(_deltaTime);

        timeSinceLastTick += _deltaTime;
        if (timeSinceLastTick >= interval)
        {
            DotDamage();
            timeSinceLastTick = 0f;
        }
    }

    void DotDamage()
    {
        float tickDamage = (float)Owner.Damage * ratio;
        target.GetDamage(tickDamage, Owner, _isSkill: true);

    }
}
