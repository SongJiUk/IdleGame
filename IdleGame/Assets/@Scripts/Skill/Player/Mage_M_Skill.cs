using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public override void UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        CreatureController target = Utils.FindRandomEnemyInRange(_caster, 20f);
        if (target != null)
        {
            foreach (var effect in effects)
            {
                effect.Excute(_caster, target);
            }
        }
        else
        {
            Debug.Log("[Mage_M_Skill] : 유효한 적이 없음");
        }

    }


}
