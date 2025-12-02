using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_Skill : SkillBase
{
    public Archer_Skill()
    {
        SetUpEffect();
    }

    protected override void SetUpEffect()
    {
        effects.Add(new BuffEffect(_duration => new StatDownBuff(_duration), 10f));
        effects.Add(new BuffEffect(_duration => new DotBuff(_duration), 5f));
    }

    public override void UseSkill(CreatureController _caster, CreatureController _target)
    {
        //TODO : 랜덤 적 하나에게 쏘기
        CreatureController randTarget= _target;

        if(randTarget != null)
        {
            foreach (var effect in effects)
            {
                effect.Excute(_caster, randTarget);
            }
        }
        else
        {
            Debug.Log("디버프 화살 : 유효한 타겟 없음");
        }


        //TODO : 쿨타임 처리

    }
}
