using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
public class UI_Inventory : UI_Base
{
    enum GameObjects
    {
        BarObject
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

    public override bool Init()
    {
        if (!base.Init()) return false;
        gameObjectsType = typeof(GameObjects);
        ButtonsType = typeof(Buttons);
        TextsType = typeof(Texts);

        BindObject(gameObjectsType);
        BindButton(ButtonsType);
        BindText(TextsType);


        GetButton(ButtonsType, (int)Buttons.AllButton).gameObject.BindEvent(OnClickAllButton);
        GetButton(ButtonsType, (int)Buttons.EquipmentButton).gameObject.BindEvent(OnClickEquipmentButton);
        GetButton(ButtonsType, (int)Buttons.ConsumableButton).gameObject.BindEvent(OnClickConsumableButton);
        GetButton(ButtonsType, (int)Buttons.OthersButton).gameObject.BindEvent(OnClickOthersButton);
        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        return true;
    }

    public void SetInfo()
    {
        //높은 등급 먼저 Linq
        //ex) Managers.ResourceM.ResourcDic.OrderByDescending(x => x.Value.name);
    }

    void OnClickAllButton()
    {
        //GetObject(gameObjectsType, (int)GameObjects.BarObject).transform.position = GetButton(ButtonsType, (int)Buttons.AllButton).transform.position;
        Transform targetTr = GetObject(gameObjectsType, (int)GameObjects.BarObject).transform;
        Vector3 endPos = GetButton(ButtonsType, (int)Buttons.AllButton).transform.position;

        targetTr.DOMove(endPos, 0.5f)
            .SetEase(Ease.OutQuad);

        // TODO : 여기에 정렬 코드 써주면될듯. 아이템에 특성에따라서
    }

    void OnClickEquipmentButton()
    {
        //GetObject(gameObjectsType, (int)GameObjects.BarObject).transform.position = GetButton(ButtonsType, (int)Buttons.EquipmentButton).transform.position;
        Transform targetTr = GetObject(gameObjectsType, (int)GameObjects.BarObject).transform;
        Vector3 endPos = GetButton(ButtonsType, (int)Buttons.EquipmentButton).transform.position;

        targetTr.DOMove(endPos, 0.5f)
            .SetEase(Ease.OutQuad);
    }

    void OnClickConsumableButton()
    {
        //GetObject(gameObjectsType, (int)GameObjects.BarObject).transform.position = GetButton(ButtonsType, (int)Buttons.ConsumableButton).transform.position;
        Transform targetTr = GetObject(gameObjectsType, (int)GameObjects.BarObject).transform;
        Vector3 endPos = GetButton(ButtonsType, (int)Buttons.ConsumableButton).transform.position;

        targetTr.DOMove(endPos, 0.5f)
            .SetEase(Ease.OutQuad);
    }

    void OnClickOthersButton()
    {
        //GetObject(gameObjectsType, (int)GameObjects.BarObject).transform.position = GetButton(ButtonsType, (int)Buttons.OthersButton).transform.position;
        Transform targetTr = GetObject(gameObjectsType, (int)GameObjects.BarObject).transform;
        Vector3 endPos = GetButton(ButtonsType, (int)Buttons.OthersButton).transform.position;

        targetTr.DOMove(endPos, 0.5f)
            .SetEase(Ease.OutQuad);
    }

    void OnClickCloseButton()
    {
        Debug.Log("Close Button");
        DOTween.Kill(this);
    }

}
