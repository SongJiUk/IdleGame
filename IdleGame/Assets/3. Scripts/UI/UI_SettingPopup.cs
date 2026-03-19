using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SettingPopup : UI_Popup
{
    #region enum

    enum Buttons
    {
        CloseButton,
        CameraShakeButton,
        PrivacyPolicyButton,
        UserIDButton,
        KoButton,
        EnButton,
        //JaButton,
        LogOutButton,
        RestoreButton,
        GameResetButton,
    }
    enum Sliders
    {
        BgmSlider,
        EffectSlider,
    }

    enum GameObjects
    {
        CameraShakeCheckImage,

    }

    enum Texts
    {
        UserIDText
    }

    #endregion


    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;


        ButtonsType = typeof(Buttons);
        SlidersType = typeof(Sliders);
        GameObjectsType = typeof(GameObjects);
        TextsType = typeof(Texts);

        BindButton(ButtonsType);
        BindSlider(SlidersType);
        BindObject(GameObjectsType);
        BindText(TextsType);

        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton(ButtonsType, (int)Buttons.CameraShakeButton).gameObject.BindEvent(OnClickCameraShakeButton);
        GetButton(ButtonsType, (int)Buttons.PrivacyPolicyButton).gameObject.BindEvent(OnClickPrivacyPolicyButton);
        GetButton(ButtonsType, (int)Buttons.UserIDButton).gameObject.BindEvent(OnClickUniqueClipboard);

        GetButton(ButtonsType, (int)Buttons.KoButton).gameObject.BindEvent(() => OnClickLanguageButton(Buttons.KoButton));
        GetButton(ButtonsType, (int)Buttons.EnButton).gameObject.BindEvent(() => OnClickLanguageButton(Buttons.EnButton));
        // GetButton(ButtonsType, (int)Buttons.JaButton).gameObject.BindEvent(() => OnClickLanguageButton(Buttons.JaButton));

        GetButton(ButtonsType, (int)Buttons.LogOutButton).gameObject.BindEvent(OnClickLogOutButton);
        GetButton(ButtonsType, (int)Buttons.LogOutButton).gameObject.SetActive(false);

        GetButton(ButtonsType, (int)Buttons.GameResetButton).gameObject.BindEvent(OnClickResetButton);
        GetButton(ButtonsType, (int)Buttons.RestoreButton).gameObject.SetActive(false);
#if UNITY_IOS
        GetButton(ButtonsType, (int)Buttons.RestoreButton).gameObject.SetActive(true);
        GetButton(ButtonsType, (int)Buttons.RestoreButton).gameObject.BindEvent(() => Managers.IAPM.RestorePurchase());
#endif



        GetText(TextsType, (int)Texts.UserIDText).text = $"Unique ID : {Managers.FirebaseM.CurrentUser.UserId}";

        GetSlider(SlidersType, (int)Sliders.BgmSlider).onValueChanged.AddListener(OnBgmVolumeChange);
        GetSlider(SlidersType, (int)Sliders.EffectSlider).onValueChanged.AddListener(OnEffectVolumeChange);

        CameraShakeCheck();
        return true;
    }

    public override void SetInfo()
    {

        if (!Managers.GameM.gameData.isGuest)
        {
            GetButton(ButtonsType, (int)Buttons.LogOutButton).gameObject.SetActive(true);
        }
    }

    async void OnClickLanguageButton(Buttons _button)
    {
        Managers.SoundM.PlayButtonClick();
        string language = "";
        switch (_button)
        {
            case Buttons.KoButton:
                language = "ko";
                break;

            case Buttons.EnButton:
                language = "en";
                break;

                // case Buttons.JaButton:
                //     language = "ja";
                //     break;
        }

        if (language == LocalizationManager.LocalAccess)
        {
            Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastSameLanguage"));
            return;
        }

        var popup = await Managers.UIM.ShowPopup<UI_ChangeLanguagePopup>();
        popup.SetInfo(language);
    }

    void OnClickCloseButton()
    {
        Managers.SoundM.PlayButtonClick();

        PlayerPrefs.SetFloat("BGM", GetSlider(SlidersType, (int)Sliders.BgmSlider).value);
        PlayerPrefs.SetFloat("EFFECT", GetSlider(SlidersType, (int)Sliders.EffectSlider).value);

        Managers.UIM.ClosePopup(this).Forget();
    }
    bool CameraShakeCheck()
    {
        bool isCameraShake = PlayerPrefs.GetInt("CAM") == 0 ? true : false;
        GetObject(GameObjectsType, (int)GameObjects.CameraShakeCheckImage).SetActive(isCameraShake);
        return isCameraShake;
    }
    void OnClickCameraShakeButton()
    {
        Managers.SoundM.PlayButtonClick();
        PlayerPrefs.SetInt("CAM", CameraShakeCheck() == true ? 1 : 0);
        CameraShakeCheck();
    }

    void OnClickPrivacyPolicyButton()
    {
        Managers.SoundM.PlayButtonClick();
        OpenURL("https://deserted-bream-361.notion.site/KOR-2f0ea58668dd80de93d3d0bee603c964?pvs=73");
    }

    void OpenURL(string _url)
    {
        Application.OpenURL(_url);
    }

    void OnClickUniqueClipboard()
    {
        Managers.SoundM.PlayButtonClick();
        GUIUtility.systemCopyBuffer = Managers.FirebaseM.CurrentUser.UserId;
        Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastNoCopy"));
    }


    void OnBgmVolumeChange(float _value)
    {
        Managers.SoundM.BgmValue = _value;
        Managers.SoundM.audioSources[0].volume = _value;
    }

    void OnEffectVolumeChange(float _value)
    {
        Managers.SoundM.SetEffectVolume(_value);
    }
    async void OnClickLogOutButton()
    {
        Managers.SoundM.PlayButtonClick();
        await Managers.UIM.ShowPopup<UI_LogOutPopup>();
    }

    async void OnClickResetButton()
    {
        Managers.SoundM.PlayButtonClick();
        await Managers.UIM.ShowPopup<UI_ResetPopup>();

    }
}
