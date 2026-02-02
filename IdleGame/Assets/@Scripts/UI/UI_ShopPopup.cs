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
        HeroGachaListButton,

        RelicOneGachaButton,
        RelicElevenGachaButton,
        RelicAdGachaButton,
        RelicGachaListButton,

        RemoveAdsButton,
        Dia300Button,

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
        HeroLegendaryConfirmedCountText,

        RelicOnGachaPriceText,
        RelicOnGachaText,
        RelicElevenGachaText,
        RelicElevenGachaPriceText,
        RelicAdGachaCountText,
        RelicGachaExpText,
        RelicGachaLevelText,
        RelicLegendaryConfirmedCountText,
    }

    enum Images
    {
        HeroImage1,
        HeroImage2,
        HeroImage3,
        HeroGachaExpFillImage,
        HeroLegendaryConfirmedFillImage,

        RelicGachaExpFillImage,
        RelicLegendaryConfirmedFillImage,
    }


    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        Managers.GameM.OnGoodsChanged += CheckGoodsCount;

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
        GetButton(ButtonsType, (int)Buttons.HeroGachaListButton).gameObject.BindEvent(OnClickHeroGachaListButton);

        GetButton(ButtonsType, (int)Buttons.RelicAdGachaButton).gameObject.BindEvent(OnClickRelicAdGachaButton);
        GetButton(ButtonsType, (int)Buttons.RelicOneGachaButton).gameObject.BindEvent(OnClickRelicOneGachaButton);
        GetButton(ButtonsType, (int)Buttons.RelicElevenGachaButton).gameObject.BindEvent(OnClickRelicElevenGachaButton);
        GetButton(ButtonsType, (int)Buttons.RelicGachaListButton).gameObject.BindEvent(OnClickRelicGachaListButton);

        GetButton(ButtonsType, (int)Buttons.RemoveAdsButton).gameObject.BindEvent(() => OnClickGoodsButton(Buttons.RemoveAdsButton));
        GetButton(ButtonsType, (int)Buttons.Dia300Button).gameObject.BindEvent(() => OnClickGoodsButton(Buttons.Dia300Button));
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
        CheckGachaRelic();
        CheckButtonTextColor();
    }

    #region 들어올때마다 초기화 해줘야하는것들

    void CheckGachaHero()
    {
        int level = Utils.Summon_Level(Managers.GameM.Hero_Summon_Count);

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
            GetText(TextsType, (int)Texts.HeroGachaExpText).text = $"({Managers.GameM.Hero_Summon_Count} / {levelValue})";
            GetImage(ImagesType, (int)Images.HeroGachaExpFillImage).fillAmount = (float)Managers.GameM.Hero_Summon_Count / (float)levelValue;
        }

        int maxCount = Managers.DataM.GachaDataDic[Utils.GachaMaxLevel].SummonCount;
        GetText(TextsType, (int)Texts.HeroLegendaryConfirmedCountText).text = $"({Managers.GameM.Hero_Confirmed_Legendary_Count} / {maxCount})";
        GetImage(ImagesType, (int)Images.HeroLegendaryConfirmedFillImage).fillAmount = (float)Managers.GameM.Hero_Confirmed_Legendary_Count / (float)maxCount;
    }
    void CheckGachaRelic()
    {
        int level = Utils.Summon_Level(Managers.GameM.Relics_Summon_Count);

        if (level >= 10)
        {
            GetText(TextsType, (int)Texts.RelicGachaLevelText).text = "소환 Lv. " + level.ToString();
            GetText(TextsType, (int)Texts.RelicGachaExpText).text = "MAX";
            GetImage(ImagesType, (int)Images.RelicGachaExpFillImage).fillAmount = 1f;
        }
        else
        {
            int levelValue = Managers.DataM.GachaDataDic[level].SummonCount;
            GetText(TextsType, (int)Texts.RelicGachaLevelText).text = "소환 Lv. " + level.ToString();
            GetText(TextsType, (int)Texts.RelicGachaExpText).text = $"({Managers.GameM.Relics_Summon_Count} / {levelValue})";
            GetImage(ImagesType, (int)Images.RelicGachaExpFillImage).fillAmount = (float)Managers.GameM.Relics_Summon_Count / (float)levelValue;
        }

        int maxCount = Managers.DataM.GachaDataDic[Utils.GachaMaxLevel].SummonCount;
        GetText(TextsType, (int)Texts.RelicLegendaryConfirmedCountText).text = $"({Managers.GameM.Relics_Confirmed_Legendary_Count} / {maxCount})";
        GetImage(ImagesType, (int)Images.RelicLegendaryConfirmedFillImage).fillAmount = (float)Managers.GameM.Relics_Confirmed_Legendary_Count / (float)maxCount;
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

            GetText(TextsType, (int)Texts.RelicOnGachaPriceText).color = Color.white;
            GetText(TextsType, (int)Texts.RelicOnGachaText).color = Color.white;
        }
        else
        {
            GetText(TextsType, (int)Texts.HeroOnGachaPriceText).color = Color.red;
            GetText(TextsType, (int)Texts.HeroOnGachaText).color = Color.red;

            GetText(TextsType, (int)Texts.RelicOnGachaPriceText).color = Color.red;
            GetText(TextsType, (int)Texts.RelicOnGachaText).color = Color.red;
        }


        if (Managers.GameM.Dia >= 3000)
        {
            GetText(TextsType, (int)Texts.HeroElevenGachaText).color = Color.white;
            GetText(TextsType, (int)Texts.HeroElevenGachaPriceText).color = Color.white;

            GetText(TextsType, (int)Texts.RelicElevenGachaText).color = Color.white;
            GetText(TextsType, (int)Texts.RelicElevenGachaPriceText).color = Color.white;
        }
        else
        {
            GetText(TextsType, (int)Texts.HeroElevenGachaText).color = Color.red;
            GetText(TextsType, (int)Texts.HeroElevenGachaPriceText).color = Color.red;

            GetText(TextsType, (int)Texts.RelicElevenGachaText).color = Color.red;
            GetText(TextsType, (int)Texts.RelicElevenGachaPriceText).color = Color.red;
        }

        //TODO : 광고
    }
    #endregion

    #region Hero Button

    async void OnClickHeroAdGachaButton()
    {
        var popup = await Managers.UIM.ShowPopup<UI_GachaPopup>();
        popup.OnGachaFinished = RefreshUI;
        await popup.GetGachaHero(11);
        RefreshUI();
        Managers.GameM.GetMission(Define.MissionTarget.HeroGacha).Progress += 11;

    }

    void OnClickHeroOneGachaButton()
    {
        if (Managers.GameM.Dia >= 300)
        {
            //TODO : 성공시 Refresh()

            Managers.GameM.GetMission(Define.MissionTarget.HeroGacha).Progress++;
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
            Managers.GameM.GetMission(Define.MissionTarget.HeroGacha).Progress += 11;
        }
        else
        {
            //TODO : Toast
        }
    }

    void OnClickHeroGachaListButton()
    {
        Managers.UIM.ShowPopup<UI_GachaListPopup>().Forget();

    }
    #endregion

    #region RelicButton
    async void OnClickRelicAdGachaButton()
    {
        var popup = await Managers.UIM.ShowPopup<UI_RelicGachaPopup>();
        popup.OnGachaFinished = RefreshUI;
        await popup.GetGachaRelic(11);
        RefreshUI();

        Managers.GameM.GetMission(Define.MissionTarget.RelicGacha).Progress += 11;
    }

    void OnClickRelicOneGachaButton()
    {
        Managers.GameM.GetMission(Define.MissionTarget.RelicGacha).Progress++;
    }

    void OnClickRelicElevenGachaButton()
    {
        Managers.GameM.GetMission(Define.MissionTarget.RelicGacha).Progress += 11;
    }

    void OnClickRelicGachaListButton()
    {
    }
    #endregion

    public void GetProuduct(string _name)
    {
        Managers.IAPM.Purchase(_name);
    }

    void OnClickGoodsButton(Buttons _clickButton)
    {
        switch (_clickButton)
        {
            case Buttons.RemoveAdsButton:
                GetProuduct(Define.IAP.removeads.ToString());
                break;

            case Buttons.Dia300Button:
                GetProuduct(Define.IAP.dia300.ToString());
                break;
        }

    }
    void OnClickCloseButton()
    {
        Managers.GameM.OnGoodsChanged -= CheckGoodsCount;
        TriggerClose(this, true).Forget();
    }
}
