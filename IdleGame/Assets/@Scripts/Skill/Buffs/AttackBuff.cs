using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBuff : BuffBase
{

    protected float ratio;

    //지속시간 설정
    public AttackBuff(float _duration, float _ratio) : base(_duration)
    {
        this.duration = _duration;
        this.ratio = _ratio;
    }

    public override void Apply(CreatureController _target)
    {
        //타겟 공격력 증가
        _target.Damage *= ratio;
    }

    public override void Remove(CreatureController _target)
    {
        _target.Damage /= ratio;
    }

    public override Define.BuffEffectType GetBuffTypes() => Define.BuffEffectType.AttackEffect;
}
