using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assassin_Skill : SkillBase
{
    public Assassin_Skill()
    {
        SetUpEffect();
    }
    protected override void SetUpEffect()
    {
        effects.Add(new DamageEffect(3f));
    }

    public override void UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        //TODO : 가장 가까이 있는 적을 강하게 공격(300%)
        CreatureController target = Utils.FindNearEnemy(_caster);

        if (target != null)
        {
            foreach (var effect in effects)
            {
                effect.Excute(_caster, target);
            }
        }
        else
        {
            Debug.Log("[어쌔신 스킬] : 유효한 적이 없음]");
        }
    }

}
