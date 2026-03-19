using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Firebase;
using Firebase.Auth;
using way2tushar.NativeAlerts;

public class UI_TitleScene : UI_Scene
{
    public override Transform WorldGoodsParent
    {
        get { return null; }
    }

    public override Transform WorldFontParent
    {
        get { return null; }
    }

    public override Transform WorldItemParent
    {
        get { return null; }
    }

    public override Transform WorldSpeechParent
    {
        get { return null; }
    }

    #region Enum

    public enum GameObjects
    {
        LoadingBarObject,
        LoginButtonObject,

    }
    public enum Buttons
    {
        StartButton,
        GoogleLoginButton,
        //AppleLoginButton
    }

    public enum Images
    {
        TapToStartImage
    }
    public enum Sliders
    {
        LoadingBar,
    }

    public enum Texts
    {
        DataLoadText,
        VersionText,
        TapToStartText,
        GoogleLoginText
    }
    #endregion
    bool isLoadEnd = false;
    bool hasSeenLogin;
    Tween blinkTween = null;
    private void Start()
    {
        Init().Forget();
    }

    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        GameObjectsType = typeof(GameObjects);
        ButtonsType = typeof(Buttons);
        ImagesType = typeof(Images);
        SlidersType = typeof(Sliders);
        TextsType = typeof(Texts);

        BindObject(GameObjectsType);
        BindButton(ButtonsType);
        BindImage(ImagesType);
        BindSlider(SlidersType);
        BindText(TextsType);

        GetText(TextsType, (int)Texts.DataLoadText).text = "Loading...";
        GetText(TextsType, (int)Texts.VersionText).text = "Version. " + Application.version;
        GetText(TextsType, (int)Texts.TapToStartText).text = Managers.LocalizationM.Get("UIClickScreen");
        GetText(TextsType, (int)Texts.GoogleLoginText).text = Managers.LocalizationM.Get("UIGoogleLogin");


        GetImage(ImagesType, (int)Images.TapToStartImage).gameObject.SetActive(false);

        GetButton(ButtonsType, (int)Buttons.StartButton).gameObject.BindEvent(OnClickStartButton);
        GetButton(ButtonsType, (int)Buttons.GoogleLoginButton).gameObject.BindEvent(OnClickGoogleLoginButton);
        //GetButton(ButtonsType, (int)Buttons.AppleLoginButton).gameObject.BindEvent(OnClickAppleLoginButton);

