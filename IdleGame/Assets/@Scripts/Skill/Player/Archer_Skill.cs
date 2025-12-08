using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;

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

        Managers.DataM.SkillDataDic.TryGetValue(_caster.DATA.SkillDataID, out Data.SkillData skillData);
        float duration = 0;
        foreach (int data in skillData.BuffList_ID)
        {
            Managers.DataM.SkillEffectDataDic.TryGetValue(data, out var buffData);
            skill_Duration = buffData.SkillDuration;

            if (buffData.AnimDuration > 0)
            {
                anim_Duration = buffData.AnimDuration;
            }

            if (buffData.Radius > 0)
            {
                attack_Radius = buffData.Radius;
            }

        }
        if (randTarget != null)
        {
            foreach (var effect in effects)
            {
                effect.Execute(_caster, randTarget);
            }

            ShowEffect(randTarget, skillData);
            ResetSkillStateAsync(_caster, anim_Duration).Forget();

            return true;
        }
        else
        {
            Debug.Log("아처 스킬 타겟이 없음");
            return false;
        }
    }
}
