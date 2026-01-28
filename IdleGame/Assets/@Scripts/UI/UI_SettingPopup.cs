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
    { UserIDText }

    #endregion

    bool isCameraShake = false;

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

        GetText(TextsType, (int)Texts.UserIDText).text = $"Unique ID : {Managers.FirebaseM.CurrentUser.UserId}";
        GetSlider(SlidersType, (int)Sliders.BgmSlider).value = Managers.SoundM.BgmValue;
        GetSlider(SlidersType, (int)Sliders.EffectSlider).value = Managers.SoundM.EffectValue;
        return true;
    }

    public override void SetInfo()
    {
        Managers.UpdateM.Register(this);
    }
  

    void OnClickCloseButton()
    {
        PlayerPrefs.SetFloat("BGM", GetSlider(SlidersType, (int)Sliders.BgmSlider).value);
        PlayerPrefs.SetFloat("EFFECT", GetSlider(SlidersType, (int)Sliders.EffectSlider).value);
        Managers.UpdateM.UnRegister(this);

        Managers.UIM.ClosePopup(this).Forget();
    }

    void OnClickCameraShakeButton()
    {
        isCameraShake = !isCameraShake;

        GetObject(GameObjectsType, (int)GameObjects.CameraShakeCheckImage).SetActive(isCameraShake);
    }

    public void Tick(float _deltaTime)
    {
        Managers.SoundM.BgmValue = GetSlider(SlidersType, (int)Sliders.BgmSlider).value;
        Managers.SoundM.EffectValue = GetSlider(SlidersType, (int)Sliders.EffectSlider).value;
    }
}
