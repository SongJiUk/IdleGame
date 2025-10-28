using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BaseScene : UI_Base
{
    enum Buttons
    {
        QuestButton,
        StatButton,
        HeroButton,
        RelicsButton,
        DungeonButton,
        EnforceButton,
        ShopButton,
        MainSkillButton,
        Character1_Button,
        Character2_Button,
        Character3_Button,
        Character4_Button,
        Character5_Button,
        LevelUpButton,

    }

    enum Texts
    {
        JewelText,
        CoinText,
        StageText,
        CharacterLevelText,
        UserNameText,
        UserCombatPowerText,
        QuestTitleText,
        QuestDescriptionText,
        QueestTutorialText,
        RewardItemText,
        ExpText,
        AttackText,
        HpText,
        NeedLevelUpText,
        GetExpText,

    }

    enum Images
    {
        CharacterImage,
        RewardItemImage,
        TutorialHandImage,
        MainSkillButton,

        Character1_Lock,
        Character1_Plus,
        Character1_Icon,
        Character1_CoolTimeImage,
        Character2_Lock,
        Character2_Plus,
        Character2_Icon,
        Character2_CoolTimeImage,
        Character3_Lock,
        Character3_Plus,
        Character3_Icon,
        Character3_CoolTimeImage,
        Character4_Lock,
        Character4_Plus,
        Character4_Icon,
        Character4_CoolTimeImage,
        Character5_Lock,
        Character5_Plus,
        Character5_Icon,
        Character5_CoolTimeImage,
        Exp_FillImage,

    }
    public override bool Init()
    {
        if (!base.Init()) return false;
        ButtonsType = typeof(Buttons);
        TextsType = typeof(Texts);
        ImagesType = typeof(Images);

        BindButton(ButtonsType);
        BindText(TextsType);
        BindImage(ImagesType);

        GetButton(ButtonsType, (int)Buttons.QuestButton).gameObject.BindEvent(OnClickQuestButton);
        GetButton(ButtonsType, (int)Buttons.StatButton).gameObject.BindEvent(OnClickStatButton);
        GetButton(ButtonsType, (int)Buttons.HeroButton).gameObject.BindEvent(OnClickHeroButton);
        GetButton(ButtonsType, (int)Buttons.RelicsButton).gameObject.BindEvent(OnClickRelicsButton);
        GetButton(ButtonsType, (int)Buttons.DungeonButton).gameObject.BindEvent(OnClickDungeonButton);
        GetButton(ButtonsType, (int)Buttons.EnforceButton).gameObject.BindEvent(OnClickEnforceButton);
        GetButton(ButtonsType, (int)Buttons.ShopButton).gameObject.BindEvent(OnClickShopButton);

        UpdateUiState();


        return true;
    }

    void UpdateUiState()
    {
        //TODO : 여기서 이제 플레이어 상황 받아와서 bool값으로 처리해 주거나 더 좋은 방법생각해보자.
        GetImage(ImagesType, (int)Images.TutorialHandImage).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character1_Plus).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character2_Plus).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character3_Plus).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character4_Plus).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character5_Plus).gameObject.SetActive(false);

        GetImage(ImagesType, (int)Images.Character1_Icon).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character2_Icon).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character3_Icon).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character4_Icon).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character5_Icon).gameObject.SetActive(false);

        GetImage(ImagesType, (int)Images.Character1_CoolTimeImage).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character2_CoolTimeImage).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character3_CoolTimeImage).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character4_CoolTimeImage).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.Character5_CoolTimeImage).gameObject.SetActive(false);
    }
    void OnClickQuestButton()
    {
        Debug.Log("Click Quest Button");
    }
    void OnClickStatButton()
    {
        Debug.Log("Click Stat Button");
    }

    void OnClickHeroButton()
    {
        Debug.Log("Click Hero Button");
    }

    void OnClickRelicsButton()
    {
        Debug.Log("Click Relics Button");
    }

    void OnClickDungeonButton()
    {
        Debug.Log("Click Dungeon Button");
    }

    void OnClickEnforceButton()
    {
        Debug.Log("Click Enforce Button");
    }

    void OnClickShopButton()
    {
        Debug.Log("Click Shop Button");
    }
}
