using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class SkillBase
{
    //해당 스킬이 발동할 효과들
    protected List<ISkillEffect> effects = new List<ISkillEffect>();

    public virtual void UseSkill(CreatureController _caster, CreatureController _target)
    {
        //사용 방법임, 해당 스킬에서 사용하면 됌
        //TODO : 쿨타임 / 사용 마나 처리

        foreach (var effect in effects)
        {
            effect.Excute(_caster, _target);
        }

        //TODO : 쿨타임 시작
    }

    //스킬 효과 설정
    protected abstract void SetUpEffect();

    public virtual void SetSkill(CreatureController _cc = null)
    {
        
    }


}
