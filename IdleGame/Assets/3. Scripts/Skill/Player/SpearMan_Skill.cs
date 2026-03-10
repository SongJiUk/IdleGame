using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class SpearMan_Skill : SkillBase
{
    public SpearMan_Skill()
    {
    }

    public override async UniTask<bool> UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        InitSkillData(_caster);

        List<CreatureController> enemiesInArea = Utils.FindEnemyForwardArea(_caster, skill_Length, skill_Width);


        if (enemiesInArea.Count > 0)
        {
            foreach (CreatureController enemy in enemiesInArea)
            {
                SetDamage(_caster, enemy);
            }

            ResetSkillStateAsync(_caster, anim_Duration).Forget();
            Managers.SoundM.Play(Define.Sound.Effect, "SpearManSkill");
            return true;
        }
        else
        {
            //Debug.Log("[스피어맨 스킬] : 범위 내에 적이 없음");
            return false;
        }
    }


}
