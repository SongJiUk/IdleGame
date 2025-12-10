using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class Mage_M_Skill : SkillBase
{
    public Mage_M_Skill()
    {
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
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


        if (target != null)
        {
            if (target.IsDead) return false;

            //TODO : 하드코딩 없애기
            SetDamage(_caster, target, 1.2f);

            ShowEffect(target);
            ResetSkillStateAsync(_caster, anim_Duration).Forget();
            return true;
        }
        else
        {
            Debug.Log("[Mage_M_Skill] : 유효한 적이 없음");
            return false;
        }

    }


}
