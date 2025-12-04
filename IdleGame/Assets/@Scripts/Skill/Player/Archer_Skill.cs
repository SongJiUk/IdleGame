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
        effects.Add(new DamageEffect(1));
        effects.Add(new BuffEffect(_duration => new StatDownBuff(_duration), 10f));
        effects.Add(new BuffEffect(_duration => new DotBuff(_duration), 10f));
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        //공격하던 몬스터를 공격하는게 맞음
        CreatureController randTarget = null;
        if (_target != null)
        {
            randTarget = _target;
        }
        else
        {
            randTarget = Utils.FindRandomEnemyInRange(_caster, _caster.DATA.AttackRange);
        }


        if (randTarget != null)
        {
            foreach (var effect in effects)
            {
                effect.Execute(_caster, randTarget);
            }
            return true;
        }
        else
        {
            Debug.Log("아처 스킬 타겟 없음]");
            return false;
        }
    }
}
