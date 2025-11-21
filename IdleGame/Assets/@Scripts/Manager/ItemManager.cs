using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager
{
    Dictionary<string, Data.ItemData> ItemDatas = new Dictionary<string, Data.ItemData>();

    
    public void Init()
    { 
        foreach(var data in Managers.DataM.ItemDataDic)
        {
            ItemDatas.Add(data.Value.Name, data.Value);
        }
    }

    public List<Data.ItemData> GetDropItem()
    {
        List<Data.ItemData> items = new List<Data.ItemData>();
        foreach (var data in ItemDatas)
        {
            float randValue = Random.Range(0, 100);
            if(randValue <= data.Value.Probability)
            {
                items.Add(data.Value);
            }
        }

        return items;
    }
}
