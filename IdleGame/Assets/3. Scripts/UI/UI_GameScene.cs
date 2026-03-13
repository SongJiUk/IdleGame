using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using TMPro;

public class UI_GameScene : UI_Scene, ITickable, IUnScaledTickable
{
    #region Enum
    enum GameObjects
    {
        LayersObject,
        WorldSpeechLayer,
        PopupLayerObject,
        DiaObject,
        CoinObject,
        StageMonsterCountObject,
        BossBoardObject,
        DungeonBoardObject,
        TreasureTroveObject,
        GoldDungeonObject,


        DeadFrameHandObject,
        ItemPopupObject,
        ItemTextPopupObject,
        Character1_Object,
        Character2_Object,
        Character3_Object,
        Character4_Object,
        Character5_Object,
        Character6_Object,
        Character1_HpFrameObject,
        Character2_HpFrameObject,
        Character3_HpFrameObject,
        Character4_HpFrameObject,
        Character5_HpFrameObject,
        Character6_HpFrameObject,

        Character1_MpFrameObject,
        Character2_MpFrameObject,
        Character3_MpFrameObject,
        Character4_MpFrameObject,
        Character5_MpFrameObject,
        Character6_MpFrameObject,

        Character1_ReadyObject,
        Character2_ReadyObject,
        Character3_ReadyObject,
        Character4_ReadyObject,
        Character5_ReadyObject,
        Character6_ReadyObject,
        AttackBuffLockObject,
        DropBuffLockObject,
        CriticalBuffLockObject,
        FastSliderObject,
        StatButtonCloseObject,
        HeroButtonCloseObject,
        RelicsButtonCloseObject,
        DungeonButtonCloseObject,
        SmeltingButtonCloseObject,
        ShopButtonCloseObject,
        //TutorialObject,

    }
    enum Buttons
    {
        SettingButton,
        //MailBoxButton,
        SavingModeButton,
        InventoryButton,
        QuestButton,
        StatButton,
        HeroButton,
        RelicsButton,
        DungeonButton,
        SmeltingButton,
        ShopButton,
        LevelUpButton,
        DeadFrameButton,
        Character1_PlusButton,
        Character2_PlusButton,
        Character3_PlusButton,
        Character4_PlusButton,
        Character5_PlusButton,
        Character6_PlusButton,
        FastButton,
        BuffsButton,
        DailyMissionButton,
        AchievementsButton,
        LeaderBoardButton,

    }

    enum Texts
    {
        DiaText,
        CoinText,
        StageText,
        StageStateText,
        CharacterLevelText,
        UserNameText,
        UserCombatPowerText,
        QuestTitleText,
        QuestDescriptionText,
        QuestTutorialText,
        RewardItemText,
        ExpText,
        AttackText,
        HpText,
        NeedLevelUpText,
        GetExpText,
        StageMonsterCountText,
        BossHPText,
        BossText,
        BossBoardStageText,
        DungeonTimeText,
        DungeonBoardStageText,
        GoldDungeonHpText,
        TreasureTroveCountText,
        ItemPopupText,
        ItemText1,
        ItemText2,
        ItemText3,
        ItemText4,
        ItemText5,
        Character0_HpText,
        Character1_HpText,
        Character2_HpText,
        Character3_HpText,
        Character4_HpText,
        Character5_HpText,
        Character6_HpText,
        Character0_MpText,
        Character1_MpText,
        Character2_MpText,
        Character3_MpText,
        Character4_MpText,
        Character5_MpText,
        Character6_MpText,
        FastTimeText

    }

    enum Images
    {
        CharacterImage,
        RewardItemImage,
        TutorialHandImage,
        Character1_Lock,
        Character2_Lock,
        Character3_Lock,
        Character4_Lock,
        Character5_Lock,
        Character6_Lock,
        Character1_Icon,
        Character2_Icon,
        Character3_Icon,
        Character4_Icon,
        Character5_Icon,
        Character6_Icon,
        Character0_HpFillImage,
        Character1_HpFillImage,
        Character2_HpFillImage,
        Character3_HpFillImage,
        Character4_HpFillImage,
        Character5_HpFillImage,
        Character6_HpFillImage,
        Character0_MpFillImage,
        Character1_MpFillImage,
        Character2_MpFillImage,
        Character3_MpFillImage,
        Character4_MpFillImage,
        Character5_MpFillImage,
        Character6_MpFillImage,
        Exp_FillImage,
        FadeImage,
        StageMonsterCountImage,
        BossHpImage,
        DungeonTimeImage,
        GoldDungeonHpImage,

        ItemPopupFrameImage,
        ItemPopupItemImage,
        FastLockImage,
        FastOnImage,
        FastSliderFill

    }

    enum Sliders
    {

    }

    #endregion

