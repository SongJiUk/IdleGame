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

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        //팀원중 hp가 가장 낮은 플레이어를 찾아 사용
        CreatureController target = Utils.FindLowestHpPlayer();

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
            Debug.Log("[Cleric_Skill] : 아군이 없음.");
            return false;
        }
    }


}
