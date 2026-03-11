using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using way2tushar.NativeAlerts;

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
                if (e.ErrorCode == (int)AuthError.CredentialAlreadyInUse)
                {
                    int result = await NativeAlert.ShowAsync(new AlertOptions
                    {
                        title = "계정 충돌",
                        message = "이 구글 계정은 이미 다른 데이터와 연동되어있습니다. 해당 계정으로 전환하시겠습니까?",
                        theme = AlertTheme.System,
                        buttons = new()
                    {
                        new() {text = "취소", style = AlertButtonStyle.Cancel},
                        new() {text = "전환하기", style = AlertButtonStyle.Default}
                    }
                    });

                    if (result == 1)
                    {
                        var syncInfo = await Managers.FirebaseM.PrepareGoogleAccountSync();

                        if (syncInfo.HasConflict)
                        {
                            int localStageValue = syncInfo.LocalData.stage;
                            int localStageForward = ((localStageValue - 1) / 20) + 1;
                            int localStageBack = ((localStageValue - 1) % 20) + 1;

                            int serverStageValue = syncInfo.serverData.stage;
                            int serverStageForward = ((serverStageValue - 1) / 20) + 1;
                            int serverStageBack = ((serverStageValue - 1) % 20) + 1;

                            int choice = await NativeAlert.ShowAsync(new AlertOptions
                            {
                                title = "데이터 충돌",
                                message = $"기기 데이터의 스테이지({localStageForward} - {localStageBack})가 서버 데이터의 스테이지({serverStageForward} - {serverStageBack})보다 앞서있습니다. 덮어 씌울까요?",
                                theme = AlertTheme.System,
                                buttons = new()
                            {
                                new() {text = "덮어씌우기", style = AlertButtonStyle.Cancel},
                                new() {text = "불러오기", style = AlertButtonStyle.Default}
                            }
                            });

                            if (choice == 0)
                            {
                                bool success = await Managers.FirebaseM.ForceUploadLocalDataToServer(syncInfo);
                                if (success)
                                {

                                    await NativeAlert.ShowAsync(new AlertOptions
                                    {
                                        title = "전환 성공",
                                        message = "기기 데이터로 서버를 덮어쓰고 구글 계정으로 전환되었습니다.",
                                        theme = AlertTheme.Light,
                                        buttons = new() { new() { text = "확인", style = AlertButtonStyle.Cancel } }
                                    });
                                }
                            }
                            else if (choice == 1)
                            {
                                bool success = await Managers.FirebaseM.LoadServerDataOnly();
                                if (success)
                                {
                                    await NativeAlert.ShowAsync(new AlertOptions
                                    {
                                        title = "전환 성공",
                                        message = "서버 데이터를 불러와 구글 계정으로 전환되었습니다.",
                                        theme = AlertTheme.Light,
                                        buttons = new() { new() { text = "확인", style = AlertButtonStyle.Cancel } }
                                    });
                                }
                            }
                        }
                        else
                        {
                            bool success = await Managers.FirebaseM.LoadServerDataOnly();
                            if (success)
                            {
                                await NativeAlert.ShowAsync(new AlertOptions
                                {
                                    title = "전환 성공",
                                    message = "서버 데이터를 불러와 구글 계정으로 전환되었습니다.",
                                    theme = AlertTheme.Light,
                                    buttons = new() { new() { text = "확인", style = AlertButtonStyle.Cancel } }
                                });
                            }
                        }
                    }
                }
                else
                {
                    await NativeAlert.ShowAsync(new AlertOptions
                    {
                        title = "오류",
                        message = "연동에 실패했습니다. 다시 시도해주세요."
                    });
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

        if (success)
        {
            Managers.UIM.ShowToast("기존 계정 데이터로 전환되었습니다.");
            Managers.UIM.ClosePopup(this).Forget();
        }
    }
}
