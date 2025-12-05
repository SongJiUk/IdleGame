using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mage_M_Skill : SkillBase
{
    public Mage_M_Skill()
    {
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {

        CreatureController target = null;
        if (_target.IsDead) _target = null;

        if (_target != null)
        {
            target = _target;
        }
        else
        {
            target = Utils.FindRandomEnemyInRange(_caster, 2f);
        }


        if (target != null)
        {
            foreach (var effect in effects)
            {
                effect.Execute(_caster, target);
            }
            return true;
        }
        else
        {
            Debug.Log("[Mage_M_Skill] : 유효한 적이 없음");
            return false;
        }

    }


}
