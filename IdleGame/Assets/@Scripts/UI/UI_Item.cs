using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Item : UI_Base
{

    enum Images
    {
        ItemBGImage,
        ItemIconImage,


    }
    enum Buttons
    {

    }


    enum Texts
    {
        ItemCountText
    }
    Item item;
    RectTransform parent;
    UI_ItemInfo itemInfo;
    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        ImagesType = typeof(Images);
        TextsType = typeof(Texts);

        BindImage(ImagesType);
        BindText(TextsType);

        gameObject.BindEvent(OnClickItem);
        gameObject.BindEvent(_dragAction: OnPointerExitItem, _type: Define.UIEvent.OnPointerExit);
        return true;
    }

    public void SetInfo(Item _item, RectTransform _parent = null)
    {
        item = _item;
        parent = _parent;
        GetImage(ImagesType, (int)Images.ItemBGImage).sprite = Managers.ResourceM.GetAtlas(_item.itemData.ItemGrade.ToString());
        GetImage(ImagesType, (int)Images.ItemIconImage).sprite = Managers.ResourceM.GetAtlas(_item.itemData.Name);
        GetText(TextsType, (int)Texts.ItemCountText).text = _item.count.ToString();
    }


    void OnClickItem()
    {
        if (item == null) return;
        Vector2 scrennPos = Input.mousePosition;
        if(parent != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, scrennPos, null, out Vector2 localPos);
            itemInfo = Managers.UIM.MakeSubItem<UI_ItemInfo>(parent);
            itemInfo.Init().Forget();
            itemInfo.SetInfo(item, localPos, parent);
        }
    }

    void OnPointerExitItem(BaseEventData _eventData)
    {
        if (itemInfo != null)
        {
            itemInfo.gameObject.SetActive(false);
            itemInfo = null;
        }
    }
}
