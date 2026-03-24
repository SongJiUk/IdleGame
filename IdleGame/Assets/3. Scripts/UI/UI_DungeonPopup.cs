using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DungeonPopup : UI_Popup, ITickable
{
    #region Enum

    enum GameObjects
    {
        RawImage
    }

    enum Texts
    {
        TimeText,
        CrystalCountText,
        SapphireCountText,
        TreasureTroveCompenstaionText,
        GoldDungeonCompenstaionText,

        TreasureTroveLevelCountText,
        GoldDungeonLevelCountText,

        TreasureTroveIngredientCountText,
        GoldDungeonIngredientCountText,

    }

    enum Buttons
    {
        TreasureTroveMinusButton,
        TreasureTrovePlusButton,
        TreasureTroveStartButton,
        GoldDungeonMinusButton,
        GoldDungeonPlusButton,
        GoldDungeonStartButton,

    }
    #endregion

    int TreasureTroveMaxLevel = 0;
    int GoldDungeonMaxLevel = 0;

    int TreasureTroveLevel = 0;
    int GoldDungeonLevel = 0;

    // 보물창고/골드던전 슬롯 UI 묶음 (인덱스 0=보물창고, 1=골드던전)
    struct DungeonSlotUI
    {
        public TextMeshProUGUI countText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI ingredientText;
    }

    DungeonSlotUI[] dungeonSlots;
    TextMeshProUGUI timeText;

    RawImage rawImage;
    bool isEntering = false;

    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        Managers.UpdateM.Register(this);

        GameObjectsType = typeof(GameObjects);
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);

        BindObject(GameObjectsType);
        BindText(TextsType);
        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.TreasureTroveStartButton).gameObject.BindEvent(() => OnClickDungeonStartButton(Buttons.TreasureTroveStartButton));
        GetButton(ButtonsType, (int)Buttons.GoldDungeonStartButton).gameObject.BindEvent(() => OnClickDungeonStartButton(Buttons.GoldDungeonStartButton));

        GetButton(ButtonsType, (int)Buttons.TreasureTroveMinusButton).gameObject.BindEvent(() => OnClickMinusButton(Buttons.TreasureTroveMinusButton));
        GetButton(ButtonsType, (int)Buttons.GoldDungeonMinusButton).gameObject.BindEvent(() => OnClickMinusButton(Buttons.GoldDungeonMinusButton));

        GetButton(ButtonsType, (int)Buttons.TreasureTrovePlusButton).gameObject.BindEvent(() => OnClickPlusButton(Buttons.TreasureTrovePlusButton));
        GetButton(ButtonsType, (int)Buttons.GoldDungeonPlusButton).gameObject.BindEvent(() => OnClickPlusButton(Buttons.GoldDungeonPlusButton));

        rawImage = GetObject(GameObjectsType, (int)GameObjects.RawImage).GetComponent<RawImage>();
        timeText = GetText(TextsType, (int)Texts.TimeText);

        dungeonSlots = new DungeonSlotUI[]
        {
            new() {
                countText      = GetText(TextsType, (int)Texts.CrystalCountText),
                levelText      = GetText(TextsType, (int)Texts.TreasureTroveLevelCountText),
                ingredientText = GetText(TextsType, (int)Texts.TreasureTroveIngredientCountText),
            },
            new() {
                countText      = GetText(TextsType, (int)Texts.SapphireCountText),
                levelText      = GetText(TextsType, (int)Texts.GoldDungeonLevelCountText),
                ingredientText = GetText(TextsType, (int)Texts.GoldDungeonIngredientCountText),
            },
        };

        TreasureTroveMaxLevel = Managers.GameM.gameData.DungeonClearLevel[0] + 1;
        GoldDungeonMaxLevel = Managers.GameM.gameData.DungeonClearLevel[1] + 1;
        TreasureTroveLevel = TreasureTroveMaxLevel;
        GoldDungeonLevel = GoldDungeonMaxLevel;
        return true;


    }

    public override void SetInfo()
    {
        Managers.SoundM.MuteEffectVolume(true);

        for (int i = 0; i < dungeonSlots.Length; i++)
        {
            var slot = dungeonSlots[i];
            slot.ingredientText.color = Managers.GameM.gameData.DungeonKey[i] != 0 ? Color.green : Color.red;
            slot.countText.text = $"({Managers.GameM.gameData.DungeonKey[i]} / 2)";
            slot.levelText.text = $"{Managers.GameM.gameData.DungeonClearLevel[i] + 1}";
        }

        GetText(TextsType, (int)Texts.TreasureTroveCompenstaionText).text = $"{(Managers.GameM.gameData.DungeonClearLevel[0] + 1) * 50}";

        int level = Managers.GameM.gameData.DungeonClearLevel[1] + 1;
        double money = Utils.Money() * level * 10;
        GetText(TextsType, (int)Texts.GoldDungeonCompenstaionText).text = Utils.ToCurrencyString(money);

        Managers.RenderM.Show(RenderType.Dungeon, rawImage);
        Managers.RenderM.DungeonNPC();
        Managers.UIM.OnDungeonPopupState?.Invoke(true);
    }

    void OnClickDungeonStartButton(Buttons _btn)
    {
        Managers.SoundM.PlayButtonClick();
        StartDungeonButtonPress(_btn).Forget();
    }

    async UniTaskVoid StartDungeonButtonPress(Buttons _btn)
    {
        if (isEntering) return;
        if (!Managers.UIM.ClickLock(0.5f)) return;

        if (!Managers.StageM.CanEnterDungeon())
        {
            Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastNoDungeon"));
            return;
        }
        isEntering = true;

        await UniTask.Yield();
        if (!Managers.StageM.CanEnterDungeon())
        {
            Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastNoDungeon"));
            isEntering = false;
            return;
        }

        // DungeonKey 체크를 StateChange(Changing) 이전에 수행
        // StateChange(Changing) 후 return하면 게임이 Changing 상태에 갇힘
        switch (_btn)
        {
            case Buttons.TreasureTroveStartButton:
                if (Managers.GameM.gameData.DungeonKey[0] == 0)
                {
                    Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastNoDia"));
                    isEntering = false;
                    return;
                }
                Managers.StageM.StateChange(Define.StageState.Changing);
                GetText(TextsType, (int)Texts.CrystalCountText).text = $"({Managers.GameM.gameData.DungeonKey[0]--} / 2)";
                Managers.StageM.StateChange(Define.StageState.Dungeon, 70000);
                Managers.QuestM.GetMission(Define.MissionTarget.Dungeon).Progress++;
                break;

            case Buttons.GoldDungeonStartButton:
                if (Managers.GameM.gameData.DungeonKey[1] == 0)
                {
                    Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastNoDia"));
                    isEntering = false;
                    return;
                }
                Managers.StageM.StateChange(Define.StageState.Changing);
                GetText(TextsType, (int)Texts.SapphireCountText).text = $"({Managers.GameM.gameData.DungeonKey[1]--} / 2)";
                Managers.StageM.StateChange(Define.StageState.Dungeon, 70001);
                Managers.QuestM.GetMission(Define.MissionTarget.Dungeon).Progress++;
                break;
        }

        isEntering = false;
        Managers.UpdateM.UnRegister(this);
        await TriggerClose(this, true);
    }


    void OnClickMinusButton(Buttons _btn)
    {
        Managers.SoundM.PlayButtonClick();
        switch (_btn)
        {
            case Buttons.TreasureTroveMinusButton:
                if (TreasureTroveLevel - 1 > 0)
                {
                    TreasureTroveLevel--;
                    GetText(TextsType, (int)Texts.TreasureTroveLevelCountText).text = $"{TreasureTroveLevel}";
                    GetText(TextsType, (int)Texts.TreasureTroveCompenstaionText).text = $"{TreasureTroveLevel * 50}";
                }
                else
                {
                    Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastLowLevel"));
                }

                break;


            case Buttons.GoldDungeonMinusButton:
                if (GoldDungeonLevel - 1 > 0)
                {
                    GoldDungeonLevel--;
                    GetText(TextsType, (int)Texts.GoldDungeonLevelCountText).text = $"{GoldDungeonLevel}";

                    double money = Utils.Money() * GoldDungeonLevel * 10;
                    GetText(TextsType, (int)Texts.GoldDungeonCompenstaionText).text = Utils.ToCurrencyString(money);


                }
                else
                {
                    Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastLowLevel"));
                }
                break;
        }
    }

    void OnClickPlusButton(Buttons _btn)
    {
        Managers.SoundM.PlayButtonClick();
        switch (_btn)
        {
            case Buttons.TreasureTrovePlusButton:
                if (TreasureTroveLevel + 1 <= TreasureTroveMaxLevel)
                {
                    TreasureTroveLevel++;
                    GetText(TextsType, (int)Texts.TreasureTroveLevelCountText).text = $"{TreasureTroveLevel}";
                    GetText(TextsType, (int)Texts.TreasureTroveCompenstaionText).text = $"{TreasureTroveLevel * 50}";
                }
                else
                {
                    Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastStageLock"));
                }
                break;


            case Buttons.GoldDungeonPlusButton:
                if (GoldDungeonLevel + 1 <= GoldDungeonMaxLevel)
                {
                    GoldDungeonLevel++;
                    GetText(TextsType, (int)Texts.GoldDungeonLevelCountText).text = $"{GoldDungeonLevel}";


                    double money = Utils.Money() * GoldDungeonLevel * 10;
                    GetText(TextsType, (int)Texts.GoldDungeonCompenstaionText).text = Utils.ToCurrencyString(money);
                }
                else
                {
                    Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastStageLock"));
                }
                break;
        }
    }

    private void OnDisable()
    {
        Managers.UIM.OnDungeonPopupState?.Invoke(false);
        Managers.RenderM.Hide();
    }

    public void Tick(float _deltaTime)
    {
        timeText.text = Utils.NextDayTimer();
    }
}
