using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Enchantress_Skill : SkillBase
{

    public Enchantress_Skill()
    {
    }

    public override async UniTask<bool> UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        InitSkillData(_caster);

        if (effects == null || effects.Count < 2)
        {
            //Debug.LogError("[EnchantressSkill] : skillEffects가 제대로 설정되지 않았음.");
            return false;
        }

        CreatureController randPlayer = Utils.FindRandomPlayer(_caster);
        if (randPlayer != null)
        {
            if (randPlayer.IsDead) return false;
            _caster.transform.LookAt(randPlayer.transform);


            var chosenEffect = (Random.Range(0, 2) == 0) ? effects[0] : effects[1];
            if (chosenEffect == null)
            {
                //Debug.LogError("[EnchantressSkill] : 랜덤선택된 이펙트가 없음.");
                return false;
            }
            chosenEffect.Execute(_caster, randPlayer);
            ShowEffect(randPlayer);

            ResetSkillStateAsync(_caster, anim_Duration).Forget();
            Managers.SoundM.Play(Define.Sound.Effect, "EnchantressSkill");
            return true;
        }
        else
        {
            //Debug.Log("[Enchantress_Skill] : 유효한 아군이 없음");
            return false;
        }

        //TODO : 쿨타임 처리(메인 캐릭터일떄)
    }
}
