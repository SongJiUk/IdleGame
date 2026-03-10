using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Cleric_Skill : SkillBase
{
    public Cleric_Skill()
    {
    }

    public override async UniTask<bool> UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        //팀원중 hp가 가장 낮은 플레이어를 찾아 사용
        CreatureController target = Utils.FindLowestHpPlayer();
        InitSkillData(_caster);


        if (target != null)
        {
            _caster.transform.LookAt(target.transform);
            //TODO: 클레릭은 공격 로직이 없음.
            foreach (var effect in effects)
            {
                effect.Execute(_caster, target);
            }
            ShowEffect(target);
            ResetSkillStateAsync(_caster, anim_Duration).Forget();
            Managers.SoundM.Play(Define.Sound.Effect, "ClericSkill");
            return true;
        }
        else
        {
            //Debug.Log("[Cleric_Skill] : 아군이 없음.");
            return false;
        }
    }
}
