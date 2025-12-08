using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Knight_Skill : SkillBase
{
    public Knight_Skill()
    {
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        Managers.DataM.SkillDataDic.TryGetValue(_caster.DATA.SkillDataID, out Data.SkillData skillData);

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
            if(buffData.Length > 0)
            {
                attack_length = buffData.Length;
            }
            
            if(buffData.Width > 0)
            {
                attack_width = buffData.Width;
            }
            
        }

        //TODO : 이거찾는거 수정해야될듯
        List<CreatureController> enemiesInArea = Utils.FindEnemyForwardArea(_caster, attack_length, attack_width);

        if (enemiesInArea.Count > 0)
        {
            foreach (CreatureController enemy in enemiesInArea)
            {
                foreach (var effect in effects)
                {
                    effect.Execute(_caster, enemy);
                }
            }
            ResetSkillStateAsync(_caster, skill_Duration).Forget();
            return true;
        }
        else
        {
            Debug.Log("[양손검 스킬] : 범위 내에 적이 없음");
            return false;
        }
    }

}
