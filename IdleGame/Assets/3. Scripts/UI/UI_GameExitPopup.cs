using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameExitPopup : UI_Popup
{
    #region Enum

    enum Buttons
    {
        GameExitButton,
        KeepGameButton
    }

    #endregion

    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        ButtonsType = typeof(Buttons);

        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.GameExitButton).gameObject.BindEvent(OnClickGameExitButton);
        GetButton(ButtonsType, (int)Buttons.KeepGameButton).gameObject.BindEvent(OnClickKeepGameButton);

        return true;
    }


    void OnClickGameExitButton()
    {
        Managers.SoundM.PlayButtonClick();
        Application.Quit();
    }

    void OnClickKeepGameButton()
    {
        Managers.SoundM.PlayButtonClick();
        Managers.UIM.ClosePopup(this).Forget();
    }
}
