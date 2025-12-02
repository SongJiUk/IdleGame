using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage_W_Skill : SkillBase
{
    BuffEffect attackBuffEffect;
    BuffEffect defenceBuffEffect;
    public Mage_W_Skill()
    {
        SetUpEffect();
    }

    protected override void SetUpEffect()
    {
        //TODO : 스킬 정보 가져와서 여기에 넣기
        //타겟에게 10초동안 attackBuff적용 효과
        attackBuffEffect = new BuffEffect(duration => new AttackBuff(10f), 10f);
        defenceBuffEffect = new BuffEffect(duration => new DefenceBuff(10f), 10f);

    }

    public override void UseSkill(CreatureController _caster, CreatureController _target)
    {
        CreatureController lowHpTeam = _target;

        if(lowHpTeam != null)
        {
            BuffEffect chosenEffect = (Random.Range(0, 2) == 0) ? attackBuffEffect : defenceBuffEffect;

            chosenEffect.Excute(_caster, lowHpTeam);
        }
    }
}
