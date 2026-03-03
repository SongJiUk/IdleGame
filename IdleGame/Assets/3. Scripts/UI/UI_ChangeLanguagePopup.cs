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
        GetText(TextsType, (int)Texts.BeforeText).text = Managers.LocalM.localData["UIChangeLanguage"].GetData();
        GetText(TextsType, (int)Texts.AfterText).text = Managers.LocalM.localData["UIChangeLanguage"].GetData(language);
    }
    void OnClickYesButton()
    {
        PlayerPrefs.SetString("LOCAL", language);
        Application.Quit();
    }

    void OnClickNoButton()
    {
        Managers.UIM.ClosePopup(this).Forget();
    }
}
