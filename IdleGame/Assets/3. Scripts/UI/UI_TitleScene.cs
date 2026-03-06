using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using DG.Tweening;

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
        GetText(TextsType, (int)Texts.VersionText).text = "Versoin. " + Application.version;

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
            await FinishLoadingProcess();


        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    async UniTask FinishLoadingProcess()
    {


        await UniTask.Delay(300); // 100%가 된 모습을 잠시 보여줌

        // 각종 매니저 초기화 로직
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
        if (!hasSeenLogin || !Managers.FirebaseM.IsLoggedIn())
        {
            var popup = await Managers.UIM.ShowPopup<UI_LoadingLogin>();
            popup.transform.SetParent(this.transform);
            popup.transform.SetAsLastSibling();
        }
        else
        {
            await Managers.FirebaseM.CheckAndApplyCurrentUser();

            bool isGuest = Managers.GameM.gameData.isGuest;
            GetObject(GameObjectsType, (int)GameObjects.LoginButtonObject).SetActive(isGuest);

#if UNITY_ANDROID
        GetButton(ButtonsType, (int)Buttons.GoogleLoginButton).transform.SetAsFirstSibling();
#elif UNITY_IOS
            //GetButton(ButtonsType, (int)Buttons.AppleLoginButton).transform.SetAsFirstSibling();
#endif
        }

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
        Managers.SoundM.PlayButtonClick();
        if (isLoadEnd)
        {
            isLoadEnd = false;
            await Managers.SceneM.LoadSceneAsync(Define.SceneType.GameScene);
            KillBlinkTween();
        }
    }

    void OnClickGoogleLoginButton()
    {
        Managers.SoundM.PlayButtonClick();
        Debug.Log("[LOGIN] Google Login Button Clicked");
        Managers.FirebaseM.LinkGoogleToCurrentUser().Forget();
    }

    void OnClickAppleLoginButton()
    {
    }
}
