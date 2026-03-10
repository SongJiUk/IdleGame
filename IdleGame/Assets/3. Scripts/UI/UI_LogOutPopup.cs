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

    async void OnClickLogOutButton()
    {
        Managers.SoundM.PlayButtonClick();

        Time.timeScale = 0f;
        Managers.UpdateM.PauseTicking(true);

        Managers.ClearAll();

#if UNITY_ANDROID || UNITY_IOS
        try
        {
            Google.GoogleSignIn.DefaultInstance?.SignOut();
        }
        catch (System.Exception e) { Debug.Log($"Google SignOut Error: {e.Message}"); }
#endif

        PlayerPrefs.DeleteKey("HasSeenLogin");
        PlayerPrefs.Save();

        await Managers.SceneM.LoadSceneAsync(Define.SceneType.TitleScene);

        Time.timeScale = 1f;
        Managers.UpdateM.PauseTicking(false);

        Debug.Log("로그아웃 프로세스 완료: 타이틀 씬으로 복귀했습니다.");
    }

    void OnClickCloseButton()
    {
        Managers.SoundM.PlayButtonClick();
        Managers.UIM.ClosePopup(this).Forget();
    }

}
