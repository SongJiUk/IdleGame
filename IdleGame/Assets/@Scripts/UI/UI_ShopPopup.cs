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
        HeroOneRecallButton,
        HeroElevenRecallButton,
        HeroAdRecallButton,
    }

    enum Texts
    {
        DiaText,
        GoldText,
        HeroOnRecallPriceText,
        HeroElevenRecallText,
        HeroAdRecallCountText,
        HeroRecallExpText,
        HeroRecallLevelText,
    }

    enum Images
    {
        HeroImage1,
        HeroImage2,
        HeroImage3,
        HeroRecallExpFillImage,
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

        GetText(TextsType, (int)Texts.DiaText).text = Utils.ToCurrencyString(Managers.GameM.Gold);
        GetText(TextsType, (int)Texts.DiaText).text = Utils.ToCurrencyString(Managers.GameM.Dia);

        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton(ButtonsType, (int)Buttons.HeroAdRecallButton).gameObject.BindEvent(OnClickHeroAdRecallButton);
        GetButton(ButtonsType, (int)Buttons.HeroOneRecallButton).gameObject.BindEvent(OnClickHeroOneRecallButton);
        GetButton(ButtonsType, (int)Buttons.HeroElevenRecallButton).gameObject.BindEvent(OnClickHeroElevenRecallButton);

        RefreshUI();
        return true;
    }

    void RefreshUI()
    {
        if (Managers.GameM.Dia >= 300)
            GetText(TextsType, (int)Texts.HeroOnRecallPriceText).color = Color.white;
        else
            GetText(TextsType, (int)Texts.HeroOnRecallPriceText).color = Color.red;

        if (Managers.GameM.Dia >= 3000)
            GetText(TextsType, (int)Texts.HeroElevenRecallText).color = Color.white;
        else
            GetText(TextsType, (int)Texts.HeroElevenRecallText).color = Color.red;

        //TODO : 광고
    }

    async void OnClickHeroAdRecallButton()
    {
        //TODO : TEST
        var popup = await Managers.UIM.ShowPopup<UI_RecallPopup>();
        popup.GetRecallHero(11);

    }

    void OnClickHeroOneRecallButton()
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

    void OnClickHeroElevenRecallButton()
    {
        if (Managers.GameM.Dia >= 3000)
        {

        }
        else
        {
            //TODO : Toast
        }
    }

    void OnClickCloseButton()
    {
        TriggerClose(this);
    }
}
