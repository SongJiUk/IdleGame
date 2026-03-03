using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class UI_LoadingLogin : UI_Popup
{
    #region enum
    enum Buttons
    {
        GoogleButton,
        GuestButton
    }
    #endregion

    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;


        ButtonsType = typeof(Buttons);
        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.GoogleButton).gameObject.BindEvent(OnClickGoogleButton);
        GetButton(ButtonsType, (int)Buttons.GuestButton).gameObject.BindEvent(OnClickGuestButton);
        return true;
    }

    async void OnClickGoogleButton()
    {
        Managers.FirebaseM.GoogleLogin();
        Managers.UIM.ClosePopup(this).Forget();
        Managers.GameM.gameData.isGuest = false;
    }

    async void OnClickGuestButton()
    {
        await Managers.FirebaseM.GuestLogin();
        Managers.UIM.ClosePopup(this).Forget();
    }
}
