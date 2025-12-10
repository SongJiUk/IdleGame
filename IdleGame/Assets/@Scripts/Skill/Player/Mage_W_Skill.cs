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
        InitSkillData(_caster);

        CreatureController randPlayer = Utils.FindRandomPlayer(_caster); ;

        if (randPlayer != null)
        {
            var chosenEffect = (Random.Range(0, 2) == 0) ? effects[0] : effects[1];

            chosenEffect.Execute(_caster, randPlayer);
            ShowEffect(randPlayer);

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
