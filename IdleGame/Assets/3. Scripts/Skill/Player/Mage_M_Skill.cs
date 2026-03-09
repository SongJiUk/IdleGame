using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class Mage_M_Skill : SkillBase
{
    public Mage_M_Skill()
    {
    }

    public override async UniTask<bool> UseSkill(CreatureController _caster, CreatureController _target = null)
    {


        InitSkillData(_caster);

        CreatureController target = null;
        if (_target != null)
        {
            target = _target;
        }
        else
        {
            target = Utils.FindRandomEnemyInRange(_caster, skill_Radius);
        }

        _caster.transform.LookAt(target.transform);
        if (target != null)
        {
            if (target.IsDead) return false;

            SetDamage(_caster, target, 1.2f);

            ShowEffect(target);
            ResetSkillStateAsync(_caster, anim_Duration).Forget();
            Managers.SoundM.Play(Define.Sound.Effect, "Mage_MSkill");
            return true;
        }
        else
        {
            Debug.Log("[Mage_M_Skill] : 유효한 적이 없음");
            return false;
        }

    }


}
