using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemManager
{
    public List<Data.ItemData> GetDropItem()
    {
        List<Data.ItemData> items = new List<Data.ItemData>();
        foreach (var data in Managers.GameM.gameData.Item_Data)
        {
            if (data.Value.data.ItemType == Define.ItemType.Consumable)
            {
                float randValue = Random.Range(0, 100);
                if (randValue <= data.Value.data.Probability)
                {
                    items.Add(data.Value.data);
                }
            }
        }

        return items;
    }
}
