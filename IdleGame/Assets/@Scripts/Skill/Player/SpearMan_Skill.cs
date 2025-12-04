using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearMan_Skill : SkillBase
{

    public SpearMan_Skill()
    {
        SetUpEffect();
    }

    protected override void SetUpEffect()
    {
        effects.Add(new DamageEffect(1.2));
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        List<CreatureController> enemiesInArea = Utils.FindEnemyForwardArea(_caster, attack_length, attack_width);

        if (enemiesInArea.Count > 0)
        {
            foreach (CreatureController enemy in enemiesInArea)
            {
                foreach (var effect in effects)
                {
                    effect.Execute(_caster, enemy);
                }
            }
            return true;
        }
        else
        {
            Debug.Log("[스피어맨 스킬] : 범위 내에 적이 없음");
            return false;
        }
    }


}
