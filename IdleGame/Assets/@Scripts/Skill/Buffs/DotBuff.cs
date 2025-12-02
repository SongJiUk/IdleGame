using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotBuff : BuffBase
{
    float damagePerTick;
    float tickInterval;
    float timeSinceLastTick;

    public DotBuff(float _duration) : base(_duration){}

    public override void Apply(CreatureController _target)
    { 
        //TODO : 스킬에서 가져오기(
    }

    public override void Remove(CreatureController _target)
    {

    }

}
