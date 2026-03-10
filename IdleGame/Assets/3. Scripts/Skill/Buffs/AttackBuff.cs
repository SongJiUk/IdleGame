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
        //Debug.Log($"[BuffStart] AttackBuff 시작 ({duration}초)");

        //타겟 공격력 증가
        _target.Damage *= ratio;
    }

    public override void Remove(CreatureController _target)
    {
        //Debug.Log($"[BuffEnd] AttackBuff 종료");

        _target.Damage /= ratio;
    }

    public override Define.BuffEffectType GetBuffTypes() => Define.BuffEffectType.AttackEffect;
}
