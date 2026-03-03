using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using Cysharp.Threading.Tasks;
public class UI_Inventory : UI_Base
{
    List<UI_Item> itemPool = new List<UI_Item>();
    enum GameObjects
    {
        BarObject,
        Content,
    }

    enum Buttons
    {
        AllButton,
        EquipmentButton,
        ConsumableButton,
        OthersButton,
        CloseButton,
    }

    enum Texts
    {
        AllText,
        EquipmentText,
        ConsumableText,
        OthersText,
        CoinText,

    }

    Transform parent = null;
    RectTransform rect = null;

    RectTransform targetRect;
    RectTransform allRect;
    RectTransform equipmentRect;
    RectTransform consumableRect;
    RectTransform othersRect;
    
    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        GameObjectsType = typeof(GameObjects);
        ButtonsType = typeof(Buttons);
        TextsType = typeof(Texts);

        BindObject(GameObjectsType);
        BindButton(ButtonsType);
        BindText(TextsType);


        GetButton(ButtonsType, (int)Buttons.AllButton).gameObject.BindEvent(OnClickAllButton);
        GetButton(ButtonsType, (int)Buttons.EquipmentButton).gameObject.BindEvent(OnClickEquipmentButton);
        GetButton(ButtonsType, (int)Buttons.ConsumableButton).gameObject.BindEvent(OnClickConsumableButton);
        GetButton(ButtonsType, (int)Buttons.OthersButton).gameObject.BindEvent(OnClickOthersButton);
        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        targetRect = GetObject(GameObjectsType, (int)GameObjects.BarObject).GetComponent<RectTransform>();

        allRect = GetButton(ButtonsType, (int)Buttons.AllButton).GetComponent<RectTransform>();
        equipmentRect = GetButton(ButtonsType, (int)Buttons.EquipmentButton).GetComponent<RectTransform>();
        consumableRect = GetButton(ButtonsType, (int)Buttons.ConsumableButton).GetComponent<RectTransform>();
        othersRect = GetButton(ButtonsType, (int)Buttons.OthersButton).GetComponent<RectTransform>();

        parent = GetObject(GameObjectsType, (int)GameObjects.Content).transform;


