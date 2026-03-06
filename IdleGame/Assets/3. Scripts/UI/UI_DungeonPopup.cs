using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    RawImage rawImage;
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
        TreasureTroveMaxLevel = Managers.GameM.gameData.DungeonClearLevel[0] + 1;
        GoldDungeonMaxLevel = Managers.GameM.gameData.DungeonClearLevel[1] + 1;
        TreasureTroveLevel = TreasureTroveMaxLevel;
        GoldDungeonLevel = GoldDungeonMaxLevel;
        return true;


    }

    public override void SetInfo()
    {

        for (int i = 0; i < Managers.GameM.gameData.DungeonKey.Length; i++)
        {
            if (Managers.GameM.gameData.DungeonKey[i] != 0)
                GetText(TextsType, (int)Texts.TreasureTroveIngredientCountText + i).color = Color.green;
            else
                GetText(TextsType, (int)Texts.TreasureTroveIngredientCountText + i).color = Color.red;

            GetText(TextsType, (int)Texts.CrystalCountText + i).text = $"({Managers.GameM.gameData.DungeonKey[i]} / 2)";
            GetText(TextsType, (int)Texts.TreasureTroveLevelCountText + i).text = $"{Managers.GameM.gameData.DungeonClearLevel[i] + 1}";
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

        if (!Managers.StageM.CanEnterDungeon())
        {
            Managers.UIM.ShowToast("현재 상태에서는 던전에 입장할 수 없습니다.");
            return;
        }

        Managers.StageM.StateChange(Define.StageState.Changing);

        switch (_btn)
        {

            case Buttons.TreasureTroveStartButton:
                if (Managers.GameM.gameData.DungeonKey[0] != 0)
                {
                    GetText(TextsType, (int)Texts.CrystalCountText).text = $"({Managers.GameM.gameData.DungeonKey[0]--} / 2)";
                    Managers.StageM.StateChange(Define.StageState.Dungeon, 70000);
                    Managers.QuestM.GetMission(Define.MissionTarget.Dungeon).Progress++;
                }
                else
                {
                    Managers.UIM.ShowToast("보석이 부족합니다.");
                    return;
                }


                break;

            case Buttons.GoldDungeonStartButton:
                if (Managers.GameM.gameData.DungeonKey[1] != 0)
                {
                    GetText(TextsType, (int)Texts.SapphireCountText).text = $"({Managers.GameM.gameData.DungeonKey[1]--} / 2)";
                    Managers.StageM.StateChange(Define.StageState.Dungeon, 70001);

                    Managers.QuestM.GetMission(Define.MissionTarget.Dungeon).Progress++;
                }
                else
                {
                    Managers.UIM.ShowToast("보석이 부족합니다.");
                    return;
                }

                break;
        }


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
                    Managers.UIM.ShowToast("최저레벨입니다.");
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
                    Managers.UIM.ShowToast("최저레벨입니다.");
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
                    Managers.UIM.ShowToast("스테이지가 잠겨있습니다.");
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
                    Managers.UIM.ShowToast("스테이지가 잠겨있습니다.");
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
        GetText(TextsType, (int)Texts.TimeText).text = Utils.NextDayTimer();
    }
}
