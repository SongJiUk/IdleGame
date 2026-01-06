using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DungeonPopup : UI_Popup
{
    #region Enum


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

    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);

        BindText(TextsType);
        BindButton(ButtonsType);



        GetButton(ButtonsType, (int)Buttons.TreasureTroveStartButton).gameObject.BindEvent(() => OnClickDungeonStartButton(Buttons.TreasureTroveStartButton));
        GetButton(ButtonsType, (int)Buttons.GoldDungeonStartButton).gameObject.BindEvent(() => OnClickDungeonStartButton(Buttons.GoldDungeonStartButton));

        GetButton(ButtonsType, (int)Buttons.TreasureTroveMinusButton).gameObject.BindEvent(() => OnClickMinusButton(Buttons.TreasureTroveMinusButton));
        GetButton(ButtonsType, (int)Buttons.GoldDungeonMinusButton).gameObject.BindEvent(() => OnClickMinusButton(Buttons.GoldDungeonMinusButton));

        GetButton(ButtonsType, (int)Buttons.TreasureTrovePlusButton).gameObject.BindEvent(() => OnClickPlusButton(Buttons.TreasureTrovePlusButton));
        GetButton(ButtonsType, (int)Buttons.GoldDungeonPlusButton).gameObject.BindEvent(() => OnClickPlusButton(Buttons.GoldDungeonPlusButton));


        return true;


    }

    public override void SetInfo()
    {
        for (int i = 0; i < Managers.GameM.gameData.DungeonKey.Length; i++)
        {
            GetText(TextsType, (int)Texts.CrystalCountText + i).text = $"({Managers.GameM.gameData.DungeonKey[i]} / 2)";
            GetText(TextsType, (int)Texts.TreasureTroveLevelCountText + i).text = $"{Managers.GameM.gameData.DungeonClearLevel[i] + 1}";
        }

        GetText(TextsType, (int)Texts.TreasureTroveCompenstaionText).text = $"{(Managers.GameM.gameData.DungeonClearLevel[0] + 1) * 50}";

        int level = (Managers.GameM.gameData.DungeonClearLevel[1] + 1) * 5;
        var value = Utils.CalculatedValue(Utils.Datas.stageData.Base_Gold, Managers.GameM.Stage, Utils.Datas.stageData.Monster_Gold);
        GetText(TextsType, (int)Texts.GoldDungeonCompenstaionText).text = Utils.ToCurrencyString(value * level);


    }


    void CheckTime()
    {

    }

    void OnClickDungeonStartButton(Buttons _btn)
    {

        StartDungeonButtonPress(_btn).Forget();

    }

    async UniTaskVoid StartDungeonButtonPress(Buttons _btn)
    {

        switch (_btn)
        {
            //TODO : 어떤 버튼 누른지 알려줘야함
            case Buttons.TreasureTroveStartButton:
                Managers.StageM.StateChange(Define.StageState.Dungeon, 0);
                break;

            case Buttons.GoldDungeonStartButton:
                Managers.StageM.StateChange(Define.StageState.Dungeon, 1);
                break;
        }

        await TriggerClose(this, true);

    }

    void OnClickMinusButton(Buttons _btn)
    {
        switch (_btn)
        {

            case Buttons.TreasureTroveMinusButton:
                // TODO : 난이도 별로 나눠서 텍스트 초기화
                break;


            case Buttons.GoldDungeonMinusButton:
                break;
        }
    }

    void OnClickPlusButton(Buttons _btn)
    {
        switch (_btn)
        {
            case Buttons.TreasureTrovePlusButton:
                break;


            case Buttons.GoldDungeonPlusButton:
                break;
        }
    }
}
