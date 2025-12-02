using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleric_Skill : SkillBase
{
    public Cleric_Skill()
    {
        SetUpEffect();
    }

    protected override void SetUpEffect()
    {
        effects.Add(new BuffEffect(_duration => new HealBuff(_duration), 0.1f));
    }

    public override void UseSkill(CreatureController _caster, CreatureController _target)
    {
        CreatureController target = _target;

        if (target != null)
        {
            foreach (var effect in effects)
            {
                effect.Excute(_caster, target);
            }
        }
        else
        {
            Debug.Log("[Cleric_Skill] : 유효한 아군이 없습니다.");
        }
    }

    
}