        rect = gameObject.GetComponent<RectTransform>();
        return true;
    }

    public override void SetInfo()
    {
        MoveTarget(allRect, false);
        RefreshItems();
    }

    public void RefreshItems()
    {
        //TODO : 켜져있는 동안에도 바꿔줄지? => 이벤트 사용해야됌 
        int needCount = Managers.GameM.gameData.Item_Data.Count;

        while (itemPool.Count < needCount)
        {
            UI_Item i = Managers.UIM.MakeSubItem<UI_Item>(parent);
            itemPool.Add(i);
        }

        foreach (var slot in itemPool)
            slot.gameObject.SetActive(false);

        int index = 0;
        //TODO : 수정
        var sort_items = Managers.GameM.gameData.Item_Data.OrderByDescending(x => x.Value.data.ItemGrade);
        foreach (var item in sort_items)
        {
            if (Managers.GameM.gameData.Item_Holder[item.Key].Count > 0)
            {
                UI_Item slot = itemPool[index++];
                slot.gameObject.SetActive(true);

                slot.Init().Forget();
                slot.SetInfo(item.Value, rect);
            }
        }

        GetText(TextsType, (int)Texts.CoinText).text = Utils.ToCurrencyString(Managers.GameM.Gold);
    }
    void OnClickAllButton()
    {
        //Transform targetTr = GetObject(GameObjectsType, (int)GameObjects.BarObject).transform;
        //Vector3 endPos = GetButton(ButtonsType, (int)Buttons.AllButton).transform.position;

        //targetTr.DOMove(endPos, 0.5f)
        //    .SetEase(Ease.OutQuad);

        MoveTarget(allRect, true);

        RefreshItems();


    }

    void OnClickEquipmentButton()
    {
        //Transform targetTr = GetObject(GameObjectsType, (int)GameObjects.BarObject).transform;
        //Vector3 endPos = GetButton(ButtonsType, (int)Buttons.EquipmentButton).transform.position;

        //targetTr.DOMove(endPos, 0.5f)
        //    .SetEase(Ease.OutQuad);

        MoveTarget(equipmentRect, true);

        foreach (var slot in itemPool)
            slot.gameObject.SetActive(false);

        int index = 0;
        var sort_items = Managers.GameM.gameData.Item_Data.OrderByDescending(x => x.Value.data.ItemGrade);
        foreach (var item in sort_items)
        {
            if (Managers.GameM.gameData.Item_Holder[item.Key].Count > 0)
            {
                if (item.Value.data.ItemType == Define.ItemType.Equipment)
                {
                    UI_Item slot = itemPool[index++];
                    slot.gameObject.SetActive(true);

                    slot.Init().Forget();
                    slot.SetInfo(item.Value, rect);
                }
            }
        }
    }

    void OnClickConsumableButton()
    {
        //Transform targetTr = GetObject(GameObjectsType, (int)GameObjects.BarObject).transform;
        //Vector3 endPos = GetButton(ButtonsType, (int)Buttons.ConsumableButton).transform.position;

        //targetTr.DOMove(endPos, 0.5f)
        //    .SetEase(Ease.OutQuad);
        MoveTarget(consumableRect, true);

        foreach (var slot in itemPool)
            slot.gameObject.SetActive(false);

        int index = 0;
        var sort_items = Managers.GameM.gameData.Item_Data.OrderByDescending(x => x.Value.data.ItemGrade);
        foreach (var item in sort_items)
        {
            if (Managers.GameM.gameData.Item_Holder[item.Key].Count > 0)
            {
                if (item.Value.data.ItemType == Define.ItemType.Consumable)
                {
                    UI_Item slot = itemPool[index++];
                    slot.gameObject.SetActive(true);

                    slot.Init().Forget();
                    slot.SetInfo(item.Value, rect);
                }
            }
        }

    }

    void OnClickOthersButton()
    {
        //Transform targetTr = GetObject(GameObjectsType, (int)GameObjects.BarObject).transform;
        //Vector3 endPos = GetButton(ButtonsType, (int)Buttons.OthersButton).transform.position;

        //targetTr.DOMove(endPos, 0.5f)
        //    .SetEase(Ease.OutQuad);

        MoveTarget(othersRect, true);

        foreach (var slot in itemPool)
            slot.gameObject.SetActive(false);

        int index = 0;
        var sort_items = Managers.GameM.gameData.Item_Data.OrderByDescending(x => x.Value.data.ItemGrade);
        foreach (var item in sort_items)
        {
            if (Managers.GameM.gameData.Item_Holder[item.Key].Count > 0)
            {
                if (item.Value.data.ItemType == Define.ItemType.Other)
                {
                    UI_Item slot = itemPool[index++];
                    slot.gameObject.SetActive(true);

                    slot.Init().Forget();
                    slot.SetInfo(item.Value, rect);
                }
            }
        }
    }

    void OnClickCloseButton()
    {
        Managers.UIM.ClosePopup();
        DOTween.Kill(this);
    }

    void MoveTarget(RectTransform _target, bool _animate = true)
    {
        Vector3 worldPos = targetRect.position;

        targetRect.SetParent(_target, false);
        targetRect.SetAsFirstSibling();

        targetRect.anchorMin = new Vector2(0f, 0f);
        targetRect.anchorMax = new Vector2(1f, 1f);
        targetRect.pivot = new Vector2(0.5f, 0.5f);
        targetRect.position = worldPos;

        if (_animate)
        {
            targetRect.localScale = Vector3.one * 0.9f;
            targetRect.DOScale(1f, 0.15f).SetEase(Ease.OutBack);
            targetRect.DOAnchorPos(Vector2.zero, 0.25f).SetEase(Ease.OutQuad);
        }
        else targetRect.anchoredPosition = Vector2.zero;
    }

}
