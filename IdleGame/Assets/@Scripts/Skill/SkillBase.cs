using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class SkillBase
{
    protected float attack_Radius = 0f;
    protected float attack_length = 0f;
    protected float attack_width = 0f;
    protected List<ISkillEffect> effects = new();

    public virtual bool UseSkill(CreatureController _caster, CreatureController _target) { return false; }

    protected abstract void SetUpEffect();

    public virtual void SetSkill(CreatureController _cc = null) { }
}
