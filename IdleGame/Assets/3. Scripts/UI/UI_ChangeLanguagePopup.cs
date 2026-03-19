using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ChangeLanguagePopup : UI_Popup
{
    #region enum

    enum Texts
    {
        BeforeText,
        AfterText
    }

    enum Buttons
    {
        YesButton,
        NoButton
    }


    #endregion
    string language = "";
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        ButtonsType = typeof(Buttons);
        TextsType = typeof(Texts);
        BindButton(ButtonsType);
        BindText(TextsType);

        GetButton(ButtonsType, (int)Buttons.YesButton).gameObject.BindEvent(OnClickYesButton);
        GetButton(ButtonsType, (int)Buttons.NoButton).gameObject.BindEvent(OnClickNoButton);
        return true;
    }

    public void SetInfo(string _language)
    {
        language = _language;
        GetText(TextsType, (int)Texts.BeforeText).text = Managers.LocalizationM.Get("UIChangeLanguage");
        GetText(TextsType, (int)Texts.AfterText).text = Managers.LocalizationM.localData["UIChangeLanguage"].GetData(language);
    }
    async void OnClickYesButton()
    {
        Managers.SoundM.PlayButtonClick();
        Managers.GameM.gameData.language = language;
        await Managers.FirebaseM.WriteData();

        Time.timeScale = 0f;
        Managers.UpdateM.PauseTicking(true);

        Managers.CharacterM.Clear();
        Managers.UpdateM.Clear();
        Managers.StageM.Clear();
        Managers.SpawnM.Clear();
        Managers.ObjectM.Clear();
        Managers.PoolM.Clear();
        Managers.ResourceM.UnLoadAll();
        Managers.SoundM.Clear();
        Managers.UIM.Clear();

        Managers.LocalizationM.Init();

        await Managers.SceneM.LoadSceneAsync(Define.SceneType.TitleScene);

        Time.timeScale = 1f;
        Managers.UpdateM.PauseTicking(false);
    }

    void OnClickNoButton()
    {
        Managers.SoundM.PlayButtonClick();
        Managers.UIM.ClosePopup(this).Forget();
    }
}
