using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Archer_Skill : SkillBase
{
    public Archer_Skill() { }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        InitSkillData(_caster);
        CreatureController randTarget = null;
        if (_target != null)
        {
            randTarget = _target;
        }
        else
        {
            randTarget = Utils.FindRandomEnemyInRange(_caster, _caster.DATA.AttackRange * 2);
        }




        if (randTarget != null)
        {
            Managers.ObjectM.Spawn<RangeAttackController>(_caster.transform.position,
                    skill_ProjectileID,
                    _caster,
                    randTarget,
                    true);


            foreach (var effect in effects)
            {
                effect.Execute(_caster, randTarget);
            }

            ShowEffect(randTarget);
            ResetSkillStateAsync(_caster, anim_Duration).Forget();

            return true;
        }
        else
        {
            Debug.Log("아처 스킬 타겟이 없음");
            return false;
        }
    }
}
