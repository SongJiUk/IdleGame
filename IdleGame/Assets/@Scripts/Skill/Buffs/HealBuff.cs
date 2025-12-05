using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBuff : BuffBase
{
    public HealBuff(float _duration, float _ratio) : base(_duration)
    {
        this.duration = _duration;
        this.ratio = _ratio;
    }

    public override void Apply(CreatureController _target)
    {
        _target.Heal(ratio);
    }

    public override void Remove(CreatureController _target) { }
}
