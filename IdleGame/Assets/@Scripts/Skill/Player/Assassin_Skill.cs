using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class Assassin_Skill : SkillBase
{
    public Assassin_Skill()
    {
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        CreatureController target = null;
        

        Managers.DataM.SkillDataDic.TryGetValue(_caster.DATA.SkillDataID, out Data.SkillData skillData);
        foreach (int data in skillData.BuffList_ID)
        {
            Managers.DataM.SkillEffectDataDic.TryGetValue(data, out var buffData);

            if (buffData.AnimDuration > 0)
            {
                anim_Duration = buffData.AnimDuration;
            }

            if (buffData.Radius > 0)
            {
                attack_Radius = buffData.Radius;
            }

        }

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
            ResetSkillStateAsync(_caster, anim_Duration).Forget();

            return true;
        }
        else
        {
            Debug.Log("[어쌔신 스킬] : 유효한 적이 없음]");
            return false;
        }
    }

}