    Button selectedButton = null;
    Button clickedButton = null;
    Button statBtn;
    Button heroBtn;
    Button relicsBtn;
    Button dungeonBtn;
    Button smeltingBtn;
    Button shopBtn;
    Button levelUpBtn;
    Button deadFrameBtn;
    #region 코인, 쥬얼리, 아이템 애니메이션 관련
    //TODO : UI_Scene에서 관리하는거임
    public override Transform WorldGoodsParent
    {
        get
        {
            return GetLayer((int)Define.UILayerIndex.Goods);
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

    public override Transform WorldSpeechParent
    {
        get
        {
            return GetSpeechLayer();
        }
    }

    #endregion

    #region 아이템 획득 애니메이션 관련
    RectTransform ItemPopupObjectRect;
    RectTransform ItemPopupFrameObjectRect;

    List<TextMeshProUGUI> itemTexts = new List<TextMeshProUGUI>();
    #endregion

    public bool isSavingMode = false;
    public UI_SavingMode savingModePopup;
    public UI_AdsBuffPopup AdsBuffPopup;
    public delegate void PlayerStatUpdateHandler(PlayerController _pc);
    public bool[] isCharacterReady;
    private Tween blinkTween = null;
    public float FastremainTime = 0.0f;
    Define.DungeonType type;
    int dungeonDataID;


    #region 하단 Popup
    private UI_HeroStatPopup ui_heroStatPopup;
    private UI_HeroPopup ui_heroPopup;
    private UI_RelicsPopup ui_relicsPopup;
    private UI_DungeonPopup ui_dungeonPopup;
    private UI_SmeltingPopup ui_smeltingPopup;
    private UI_ShopPopup ui_shopPopup;
    private UI_Popup currentBottomPopup;
    private Buttons currentBottomPopupButton;
    #endregion
    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        Managers.StageM.readyEvent += OnReady;
        Managers.StageM.playEvent += OnPlay;
        Managers.StageM.bossPlayEvent += OnBossPlay;
        Managers.StageM.clearEvent += OnClear;
        Managers.StageM.deadEvent += OnDead;
        Managers.StageM.dungeonEvent += OnDungeon;
        Managers.StageM.dungeonClearEvent += OnDungeonClear;
        Managers.StageM.dungeonFailEvent += OnDungeonFail;

        Managers.GameM.OnGoodsChanged += OnRefreshGoods;
        Managers.StageM.OnChangeCount += OnCheckStageMonsterCount;
        Managers.StageM.OnChangeCount += OnCheckTreasureTroveMonsterCount;
        Managers.CharacterM.OnCharacterAdd += OnRegisterCharacterEvents;
        Managers.QuestM.OnQuestDataChanged += OnChangeQuestInfo;
        Managers.RelicM.OnChangeUI += ChangeLevelUpButtonUI;

        Managers.OnAppPause += HandleAppResume;

        //TODO :여기선 mPlayer가 null값이 떠서 사용 x(나중에 메인 플레이어를 알고 있는 상태면 미리 생성해놓고 사용? 어떡할지 고민좀해보자)
        // Managers.GameM.mPlayer.OnPlayerDataUpdate += OnCheckStageMonsterCount;
        // Managers.GameM.mPlayer.OnPlayerDataUpdate += OnPlayerStatChange;
        Managers.UpdateM.Register(this);
        isCharacterReady = new bool[Managers.CharacterM.Characters.Length];
        GameObjectsType = typeof(GameObjects);
        ButtonsType = typeof(Buttons);
        TextsType = typeof(Texts);
        ImagesType = typeof(Images);
        SlidersType = typeof(Sliders);

        BindObject(GameObjectsType);
        BindButton(ButtonsType);
        BindText(TextsType);
        BindImage(ImagesType);
        BindSlider(SlidersType);


        coinDirectingTr = GetObject(GameObjectsType, (int)GameObjects.CoinObject).GetComponent<RectTransform>();
        diaDirectingTr = GetObject(GameObjectsType, (int)GameObjects.DiaObject).GetComponent<RectTransform>();
        ItemPopupObjectRect = GetObject(GameObjectsType, (int)GameObjects.ItemPopupObject).GetComponent<RectTransform>();
        ItemPopupFrameObjectRect = GetImage(ImagesType, (int)Images.ItemPopupFrameImage).GetComponent<RectTransform>();

        GameObject obj = GetObject(GameObjectsType, (int)GameObjects.ItemTextPopupObject);
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            itemTexts.Add(GetText(TextsType, (int)Texts.ItemText1 + i));
            itemTexts[i].gameObject.SetActive(false);
        }

        layers = GetObject(GameObjectsType, (int)GameObjects.LayersObject).GetComponent<Transform>();
        speechLayer = GetObject(GameObjectsType, (int)GameObjects.WorldSpeechLayer).GetComponent<Transform>();
        popupLayer = GetObject(GameObjectsType, (int)GameObjects.PopupLayerObject).GetComponent<Transform>();
        foreach (Buttons buttonType in Enum.GetValues(typeof(Buttons)))
        {
            GetButton(ButtonsType, (int)buttonType).gameObject.BindEvent(() => OnClickAnyButtons(buttonType).Forget());
        }

        //TDOO : 하단 버튼 CloseImage
        for (int i = 0; i < 6; i++)
        {
            GetObject(GameObjectsType, (int)GameObjects.StatButtonCloseObject + i).SetActive(false);
        }

        for (int i = 1; i < Managers.CharacterM.Characters.Length; i++)
        {
            GetButton(ButtonsType, (int)Buttons.Character1_PlusButton + (i - 1)).gameObject.BindEvent(() => OnClickCharacterPlus(i - 1));
        }

        GetButton(ButtonsType, (int)Buttons.LevelUpButton).gameObject.BindEvent(ClickDown, _type: Define.UIEvent.PointerDown);
        GetButton(ButtonsType, (int)Buttons.LevelUpButton).gameObject.BindEvent(ClickUp, _type: Define.UIEvent.PointerUp);

        GetObject(GameObjectsType, (int)GameObjects.GoldDungeonObject).SetActive(false);
        GetObject(GameObjectsType, (int)GameObjects.TreasureTroveObject).SetActive(false);

        GetButton(ButtonsType, (int)Buttons.InventoryButton).gameObject.BindEvent(OnClickInventory);

        statBtn = GetButton(ButtonsType, (int)Buttons.StatButton);
        heroBtn = GetButton(ButtonsType, (int)Buttons.HeroButton);
        relicsBtn = GetButton(ButtonsType, (int)Buttons.RelicsButton);
        dungeonBtn = GetButton(ButtonsType, (int)Buttons.DungeonButton);
        smeltingBtn = GetButton(ButtonsType, (int)Buttons.SmeltingButton);
        shopBtn = GetButton(ButtonsType, (int)Buttons.ShopButton);
        levelUpBtn = GetButton(ButtonsType, (int)Buttons.LevelUpButton);

        Managers.isFast = PlayerPrefs.GetInt("Fast") == 1 ? true : false;
        Time.timeScale = Managers.isFast ? 1.5f : 1.0f;
        UpdateUIState();

        //TODO : 퀘스트(옮겨주기)

        OnChangeQuestInfo();

#if UNITY_ANDROID
        GetButton(ButtonsType, (int)Buttons.LeaderBoardButton).gameObject.SetActive(true);
#elif UNITY_IOS
        GetButton(ButtonsType, (int)Buttons.LeaderBoardButton).gameObject.SetActive(false);
#endif

        Managers.BuffM.RestoreFromSave();


        AllOff();
        StartSpawn();

        return true;
    }
    void OnDestroy()
    {
        Managers.StageM.readyEvent -= OnReady;
        Managers.StageM.playEvent -= OnPlay;
        Managers.StageM.bossPlayEvent -= OnBossPlay;
        Managers.StageM.clearEvent -= OnClear;
        Managers.StageM.deadEvent -= OnDead;
        Managers.StageM.dungeonEvent -= OnDungeon;
        Managers.StageM.dungeonClearEvent -= OnDungeonClear;
        Managers.StageM.dungeonFailEvent -= OnDungeonFail;

        Managers.GameM.OnGoodsChanged -= OnRefreshGoods;
        Managers.StageM.OnChangeCount -= OnCheckStageMonsterCount;
        Managers.StageM.OnChangeCount -= OnCheckTreasureTroveMonsterCount;
        Managers.QuestM.OnQuestDataChanged -= OnChangeQuestInfo;
        Managers.RelicM.OnChangeUI -= ChangeLevelUpButtonUI;
        Managers.OnAppPause -= HandleAppResume;

        if (Managers.CharacterM != null)
        {
            Managers.CharacterM.OnCharacterAdd -= OnRegisterCharacterEvents;
        }


    }

