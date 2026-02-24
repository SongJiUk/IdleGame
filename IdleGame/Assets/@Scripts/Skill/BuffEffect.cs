using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffect : ISkillEffect
{
    public delegate IBuff BuffFactory(float _duration, float _ratio, float _interval = 0, CreatureController _owner = null);
    readonly BuffFactory buffFactory;

    public Define.BuffEffectType buffType;
    private float buffDurtaion;
    float buffRatio;
    float buffInterval;
    CreatureController buffOwner;

    public BuffEffect(BuffFactory _factory, float _duration, float _ratio, float _interval = 0, CreatureController _owner = null)
    {
        buffFactory = _factory;
        buffDurtaion = _duration;
        buffRatio = _ratio;
        buffInterval = _interval;
        buffOwner = _owner;

    }

    public void Execute(CreatureController _caster, CreatureController _target, float _delayTime = 0f)
    {
        if (_target == null || _target.IsDead) return; 

        IBuff newBuff = buffFactory(buffDurtaion, buffRatio, buffInterval, buffOwner);
        Debug.Log($"<color=cyan>[Buff_Step 1]</color> {newBuff.GetType().Name} 생성됨. 대상: {_target.name}, 수치: {buffRatio}");
        if (_target.buffController != null)
        {
            _target.buffController.AddBuff(newBuff);
        }
        


        //effects.Add(new BuffEffect(typeof(AttackBuff), 15f));
    }
}
