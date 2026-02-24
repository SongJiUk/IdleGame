using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.CompilerServices;

public class UI_ShopPopup : UI_Popup
{

    enum GameObjects
    {
        RemoveADObject,
        BottomButtonBarObject,
        ScrollView,
        Content,
        GachaTitle,
        RemoveAdsTitle,
        GoodsTitle,
        DailyTitle

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



        GachaBottomButton,
        RemoveAdsBottomButton,
        GoodsBottomButton,
        DailyShopBottomButton


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


    RectTransform gachaRect;
    RectTransform removeAdsRect;
    RectTransform goodsRect;
    RectTransform dailyRect;

    RectTransform targetRect;
    RectTransform gachaButtonRect;
    RectTransform removeAdsButtonRect;
    RectTransform goodsButtonRect;
    RectTransform dailyButtonRect;

    ScrollRect scrollRect;
    RectTransform content;
    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        Managers.GameM.OnGoodsChanged += CheckGoodsCount;
        Managers.IAPM.OnPurchaseSuccess += OnCheckRemoveAD;

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

        GetButton(ButtonsType, (int)Buttons.GachaBottomButton).gameObject.BindEvent(OnClickGachaButton);
        GetButton(ButtonsType, (int)Buttons.RemoveAdsBottomButton).gameObject.BindEvent(OnClickPackageButton);
        GetButton(ButtonsType, (int)Buttons.GoodsBottomButton).gameObject.BindEvent(OnClickGoodsButton);
        GetButton(ButtonsType, (int)Buttons.DailyShopBottomButton).gameObject.BindEvent(OnClickDailyButton);

        targetRect = GetObject(GameObjectsType, (int)GameObjects.BottomButtonBarObject).GetComponent<RectTransform>();

        gachaButtonRect = GetButton(ButtonsType, (int)Buttons.GachaBottomButton).GetComponent<RectTransform>();
        removeAdsButtonRect = GetButton(ButtonsType, (int)Buttons.RemoveAdsBottomButton).GetComponent<RectTransform>();
        goodsButtonRect = GetButton(ButtonsType, (int)Buttons.GoodsBottomButton).GetComponent<RectTransform>();
        dailyButtonRect = GetButton(ButtonsType, (int)Buttons.DailyShopBottomButton).GetComponent<RectTransform>();

        gachaRect = GetObject(GameObjectsType, (int)GameObjects.GachaTitle).GetComponent<RectTransform>();
        removeAdsRect = GetObject(GameObjectsType, (int)GameObjects.RemoveAdsTitle).GetComponent<RectTransform>();
        goodsRect = GetObject(GameObjectsType, (int)GameObjects.GoodsTitle).GetComponent<RectTransform>();
        dailyRect = GetObject(GameObjectsType, (int)GameObjects.DailyTitle).GetComponent<RectTransform>();


        scrollRect = GetObject(GameObjectsType, (int)GameObjects.ScrollView).GetComponent<ScrollRect>();
        content = GetObject(GameObjectsType, (int)GameObjects.Content).GetComponent<RectTransform>();

        return true;
    }
    public override void SetInfo()
    {
        ScrollToTitle(gachaRect, true);
        MoveTarget(gachaButtonRect, false);
        RefreshUI();
    }
    void RefreshUI()
    {
        CheckGoodsCount();
        CheckGachaHero();
        CheckGachaRelic();
        CheckButtonTextColor();
        OnCheckRemoveAD();
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
        GetText(TextsType, (int)Texts.GoldText).text = Utils.ToCurrencyString(Managers.GameM.Gold);
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
        if (!Managers.UIM.ClickLock(0.5f)) return;

        var popup = await Managers.UIM.ShowPopup<UI_GachaPopup>();
        popup.OnGachaFinished = RefreshUI;
        await popup.GetGachaHero(11);
        RefreshUI();
        Managers.QuestM.GetMission(Define.MissionTarget.HeroGacha).Progress += 11;

    }

    async void OnClickHeroOneGachaButton()
    {
        if (!Managers.UIM.ClickLock(0.5f)) return;

        if (Managers.GameM.Dia >= 300)
        {
            //TODO : 성공시 Refresh()

            Managers.QuestM.GetMission(Define.MissionTarget.HeroGacha).Progress++;
        }
        else
        {
            //TODO : Toast
        }
    }

