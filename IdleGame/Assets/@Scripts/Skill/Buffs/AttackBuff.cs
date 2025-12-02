using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBuff : BuffBase
{
    //지속시간 설정
    public AttackBuff(float _duration) : base(_duration)
    {
        
    }

    public override void Apply(CreatureController _target)
    {
        //타겟 공격력 증가
        _target.Damage *= 1.1;
    }

    public override void Remove(CreatureController _target)
    {
        _target.Damage /= 1.1;
    }
}
