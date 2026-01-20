using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RelicManager
{
    
    public void Init()
    {
        if (Managers.ItemM.ItemCheck("Gold_Dice"))
        {
            DelegateHolder.MonsterDeadEvent -= GoldDice;
            DelegateHolder.MonsterDeadEvent += GoldDice;
        }

        if(Managers.ItemM.ItemCheck("Axe"))
        {
            DelegateHolder.PlayerAttackEvent -= Axe;
            DelegateHolder.PlayerAttackEvent += Axe;
        }

        if(Managers.ItemM.ItemCheck("GoddessTears"))
        {
            DelegateHolder.PlayerHitEvent -= GoddessTears;
            DelegateHolder.PlayerHitEvent += GoddessTears;

        }
    }
    public void GoldDice(MonsterController _mc)
    {
        if (!RandomNum(30)) return;

        Vector3 pos = _mc.transform.position;
        GameObject go = Managers.ResourceM.Instantiate("Gold_Dice", _pooling: true);
        go.transform.position = pos;
    }

    public void Axe(PlayerController _pc, CreatureController _mc)
    {
        Vector3 pos = _mc.transform.position;
        GameObject go = Managers.ResourceM.Instantiate("Axe", _pooling: true);
        go.transform.position = pos;

        var monsters = Utils.FindEnemyInSphereArea(_mc, 1.0f);
        foreach(var monster in monsters)
        {
            if (monster == _mc) continue;

            monster.GetDamage(_pc.Damage * 0.3f, _pc);
        }
    }

    public void GoddessTears(PlayerController _pc)
    {
        if (!RandomNum(15)) return;

        _pc.GetMp(3);
    }

    bool RandomNum(float _num)
    {
        float randNum = Random.Range(0.0f, 100.0f);
        if(randNum <= _num)
        {
            return true;
        }
        return false;
    }
}
