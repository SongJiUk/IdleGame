using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage_M_Skill : SkillBase
{
    public Mage_M_Skill()
    {
        SetUpEffect();
    }

    protected override void SetUpEffect()
    {
        effects.Add(new DamageEffect(1.5, 5, 0.5));

    }

    public override void UseSkill(CreatureController _caster, CreatureController _target)
    {
        if(_target != null)
        {
            foreach(var effect in effects)
            {
                effect.Excute(_caster, _target);
            }
        }

        //TODO : 쿨타임 처리
    }


}
