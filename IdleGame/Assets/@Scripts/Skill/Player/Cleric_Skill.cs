using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Cleric_Skill : SkillBase
{
    public Cleric_Skill()
    {
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        //팀원중 hp가 가장 낮은 플레이어를 찾아 사용
        CreatureController target = Utils.FindLowestHpPlayer();

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

        if (target != null)
        {
            foreach (var effect in effects)
            {
                effect.Execute(_caster, target);
            }
            ShowEffect(target, skillData);
            ResetSkillStateAsync(_caster, anim_Duration).Forget();

            return true;
        }
        else
        {
            Debug.Log("[Cleric_Skill] : 아군이 없음.");
            return false;
        }
    }
}
