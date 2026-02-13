using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SettingPopup : UI_Popup, ITickable
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
        JaButton,
        LogOutButton,
        RestoreButton,
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
        GetButton(ButtonsType, (int)Buttons.JaButton).gameObject.BindEvent(() => OnClickLanguageButton(Buttons.JaButton));

        GetButton(ButtonsType, (int)Buttons.LogOutButton).gameObject.BindEvent(OnClickLogOutButton);
        GetButton(ButtonsType, (int)Buttons.LogOutButton).gameObject.SetActive(false);
#if UNITY_IOS
        GetButton(ButtonsType, (int)Buttons.RestoreButton).gameObject.SetActive(true);
        GetButton(ButtonsType, (int)Buttons.RestoreButton).gameObject.BindEvent(() => Managers.IAPM.RestorePurchase());
#endif


        GetButton(ButtonsType, (int)Buttons.RestoreButton).gameObject.SetActive(false);
        GetText(TextsType, (int)Texts.UserIDText).text = $"Unique ID : {Managers.FirebaseM.CurrentUser.UserId}";
        GetSlider(SlidersType, (int)Sliders.BgmSlider).value = Managers.SoundM.BgmValue;
        GetSlider(SlidersType, (int)Sliders.EffectSlider).value = Managers.SoundM.EffectValue;

        CameraShakeCheck();
        return true;
    }

    public override void SetInfo()
    {
        Managers.UpdateM.Register(this);

        if (Managers.GameM.gameData.isGuest)
        {
            GetButton(ButtonsType, (int)Buttons.LogOutButton).gameObject.SetActive(true);
        }
    }

    async void OnClickLanguageButton(Buttons _button)
    {
        var popup = await Managers.UIM.ShowPopup<UI_ChangeLanguagePopup>();
        string language = "";
        switch (_button)
        {
            case Buttons.KoButton:
                language = "ko";
                break;

            case Buttons.EnButton:
                language = "en";
                break;

            case Buttons.JaButton:
                language = "ja";
                break;
        }
        popup.SetInfo(language);
    }

    void OnClickCloseButton()
    {
        PlayerPrefs.SetFloat("BGM", GetSlider(SlidersType, (int)Sliders.BgmSlider).value);
        PlayerPrefs.SetFloat("EFFECT", GetSlider(SlidersType, (int)Sliders.EffectSlider).value);
        Managers.UpdateM.UnRegister(this);

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

        PlayerPrefs.SetInt("CAM", CameraShakeCheck() == true ? 1 : 0);
        CameraShakeCheck();
    }

    void OnClickPrivacyPolicyButton()
    {
        OpenURL("https://deserted-bream-361.notion.site/KOR-2f0ea58668dd80de93d3d0bee603c964?pvs=73");
    }

    void OpenURL(string _url)
    {
        Application.OpenURL(_url);
    }

    void OnClickUniqueClipboard()
    {
        GUIUtility.systemCopyBuffer = Managers.FirebaseM.CurrentUser.UserId;
        Managers.UIM.ShowToast("복사 완료");
    }
    public void Tick(float _deltaTime)
    {
        Managers.SoundM.BgmValue = GetSlider(SlidersType, (int)Sliders.BgmSlider).value;
        Managers.SoundM.audioSources[0].volume = Managers.SoundM.BgmValue;
        Managers.SoundM.EffectValue = GetSlider(SlidersType, (int)Sliders.EffectSlider).value;
        Managers.SoundM.audioSources[1].volume = Managers.SoundM.EffectValue;
    }

    async void OnClickLogOutButton()
    {
        await Managers.UIM.ShowPopup<UI_LogOutPopup>();

    }
}
