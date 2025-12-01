using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffect : ISkillEffect
{
    private IBuff buff;
    private System.Type buffType;
    private float buffDurtaion;
    public BuffEffect(System.Type _buffType, float _duration)
    {
        buffType = _buffType;
        buffDurtaion = _duration;

    }

    public void Excute(CreatureController _caster, CreatureController _target)
    {
        //target의 buff에 넣기.
        IBuff newBuff = (IBuff)System.Activator.CreateInstance(buffType, buffDurtaion);
        _target.buffController.AddBuff(newBuff);


        //TDOO: 지울거(사용방법)
        //effects.Add(new BuffEffect(typeof(AttackBuff), 15f));
    }
}
