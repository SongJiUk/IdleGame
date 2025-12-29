using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager
{ 
    public void GetItem(Data.ItemData _item, int _count = 1)
    {
        if (Managers.GameM.gameData.Item_Data.ContainsKey(_item.Name))
        {
            Managers.GameM.gameData.Item_Data[_item.Name].holder.Count += _count;
        }
    }
}