    async void OnClickHeroElevenGachaButton()
    {
        if (!Managers.UIM.ClickLock(0.5f)) return;
        if (Managers.GameM.Dia >= 3000)
        {
            Managers.QuestM.GetMission(Define.MissionTarget.HeroGacha).Progress += 11;
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
        if (!Managers.UIM.ClickLock(0.5f)) return;
        var popup = await Managers.UIM.ShowPopup<UI_RelicGachaPopup>();
        popup.OnGachaFinished = RefreshUI;
        await popup.GetGachaRelic(11);
        RefreshUI();

        Managers.QuestM.GetMission(Define.MissionTarget.RelicGacha).Progress += 11;
    }

    void OnClickRelicOneGachaButton()
    {
        if (!Managers.UIM.ClickLock(0.5f)) return;
        Managers.QuestM.GetMission(Define.MissionTarget.RelicGacha).Progress++;
    }

    void OnClickRelicElevenGachaButton()
    {
        if (!Managers.UIM.ClickLock(0.5f)) return;
        Managers.QuestM.GetMission(Define.MissionTarget.RelicGacha).Progress += 11;
    }

    void OnClickRelicGachaListButton()
    {
        Managers.UIM.ShowPopup<UI_GachaListPopup>().Forget();
    }
    #endregion


    #region RemoveAd

    //TODO : 이거 구매되면 바로 true로 바꾸고싶은데 .
    void OnCheckRemoveAD()
    {
        if (Managers.GameM.gameData.ADS_Remove)
        {
            GetObject(GameObjectsType, (int)GameObjects.RemoveADObject).SetActive(true);
        }
        else
            GetObject(GameObjectsType, (int)GameObjects.RemoveADObject).SetActive(false);
    }

    #endregion

    #region bottom Button
    void OnClickGachaButton()
    {
        ScrollToTitle(gachaRect, true);
        MoveTarget(gachaButtonRect, true);
    }

    void OnClickPackageButton()
    {
        ScrollToTitle(removeAdsRect, true);
        MoveTarget(removeAdsButtonRect, true);
    }

    void OnClickGoodsButton()
    {
        ScrollToTitle(goodsRect, true);
        MoveTarget(goodsButtonRect, true);
    }

    void OnClickDailyButton()
    {
        ScrollToTitle(dailyRect, true);
        MoveTarget(dailyButtonRect, true);
    }


    public void ScrollToTitle(RectTransform _target, bool _animate = true)
    {
        Canvas.ForceUpdateCanvases();

        RectTransform viewport = scrollRect.viewport != null
            ? scrollRect.viewport
            : scrollRect.GetComponent<RectTransform>();

        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;

        float scrollableHeight = Mathf.Max(1f, contentHeight - viewportHeight);

        Vector2 localPos = content.InverseTransformPoint(_target.position);
        float yFromTop = -localPos.y;

        float targetHalfHeight = _target.rect.height * 0.5f;

        float margin = 20f;

        float adjustedY = yFromTop - targetHalfHeight - margin;
        float normalized = Mathf.Clamp01(adjustedY / scrollableHeight);
        float finalValue = 1f - normalized;

        if (_animate)
        {
            DOTween.To(
                () => scrollRect.verticalNormalizedPosition,
                v => scrollRect.verticalNormalizedPosition = v,
                finalValue,
                0.25f
            ).SetEase(Ease.OutQuad);
        }
        else
        {
            scrollRect.verticalNormalizedPosition = finalValue;
        }
    }

    void MoveTarget(RectTransform _target, bool _animate = true)
    {
        Vector3 worldPos = targetRect.position;

        targetRect.SetParent(_target, false);
        targetRect.SetAsFirstSibling();

        targetRect.anchorMin = new Vector2(0.5f, 0.5f);
        targetRect.anchorMax = new Vector2(0.5f, 0.5f);
        targetRect.pivot = new Vector2(0.5f, 0.5f);
        targetRect.position = worldPos;

        if (_animate)
        {
            targetRect.localScale = Vector3.one * 0.9f;
            targetRect.DOScale(1f, 0.15f).SetEase(Ease.OutBack);
            targetRect.DOAnchorPos(Vector2.zero, 0.25f).SetEase(Ease.OutQuad);
        }
        else targetRect.anchoredPosition = Vector2.zero;
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
        Managers.IAPM.OnPurchaseSuccess -= OnCheckRemoveAD;
        TriggerClose(this, true).Forget();
    }
}
