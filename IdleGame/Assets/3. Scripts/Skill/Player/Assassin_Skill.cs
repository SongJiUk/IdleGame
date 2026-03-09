using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class Assassin_Skill : SkillBase
{
    public Assassin_Skill()
    {
    }

    public override async UniTask<bool> UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        CreatureController target = null;
        InitSkillData(_caster);


        if (_target != null) target = _target;
        else target = Utils.FindNearEnemy(_caster);


        if (target != null)
        {
            _caster.transform.LookAt(target.transform);
            //TODO : 그럼 이건, 여러번 공격으로 바꾸자
            SetDamage(_caster, target);

            foreach (var effect in effects)
            {
                effect.Execute(_caster, target);
            }

            ResetSkillStateAsync(_caster, anim_Duration).Forget();
            Managers.SoundM.Play(Define.Sound.Effect, "AssassinSkill");
            return true;
        }
        else
        {
            Debug.Log("[어쌔신 스킬] : 유효한 적이 없음]");
            return false;
        }
    }

}
