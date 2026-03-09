using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ResetPopup : UI_Popup
{
    #region enum
    enum Buttons
    {
        ResetButton,
        CloseButton
    }

    #endregion


    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        ButtonsType = typeof(Buttons);

        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.ResetButton).gameObject.BindEvent(OnClickResetButton);
        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        return true;
    }


    void OnClickResetButton()
    {
        Managers.SoundM.PlayButtonClick();
        Managers.FirebaseM.DeleteUserData().Forget();
    }

    void OnClickCloseButton()
    {
        Managers.SoundM.PlayButtonClick();
        Managers.UIM.ClosePopup(this).Forget();
    }
}
