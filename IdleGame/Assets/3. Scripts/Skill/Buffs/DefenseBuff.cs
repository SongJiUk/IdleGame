using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseBuff : BuffBase
{
    public DefenseBuff(float _duration, float _ratio) : base(_duration)
    {
        this.duration = _duration;
        this.ratio = _ratio;
    }

    public override void Apply(CreatureController _target)
    {
        _target.Defense *= ratio;
    }

    public override void Remove(CreatureController _target)
    {
        _target.Defense /= ratio;
    }

    public override Define.BuffEffectType GetBuffTypes() => Define.BuffEffectType.DefenseEffect;

}
