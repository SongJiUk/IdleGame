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

    public bool UseItem(string _itemName, int _count = 1)
    {
        if (Managers.GameM.gameData.Item_Data.TryGetValue(_itemName, out var itemHolder))
        {
            if (itemHolder.holder.Count >= _count)
            {
                itemHolder.holder.Count -= _count;
                return true;
            }
        }

        return false;
    }

    public int GetItemCount(string _itemName)
    {
        if (Managers.GameM.gameData.Item_Data.TryGetValue(_itemName, out var itemHolder))
        {
            return itemHolder.holder.Count;
        }
        return 0;
    }
}
