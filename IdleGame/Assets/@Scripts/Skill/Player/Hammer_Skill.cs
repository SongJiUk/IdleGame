using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer_Skill : SkillBase
{
    public Hammer_Skill()
    {
    }

    public override bool UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        // //TODO : 하드코딩들 처리
        // SpinAttackBuff spinBuff = new SpinAttackBuff(_caster, 2);
        // _caster.buffController.AddBuff(spinBuff);

        List<CreatureController> enemiesInArea = Utils.FindEnemyInSphereArea(_caster, attack_Radius);
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
            Debug.Log("회전 공격 : 주변에 적 없음");
            return false;
        }
        //TODO : 쿨타임 처리 해야함
    }
}
