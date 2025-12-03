using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Archer_Skill : SkillBase
{
    public Archer_Skill()
    {
        SetUpEffect();
    }

    protected override void SetUpEffect()
    {
        effects.Add(new BuffEffect(_duration => new StatDownBuff(_duration), 10f));
        effects.Add(new BuffEffect(_duration => new DotBuff(_duration), 5f));
    }

    public override void UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        CreatureController randTarget = Utils.FindRandomEnemyInRange(_caster, _caster.DATA.AttackRange);

        if (randTarget != null)
        {
            foreach (var effect in effects)
            {
                effect.Excute(_caster, randTarget);
            }
        }
        else
        {
            Debug.Log("아처 스킬 타겟 없음]");
        }
    }
}