        GetObject(GameObjectsType, (int)GameObjects.LoginButtonObject).SetActive(false);
        SetInfo().Forget();
        return true;
    }


    async UniTask SetInfo()
    {
        try
        {
            float realProgress = 0f;
            float displayedProgress = 0f;
            var slider = GetSlider(SlidersType, (int)Sliders.LoadingBar);

            var loadTask = Managers.ResourceM.LoadGroupAsync<UnityEngine.Object>("PrevLoad", (key, count, max) =>
            {
                realProgress = (float)count / max;
            });

            while (displayedProgress < 1f || !loadTask.GetAwaiter().IsCompleted)
            {
                displayedProgress = Mathf.MoveTowards(displayedProgress, realProgress, Time.deltaTime * 0.5f);

                slider.value = displayedProgress;

                await UniTask.Yield();

                if (loadTask.GetAwaiter().IsCompleted && displayedProgress >= 0.99f)
                {
                    slider.value = 1f;
                    break;
                }
            }

            await loadTask;
            await FinishLoadingProcess();


        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message + e.ToString());
        }
    }

    async UniTask FinishLoadingProcess()
    {
        await UniTask.Delay(300);
        Managers.DataM.Init();
        Managers.LocalizationM.Init();
        Managers.TimeM.Init();

        Managers.ObjectM.Init();
        Managers.SoundM.Init();

        Managers.AdM.Init();
        Managers.IAPM.InitUnityIAP();


        Managers.SoundM.Play(Define.Sound.Bgm, "Bgm_Title");


        await Managers.FirebaseM.Init();

        hasSeenLogin = PlayerPrefs.GetInt("HasSeenLogin", 0) == 1;
        bool isLoggedIn = Managers.FirebaseM.IsLoggedIn();

        if (!hasSeenLogin || !isLoggedIn)
        {
            var popup = await Managers.UIM.ShowPopup<UI_LoadingLogin>();
            popup.transform.SetParent(this.transform);
            popup.OnSuccessLogin += OnSuccessLogin;
            popup.transform.SetAsLastSibling();
        }
        else
        {
            await Managers.FirebaseM.CheckAndApplyCurrentUser();
            await Managers.FirebaseM.ReadData();
        }

        Managers.LocalizationM.Init();

        GetText(TextsType, (int)Texts.TapToStartText).text = Managers.LocalizationM.Get("UIClickScreen");
        GetText(TextsType, (int)Texts.GoogleLoginText).text = Managers.LocalizationM.Get("UIGoogleLogin");

        foreach (var loc in UnityEngine.Object.FindObjectsOfType<UI_Localization>())
            loc.SetLocalData();

        bool isGuest = Managers.GameM.gameData.isGuest;
        GetObject(GameObjectsType, (int)GameObjects.LoginButtonObject).SetActive(isGuest);

#if UNITY_ANDROID
        GetButton(ButtonsType, (int)Buttons.GoogleLoginButton).transform.SetAsFirstSibling();
#endif


        GetImage(ImagesType, (int)Images.TapToStartImage).gameObject.SetActive(true);
        GetText(TextsType, (int)Texts.DataLoadText).text = Managers.LocalizationM.Get("UIDataLoadingSuccess");
        StartBlinkTween();

        isLoadEnd = true;
        Managers.QuestM.GetMission(Define.MissionTarget.DailyAttendance).Progress++;

    }

    public void StartBlinkTween()
    {
        KillBlinkTween();

        blinkTween = GetImage(ImagesType, (int)Images.TapToStartImage).DOFade(0.0f, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetLink(gameObject);
    }

    public void KillBlinkTween()
    {
        if (blinkTween != null && blinkTween.IsActive())
        {
            blinkTween.Kill();
            blinkTween = null;
        }
    }

    async void OnClickStartButton()
    {

        if (isLoadEnd)
        {
            Managers.SoundM.PlayButtonClick();
            isLoadEnd = false;
            await Managers.SceneM.LoadSceneAsync(Define.SceneType.GameScene);
            KillBlinkTween();
        }
    }

    async void OnClickGoogleLoginButton()
    {
        Managers.SoundM.PlayButtonClick();
        try
        {
            await Managers.FirebaseM.LinkGoogleToCurrentUser();
            await Managers.FirebaseM.ReadData();

            GetObject(GameObjectsType, (int)GameObjects.LoginButtonObject).SetActive(false);
            await NativeAlert.ShowAsync(new AlertOptions
            {
                title = Managers.LocalizationM.Get("System_LinkingSuccess"),
                message = Managers.LocalizationM.Get("System_LinkGoogleAcount"),
                theme = AlertTheme.Light,
                buttons = new() { new() { text = Managers.LocalizationM.Get("Check"), style = AlertButtonStyle.Cancel } }
            });
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"[ERROR] 구글 연동 실패: {e.Message} (코드: {e.ErrorCode})");

            if (e.ErrorCode == (int)AuthError.CredentialAlreadyInUse)
            {

                int result = await NativeAlert.ShowAsync(new AlertOptions
                {
                    title = Managers.LocalizationM.Get("System_AccountConflict"),
                    message = Managers.LocalizationM.Get("System_GoogleAccount_Already"),
                    theme = AlertTheme.System,
                    buttons = new()
                    {
                        new() {text = Managers.LocalizationM.Get("Cancel"), style = AlertButtonStyle.Cancel},
                        new() {text = Managers.LocalizationM.Get("System_Switch"), style = AlertButtonStyle.Default}
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

                        int serverStageValue = syncInfo.ServerData.stage;
                        int serverStageForward = ((serverStageValue - 1) / 20) + 1;
                        int serverStageBack = ((serverStageValue - 1) % 20) + 1;

                        int choice = await NativeAlert.ShowAsync(new AlertOptions
                        {
                            title = Managers.LocalizationM.Get("System_DataConflict"),
                            message = string.Format(Managers.LocalizationM.Get("System_DataOverwrite"), localStageForward, localStageBack, serverStageForward, serverStageBack),
                            theme = AlertTheme.System,
                            buttons = new()
                            {
                                new() {text = Managers.LocalizationM.Get("System_OverWrite"), style = AlertButtonStyle.Cancel},
                                new() {text = Managers.LocalizationM.Get("System_Load"), style = AlertButtonStyle.Default}
                            }
                        });

                        if (choice == 0)
                        {
                            bool success = await Managers.FirebaseM.ForceUploadLocalDataToServer(syncInfo);
                            if (success)
                            {
                                GetObject(GameObjectsType, (int)GameObjects.LoginButtonObject).SetActive(false);
                                await NativeAlert.ShowAsync(new AlertOptions
                                {
                                    title = Managers.LocalizationM.Get("System_ConversionSuccess"),
                                    message = Managers.LocalizationM.Get("System_OverWriteSuccess"),
                                    theme = AlertTheme.Light,
                                    buttons = new() { new() { text = Managers.LocalizationM.Get("Check"), style = AlertButtonStyle.Cancel } }
                                });

                            }
                        }
                        else if (choice == 1)
                        {
                            bool success = await Managers.FirebaseM.LoadServerDataOnly();
                            if (success)
                            {
                                await Managers.FirebaseM.ReadData();
                                GetObject(GameObjectsType, (int)GameObjects.LoginButtonObject).SetActive(false);
                                await NativeAlert.ShowAsync(new AlertOptions
                                {
                                    title = Managers.LocalizationM.Get("System_ConversionSuccess"),
                                    message = Managers.LocalizationM.Get("System_LoadGoogleAcountSuccess"),
                                    theme = AlertTheme.Light,
                                    buttons = new() { new() { text = Managers.LocalizationM.Get("Check"), style = AlertButtonStyle.Cancel } }
                                });

                            }
                        }
                    }
                    else
                    {
                        bool success = await Managers.FirebaseM.LoadServerDataOnly();
                        if (success)
                        {
                            await Managers.FirebaseM.ReadData();

                            await NativeAlert.ShowAsync(new AlertOptions
                            {
                                title = Managers.LocalizationM.Get("System_ConversionSuccess"),
                                message = Managers.LocalizationM.Get("System_LoadGoogleAcountSuccess"),
                                theme = AlertTheme.Light,
                                buttons = new() { new() { text = Managers.LocalizationM.Get("Check"), style = AlertButtonStyle.Cancel } }
                            });
                            GetObject(GameObjectsType, (int)GameObjects.LoginButtonObject).SetActive(false);
                        }
                    }
                }
            }
            else
            {
                await NativeAlert.ShowAsync(new AlertOptions
                {
                    title = Managers.LocalizationM.Get("System_Error"),
                    message = Managers.LocalizationM.Get("System_LinkingFail")
                });
            }
        }
    }

    void OnSuccessLogin()
    {
        bool isGuest = Managers.GameM.gameData.isGuest;
        Debug.Log("OnSuccessLogin" + isGuest);
        GetObject(GameObjectsType, (int)GameObjects.LoginButtonObject).SetActive(isGuest);
    }

    void OnClickAppleLoginButton()
    {

    }

    void OnDestroy()
    {

    }
}
