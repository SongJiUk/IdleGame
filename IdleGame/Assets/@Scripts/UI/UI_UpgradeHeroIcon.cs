using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_UpgradeHeroIcon : UI_Base
{
    #region Enum
    enum Images
    {
        HeroBGImage,
        HeroImage,
        HereBlinkImage,

    }

    enum Texts
    {
        BeforeLevelText,
        NextLevelText
    }
    #endregion
    CharacterHolder characterHolder;
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        ImagesType = typeof(Images);
        TextsType = typeof(Texts);

        BindImage(ImagesType);
        BindText(TextsType);


        return true;
    }

    public void SetInfo(CharacterHolder _characterHolder)
    {
        characterHolder = _characterHolder;
        GetImage(ImagesType, (int)Images.HeroBGImage).sprite = Managers.ResourceM.GetAtlas(characterHolder.data.CharacterGrade.ToString());
        GetImage(ImagesType, (int)Images.HeroImage).sprite = Managers.ResourceM.GetAtlas(characterHolder.data.Name);


        GetText(TextsType, (int)Texts.BeforeLevelText).text = (characterHolder.holder.Level - 1).ToString();
        GetText(TextsType, (int)Texts.NextLevelText).text = characterHolder.holder.Level.ToString();

        var blinkImage = GetImage(ImagesType, (int)Images.HereBlinkImage);

        blinkImage.color = new Color(1, 1, 1, 1);
        blinkImage.DOKill();
        blinkImage.DOFade(0.0f, 0.3f)
        .SetEase(Ease.OutQuart)
        .SetLink(gameObject);
    }

}
