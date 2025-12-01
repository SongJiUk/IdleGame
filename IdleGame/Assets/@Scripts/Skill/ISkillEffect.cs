using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillEffect
{
    void Excute(CreatureController _caster, CreatureController _target);
}
