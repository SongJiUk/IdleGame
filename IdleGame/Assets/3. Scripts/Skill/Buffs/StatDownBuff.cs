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
        Debug.Log($"{_target.name}에게 종합 디버프 적용중 : {ratio}%");
    }

    public override void Remove(CreatureController _target)
    {
    }

    public override Define.BuffEffectType GetBuffTypes() => Define.BuffEffectType.StatDownEffect;
}