    void AllOff()
    {
        GetObject(GameObjectsType, (int)GameObjects.StageMonsterCountObject).SetActive(false);
        GetObject(GameObjectsType, (int)GameObjects.BossBoardObject).SetActive(false);
        GetObject(GameObjectsType, (int)GameObjects.DungeonBoardObject).SetActive(false);

        GetButton(ButtonsType, (int)Buttons.DeadFrameButton).gameObject.SetActive(false);

        GetObject(GameObjectsType, (int)GameObjects.ItemPopupObject).SetActive(false);
    }
    void UpdateUIState()
    {

        if (Managers.isFast) Managers.UpdateM.Register(_unscaledTickable: this);
        UpdateFastUI();

        //TODO : 여기서 이제 플레이어 상황 받아와서 bool값으로 처리해 주거나 더 좋은 방법생각해보자.
        GetImage(ImagesType, (int)Images.TutorialHandImage).gameObject.SetActive(false);

        // GetImage(ImagesType, (int)Images.Character1_Plus).gameObject.SetActive(false);
        // GetImage(ImagesType, (int)Images.Character2_Plus).gameObject.SetActive(false);
        // GetImage(ImagesType, (int)Images.Character3_Plus).gameObject.SetActive(false);
        // GetImage(ImagesType, (int)Images.Character4_Plus).gameObject.SetActive(false);
        // GetImage(ImagesType, (int)Images.Character5_Plus).gameObject.SetActive(false);
        // GetImage(ImagesType, (int)Images.Character6_Plus).gameObject.SetActive(false);

        //TODO : 캐릭터 수만큼


        for (int i = 0; i < 6; i++)
        {
            GetImage(ImagesType, (int)Images.Character1_Lock + i).gameObject.SetActive(false);
            GetImage(ImagesType, (int)Images.Character1_Icon + i).gameObject.SetActive(false);
            GetObject(GameObjectsType, (int)GameObjects.Character1_HpFrameObject + i).SetActive(false);
            GetObject(GameObjectsType, (int)GameObjects.Character1_MpFrameObject + i).SetActive(false);
            GetObject(GameObjectsType, (int)GameObjects.Character1_ReadyObject + i).SetActive(false);
        }



        CheckCharactersState();
    }
    //TODO :  코인, 쥬얼리 텍스트
    public void OnRefreshGoods()
    {
        GetText(TextsType, (int)Texts.CoinText).text = Utils.ToCurrencyString(Managers.GameM.Gold);
        GetText(TextsType, (int)Texts.DiaText).text = Utils.ToCurrencyString(Managers.GameM.Dia);
        CheckTexts();
    }
    async UniTaskVoid OnClickAnyButtons(Buttons _clickButtonType)
    {
        clickedButton = null;
        if (_clickButtonType != Buttons.LevelUpButton)
            Managers.SoundM.PlayButtonClick();

        //Managers.UIM.CloseAllPopup();
        switch (_clickButtonType)
        {
            case Buttons.SettingButton:
                var settingPopup = await Managers.UIM.ShowPopup<UI_SettingPopup>();
                break;

            //case Buttons.MailBoxButton:
            //    break;
            case Buttons.SavingModeButton:
                savingModePopup = await Managers.UIM.ShowPopup<UI_SavingMode>();
                isSavingMode = true;
                break;

            case Buttons.InventoryButton:
                //await Managers.UIM.ShowPopup<UI_Inventory>();
                break;

            case Buttons.FastButton:
                OnClickGetFast();
                break;
            case Buttons.BuffsButton:
                AdsBuffPopup = await Managers.UIM.ShowPopup<UI_AdsBuffPopup>();

                break;
            case Buttons.DailyMissionButton:
                await Managers.UIM.ShowPopup<UI_MissionPopup>();
                break;
            case Buttons.AchievementsButton:

                await Managers.UIM.ShowPopup<UI_AchievementPopup>();
                break;

            case Buttons.LeaderBoardButton:
#if UNITY_ANDROID
                Managers.GPGSM.ShowLeaderboardUI();
#endif
                break;
            case Buttons.QuestButton:
                bool isReward;
                int rewardcount;
                (isReward, rewardcount) = Managers.QuestM.GetQuestButton();

                if (!isReward) return;

                Vector3 pos = GetButton(ButtonsType, (int)Buttons.QuestButton).transform.position;
                var GoodsDirecting = Managers.ObjectM.Spawn<GoodsDirecting>(pos, Utils.GoodsDirectingDataID);
                GoodsDirecting.Init(Define.GoodsType.Dia, pos, rewardcount, false);

                Managers.SoundM.Play(Define.Sound.Effect, "Reward");
                break;


            case Buttons.StatButton:

                if (!Managers.UIM.ClickLock(0.5f)) return;

                if (selectedButton != statBtn && currentBottomPopup != null)
                {
                    currentBottomPopup.ClosePopup(false).Forget();
                }

                if (selectedButton == statBtn && currentBottomPopup != null)
                {
                    if (currentBottomPopup is UI_HeroStatPopup stat)
                    {
                        ScaleDownSelectButton();
                        GetObject(GameObjectsType, (int)GameObjects.StatButtonCloseObject).SetActive(false);
                        stat.ClosePopup(true).Forget();
                        Managers.SoundM.MuteEffectVolume(false);
                    }
                    return;
                }

                clickedButton = statBtn;
                GetObject(GameObjectsType, (int)GameObjects.StatButtonCloseObject).SetActive(true);
                ScaleUpSelectButton();

                UI_HeroStatPopup statPopup = await Managers.UIM.ShowPopup<UI_HeroStatPopup>(_isFade: true, _parent: GetPopUpLayer());
                currentBottomPopup = statPopup;
                currentBottomPopupButton = Buttons.StatButton;
                statPopup.OnThisPopupClosed = () =>
                {
                    ScaleDownSelectButton();
                    GetObject(GameObjectsType, (int)GameObjects.StatButtonCloseObject).SetActive(false);
                    if (currentBottomPopup == statPopup) currentBottomPopup = null;
                    Managers.SoundM.MuteEffectVolume(false);
                    statPopup = null;
                };




                break;

            case Buttons.HeroButton:
                if (!Managers.UIM.ClickLock(0.5f)) return;

                if (selectedButton != heroBtn && currentBottomPopup != null)
                {
                    currentBottomPopup.ClosePopup(false).Forget();
                }

                if (selectedButton == heroBtn && currentBottomPopup != null)
                {
                    if (currentBottomPopup is UI_HeroPopup hero)
                    {
                        ScaleDownSelectButton();
                        GetObject(GameObjectsType, (int)GameObjects.HeroButtonCloseObject).SetActive(false);
                        hero.ClosePopup(true).Forget();
                        Managers.SoundM.MuteEffectVolume(false);
                    }
                    return;
                }

                clickedButton = heroBtn;
                GetObject(GameObjectsType, (int)GameObjects.HeroButtonCloseObject).SetActive(true);
                ScaleUpSelectButton();

                UI_HeroPopup heroPopup = await Managers.UIM.ShowPopup<UI_HeroPopup>(_isFade: true, _parent: GetPopUpLayer());
                currentBottomPopup = heroPopup;
                currentBottomPopupButton = Buttons.HeroButton;
                heroPopup.OnThisPopupClosed = () =>
                {
                    ScaleDownSelectButton();
                    GetObject(GameObjectsType, (int)GameObjects.HeroButtonCloseObject).SetActive(false);
                    if (currentBottomPopup == heroPopup) currentBottomPopup = null;
                    Managers.SoundM.MuteEffectVolume(false);
                };

                break;

            case Buttons.RelicsButton:
                if (!Managers.UIM.ClickLock(0.5f)) return;

                if (selectedButton != relicsBtn && currentBottomPopup != null)
                {
                    currentBottomPopup.ClosePopup(false).Forget();
                }
                if (selectedButton == relicsBtn && currentBottomPopup != null)
                {
                    if (currentBottomPopup is UI_RelicsPopup relics)
                    {
                        ScaleDownSelectButton();
                        GetObject(GameObjectsType, (int)GameObjects.RelicsButtonCloseObject).SetActive(false);
                        relics.ClosePopup(false).Forget();
                        Managers.SoundM.MuteEffectVolume(false);
                    }
                    return;
                }

                clickedButton = relicsBtn;
                GetObject(GameObjectsType, (int)GameObjects.RelicsButtonCloseObject).SetActive(true);
                ScaleUpSelectButton();

                UI_RelicsPopup relicPopup = await Managers.UIM.ShowPopup<UI_RelicsPopup>(_isFade: false, _parent: GetPopUpLayer());
                currentBottomPopup = relicPopup;
                currentBottomPopupButton = Buttons.RelicsButton;
                relicPopup.OnThisPopupClosed = () =>
                {
                    ScaleDownSelectButton();
                    GetObject(GameObjectsType, (int)GameObjects.RelicsButtonCloseObject).SetActive(false);
                    if (currentBottomPopup == relicPopup) currentBottomPopup = null;
                    relicPopup = null;
                    Managers.SoundM.MuteEffectVolume(false);
                };
                break;

            case Buttons.DungeonButton:
                if (!Managers.UIM.ClickLock(0.5f)) return;

                if (selectedButton != dungeonBtn && currentBottomPopup != null)
                {
                    currentBottomPopup.ClosePopup(false).Forget();
                }
                if (selectedButton == dungeonBtn && currentBottomPopup != null)
                {
                    if (currentBottomPopup is UI_DungeonPopup dungeon)
                    {
                        ScaleDownSelectButton();
                        GetObject(GameObjectsType, (int)GameObjects.DungeonButtonCloseObject).SetActive(false);
                        dungeon.ClosePopup(true).Forget();
                        Managers.SoundM.MuteEffectVolume(false);
                    }
                    return;
                }

                clickedButton = dungeonBtn;
                GetObject(GameObjectsType, (int)GameObjects.DungeonButtonCloseObject).SetActive(true);
                ScaleUpSelectButton();

                UI_DungeonPopup dungeonPopup = await Managers.UIM.ShowPopup<UI_DungeonPopup>(_isFade: true, _parent: GetPopUpLayer());
                currentBottomPopup = dungeonPopup;
                currentBottomPopupButton = Buttons.DungeonButton;
                dungeonPopup.OnThisPopupClosed = () =>
                {
                    ScaleDownSelectButton();
                    GetObject(GameObjectsType, (int)GameObjects.DungeonButtonCloseObject).SetActive(false);
                    if (currentBottomPopup == dungeonPopup) currentBottomPopup = null;
                    Managers.SoundM.MuteEffectVolume(false);
                    dungeonPopup = null;
                };


                break;

            case Buttons.SmeltingButton:
                if (!Managers.UIM.ClickLock(0.5f)) return;
                if (selectedButton != smeltingBtn && currentBottomPopup != null)
                {
                    currentBottomPopup.ClosePopup(false).Forget();
                }
                if (selectedButton == smeltingBtn && currentBottomPopup != null)
                {
                    if (currentBottomPopup is UI_SmeltingPopup smelting)
                    {
                        ScaleDownSelectButton();
                        GetObject(GameObjectsType, (int)GameObjects.SmeltingButtonCloseObject).SetActive(false);
                        smelting.ClosePopup(false).Forget();
                        Managers.SoundM.MuteEffectVolume(false);
                    }
                    return;
                }

                clickedButton = smeltingBtn;
                GetObject(GameObjectsType, (int)GameObjects.SmeltingButtonCloseObject).SetActive(true);
                ScaleUpSelectButton();

                UI_SmeltingPopup smeltingPopup = await Managers.UIM.ShowPopup<UI_SmeltingPopup>(_isFade: false, _parent: GetPopUpLayer());
                currentBottomPopup = smeltingPopup;
                currentBottomPopupButton = Buttons.SmeltingButton;
                smeltingPopup.OnThisPopupClosed = () =>
                {
                    ScaleDownSelectButton();
                    GetObject(GameObjectsType, (int)GameObjects.SmeltingButtonCloseObject).SetActive(false);
                    if (currentBottomPopup == smeltingPopup) currentBottomPopup = null;
                    smeltingPopup = null;
                };

                break;

            case Buttons.ShopButton:
                if (!Managers.UIM.ClickLock(0.5f)) return;
                if (selectedButton != shopBtn && currentBottomPopup != null)
                {
                    currentBottomPopup.ClosePopup(false).Forget();
                }

                if (selectedButton == shopBtn && currentBottomPopup != null)
                {
                    if (currentBottomPopup is UI_ShopPopup shop)
                    {
                        ScaleDownSelectButton();
                        GetObject(GameObjectsType, (int)GameObjects.ShopButtonCloseObject).SetActive(false);
                        shop.ClosePopup(true).Forget();
                        Managers.SoundM.MuteEffectVolume(false);
                    }
                    return;
                }

                clickedButton = shopBtn;
                GetObject(GameObjectsType, (int)GameObjects.ShopButtonCloseObject).SetActive(true);
                ScaleUpSelectButton();

                UI_ShopPopup shopPopup = await Managers.UIM.ShowPopup<UI_ShopPopup>(_isFade: true, _parent: GetPopUpLayer());
                currentBottomPopup = shopPopup;
                currentBottomPopupButton = Buttons.ShopButton;
                shopPopup.OnThisPopupClosed = () =>
                {
                    ScaleDownSelectButton();
                    GetObject(GameObjectsType, (int)GameObjects.ShopButtonCloseObject).SetActive(false);
                    if (currentBottomPopup == shopPopup) currentBottomPopup = null;
                    shopPopup = null;
                    Managers.SoundM.MuteEffectVolume(false);
                };

                break;

            case Buttons.DeadFrameButton:
                Managers.StageM.isDead = false;
                Managers.StageM.StateChange(Define.StageState.Boss);

                break;
        }
    }

