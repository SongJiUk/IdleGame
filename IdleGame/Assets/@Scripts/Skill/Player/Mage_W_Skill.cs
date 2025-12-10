using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Mage_W_Skill : SkillBase
{

    public Mage_W_Skill()
    {
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        InitSkillData(_caster);

        if (effects == null || effects.Count < 2)
        {
            Debug.LogError("[Mage_W_Skill] : skillEffects가 제대로 설정되지 않았음.");
            return false;
        }

        CreatureController randPlayer = Utils.FindRandomPlayer(_caster); ;

        if (randPlayer != null)
        {
            if (randPlayer.IsDead) return false;


            var chosenEffect = (Random.Range(0, 2) == 0) ? effects[0] : effects[1];
            if (chosenEffect == null)
            {
                Debug.LogError("[Mage_W_Skill] : 랜덤선택된 이펙트가 없음.");
                return false;
            }
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
