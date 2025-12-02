using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillEffect
{
    //스킬 발동시 호출 : 시전자, 타겟 정보를 받아 효과 실행
    void Excute(CreatureController _caster, CreatureController _target);
}