    private void CloseCurrentPopup()
    {
        if (currentBottomPopup != null)
        {

            ScaleDownSelectButton();
            GetObject(GameObjectsType, (int)GameObjects.StatButtonCloseObject).SetActive(false);
            GetObject(GameObjectsType, (int)GameObjects.HeroButtonCloseObject).SetActive(false);
            GetObject(GameObjectsType, (int)GameObjects.RelicsButtonCloseObject).SetActive(false);
            GetObject(GameObjectsType, (int)GameObjects.DungeonButtonCloseObject).SetActive(false);
            GetObject(GameObjectsType, (int)GameObjects.ShopButtonCloseObject).SetActive(false);
            currentBottomPopup.ClosePopup(true).Forget();
            currentBottomPopup = null;
        }
    }
    public void SetBuffs(Define.BuffType _type, bool _isLock)
    {
        switch (_type)
        {
            case Define.BuffType.AttackUp:
                GetObject(GameObjectsType, (int)GameObjects.AttackBuffLockObject).SetActive(!_isLock);
                break;

            case Define.BuffType.GoldUp:
                GetObject(GameObjectsType, (int)GameObjects.DropBuffLockObject).SetActive(!_isLock);
                break;

            case Define.BuffType.CriticalUp:
                GetObject(GameObjectsType, (int)GameObjects.CriticalBuffLockObject).SetActive(!_isLock);
                break;
        }



    }
    public void OnClickGetFast()
    {
        Managers.SoundM.PlayButtonClick();
        bool fast = !Managers.isFast;
        if (fast)
        {
            if (FastremainTime <= 0.0f)
            {
                Action rewardedAction = () => { FastremainTime = 600f; };

                Action resumeAction = async () =>
                {
                    await CheckFast(fast);
                    UpdateFastUI();
                    Managers.UpdateM.Register(_unscaledTickable: this);
                    StartBlinkTween();
                    Managers.SoundM.Play(Define.Sound.Effect, "Reward");
                };

                Managers.AdM.ShowRewardedAd(rewardedAction, resumeAction);
            }
            else
            {
                CheckFast(fast).Forget();
                UpdateFastUI();
                Managers.UpdateM.Register(_unscaledTickable: this);
                StartBlinkTween();
            }

        }
        else
        {
            CheckFast(fast).Forget();
            UpdateFastUI();
            Managers.UpdateM.UnRegister(_unscaledTickable: this);
            KillBlinkTween();
        }
    }

    public void UpdateFastUI()
    {
        GetImage(ImagesType, (int)Images.FastLockImage).gameObject.SetActive(Managers.isFast);
        GetImage(ImagesType, (int)Images.FastOnImage).gameObject.SetActive(Managers.isFast);
        GetObject(GameObjectsType, (int)GameObjects.FastSliderObject).gameObject.SetActive(Managers.isFast);
    }

    public async UniTask CheckFast(bool _fast)
    {
        Managers.isFast = _fast;
        PlayerPrefs.SetInt("Fast", _fast == true ? 1 : 0);
        await UniTask.Yield();

        Time.timeScale = _fast ? 1.5f : 1.0f;
    }

