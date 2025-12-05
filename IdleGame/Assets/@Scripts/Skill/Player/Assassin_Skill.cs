using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assassin_Skill : SkillBase
{
    public Assassin_Skill()
    {
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        CreatureController target = null;
        if (_target != null)
        {
            target = _target;
        }
        else
        {
            target = Utils.FindNearEnemy(_caster);
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
            Debug.Log("[어쌔신 스킬] : 유효한 적이 없음]");
            return false;
        }
    }

}
