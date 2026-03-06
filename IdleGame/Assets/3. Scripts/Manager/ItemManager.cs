using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemManager
{

    public bool ItemCheck(string _name)
    {
        for (int i = 0; i < Managers.GameM.gameData.EquippedRelics.Length; i++)
        {
            if (Managers.GameM.gameData.EquippedRelics[i] == null) continue;

            if (Managers.GameM.gameData.EquippedRelics[i].Name == _name) return true;
        }

        return false;
    }

    public void SetItem(int _value, string _name)
    {
        Managers.GameM.gameData.EquippedRelics[_value] = Managers.GameM.gameData.Item_Data[_name].data;
    }

    public void RemoveItem(string _name)
    {
        for (int i = 0; i < Managers.GameM.gameData.EquippedRelics.Length; i++)
        {
            if (Managers.GameM.gameData.EquippedRelics[i] == null) continue;

            if (Managers.GameM.gameData.EquippedRelics[i].Name == _name) Managers.GameM.gameData.EquippedRelics[i] = null;
        }

        Managers.RelicM.Init();
    }
    public ItemHolder GetItemData(string _name)
    {
        Managers.GameM.gameData.Item_Data.TryGetValue(_name, out var itemHolder);

        // if (ItemCheck(_name)) return itemHolder;
        // else return null;
        if (itemHolder != null) return itemHolder;
        else return null;

    }


    public void DisableItem(int _value)
    {
        Managers.GameM.gameData.EquippedRelics[_value] = null;
    }


    public List<Data.ItemData> GetDropItem()
    {
        List<Data.ItemData> dropItems = new List<Data.ItemData>();
        var candidates = GetCandidateItems();

        float bonus = Managers.PlayerM.ItemDrop();

        foreach (var item in candidates)
        {

            float randValue = Random.Range(0, 100f);
            float finalProb = item.Probability + bonus;

            if (randValue <= Mathf.Clamp(finalProb, 0f, 100f))
            {
                dropItems.Add(item);
            }
        }

        return dropItems;
    }

    public List<Data.ItemData> GetBatchDropItems(int _dropCount)
    {
        List<Data.ItemData> allRewards = new List<Data.ItemData>();

        var candidates = GetCandidateItems();
        float bonus = Managers.PlayerM.ItemDrop();

        for (int i = 0; i < _dropCount; i++)
        {
            foreach (var item in candidates)
            {
                float randValue = Random.Range(0, 100f);
                float finalProb = item.Probability + bonus;

                if (randValue <= Mathf.Clamp(finalProb, 0f, 100f))
                {
                    allRewards.Add(item);
                }
            }
        }

        return allRewards;
    }

    public List<Data.ItemData> GetCandidateItems()
    {
        List<Data.ItemData> candidates = new List<Data.ItemData>();

        int currentStage = Managers.GameM.Stage;

        foreach (var entry in Managers.GameM.gameData.Item_Data)
        {
            var item = entry.Value.data;
            if (item.ItemType == Define.ItemType.Currency) continue;
            if (item.MinStage > currentStage) continue;

            candidates.Add(item);
        }

        return candidates;
    }
}
