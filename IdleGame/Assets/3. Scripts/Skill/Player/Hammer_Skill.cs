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


        LoopSkill(_caster).Forget();
        ResetSkillStateAsync(_caster, skill_Duration).Forget();
        return true;
        //TODO : 쿨타임 처리 해야함
    }
}
