using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GachaListPopup : UI_Popup
{
    #region Enum
    enum Texts
    {
        CommonProbabilityText,
        UnCommonProbabilityText,
        RareProbabilityText,
        UniqueProbabilityText,
        LegendaryProbabilityText,
        GachaLevelText,

    }

    enum Buttons
    {
        CloseButton,
        GachaLevelBeforeButton,
        GachaLevelNextButton
    }
    #endregion
    int Level = 0;

    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);

        BindText(TextsType);
        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton(ButtonsType, (int)Buttons.GachaLevelBeforeButton).gameObject.BindEvent(OnClickLevelBeforeButton);
        GetButton(ButtonsType, (int)Buttons.GachaLevelNextButton).gameObject.BindEvent(OnClickLevelNextButton);

        Level = Utils.Summon_Level(Managers.GameM.Hero_Summon_Count);
        return true;
    }

    public override void SetInfo()
    {
        Level = Utils.Summon_Level(Managers.GameM.Hero_Summon_Count);
        RefreshUI();
    }

    void RefreshUI()
    {
        Data.GachaData data;
        if (Level == 0) Level = 1;
        Managers.DataM.GachaDataDic.TryGetValue(Level, out data);

        if (data == null) return;

        GetText(TextsType, (int)Texts.CommonProbabilityText).text = string.Format("{0:0.000}", data.Common.ToString()) + "%";
        GetText(TextsType, (int)Texts.UnCommonProbabilityText).text = string.Format("{0:0.000}", data.UnCommon.ToString()) + "%";
        GetText(TextsType, (int)Texts.RareProbabilityText).text = string.Format("{0:0.000}", data.Rare.ToString()) + "%";
        GetText(TextsType, (int)Texts.UniqueProbabilityText).text = string.Format("{0:0.000}", data.Unique.ToString()) + "%";
        GetText(TextsType, (int)Texts.LegendaryProbabilityText).text = string.Format("{0:0.000}", data.Legendary.ToString()) + "%";
        GetText(TextsType, (int)Texts.GachaLevelText).text = "Level. " + Level.ToString();
    }

    void OnClickLevelBeforeButton()
    {
        Managers.SoundM.PlayButtonClick();
        int value = Level - 1;

        if (value <= 0) value = 10;
        Level = value;

        RefreshUI();
    }

    void OnClickLevelNextButton()
    {
        Managers.SoundM.PlayButtonClick();
        int value = Level + 1;
        if (value > 10) value = 1;
        Level = value;

        RefreshUI();
    }

    void OnClickCloseButton()
    {
        Managers.SoundM.PlayButtonClick();
        Managers.UIM.ClosePopup(this).Forget();
    }
}
