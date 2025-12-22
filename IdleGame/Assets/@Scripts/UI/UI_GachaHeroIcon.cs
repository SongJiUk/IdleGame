using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_GachaHeroIcon : UI_Base
{
    enum Images
    {
        HeroBGImage,
        HeroImage,
        HereBlinkImage,

    }

    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        ImagesType = typeof(Images);

        BindImage(ImagesType);


        return true;
    }


    public void SetHeroIcon(Data.CreatureData _data)
    {
        GetImage(ImagesType, (int)Images.HeroBGImage).sprite = Managers.ResourceM.GetAtlas(_data.CharacterGrade.ToString());
        GetImage(ImagesType, (int)Images.HeroImage).sprite = Managers.ResourceM.GetAtlas(_data.Name);

        GetImage(ImagesType, (int)Images.HeroBGImage).SetNativeSize();
        GetImage(ImagesType, (int)Images.HeroImage).SetNativeSize();

        var blinkImage = GetImage(ImagesType, (int)Images.HereBlinkImage);

        blinkImage.color = new Color(1, 1, 1, 1);

        blinkImage.DOFade(0.0f, 0.3f)
            .SetEase(Ease.OutQuart)
            .SetLink(gameObject);

    }
}
