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

        money = (Utils.Money() * Utils.TimerCheck()) / 3;
        GetText(TextsType, (int)Texts.OfflinePopupGoldText).text = Utils.ToCurrencyString(money);

        TimeSpan span = TimeSpan.FromSeconds(Utils.TimerCheck());
        GetText(TextsType, (int)Texts.OfflinePopupTimeText).text = span.Hours + " : " + span.Minutes;

        GetItem();

        foreach(var item in itemsDic)
        {
            UI_Item ui_item =  Managers.UIM.MakeSubItem<UI_Item>(parent);
            ui_item.Init().Forget();
            ui_item.SetInfo(item.Value);
        }

        return true;
    }

    void GetItem()
    {
        int value = (int)Utils.TimerCheck() / 3;
        for (int i = 0; i < value; i++)
        {
            var item = Managers.ItemM.GetDropItem();

            for (int j = 0; j < item.Count; j++)
            {
                if (itemsDic.ContainsKey(item[j].Name))
                {
                    itemsDic[item[j].Name].holder.Count++;
                }
                else
                {
                    var itemData = new ItemHolder();
                    itemData.data = item[j];
                    itemData.holder = new Holder();
                    itemData.holder.Count = 1;

                    itemsDic.Add(item[j].Name, itemData);
                }
            }
        }
    }
    void OnClickGetButton()
    {
        //TODO :Managers.InventoryM.items
        Managers.GameM.gameData.gold += money;
        foreach(var item in itemsDic)
        {
            Managers.InventoryM.GetItem(item.Value.data, item.Value.holder.Count);
        }
        (Managers.UIM.SceneUI as UI_GameScene).OnRefreshGoods();
        Managers.UIM.ClosePopup(this).Forget();
    }

    void OnClickAdGetButton()
    {
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
}
