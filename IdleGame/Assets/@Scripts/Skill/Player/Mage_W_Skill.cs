using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage_W_Skill : SkillBase
{
    BuffEffect attackBuffEffect;
    BuffEffect defenseBuffEffect;
    public Mage_W_Skill()
    {
        SetUpEffect();
    }

    protected override void SetUpEffect()
    {
        //TODO : 스킬 정보 가져와서 여기에 넣기
        //타겟에게 10초동안 attackBuff적용 효과
        attackBuffEffect = new BuffEffect(duration => new AttackBuff(10f), 10f);
        defenseBuffEffect = new BuffEffect(duration => new DefenseBuff(10f), 10f);

    }

    public override void UseSkill(CreatureController _caster, CreatureController _target = null)
    {
        CreatureController randPlayer = Utils.FindRandomPlayer(_caster); ;

        if (randPlayer != null)
        {
            BuffEffect chosenEffect = (Random.Range(0, 2) == 0) ? attackBuffEffect : defenseBuffEffect;

            chosenEffect.Excute(_caster, randPlayer);
        }

        //TODO : 쿨타임 처리(메인 캐릭터일떄)
    }
}
