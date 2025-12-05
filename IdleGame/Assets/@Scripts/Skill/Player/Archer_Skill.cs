using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Archer_Skill : SkillBase
{
    public Archer_Skill()
    {
        //effects.Add(new DamageEffect(1));
        //effects.Add(new BuffEffect(_duration => new StatDownBuff(_duration), 10f));
        //effects.Add(new BuffEffect(_duration => new DotBuff(_duration), 10f));
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
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
            Debug.Log("��ó ��ų Ÿ�� ����]");
            return false;
        }
    }
}
