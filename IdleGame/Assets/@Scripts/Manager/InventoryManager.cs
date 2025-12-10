using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager
{
    public Dictionary<string, Item> items = new Dictionary<string, Item>();


    public void GetItem(Data.ItemData _item)
    {
        if (items.ContainsKey(_item.Name))
        {
            items[_item.Name].count++;
        }
        else
            items.Add(_item.Name, new Item { itemData = _item, count = 1 });
    }
}
