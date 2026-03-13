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

        GetText(TextsType, (int)Texts.DataLoadText).text = "데이터를 로딩중입니다,";
        GetText(TextsType, (int)Texts.VersionText).text = "Version. " + Application.version;

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
            Debug.LogError(e.Message);
        }
    }

    async UniTask FinishLoadingProcess()
    {
        await UniTask.Delay(300); 

        Managers.DataM.Init();
        Managers.ObjectM.Init();
        Managers.AdM.Init();
        Managers.IAPM.InitUnityIAP();
        Managers.TimeM.Init();
        Managers.SoundM.Init();
        Managers.SoundM.Play(Define.Sound.Bgm, "Bgm_Title");
        Managers.LocalM.Init();

        await Managers.FirebaseM.Init();

        hasSeenLogin = PlayerPrefs.GetInt("HasSeenLogin", 0) == 1;
        bool isLoggedIn = Managers.FirebaseM.IsLoggedIn();

        if (!hasSeenLogin || !isLoggedIn)
        {
            var popup = await Managers.UIM.ShowPopup<UI_LoadingLogin>();
            popup.transform.SetParent(this.transform);
            popup.transform.SetAsLastSibling();
        }
        else
        {
            await Managers.FirebaseM.CheckAndApplyCurrentUser();
            await Managers.FirebaseM.ReadData();
        }

        bool isGuest = Managers.GameM.gameData.isGuest;
        
        GetObject(GameObjectsType, (int)GameObjects.LoginButtonObject).SetActive(isGuest);

#if UNITY_ANDROID
        GetButton(ButtonsType, (int)Buttons.GoogleLoginButton).transform.SetAsFirstSibling();
#endif


        GetImage(ImagesType, (int)Images.TapToStartImage).gameObject.SetActive(true);
        GetText(TextsType, (int)Texts.DataLoadText).text = "데이터가 정상적으로 로딩되었습니다,";
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
                title = "연동 성공",
                message = "구글 계정으로 연동되었습니다.",
                theme = AlertTheme.Light,
                buttons = new() { new() { text = "확인", style = AlertButtonStyle.Cancel } }
            });
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"[ERROR] 구글 연동 실패: {e.Message} (코드: {e.ErrorCode})");

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

                        int serverStageValue = syncInfo.ServerData.stage;
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
                                GetObject(GameObjectsType, (int)GameObjects.LoginButtonObject).SetActive(false);
                            }
                        }
                        else if (choice == 1)
                        {
                            bool success = await Managers.FirebaseM.LoadServerDataOnly();
                            if (success)
                            {
                                await Managers.FirebaseM.ReadData();
                                await NativeAlert.ShowAsync(new AlertOptions
                                {
                                    title = "전환 성공",
                                    message = "서버 데이터를 불러와 구글 계정으로 전환되었습니다.",
                                    theme = AlertTheme.Light,
                                    buttons = new() { new() { text = "확인", style = AlertButtonStyle.Cancel } }
                                });
                                GetObject(GameObjectsType, (int)GameObjects.LoginButtonObject).SetActive(false);
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
                                title = "전환 성공",
                                message = "서버 데이터를 불러와 구글 계정으로 전환되었습니다.",
                                theme = AlertTheme.Light,
                                buttons = new() { new() { text = "확인", style = AlertButtonStyle.Cancel } }
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
                    title = "오류",
                    message = "연동에 실패했습니다. 다시 시도해주세요."
                });
            }
        }
    }
   
    void OnClickAppleLoginButton()
    {
    }
}
