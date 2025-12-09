using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class Hammer_Skill : SkillBase
{
    public Hammer_Skill()
    {
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        InitSkillData(_caster);

        List<CreatureController> enemiesInArea = Utils.FindEnemyInSphereArea(_caster, skill_Radius);
        if (enemiesInArea.Count > 0)
        {
            LoopSkill(_caster).Forget();
            ResetSkillStateAsync(_caster, skill_Duration).Forget();
            return true;
        }
        else
        {
            Debug.Log("회전 공격 : 주변에 적 없음");
            return false;
        }
        //TODO : 쿨타임 처리 해야함
    }
}
