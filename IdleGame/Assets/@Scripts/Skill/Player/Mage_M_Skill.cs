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


        CreatureController target = null;
        if (_target.IsDead) _target = null;

        if (_target != null)
        {
            target = _target;
        }
        else
        {
            target = Utils.FindRandomEnemyInRange(_caster, attack_Radius);
        }


        if (target != null)
        {
            //TODO : 데미지와, 이펙트 부분을 나눈것
            DamageEffect damageEffect = effects.Find(e => e is DamageEffect) as DamageEffect;
            List<ISkillEffect> buffEffects = effects.FindAll(e => e is BuffEffect);

            if (damageEffect != null)
            {
                damageEffect.Execute(_caster, target);
            }

            foreach (var effect in buffEffects)
            {
                effect.Execute(_caster, target, skill_Duration);
            }

            ShowEffect(target, skillData);
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
