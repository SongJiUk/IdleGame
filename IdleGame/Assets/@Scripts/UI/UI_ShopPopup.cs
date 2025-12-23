using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShopPopup : UI_Popup
{

    enum GameObjects
    {
    }

    enum Buttons
    {
        CloseButton,
        HeroOneGachaButton,
        HeroElevenGachaButton,
        HeroAdGachaButton,
        GachaListButton,

    }

    enum Texts
    {
        DiaText,
        GoldText,
        HeroOnGachaPriceText,
        HeroOnGachaText,
        HeroElevenGachaText,
        HeroElevenGachaPriceText,
        HeroAdGachaCountText,
        HeroGachaExpText,
        HeroGachaLevelText,
        LegendaryConfirmedCountText,
    }

    enum Images
    {
        HeroImage1,
        HeroImage2,
        HeroImage3,
        HeroGachaExpFillImage,
        LegendaryConfirmedFillImage,
    }


    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        GameObjectsType = typeof(GameObjects);
        ButtonsType = typeof(Buttons);
        TextsType = typeof(Texts);
        ImagesType = typeof(Images);

        BindObject(GameObjectsType);
        BindButton(ButtonsType);
        BindText(TextsType);
        BindImage(ImagesType);

        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton(ButtonsType, (int)Buttons.HeroAdGachaButton).gameObject.BindEvent(OnClickHeroAdGachaButton);
        GetButton(ButtonsType, (int)Buttons.HeroOneGachaButton).gameObject.BindEvent(OnClickHeroOneGachaButton);
        GetButton(ButtonsType, (int)Buttons.HeroElevenGachaButton).gameObject.BindEvent(OnClickHeroElevenGachaButton);
        GetButton(ButtonsType, (int)Buttons.GachaListButton).gameObject.BindEvent(OnClickGachaListButton);
        return true;
    }
    public override void SetInfo()
    {
        RefreshUI();
    }
    void RefreshUI()
    {
        CheckGoodsCount();
        CheckGachaHero();
        CheckButtonTextColor();
    }

    #region 들어올때마다 초기화 해줘야하는것들

    void CheckGachaHero()
    {
        int level = Utils.Summon_Level(Managers.GameM.Summon_Count);
        
        if (level >= 10)
        {
            GetText(TextsType, (int)Texts.HeroGachaLevelText).text = "소환 Lv. " + level.ToString();
            GetText(TextsType, (int)Texts.HeroGachaExpText).text = "MAX";
            GetImage(ImagesType, (int)Images.HeroGachaExpFillImage).fillAmount = 1f;
        }
        else
        {
            int levelValue = Managers.DataM.GachaDataDic[level].SummonCount; 
            GetText(TextsType, (int)Texts.HeroGachaLevelText).text = "소환 Lv. " + level.ToString();
            GetText(TextsType, (int)Texts.HeroGachaExpText).text = $"({Managers.GameM.Summon_Count} / {levelValue})";
            GetImage(ImagesType, (int)Images.HeroGachaExpFillImage).fillAmount = (float)Managers.GameM.Summon_Count / (float)levelValue;
        }

        int maxCount = Managers.DataM.GachaDataDic[Utils.GachaMaxLevel].SummonCount;
        GetText(TextsType, (int)Texts.LegendaryConfirmedCountText).text = $"({Managers.GameM.Confirmed_Legendary_Count} / {maxCount})";
        GetImage(ImagesType, (int)Images.LegendaryConfirmedFillImage).fillAmount = (float)Managers.GameM.Confirmed_Legendary_Count / (float)maxCount;
    }

    void CheckGoodsCount()
    {
        GetText(TextsType, (int)Texts.DiaText).text = Utils.ToCurrencyString(Managers.GameM.Gold);
        GetText(TextsType, (int)Texts.DiaText).text = Utils.ToCurrencyString(Managers.GameM.Dia);
    }

    void CheckButtonTextColor()
    {
        if (Managers.GameM.Dia >= 300)
        {
            GetText(TextsType, (int)Texts.HeroOnGachaPriceText).color = Color.white;
            GetText(TextsType, (int)Texts.HeroOnGachaText).color = Color.white;
        }
        else
        {
            GetText(TextsType, (int)Texts.HeroOnGachaPriceText).color = Color.red;
            GetText(TextsType, (int)Texts.HeroOnGachaText).color = Color.red;
        }


        if (Managers.GameM.Dia >= 3000)
        {
            GetText(TextsType, (int)Texts.HeroElevenGachaText).color = Color.white;
            GetText(TextsType, (int)Texts.HeroElevenGachaPriceText).color = Color.white;
        }
        else
        {
            GetText(TextsType, (int)Texts.HeroElevenGachaText).color = Color.red;
            GetText(TextsType, (int)Texts.HeroElevenGachaPriceText).color = Color.red;
        }

        //TODO : 광고
    }
    #endregion

    #region Button

    async void OnClickHeroAdGachaButton()
    {
        //TODO : TEST
        var popup = await Managers.UIM.ShowPopup<UI_GachaPopup>();
        await popup.GetGachaHero(11);
        RefreshUI();
    }

    void OnClickHeroOneGachaButton()
    {
        if (Managers.GameM.Dia >= 300)
        {
            //TODO : 성공시 Refresh()
        }
        else
        {
            //TODO : Toast
        }
    }

    void OnClickHeroElevenGachaButton()
    {
        if (Managers.GameM.Dia >= 3000)
        {

        }
        else
        {
            //TODO : Toast
        }
    }

    void OnClickGachaListButton()
    {
        Managers.UIM.ShowPopup<UI_GachaListPopup>().Forget();

    }

    void OnClickCloseButton()
    {
        TriggerClose(this);
    }

    #endregion
}
