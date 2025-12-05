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
        
        
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        Managers.DataM.SkillDataDic.TryGetValue(_caster.DATA.SkillDataID, out Data.SkillData skilldata);

        //if (skilldata != null)
        //{

        //    if (skilldata.BuffList_ID.Count > 1)
        //    {
        //        for(int i =0; i<skilldata.BuffList_ID.Count; i++)
        //        {
        //            Managers.DataM.SkillEffectDataDic.TryGetValue(skilldata.BuffList_ID[i], out var buffData);


        //        }
        //    }

        //}
        //TODO : 수정
        Managers.DataM.SkillEffectDataDic.TryGetValue(skilldata.BuffList_ID[0], out var buffData);

        effects.Add(new BuffEffect(_duration => new HealBuff(_duration), buffData.ValueRatio));

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
