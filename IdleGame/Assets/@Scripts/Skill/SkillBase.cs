using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkillBase : BaseController
{
    protected List<PlayerController> players;
    protected List<MonsterController> monsters;

    public override bool Init()
    {
        if (!base.Init()) return false;
        return true;
    }

    public virtual void SetSkill(CreatureController _cc = null)
    {
        players = Managers.ObjectM.pcSet.ToList();
        monsters = Managers.ObjectM.mcSet.ToList();
    }
}
