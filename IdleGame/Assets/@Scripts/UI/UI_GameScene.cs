using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class UI_GameScene : UI_Scene
{
    #region Enum
    enum GameObjects
    {
        LayersObject,
        JewelObject,
        CoinObject
    }
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

    #endregion

    Button selectedButton = null;
    Button clickedButton = null;
    Button statBtn;
    Button heroBtn;
    Button relicsBtn;
    Button dungeonBtn;
    Button enforceBtn;
    Button shopBtn;
    #region 코인, 쥬얼리, 아이템 애니메이션 관련
    //TODO : UI_Scene에서 관리하는거임
    public override Transform WorldCoinParent
    {
        get
        {
            return GetLayer((int)Define.UILayerIndex.Coin);
        }
    }
    public override Transform WorldJewelParent
    {
        get
        {
            return GetLayer((int)Define.UILayerIndex.Coin);
        }
    }

    public override Transform WorldFontParent
    {
        get
        {
            return GetLayer((int)Define.UILayerIndex.DamageFont);
        }
    }
    public override Transform WorldItemParent
    {
        get
        {
            return GetLayer((int)Define.UILayerIndex.ItemRect);
        }
    }


    #endregion

    UI_HeroPopup ui_HeroPopup;

    public UI_HeroPopup Ui_HeroPopup { get { return ui_HeroPopup; } }

    public override bool Init()
    {
        if (!base.Init()) return false;
        GameObjectsType = typeof(GameObjects);
        ButtonsType = typeof(Buttons);
        TextsType = typeof(Texts);
        ImagesType = typeof(Images);

        BindObject(GameObjectsType);
        BindButton(ButtonsType);
        BindText(TextsType);
        BindImage(ImagesType);


        coinDirectingTr = GetObject(GameObjectsType, (int)GameObjects.CoinObject).GetComponent<RectTransform>();
        jewelDirectingTr = GetObject(GameObjectsType, (int)GameObjects.JewelObject).GetComponent<RectTransform>();
        layers = GetObject(GameObjectsType, (int)GameObjects.LayersObject).GetComponent<Transform>();

        foreach (Buttons buttonType in Enum.GetValues(typeof(Buttons)))
        {
            GetButton(ButtonsType, (int)buttonType).gameObject.BindEvent(() => OnClickAnyButtons(buttonType));
        }
        // GetButton(ButtonsType, (int)Buttons.QuestButton).gameObject.BindEvent(OnClickQuestButton);
        // GetButton(ButtonsType, (int)Buttons.StatButton).gameObject.BindEvent(OnClickStatButton);
        // GetButton(ButtonsType, (int)Buttons.HeroButton).gameObject.BindEvent(OnClickHeroButton);
        // GetButton(ButtonsType, (int)Buttons.RelicsButton).gameObject.BindEvent(OnClickRelicsButton);
        // GetButton(ButtonsType, (int)Buttons.DungeonButton).gameObject.BindEvent(OnClickDungeonButton);
        // GetButton(ButtonsType, (int)Buttons.EnforceButton).gameObject.BindEvent(OnClickEnforceButton);
        // GetButton(ButtonsType, (int)Buttons.ShopButton).gameObject.BindEvent(OnClickShopButton);

        statBtn = GetButton(ButtonsType, (int)Buttons.StatButton);
        heroBtn = GetButton(ButtonsType, (int)Buttons.HeroButton);
        relicsBtn = GetButton(ButtonsType, (int)Buttons.RelicsButton);
        dungeonBtn = GetButton(ButtonsType, (int)Buttons.DungeonButton);
        enforceBtn = GetButton(ButtonsType, (int)Buttons.EnforceButton);
        shopBtn = GetButton(ButtonsType, (int)Buttons.ShopButton);

        ui_HeroPopup = Managers.UIM.ShowPopup<UI_HeroPopup>();
        ui_HeroPopup.Init();

        AllOff();
        UpdateUIState();
        //UI_Toast ui_Toast = Managers.UIM.ShowPopup<UI_Toast>();
        StartSpawnAfterDelay().Forget();


        return true;
    }

    void UpdateUIState()
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
    void AllOff()
    {
        ui_HeroPopup.isOpen = false;

        ui_HeroPopup.gameObject.SetActive(false);
    }

    void OnClickAnyButtons(Buttons _clickButtonType)
    {
        clickedButton = null;
        switch (_clickButtonType)
        {
            case Buttons.QuestButton:
                Debug.Log("Click Quest Button");
                break;
            case Buttons.StatButton:
                Debug.Log("Click Stat Button");
                clickedButton = statBtn;
                break;

            case Buttons.HeroButton:
                Debug.Log("Click Hero Button");
                clickedButton = heroBtn;
                if(!ui_HeroPopup.isOpen)
                {
                    ui_HeroPopup.isOpen = true;
                    ui_HeroPopup.gameObject.SetActive(true);
                    ui_HeroPopup.SetInfo();
                }
                
                break;

            case Buttons.RelicsButton:
                Debug.Log("Click Relics Button");
                clickedButton = relicsBtn;
                break;

            case Buttons.DungeonButton:
                Debug.Log("Click Dungeon Button");
                clickedButton = dungeonBtn;

                break;

            case Buttons.EnforceButton:
                Debug.Log("Click Enforce Button");
                clickedButton = enforceBtn;

                break;

            case Buttons.ShopButton:
                Debug.Log("Click Shop Button");
                clickedButton = shopBtn;
                break;
        }

        if (clickedButton == null) return;

        if (selectedButton != null && selectedButton != clickedButton)
        {
            selectedButton.transform.DOScale(Vector3.one, 0.2f);
        }

        clickedButton.transform.DOScale(Vector3.one * 1.2f, 0.2f);

        selectedButton = clickedButton;
    }


    async UniTaskVoid StartSpawnAfterDelay()
    {
        await UniTask.Yield();

        Managers.SpawnM.StartSpawn();
    }

}
