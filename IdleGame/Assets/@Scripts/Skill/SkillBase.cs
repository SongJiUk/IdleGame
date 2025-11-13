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
        //TODO : 이거 플레이어는 스폰매니저에서 하는게 나을거같음, 오브젝트 매니저로하면 애니메이션 안나오고 바로 사라져버림(캐릭터는 상관없을거같긴함)
        players = Managers.SpawnM.players;
        monsters = Managers.ObjectM.mcList.ToList();
    }

}
