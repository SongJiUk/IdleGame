using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class RelicManager
{
    public List<ItemHolder> EquippedRelics = new List<ItemHolder>();
    public Action OnChangeUI;

    public void Init()
    {
        UpdateEquippedRelics();
        RegisterMonsterDeadEvents();
        RegisterPlayerAttackEvents();
        RegisterPlayerHitEvents();

        CheckEquipContinueRelic();
    }


    public void UpdateEquippedRelics()
    {
        EquippedRelics.Clear();

        for (int i = 0; i < Managers.GameM.gameData.EquippedRelics.Length; i++)
        {
            if (Managers.ItemM.ItemCheck((Define.RelicName.Gold_Dice + i).ToString()))
            {
                var itemData = Managers.ItemM.GetItemData((Define.RelicName.Gold_Dice + i).ToString());

                if (itemData != null)
                    EquippedRelics.Add(itemData);
            }
        }

    }

    public float GetPassiveBonus(string _name)
    {
        float total = 0f;

        foreach (var relic in EquippedRelics)
        {
            if (relic.data.Name == _name)
            {
                total += relic.GetCurrentValues()[0];
                break;
            }
        }

        return total;
    }
    void RegisterMonsterDeadEvents()
    {
        if (Managers.ItemM.ItemCheck(Define.RelicName.Gold_Dice.ToString()))
        {
            DelegateHolder.MonsterDeadEvent -= GoldDice;
            DelegateHolder.MonsterDeadEvent += GoldDice;
        }
    }

    void RegisterPlayerAttackEvents()
    {
        if (Managers.ItemM.ItemCheck(Define.RelicName.Axe.ToString()))
        {
            DelegateHolder.PlayerAttackEvent -= Axe;
            DelegateHolder.PlayerAttackEvent += Axe;
        }
    }

    void RegisterPlayerHitEvents()
    {
        if (Managers.ItemM.ItemCheck(Define.RelicName.GoddessTears.ToString()))
        {
            DelegateHolder.PlayerHitEvent -= GoddessTears;
            DelegateHolder.PlayerHitEvent += GoddessTears;
        }
    }

    void CheckEquipContinueRelic()
    {
        if (Managers.ItemM.ItemCheck(Define.RelicName.GoldenRing.ToString()))
        {
            OnChangeUI?.Invoke();
        }
    }

    #region  Relic Logic
    public void GoldDice(MonsterController _mc)
    {
        Managers.GameM.gameData.Item_Data.TryGetValue(Define.RelicName.Gold_Dice.ToString(), out var itemData);
        var values = itemData.GetCurrentValues();

        if (!RandomNum(values[0])) return;

        Vector3 pos = _mc.transform.position;
        GameObject go = Managers.ResourceM.Instantiate(Define.RelicName.Gold_Dice.ToString(), _pooling: true);
        go.transform.position = pos;

        var dice = go.GetComponent<Gold_Dice>();

        double baseGold = Utils.Money() * Managers.PlayerM.GoldDrop();
        dice.SetBaseGold(baseGold);
    }

    public void Axe(PlayerController _pc, CreatureController _mc)
    {
        Managers.GameM.gameData.Item_Data.TryGetValue(Define.RelicName.Axe.ToString(), out var itemData);
        var values = itemData.GetCurrentValues();

        float splshRate = values[0] / 100f;

        Vector3 pos = _mc.transform.position;
        GameObject go = Managers.ResourceM.Instantiate(Define.RelicName.Axe.ToString(), _pooling: true);
        go.transform.position = pos;

        var monsters = Utils.FindEnemyInSphereArea(_mc, 1.0f);
        foreach (var monster in monsters)
        {
            if (monster == _mc) continue;

            monster.GetDamage(_pc.Damage * splshRate, _pc);
        }
    }

    public void GoddessTears(PlayerController _pc)
    {
        Managers.GameM.gameData.Item_Data.TryGetValue(Define.RelicName.GoddessTears.ToString(), out var itemData);
        var values = itemData.GetCurrentValues();
        if (!RandomNum(values[0])) return;

        _pc.GetMp((int)values[1]);
    }

    public void SilverRing(MonsterController _pc)
    {

    }

    public void GoldenRing(MonsterController _pc)
    {

    }

    #endregion
    bool RandomNum(float _num)
    {
        float randNum = UnityEngine.Random.Range(0.0f, 100.0f);
        if (randNum <= _num)
        {
            return true;
        }
        return false;
    }
}