    public void StartBlinkTween()
    {
        KillBlinkTween();

        blinkTween = GetImage(ImagesType, (int)Images.FastOnImage).DOFade(0.0f, 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }

    public void KillBlinkTween()
    {
        if (blinkTween != null && blinkTween.IsActive())
        {
            blinkTween.Kill();
            blinkTween = null;
        }
    }


    private void ScaleUpSelectButton()
    {
        if (clickedButton == null) return;

        if (selectedButton != null && selectedButton != clickedButton)
        {
            selectedButton.transform.DOScale(Vector3.one, 0.2f);
        }

        clickedButton.transform.DOScale(Vector3.one * 1.3f, 0.2f);

        selectedButton = clickedButton;
    }

    private void ScaleDownSelectButton()
    {
        if (selectedButton != null)
        {
            selectedButton.transform.DOScale(Vector3.one, 0.2f);
            selectedButton = null;
        }
    }

    //TODO : 퀘스트
    void OnChangeQuestInfo()
    {
        GetText(TextsType, (int)Texts.QuestTitleText).text = $"퀘스트 {Managers.GameM.QuestCount + 1}";
        GetText(TextsType, (int)Texts.QuestDescriptionText).text = Managers.QuestM.Localization_Counting(Managers.QuestM.GetState());
        GetText(TextsType, (int)Texts.QuestTutorialText).text = $"({Managers.QuestM.Counting(Managers.QuestM.GetState())} / {Managers.QuestM.GetQuestValue()})";
        GetText(TextsType, (int)Texts.QuestTutorialText).color = Managers.QuestM.GetCountColor();
        GetText(TextsType, (int)Texts.RewardItemText).text = Managers.QuestM.GetReward().ToString();
        GetImage(ImagesType, (int)Images.TutorialHandImage).gameObject.SetActive(Managers.QuestM.isGetReward());
    }

    void OnCheckStageMonsterCount()
    {
        if (Managers.StageM.isDungeon) return;

        float maxCount = (float)Managers.StageM.maxCount;
        if (maxCount <= 0) maxCount = 1.0f;

        float rawValue = (float)Managers.StageM.COUNT / maxCount;
        float clampedValue = Mathf.Clamp01(rawValue);

        if (clampedValue >= 1.0f && Managers.StageM.stageState != Define.StageState.Boss)
        {
            Managers.StageM.StateChange(Define.StageState.Boss);
        }
        GetImage(ImagesType, (int)Images.StageMonsterCountImage).fillAmount = clampedValue;
        GetText(TextsType, (int)Texts.StageMonsterCountText).text = string.Format("{0:0.0}", clampedValue * 100.0f) + "%";

    }

    void OnCheckTreasureTroveMonsterCount()
    {
        if (Managers.StageM.isDungeon)
        {
            int count = Managers.SpawnM.SpawnMaxCount - Managers.StageM.COUNT;
            GetText(TextsType, (int)Texts.TreasureTroveCountText).text = $"({count})";

            if (count <= 0)
            {
                Managers.StageM.StateChange(Define.StageState.DungeonClear);
            }
        }
    }

    void OnRegisterCharacterEvents(PlayerController _pc)
    {
        _pc.OnPlayerDataUpdate -= OnPlayerStatChange;
        _pc.OnPlayerDataUpdate += OnPlayerStatChange;

        CheckCharactersState();

    }
    public void UpdateBossInfo(MonsterController _cc)
    {
        float hp = (float)_cc.HP / (float)_cc.MaxHP;
        if (hp <= 0.0f)
        {
            hp = 0f;
        }

        if (Managers.StageM.isDungeon)
        {
            GetImage(ImagesType, (int)Images.GoldDungeonHpImage).fillAmount = (float)hp;
            GetText(TextsType, (int)Texts.GoldDungeonHpText).text = string.Format("{0:0.0}", hp * 100.0f) + "%";
        }
        else
        {
            GetImage(ImagesType, (int)Images.BossHpImage).fillAmount = (float)hp;
            GetText(TextsType, (int)Texts.BossHPText).text = string.Format("{0:0.0}", hp * 100.0f) + "%";
        }



    }


    public async UniTaskVoid UpdateDungeonInfo()
    {
        float totalTime = 30.0f;
        float currentTime = totalTime;

        var cts = this.GetCancellationTokenOnDestroy();
        try
        {

            while (currentTime >= 0.0f)
            {

                if (!Managers.StageM.isDungeon) break;

                currentTime -= Time.deltaTime;
                float ratio = currentTime / totalTime;

                int seconds = Mathf.CeilToInt(currentTime);

                GetImage(ImagesType, (int)Images.DungeonTimeImage).fillAmount = ratio;
                GetText(TextsType, (int)Texts.DungeonTimeText).text = seconds.ToString() + "/s";

                await UniTask.Yield(PlayerLoopTiming.Update, cts);
            }


            if (Managers.StageM.isDungeon)
            {
                GetText(TextsType, (int)Texts.DungeonTimeText).text = "0.0";
                Managers.StageM.StateChange(Define.StageState.DungeonFail);
            }

        }
        catch (Exception e) { }
    }

    public void ResetStageBoard()
    {
        GetImage(ImagesType, (int)Images.StageMonsterCountImage).fillAmount = 0;
        GetText(TextsType, (int)Texts.StageMonsterCountText).text = "0%";
    }

    public void ResetBossBoard()
    {
        GetImage(ImagesType, (int)Images.BossHpImage).fillAmount = 1f;
        GetText(TextsType, (int)Texts.BossHPText).text = "100%";

        int stageValue = Managers.GameM.Stage;
        int stageForward = ((stageValue - 1) / 20) + 1;
        int stageBack = ((stageValue - 1) % 20) + 1;

        GetText(TextsType, (int)Texts.BossBoardStageText).text = stageForward.ToString() + " - " + stageBack.ToString();
    }

    public void ResetDungeonBoard()
    {
        GetImage(ImagesType, (int)Images.DungeonTimeImage).fillAmount = 1f;
        GetText(TextsType, (int)Texts.DungeonTimeText).text = "30/s";

        GetImage(ImagesType, (int)Images.GoldDungeonHpImage).fillAmount = 1f;
        GetText(TextsType, (int)Texts.GoldDungeonHpText).text = "100%";

        GetText(TextsType, (int)Texts.TreasureTroveCountText).text = "30";


    }
    public async void OnClickInventory()
    {
        Managers.SoundM.PlayButtonClick();
        await Managers.UIM.ShowPopup<UI_Inventory>();
    }

    #region 하단 캐릭터들 정보 업데이트
    public async void OnClickCharacterPlus(int _index)
    {
        Managers.SoundM.PlayButtonClick();
        //await Managers.UIM.ShowPopup<UI_HeroPopup>(_isFade: true);
        if (selectedButton != heroBtn && currentBottomPopup != null)
        {
            currentBottomPopup.ClosePopup(false).Forget();
        }

        if (selectedButton == heroBtn && currentBottomPopup != null)
        {
            if (currentBottomPopup is UI_HeroPopup hero)
            {
                ScaleDownSelectButton();
                GetObject(GameObjectsType, (int)GameObjects.HeroButtonCloseObject).SetActive(false);
                hero.ClosePopup(true).Forget();
            }
            return;
        }

        clickedButton = heroBtn;
        GetObject(GameObjectsType, (int)GameObjects.HeroButtonCloseObject).SetActive(true);
        ScaleUpSelectButton();

        UI_HeroPopup heroPopup = await Managers.UIM.ShowPopup<UI_HeroPopup>(_isFade: true, _parent: GetPopUpLayer());
        currentBottomPopup = heroPopup;
        heroPopup.OnThisPopupClosed = () =>
        {
            ScaleDownSelectButton();
            GetObject(GameObjectsType, (int)GameObjects.HeroButtonCloseObject).SetActive(false);
            if (currentBottomPopup == heroPopup) currentBottomPopup = null;
        };

    }


    public void CheckCharactersState()
    {
        int index = 1;
        for (int i = 0; i < Managers.CharacterM.Characters.Length - 1; i++)
        {
            if (Managers.CharacterM.Characters[i + 1] != null)
            {

                GetButton(ButtonsType, (int)Buttons.Character1_PlusButton + i).gameObject.SetActive(false);
                GetImage(ImagesType, (int)Images.Character1_Icon + i).gameObject.SetActive(true);
                GetImage(ImagesType, (int)Images.Character1_Icon + i).sprite = Managers.ResourceM.GetAtlas(Managers.CharacterM.Characters[i + 1].data.Name);
                GetImage(ImagesType, (int)Images.Character1_Icon + i).SetNativeSize();
                //GetImage(ImagesType, (int)Images.Character1_Icon + (i - 1)).rectTransform.localScale = GetImage(ImagesType, (int)Images.Character1_Icon + (i - 1)).rectTransform.localScale / 6;


                //지금 스폰 되어있는거랑 아닌거랑 비교하기?
                if (Managers.CharacterM.players[i + 1] != null)
                {
                    //만약 생성되어있다면.
                    GetObject(GameObjectsType, (int)GameObjects.Character1_ReadyObject + i).gameObject.SetActive(false);
                    GetObject(GameObjectsType, (int)GameObjects.Character1_HpFrameObject + i).gameObject.SetActive(true);
                    GetObject(GameObjectsType, (int)GameObjects.Character1_MpFrameObject + i).gameObject.SetActive(true);
                }
                else
                {
                    GetObject(GameObjectsType, (int)GameObjects.Character1_ReadyObject + i).gameObject.SetActive(true);
                    GetObject(GameObjectsType, (int)GameObjects.Character1_HpFrameObject + i).gameObject.SetActive(false);
                    GetObject(GameObjectsType, (int)GameObjects.Character1_MpFrameObject + i).gameObject.SetActive(false);
                }

                GetObject(GameObjectsType, (int)GameObjects.Character1_Object + i).transform.SetSiblingIndex(index);
                index++;
            }
            //TODO : 이거 나중에 오류나면 Icon으로 뺀것빼문임. 
            else
            {
                GetButton(ButtonsType, (int)Buttons.Character1_PlusButton + i).gameObject.SetActive(true);
                GetImage(ImagesType, (int)Images.Character1_Icon + i).gameObject.SetActive(false);
                GetImage(ImagesType, (int)Images.Character1_Icon + i).sprite = null;
                GetObject(GameObjectsType, (int)GameObjects.Character1_ReadyObject + i).gameObject.SetActive(false);
                GetObject(GameObjectsType, (int)GameObjects.Character1_HpFrameObject + i).gameObject.SetActive(false);
                GetObject(GameObjectsType, (int)GameObjects.Character1_MpFrameObject + i).gameObject.SetActive(false);
            }
        }
    }

    public void OnPlayerStatChange(PlayerController _pc)
    {
        if (_pc.index == Managers.CharacterM.Characters.Length) return;

        GetImage(ImagesType, (int)Images.Character0_HpFillImage + (_pc.index)).fillAmount = (float)_pc.HP / (float)_pc.MaxHP;
        GetText(TextsType, (int)Texts.Character0_HpText + (_pc.index)).text = $"{Utils.ToCurrencyString(_pc.HP)} / {Utils.ToCurrencyString(_pc.MaxHP)}";
        GetImage(ImagesType, (int)Images.Character0_MpFillImage + (_pc.index)).fillAmount = (float)_pc.MP / (float)_pc.MaxMp;
        GetText(TextsType, (int)Texts.Character0_MpText + (_pc.index)).text = _pc.MP.ToString() + " / " + _pc.MaxMp.ToString();
    }

    #endregion

    #region Event
    public void OnReady()
    {
        AsyncFadeInOut(true).Forget();

        CheckCharactersState();
    }
    public void OnPlay(Define.StageState _state)
    {
        switch (_state)
        {
            case Define.StageState.Play:
                AllOff();
                if (Managers.StageM.isDead)
                {
                    GetButton(ButtonsType, (int)Buttons.DeadFrameButton).gameObject.SetActive(true);
                }
                else
                {
                    GetObject(GameObjectsType, (int)GameObjects.StageMonsterCountObject).SetActive(true);
                    ResetStageBoard();
                }

                break;

            case Define.StageState.Dungeon:
                UpdateDungeonInfo().Forget();
                break;
        }

    }

    public void OnBossPlay()
    {
        AllOff();

        if (Managers.StageM.isDungeon)
        {
            GetObject(GameObjectsType, (int)GameObjects.DungeonBoardObject).SetActive(true);
            UpdateDungeonInfo().Forget();
        }
        else
        {
            GetObject(GameObjectsType, (int)GameObjects.BossBoardObject).SetActive(true);
            ResetBossBoard();
        }



    }

    public void OnClear()
    {
        AllOff();
    }

    public void OnDead()
    {
        AllOff();
    }


    public void OnDungeon(int _dungeonDataID)
    {
        AllOff();
        ResetDungeonBoard();
        GetObject(GameObjectsType, (int)GameObjects.DungeonBoardObject).SetActive(true);


        type = Define.DungeonType.TreasureTrove;
        dungeonDataID = _dungeonDataID;
        if (_dungeonDataID == 70000) type = Define.DungeonType.TreasureTrove;
        else type = Define.DungeonType.GoldDungeon;
        int level = 0;
        switch (_dungeonDataID)
        {
            case 70000:
                type = Define.DungeonType.TreasureTrove;
                level = Managers.GameM.gameData.DungeonClearLevel[0] + 1;
                GetText(TextsType, (int)Texts.DungeonBoardStageText).text = $"보물창고 Lv.{level}";
                GetObject(GameObjectsType, (int)GameObjects.GoldDungeonObject).SetActive(false);
                GetObject(GameObjectsType, (int)GameObjects.TreasureTroveObject).SetActive(true);
                break;

            case 70001:
                type = Define.DungeonType.GoldDungeon;
                level = Managers.GameM.gameData.DungeonClearLevel[1] + 1;
                GetText(TextsType, (int)Texts.DungeonBoardStageText).text = $"골드던전 Lv.{level}";
                GetObject(GameObjectsType, (int)GameObjects.TreasureTroveObject).SetActive(false);
                GetObject(GameObjectsType, (int)GameObjects.GoldDungeonObject).SetActive(true);
                break;
        }


        OnReady();

    }

    public async void OnDungeonClear()
    {
        var popup = await Managers.UIM.ShowPopup<UI_DungeonResultInfoPopup>();
        popup.SetInfo(dungeonDataID, true);
        Managers.GameM.gameData.DungeonClearLevel[(int)type]++;
        Managers.GameM.SetDungeonClear(type, Managers.GameM.gameData.DungeonClearLevel[(int)type]);
        OnClear();
    }

    public async void OnDungeonFail()
    {
        var popup = await Managers.UIM.ShowPopup<UI_DungeonResultInfoPopup>();
        popup.SetInfo(dungeonDataID, false);
        OnDead();
    }

    #endregion

    #region 레벨업 버튼 애니메이션 관련
    void ClickDown()
    {
        ExpUPAnim();
        coroutine = StartCoroutine(coPush());

    }

    void ClickUp()
    {
        isLevelUpButtonPush = false;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        timer = 0.0f;
    }

    double needGold;
    public void OnClickLevelupButton()
    {
        Managers.SoundM.Play(Define.Sound.Effect, "LevelUpButton");
        needGold = Utils.CalculatedValue(Utils.Datas.levelData.Base_Gold, Managers.GameM.Level, Utils.Datas.levelData.Player_Gold);
        if (Managers.GameM.Gold >= needGold)
        {
            Managers.GameM.Gold -= needGold;
            Managers.PlayerM.ExpUp();
            CheckTexts();
        }


    }

    //TODO : 반드시 게임씬에 들어가있는것들 나누기
    public void CheckTexts()
    {
        needGold = Utils.CalculatedValue(Utils.Datas.levelData.Base_Gold, Managers.GameM.Level, Utils.Datas.levelData.Player_Gold);

        GetText(TextsType, (int)Texts.StageStateText).text = Managers.StageM.isDead ? "반복중..." : "진행중...";
        GetText(TextsType, (int)Texts.StageStateText).color = Managers.StageM.isDead ? Color.yellow : Color.blue;

        int stageValue = Managers.GameM.Stage;
        int stageForward = ((stageValue - 1) / 20) + 1;
        int stageBack = ((stageValue - 1) % 20) + 1;

        GetText(TextsType, (int)Texts.StageText).text = stageForward.ToString() + " - " + stageBack.ToString();



        ChangeLevelUpButtonUI();


        GetText(TextsType, (int)Texts.CoinText).text = Utils.ToCurrencyString(Managers.GameM.Gold);
        GetText(TextsType, (int)Texts.DiaText).text = Utils.ToCurrencyString(Managers.GameM.Dia);

        CheckPlayer();
    }

    void CheckPlayer()
    {
        GetText(TextsType, (int)Texts.CharacterLevelText).text = $"LV : {Managers.GameM.Level}";
        GetText(TextsType, (int)Texts.UserNameText).text = Managers.GameM.gameData.playerName;
        GetText(TextsType, (int)Texts.UserCombatPowerText).text = Utils.ToCurrencyString(Managers.PlayerM.AverageCombatPower());
    }

    public void ChangeLevelUpButtonUI()
    {
        GetImage(ImagesType, (int)Images.Exp_FillImage).fillAmount = Managers.PlayerM.ExpPercent();
        GetText(TextsType, (int)Texts.ExpText).text = string.Format("{0:0.00}", Managers.PlayerM.ExpPercent() * 100.0f) + "%";
        GetText(TextsType, (int)Texts.AttackText).text = $"+ {Utils.ToCurrencyString(Utils.Datas.levelData.Damage())}";
        GetText(TextsType, (int)Texts.HpText).text = $"+ {Utils.ToCurrencyString(Utils.Datas.levelData.HP())}";
        GetText(TextsType, (int)Texts.NeedLevelUpText).text = Utils.ToCurrencyString(needGold);
        GetText(TextsType, (int)Texts.NeedLevelUpText).color = Utils.CoinCheck(needGold) ? Color.green : Color.red;
        float exp = Managers.PlayerM.NextExp();
        GetText(TextsType, (int)Texts.GetExpText).text = $"<color=#00FF00>EXP</color> + {string.Format("{0:0.00}", exp)}%";
    }

    IEnumerator coPush()
    {
        yield return new WaitForSeconds(1f);
        isLevelUpButtonPush = true;
    }
    void StartSpawn()
    {
        Managers.SpawnM.Init();
        int[] savedIds = Managers.GameM.gameData.TeamPlacementID;
        if (savedIds != null)
        {
            for (int i = 0; i < savedIds.Length; i++)
            {
                int id = savedIds[i];
                if (id <= 0) continue;

                if (Managers.DataM.CreatureDataDic.TryGetValue(id, out var data))
                {
                    Managers.CharacterM.SetCharacter(i, data.Name);
                }
            }
        }
        Managers.StageM.StateChange(Define.StageState.Ready);
    }

    Coroutine coroutine;
    void ExpUPAnim()
    {
        GetButton(ButtonsType, (int)Buttons.LevelUpButton).transform.DORewind();
        GetButton(ButtonsType, (int)Buttons.LevelUpButton).transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.25f);
        OnClickLevelupButton();
    }

