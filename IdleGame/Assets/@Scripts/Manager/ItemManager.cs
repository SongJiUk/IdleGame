using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemManager
{

    public bool ItemCheck(string _name)
    {
        for(int i =0; i<Managers.GameM.gameData.Items.Length; i++)
        {
            if (Managers.GameM.gameData.Items[i].Name == _name) return true;
        }

        return false;
    }

    public void SetItem(int _value, string _name)
    {
        Managers.GameM.gameData.Items[_value] = Managers.GameM.gameData.Item_Data[_name].data;
    }

    public void RemoveItem(string _name)
    {
        for (int i = 0; i < Managers.GameM.gameData.Items.Length; i++)
        {
            if (Managers.GameM.gameData.Items[i] == null) continue;

            if (Managers.GameM.gameData.Items[i].Name == _name) Managers.GameM.gameData.Items[i] = null;
        }
    }

    public void DisableItem(int _value)
    {
        Managers.GameM.gameData.Items[_value] = null;
    }


    public List<Data.ItemData> GetDropItem()
    {
        List<Data.ItemData> items = new List<Data.ItemData>();
        foreach (var data in Managers.GameM.gameData.Item_Data)
        {
            if(data.Value.data.MinStage <= Managers.GameM.Stage)
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
