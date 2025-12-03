using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatDownBuff : BuffBase
{
    float reductionRate = 0.1f;
    public StatDownBuff(float _duration) : base(_duration) { }

    public override void Apply(CreatureController _target)
    {
        _target.Damage *= (1 - reductionRate);
        _target.Defence *= (1 - reductionRate);
        _target.Speed *= (1 - reductionRate);
    }

    public override void Remove(CreatureController _target)
    {
        _target.Damage /= (1 - reductionRate);
        _target.Defence /= (1 - reductionRate);
        _target.Speed /= (1 - reductionRate);
    }
}