    #endregion

    #region ItemPopup(TOP)

    private DG.Tweening.Sequence popupSequence;
    public void SetHighGradeItem(Data.ItemData _data)
    {
        GetImage(ImagesType, (int)Images.ItemPopupFrameImage).sprite = Managers.ResourceM.GetAtlas(_data.ItemGrade.ToString());
        GetImage(ImagesType, (int)Images.ItemPopupItemImage).sprite = Managers.ResourceM.GetAtlas(_data.Name);
        //TODO : Localiztion
        //GetText(TextsType, (int)Texts.ItemPopupText).text = Utils.StringToColorGrade(_data.ItemGrade) + _data.NameKR + "</color>을 획득하였습니다";
        GetText(TextsType, (int)Texts.ItemPopupText).text = string.Format(Managers.LocalizationM.localData["PopupGetItem02"].GetData(), Utils.StringToColorGrade(_data.ItemGrade) + _data.NameKR);

        PlayLegendaryPopupAnim();
    }
    public void PlayLegendaryPopupAnim()
    {

        if (popupSequence != null && popupSequence.IsActive())
        {
            popupSequence.Kill();
            popupSequence = null;
            GetObject(GameObjectsType, (int)GameObjects.ItemPopupObject).SetActive(false);
        }
        GetObject(GameObjectsType, (int)GameObjects.ItemPopupObject).SetActive(true);


        ItemPopupObjectRect.localScale = new Vector3(0f, 1f, 1f);
        ItemPopupFrameObjectRect.localScale = Vector3.zero;

        var seq = DOTween.Sequence();
        popupSequence = seq;
        seq.SetUpdate(UpdateType.Normal, true);
        seq.Append(ItemPopupObjectRect.DOScaleX(1.1f, 0.1f));
        seq.Append(ItemPopupObjectRect.DOScaleX(1.0f, 0.05f));

        seq.Append(ItemPopupFrameObjectRect.DOScale(new Vector3(1.1f, 0.9f, 1.0f), 0.1f));
        seq.Append(ItemPopupFrameObjectRect.DOScale(new Vector3(1f, 1.1f, 1f), 0.1f));
        seq.Append(ItemPopupFrameObjectRect.DOScale(Vector3.one, 0.05f));

        float startTime = 0.2f;

        seq.Insert(startTime, ItemPopupFrameObjectRect.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.1f));
        seq.Insert(startTime + 0.1f, ItemPopupFrameObjectRect.DOScale(new Vector3(1f, 1.1f, 1f), 0.05f));
        seq.Insert(startTime + 0.15f, ItemPopupFrameObjectRect.DOScale(Vector3.one, 0.05f));

