using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
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
    }

    Transform parent = null;
    RectTransform rect = null;
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

        parent = GetObject(GameObjectsType, (int)GameObjects.Content).transform;
        rect = gameObject.GetComponent<RectTransform>();
        return true;
    }

    public override void SetInfo()
    {
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
    }
    void OnClickAllButton()
    {
        //GetObject(GameObjectsType, (int)GameObjects.BarObject).transform.position = GetButton(ButtonsType, (int)Buttons.AllButton).transform.position;
        Transform targetTr = GetObject(GameObjectsType, (int)GameObjects.BarObject).transform;
        Vector3 endPos = GetButton(ButtonsType, (int)Buttons.AllButton).transform.position;

        targetTr.DOMove(endPos, 0.5f)
            .SetEase(Ease.OutQuad);

        
    }

    void OnClickEquipmentButton()
    {
        //GetObject(GameObjectsType, (int)GameObjects.BarObject).transform.position = GetButton(ButtonsType, (int)Buttons.EquipmentButton).transform.position;
        Transform targetTr = GetObject(GameObjectsType, (int)GameObjects.BarObject).transform;
        Vector3 endPos = GetButton(ButtonsType, (int)Buttons.EquipmentButton).transform.position;

        targetTr.DOMove(endPos, 0.5f)
            .SetEase(Ease.OutQuad);
    }

    void OnClickConsumableButton()
    {
        //GetObject(GameObjectsType, (int)GameObjects.BarObject).transform.position = GetButton(ButtonsType, (int)Buttons.ConsumableButton).transform.position;
        Transform targetTr = GetObject(GameObjectsType, (int)GameObjects.BarObject).transform;
        Vector3 endPos = GetButton(ButtonsType, (int)Buttons.ConsumableButton).transform.position;

        targetTr.DOMove(endPos, 0.5f)
            .SetEase(Ease.OutQuad);
    }

    void OnClickOthersButton()
    {
        //GetObject(GameObjectsType, (int)GameObjects.BarObject).transform.position = GetButton(ButtonsType, (int)Buttons.OthersButton).transform.position;
        Transform targetTr = GetObject(GameObjectsType, (int)GameObjects.BarObject).transform;
        Vector3 endPos = GetButton(ButtonsType, (int)Buttons.OthersButton).transform.position;

        targetTr.DOMove(endPos, 0.5f)
            .SetEase(Ease.OutQuad);
    }

    void OnClickCloseButton()
    {
        Managers.UIM.ClosePopup();
        DOTween.Kill(this);
    }

}
