using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemManager
{

    public bool ItemCheck(string _name)
    {
        for (int i = 0; i < Managers.GameM.gameData.Items.Length; i++)
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
        List<Data.ItemData> dropitems = new List<Data.ItemData>();
        int currentStage = Managers.GameM.Stage;

        float bounus = Managers.PlayerM.ItemDrop();

        foreach (var data in Managers.GameM.gameData.Item_Data)
        {
            var item = data.Value.data;

            if (data.Value.data.ItemType == Define.ItemType.Currency) continue;
            if (item.MinStage > currentStage) continue;

            float randValue = Random.Range(0, 100f);
            float finalProb = item.Probability + bounus;

            finalProb = Mathf.Clamp(finalProb, 0f, 100f);
            if (randValue <= finalProb)
            {
                dropitems.Add(data.Value.data);
            }
        }

        return dropitems;
    }
}