        float displayTime = 2f;
        seq.AppendInterval(displayTime);
        seq.OnComplete(() =>
        {
            popupSequence = null;
            PlayLegendaryPopupCloseAnim();
        });

        seq.Play();
    }
    public void PlayLegendaryPopupCloseAnim()
    {
        if (popupSequence != null && popupSequence.IsActive())
        {
            popupSequence.Kill();
            popupSequence = null;
        }

        var seq = DOTween.Sequence();
        popupSequence = seq;
        seq.SetUpdate(UpdateType.Normal, true);

        seq.Append(ItemPopupFrameObjectRect.DOScale(new Vector3(1f, 1.1f, 1f), 0.05f));
        seq.Append(ItemPopupFrameObjectRect.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.1f));
        seq.Append(ItemPopupFrameObjectRect.DOScale(Vector3.zero, 0.1f));

        seq.Append(ItemPopupObjectRect.DOScaleX(1.1f, 0.1f));
        seq.Append(ItemPopupObjectRect.DOScaleX(0f, 0.15f));

        seq.OnComplete(() =>
                {
                    popupSequence = null;
                    GetObject(GameObjectsType, (int)GameObjects.ItemPopupObject).SetActive(false);
                });

        seq.Play();
    }
    #endregion

    #region ItemPopup(Bottom)
    private Dictionary<TextMeshProUGUI, CancellationTokenSource> cancellationSources =
    new Dictionary<TextMeshProUGUI, CancellationTokenSource>();
    public void GetItem(Data.ItemData _data)
    {
        TextMeshProUGUI slotToUse = null;
        bool AllActive = true;


        for (int i = 0; i < itemTexts.Count; i++)
        {

            if (!itemTexts[i].gameObject.activeSelf)
            {
                slotToUse = itemTexts[i];
                AllActive = false;
                break;
            }
        }


        if (AllActive)
        {
            slotToUse = FindOldSlot();
            if (slotToUse == null)
            {
                Debug.LogWarning("아이템 메시지 슬롯이 모두 꽉 찼습니다.");
                return;
            }
        }
        else
        {
            CleanUpSlot(slotToUse);
        }

        foreach (var text in itemTexts)
        {
            if (text.gameObject.activeSelf && text != slotToUse)
            {
                RectTransform rect = text.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + 50f);
            }
        }

        RectTransform newRect = slotToUse.GetComponent<RectTransform>();
        newRect.anchoredPosition = Vector2.zero;
        slotToUse.gameObject.SetActive(true);

        Color currentSlotColor = slotToUse.color;
        if (currentSlotColor.a < 1f)
        {
            currentSlotColor.a = 1f;
            slotToUse.color = currentSlotColor;
        }

        //slotToUse.text = "아이템을 획득하였습니다 : " + Utils.StringToColorGrade(_data.ItemGrade) + "[" + _data.NameKR + "]</color>";
        slotToUse.text = string.Format(Managers.LocalizationM.localData["PopupGetItem01"].GetData(), Utils.StringToColorGrade(_data.ItemGrade) + "[" + Managers.LocalizationM.localData[_data.Name].GetData() + "]</color>");
        PlayTextFadeOut(slotToUse).Forget();


        if ((int)_data.ItemGrade >= (int)Define.Grade.Rare)
        {
            SetHighGradeItem(_data);

        }
    }


    public async UniTask PlayTextFadeOut(TextMeshProUGUI _text)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;
        cancellationSources[_text] = cts;

        RectTransform rect = _text.GetComponent<RectTransform>();
        Color startColor = _text.color;
        startColor.a = 1f;
        _text.color = startColor;

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f), ignoreTimeScale: false, cancellationToken: token);
            await _text.DOFade(0f, 0.5f).AsyncWaitForCompletion();
        }
        catch (OperationCanceledException)
        { }
        finally
        {
            if (cancellationSources.ContainsKey(_text))
            {
                cancellationSources.Remove(_text);
                cts.Dispose(); // 중요!
            }

            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                //rect.gameObject.SetActive(false);
            }
        }
    }
    public TextMeshProUGUI FindOldSlot()
    {
        //foreach(var text in itemTexts)
        //{
        //    if (!text.gameObject.activeSelf) return text;
        //}

        TextMeshProUGUI oldestText = null;
        float maxY = float.MinValue;

        foreach (var text in itemTexts)
        {
            if (text.gameObject.activeSelf)
            {
                RectTransform rect = text.GetComponent<RectTransform>();
                if (rect.anchoredPosition.y > maxY)
                {
                    maxY = rect.anchoredPosition.y;
                    oldestText = text;
                }
            }


        }

        if (oldestText != null)
        {
            CleanUpSlot(oldestText);

            oldestText.DOKill(true);
            oldestText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            oldestText.gameObject.SetActive(false);

            return oldestText;
        }

        return null;
    }

    void CleanUpSlot(TextMeshProUGUI _slot)
    {
        _slot.DOKill(true);

        Color cancelColor = _slot.color;
        cancelColor.a = 0;
        _slot.color = cancelColor;

        CancellationTokenSource ctsToCancel = null;
        if (cancellationSources.TryGetValue(_slot, out ctsToCancel))
        {
            cancellationSources.Remove(_slot);
        }
        if (ctsToCancel != null)
        {
            ctsToCancel.Cancel();
            ctsToCancel.Dispose();
        }
    }

    #endregion

    #region LevelUpButton
    float timer = 0f;
    bool isLevelUpButtonPush = false;
    public void Tick(float _deltaTime)
    {

        if (isLevelUpButtonPush)
        {
            timer += _deltaTime;
            if (timer >= 0.01f)
            {
                timer = 0.0f;
                ExpUPAnim();
            }
        }

        //TODO : 지울거임
        if (Input.GetKeyDown(KeyCode.G))
        {
            Managers.GameM.gameData.DungeonKey[0]++;
            Managers.GameM.gameData.DungeonKey[1]++;
            Managers.InventoryM.GetItem(Managers.GameM.gameData.Item_Data["Axe"].data);
            Managers.InventoryM.GetItem(Managers.GameM.gameData.Item_Data["Gold_Dice"].data);
            Managers.InventoryM.GetItem(Managers.GameM.gameData.Item_Data["GoddessTears"].data);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Managers.UIM.popupStack.Count > 0)
            {
                var topPopup = Managers.UIM.popupStack.Peek();

                if (currentBottomPopup == topPopup)
                {
                    currentBottomPopup.ClosePopup(true).Forget();
                    currentBottomPopup = null;
                    return;
                }

                if (topPopup != null && topPopup.gameObject.activeSelf)
                {
                    Managers.UIM.ClosePopup(topPopup).Forget();
                    return;
                }
            }
        }
    }

    public void UnscaledTick(float _unscaledDeltaTime)
    {
        if (!Managers.isFast) return;

        if (FastremainTime > 0.0f)
        {
            FastremainTime -= _unscaledDeltaTime;
            int min = Mathf.FloorToInt(FastremainTime / 60f);
            int hour = Mathf.FloorToInt(FastremainTime % 60f);
            string timeString = string.Format("{0:00} : {1:00}", min, hour);
            GetText(TextsType, (int)Texts.FastTimeText).text = timeString;
            GetImage(ImagesType, (int)Images.FastSliderFill).fillAmount = FastremainTime / 600f;
        }
        else
        {
            FastremainTime = 0.0f;
            CheckFast(false);
            UpdateFastUI();
            if (Managers.isFast) Managers.UpdateM.UnRegister(_unscaledTickable: this);
        }
    }
    #endregion

    #region FadeInOut
    //TODO : 그냥 const로 만들어버릴까
    float fadeDuration = 1;
    public async UniTask AsyncFadeInOut(bool _isFadeIn, bool _isSibling = false)
    {
        Image fadeImage = GetImage(ImagesType, (int)Images.FadeImage);

        if (_isSibling)
        {

            fadeImage.transform.SetParent(Managers.UIM.Root.transform, false);
            fadeImage.transform.SetAsLastSibling();
        }
        else
        {

            fadeImage.transform.SetParent(this.transform, false);
            fadeImage.transform.SetSiblingIndex(0);
        }

        await CoFadeInOutAsync(_isFadeIn);
    }

    private async UniTask CoFadeInOutAsync(bool _isFadeIn)
    {
        Image fadeImage = GetImage(ImagesType, (int)Images.FadeImage);

        float current = 0.0f;
        float percent = 0.0f;
        float start = _isFadeIn ? 1.0f : 0.0f;
        float end = _isFadeIn ? 0.0f : 1.0f;
        fadeImage.raycastTarget = true;

        while (percent < 1.0f)
        {
            current += Time.deltaTime * 2;
            percent = current / fadeDuration;
            float LerpPos = Mathf.Lerp(start, end, percent);
            fadeImage.color = new Color(0, 0, 0, LerpPos);

            await UniTask.Yield();
        }

        fadeImage.raycastTarget = false;
    }


    #endregion

    //public void StartTutorial(Button _button)
    //{
    //    GameObject panel = GetObject(GameObjectsType, (int)GameObjects.TutorialObject);
    //    panel.SetActive(true);
    //    GameObject copy = Instantiate(_button.gameObject);

    //    RectTransform copyRect = copy.GetComponent<RectTransform>();
    //    RectTransform originRect = _button.GetComponent<RectTransform>();

    //    copy.transform.SetParent(panel.transform);

    //    copyRect.anchorMin = new Vector2(0.5f, 0.5f);
    //    copyRect.anchorMax = new Vector2(0.5f, 0.5f);
    //    copyRect.pivot = new Vector2(0.5f, 0.5f);
    //    copyRect.sizeDelta = originRect.sizeDelta;

    //    copyRect.position = originRect.position;
    //    copyRect.localScale = Vector3.one;

    //    Button copyButton = copy.GetComponent<Button>();
    //    copyButton.onClick = _button.onClick;
    //    copyButton.onClick.AddListener(() => EndTutorial());
    //}
    //public void EndTutorial()
    //{
    //    //GameObject panel = GetObject(GameObjectsType, (int)GameObjects.TutorialObject);
    //    for (int i = 0; i < panel.transform.childCount; i++)
    //    {
    //        Destroy(panel.transform.GetChild(i).gameObject);
    //    }
    //    panel.SetActive(false);
    //}

    public async UniTask OpenShopFormOtherUI()
    {
        if (currentBottomPopup is UI_ShopPopup) return;


        if (currentBottomPopup != null && currentBottomPopup.gameObject.activeSelf)
        {
            var tempPopup = currentBottomPopup;
            currentBottomPopup = null;
            await tempPopup.ClosePopup(false);
        }


        OnClickAnyButtons(Buttons.ShopButton).Forget();
    }

    void HandleAppResume(bool _isPause)
    {
        if (!_isPause)
        {
            Managers.FirebaseM.SyncDataOnly().Forget();

            CheckOffLineReward();
        }
    }

    void CheckOffLineReward()
    {
        double elapsedSeconds = GetOfflineSeconds();

        if (elapsedSeconds >= 10.0d)
        {
            Managers.UIM.ShowPopup<UI_OfflinePopup>().Forget();
        }
    }

    private double GetOfflineSeconds()
    {
        long lastTicks = Managers.GameM.gameData.LastSaveTimeTicks;
        long nowTicks = TimerNTP.NowTime.Ticks;

        if (lastTicks == 0 || lastTicks > nowTicks) return 0;

        long elapsedTicks = nowTicks - lastTicks;
        return (double)elapsedTicks / TimeSpan.TicksPerSecond;
    }
}
