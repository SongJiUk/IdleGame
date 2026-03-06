using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LogOutPopup : UI_Popup
{
    #region  enum
    enum Buttons
    {
        LogOutButton,
        CloseButton,
    }
    #endregion

    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        ButtonsType = typeof(Buttons);
        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.LogOutButton).gameObject.BindEvent(OnClickLogOutButton);
        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        return true;
    }

    void OnClickLogOutButton()
    {
        Managers.SoundM.PlayButtonClick();
        Managers.FirebaseM.SignOutFM();

#if UNITY_ANDROID || UNITY_IOS
        try
        {
            Google.GoogleSignIn.DefaultInstance.SignOut();
        }
        catch { }
#endif

        PlayerPrefs.DeleteKey("HasSeenLogin");
        PlayerPrefs.Save();

        Managers.GameM.ResetGameData();
        Managers.GameM.gameData.isGuest = true;
        Managers.GameM.gameData.playerName = "Guest";

        Managers.SceneM.LoadSceneAsync(Define.SceneType.TitleScene).Forget();
    }

    void OnClickCloseButton()
    {
        Managers.SoundM.PlayButtonClick();
        Managers.UIM.ClosePopup(this).Forget();
    }

}
