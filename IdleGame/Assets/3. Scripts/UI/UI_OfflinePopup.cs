using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_OfflinePopup : UI_Popup
{
    #region enum

    enum GameObjects
    {
        Content
    }

    enum Texts
    {
        OfflinePopupTimeText,
        OfflinePopupGoldText,

    }

    enum Buttons
    {
        GetButton,
        AdGetButton
    }
    #endregion

    Transform parent = null;
    double money;
    Dictionary<string, ItemHolder> itemsDic = new Dictionary<string, ItemHolder>();

    List<UI_Item> itemPool = new List<UI_Item>();
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        GameObjectsType = typeof(GameObjects);
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);

        BindObject(GameObjectsType);
        BindText(TextsType);
        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.GetButton).gameObject.BindEvent(OnClickGetButton);
        GetButton(ButtonsType, (int)Buttons.AdGetButton).gameObject.BindEvent(OnClickAdGetButton);

        parent = GetObject(GameObjectsType, (int)GameObjects.Content).transform;

        return true;
    }


    public void SetInfo()
    {
        money = Utils.CalculateOfflineMoney(Utils.TimerCheck());

        GetText(TextsType, (int)Texts.OfflinePopupGoldText).text = Utils.ToCurrencyString(money);

        TimeSpan span = TimeSpan.FromSeconds(Utils.TimerCheck());
        GetText(TextsType, (int)Texts.OfflinePopupTimeText).text = span.ToString(@"hh\:mm\:ss");

        GetItem();

        int index = 0;

        foreach(var item in itemsDic)
        {
            UI_Item ui_item;
            if(index < itemPool.Count)
            {
                ui_item = itemPool[index];
                ui_item.gameObject.SetActive(true);
            }
            else
            {
                ui_item = Managers.UIM.MakeSubItem<UI_Item>(parent);
                ui_item.Init().Forget();
                itemPool.Add(ui_item);
            }

            ui_item.SetInfo(item.Value);
            index++;
        }

        for(int i =index; i<itemPool.Count; i++)
        {
            itemPool[i].gameObject.SetActive(false);
        }
    }

    void GetItem()
    {
        double maxOfflineSeconds = 1200;
        double actualSeconds = Math.Min(Utils.TimerCheck(), maxOfflineSeconds);


        int totalDrops = (int)(actualSeconds / 3);
        var allDropItems = Managers.ItemM.GetBatchDropItems(totalDrops);


        foreach (var item in allDropItems)
        {
            if (itemsDic.ContainsKey(item.Name))
            {
                itemsDic[item.Name].holder.Count++;
            }
            else
            {
                var itemData = new ItemHolder();
                itemData.data = item;
                itemData.holder = new Holder();
                itemData.holder.Count = 1;
                itemsDic.Add(item.Name, itemData);
            }
        }
    }
    void OnClickGetButton()
    {
        Managers.SoundM.PlayButtonClick();
        Managers.GameM.gameData.gold += money;
        foreach (var item in itemsDic)
        {
            Managers.InventoryM.GetItem(item.Value.data, item.Value.holder.Count);
        }
        (Managers.UIM.SceneUI as UI_GameScene).OnRefreshGoods();
        Managers.UIM.ClosePopup(this).Forget();
    }

    void OnClickAdGetButton()
    {
        Managers.SoundM.PlayButtonClick();
        Action rewardedAction = () =>
        {
            Managers.GameM.gameData.gold += (money * 2);
            foreach (var item in itemsDic)
            {
                Managers.InventoryM.GetItem(item.Value.data, item.Value.holder.Count * 2);
            }
        };

        Managers.AdM.ShowRewardedAd(rewardedAction, null);

        (Managers.UIM.SceneUI as UI_GameScene).OnRefreshGoods();

        Managers.UIM.ClosePopup(this).Forget();
    }

    private void OnDestroy()
    {
        foreach(var item in itemPool)
        {
            if (item != null) Managers.ResourceM.Destroy(item.gameObject);
        }

        itemPool.Clear();

    }
}
