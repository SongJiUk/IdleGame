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

    public override void UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        //TODO : ?•„êµ? ìºë¦­?„°ì¤? ì²´ë ¥?´ ê°??¥ ?‚®??? ìºë¦­?„°?—ê²? ? ?š©
        CreatureController target = Utils.FindLowestHpPlayer();

        if (target != null)
        {
            foreach (var effect in effects)
            {
                effect.Excute(_caster, target);
            }
        }
        else
        {
            Debug.Log("[Cleric_Skill] : ?œ ?š¨?•œ ?•„êµ°ì´ ?—†?Šµ?‹ˆ?‹¤.");
        }
    }


}
