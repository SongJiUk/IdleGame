using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Mage_W_Skill : SkillBase
{
    BuffEffect attackBuffEffect;
    BuffEffect defenseBuffEffect;

    public Mage_W_Skill()
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
        }

        //TODO : 이거 공격력, 방어력 버프 이펙트도 나눠야될듯


        CreatureController randPlayer = Utils.FindRandomPlayer(_caster); ;

        if (randPlayer != null)
        {
            var chosenEffect = (Random.Range(0, 2) == 0) ? effects[0] : effects[1];

            chosenEffect.Execute(_caster, randPlayer);
            ShowEffect(_caster, skillData);

            ResetSkillStateAsync(_caster, anim_Duration).Forget();

            return true;
        }
        else
        {
            Debug.Log("[Mage_W_Skill] : 유효한 아군이 없음");
            return false;
        }

        //TODO : 쿨타임 처리(메인 캐릭터일떄)
    }
}
