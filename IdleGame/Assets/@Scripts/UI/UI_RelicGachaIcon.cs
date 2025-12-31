using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_RelicGachaIcon : UI_Base
{
    #region Enum
    enum Images
    {
        RelicBGImage,
        RelicImage,
        RelicBlinkImage,
    }

    #endregion
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        ImagesType = typeof(Images);
        BindImage(ImagesType);

        return true;
    }

    public void SetRelicIcon(Data.ItemData _data)
    {
        GetImage(ImagesType, (int)Images.RelicBGImage).sprite = Managers.ResourceM.GetAtlas(_data.ItemGrade.ToString());
        GetImage(ImagesType, (int)Images.RelicImage).sprite = Managers.ResourceM.GetAtlas(_data.Name);

        GetImage(ImagesType, (int)Images.RelicBGImage).SetNativeSize();
        GetImage(ImagesType, (int)Images.RelicImage).SetNativeSize();

        var blinkImage = GetImage(ImagesType, (int)Images.RelicBlinkImage);

        blinkImage.color = new Color(1, 1, 1, 1);
        blinkImage.DOKill();
        blinkImage.DOFade(0.0f, 0.3f)
            .SetEase(Ease.OutQuart)
            .SetLink(gameObject);

    }
}
