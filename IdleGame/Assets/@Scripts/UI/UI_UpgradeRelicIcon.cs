using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_UpgradeRelicIcon : UI_Base
{
    #region Enum
    enum Images
    {
        RelicBGImage,
        RelicImage,
        RelicBlinkImage,
    }
    enum Texts
    {
        BeforeLevelText,
        NextLevelText
    }
    #endregion

    ItemHolder item;
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        ImagesType = typeof(Images);
        TextsType = typeof(Texts);

        BindImage(ImagesType);
        BindText(TextsType);


        return true;
    }

    public void SetInfo(ItemHolder _item)
    {
        item = _item;
        GetImage(ImagesType, (int)Images.RelicBGImage).sprite = Managers.ResourceM.GetAtlas(item.data.ItemGrade.ToString());
        GetImage(ImagesType, (int)Images.RelicImage).sprite = Managers.ResourceM.GetAtlas(item.data.Name);
        GetImage(ImagesType, (int)Images.RelicImage).SetNativeSize();


        GetText(TextsType, (int)Texts.BeforeLevelText).text = (item.holder.Level - 1).ToString();
        GetText(TextsType, (int)Texts.NextLevelText).text = item.holder.Level.ToString();

        var blinkImage = GetImage(ImagesType, (int)Images.RelicBlinkImage);

        blinkImage.color = new Color(1, 1, 1, 1);
        blinkImage.DOKill();
        blinkImage.DOFade(0.0f, 0.3f)
        .SetEase(Ease.OutQuart)
        .SetLink(gameObject);
    }

}
