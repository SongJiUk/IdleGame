using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class Hammer_Skill : SkillBase
{
    public Hammer_Skill()
    {
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        Managers.DataM.SkillDataDic.TryGetValue(_caster.DATA.SkillDataID, out Data.SkillData skillData);
        
        foreach (int data in skillData.BuffList_ID)
        {
            Managers.DataM.SkillEffectDataDic.TryGetValue(data, out var buffData);
            skill_Duration = buffData.SkillDuration;
            
            if(buffData.AnimDuration > 0)
            {
                anim_Duration = buffData.AnimDuration;
            }

            if(buffData.Radius > 0)
            {
                attack_Radius = buffData.Radius;
            }

        }

        List<CreatureController> enemiesInArea = Utils.FindEnemyInSphereArea(_caster, attack_Radius);
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
            Debug.Log("회전 공격 : 주변에 적 없음");
            return false;
        }
        //TODO : 쿨타임 처리 해야함
    }
}
