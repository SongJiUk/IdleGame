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
        itemCountBG,


    }
    enum Buttons
    {

    }


    enum Texts
    {
        ItemCountText
    }
    ItemHolder item;
    RectTransform parent;
    UI_ItemInfo itemInfo;
    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        transform.localScale = Vector3.one;

        ImagesType = typeof(Images);
        TextsType = typeof(Texts);

        BindImage(ImagesType);
        BindText(TextsType);
        gameObject.BindEvent(OnClickItem);
        gameObject.BindEvent(_dragAction: OnPointerExitItem, _type: Define.UIEvent.OnPointerExit);
        return true;
    }

    public void SetInfo(ItemHolder _item, RectTransform _parent = null)
    {
        item = _item;
        parent = _parent;
        GetImage(ImagesType, (int)Images.ItemBGImage).sprite = Managers.ResourceM.GetAtlas(item.data.ItemGrade.ToString());
        GetImage(ImagesType, (int)Images.ItemIconImage).sprite = Managers.ResourceM.GetAtlas(item.data.Name);
        GetText(TextsType, (int)Texts.ItemCountText).text = item.holder.Count.ToString();

        RectTransform myRect = GetComponent<RectTransform>();

        float targetWidth = myRect.rect.width / 3f;
        RectTransform bgRect = GetImage(ImagesType, (int)Images.itemCountBG).rectTransform;

        bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
        bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetWidth);
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
