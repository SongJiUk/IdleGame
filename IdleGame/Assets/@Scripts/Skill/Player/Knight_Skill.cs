using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Knight_Skill : SkillBase
{
    public Knight_Skill()
    {
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        InitSkillData(_caster);
        List<CreatureController> enemiesInArea = Utils.FindEnemyForwardArea(_caster, skill_Length, skill_Width);

        if (enemiesInArea.Count > 0)
        {
            foreach (CreatureController enemy in enemiesInArea)
            {
                SetDamage(_caster, enemy, anim_Duration - 1f);
            }


            ResetSkillStateAsync(_caster, anim_Duration).Forget();
            return true;
        }
        else
        {
            Debug.Log("[양손검 스킬] : 범위 내에 적이 없음");
            return false;
        }
    }

}
