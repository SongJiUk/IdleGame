using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatDownBuff : BuffBase
{
    public StatDownBuff(float _duration, float _ratio) : base(_duration)
    {
        ratio = _ratio;
    }

    public override void Apply(CreatureController _target)
    {
        _target.Damage *= (1 - ratio);
        _target.Defense *= (1 - ratio);
        _target.Speed *= (1 - ratio);
    }

    public override void Remove(CreatureController _target)
    {
        _target.Damage /= (1 - ratio);
        _target.Defense /= (1 - ratio);
        _target.Speed /= (1 - ratio);
    }
}
