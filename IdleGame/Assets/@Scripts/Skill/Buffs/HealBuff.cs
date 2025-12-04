using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBuff : BuffBase
{
    float healAmount = 50f;
    public HealBuff(float _duration) : base(_duration) { }

    public override void Apply(CreatureController _target)
    {
        _target.Heal(healAmount);
        //TODO : 여기서 힐 이펙트
    }

    public override void Remove(CreatureController _target) { }
}
