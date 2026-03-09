using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;

public class UI_LoadingLogin : UI_Popup
{
    #region enum
    enum Buttons
    {
        GoogleButton,
        GuestButton
    }
    #endregion
    private bool isLoggingIn = false;
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
        Managers.SoundM.PlayButtonClick();

        if (isLoggingIn) return;
        isLoggingIn = true;

        if (Managers.FirebaseM.Auth.CurrentUser != null && Managers.FirebaseM.Auth.CurrentUser.IsAnonymous)
        {
            try
            {
                await Managers.FirebaseM.LinkGoogleToCurrentUser();
                isLoggingIn = false;
            }
            catch (FirebaseException e)
            {
                Debug.LogError($"[DEBUG] 에러 코드 확인: {e.ErrorCode}");
                if (e.ErrorCode == (int)AuthError.CredentialAlreadyInUse)
                {
                    var conflictPopup = await Managers.UIM.ShowPopup<UI_AccountConflictPopup>();

                    conflictPopup.SetCallBack(async () =>
                    {
                        bool success = await Managers.FirebaseM.SwitchToGoogleAccount();
                        if (success) Managers.UIM.ClosePopup(this).Forget();
                        isLoggingIn = false;
                    });
                }
                else
                {
                    Managers.UIM.ShowToast("연동 중 오류가 발생했습니다.");
                    isLoggingIn = false;
                }
            }
        }
        else
        {

            bool isLoginSuccess = await Managers.FirebaseM.GoogleLogin();

            if (isLoginSuccess)
            {
                Debug.Log("로그인 성공 ! 동기화 시작");

                await Managers.FirebaseM.CheckAndApplyCurrentUser();
                Managers.GameM.gameData.isGuest = false;
                Managers.UIM.ClosePopup(this).Forget();
            }
            else
            {
                Debug.LogError("로그인 실패");
                Managers.UIM.ShowToast("로그인에 실패했습니다.");
                isLoggingIn = false;
            }

            isLoggingIn = false;
        }
    }

    async void OnClickGuestButton()
    {
        await Managers.FirebaseM.GuestLogin();
        Managers.UIM.ClosePopup(this).Forget();
    }

    void OnLinkingFailed()
    {
        ConfirmSwitchAccount();
    }

    async void ConfirmSwitchAccount()
    {
        Managers.FirebaseM.SignOutFM();

        bool success = await Managers.FirebaseM.GoogleLogin();

        if(success)
        {
            Managers.UIM.ShowToast("기존 계정 데이터로 전환되었습니다.");
            Managers.UIM.ClosePopup(this).Forget();
        }
    }
}
