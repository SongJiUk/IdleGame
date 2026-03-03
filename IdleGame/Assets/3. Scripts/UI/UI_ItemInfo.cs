using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class UI_ItemInfo : UI_Popup
{

    #region Enum
    enum Images
    {
        ItemImage,
    }

    enum Texts
    {
        ItemNameText,
        ItemGradeText,
        ItemDescriptionText
    }
    #endregion

    RectTransform rect;
    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        ImagesType = typeof(Images);
        TextsType = typeof(Texts);

        BindImage(ImagesType);
        BindText(TextsType);

        rect = GetComponent<RectTransform>();


        return true;
    }

    public void SetInfo(ItemHolder _itemData, Vector2 _pos, RectTransform _parent)
    {
        transform.localScale = Vector3.one;
        GetImage(ImagesType, (int)Images.ItemImage).sprite = Managers.ResourceM.GetAtlas(_itemData.data.Name);
        GetText(TextsType, (int)Texts.ItemNameText).text = _itemData.data.NameKR;
        GetText(TextsType, (int)Texts.ItemDescriptionText).text = _itemData.data.Description;
        GetText(TextsType, (int)Texts.ItemGradeText).text = Utils.StringToColorGrade(_itemData.data.ItemGrade) + _itemData.data.ItemGrade + "</color>";

        if (rect == null)
            rect = GetComponent<RectTransform>();


        rect.pivot = PivotPoint(_pos);
        rect.anchoredPosition = _pos;



    }


}
