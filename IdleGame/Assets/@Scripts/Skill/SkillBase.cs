using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class SkillBase : BaseController
{
    protected List<PlayerController> players;
    protected List<MonsterController> monsters;

    protected List<ISkillEffect> effects = new List<ISkillEffect>();

    public void UseSkill(CreatureController _caster, CreatureController _target)
    {
        //TODO : 쿨타임 / 사용 마나 처리

        foreach (var effect in effects)
        {
            effect.Excute(_caster, _target);
        }

        //TODO : 쿨타임 시작
    }

    protected abstract void SetUpEffect();

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
