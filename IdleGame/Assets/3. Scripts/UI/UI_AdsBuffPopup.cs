using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UI_AdsBuffPopup : UI_Popup, IUnScaledTickable
{


    #region Enum
    enum GameObjects
    {
        AttackUpTimeObject,
        GoldUpTimeObject,
        CriticalUpTimeObject,
        AttackUpButtonLockObject,
        GoldUpButtonLockObject,
        CriticalUpButtonLockObject,
        AttackUpLockObject,
        GoldUpLockObject,
        CriticalUpLockObject,
        AttackUpCoolTimeObject,
        GoldUpCoolTimeObject,
        CriticalUpCoolTimeObject,


    }

    enum Texts
    {
        Level_Text,
        Count_Text,
        AttackUpTimeText,
        GoldUpTimeText,
        CriticalUpTimeText,

    }

    enum Images
    {
        SliderFillImage,
        AttackUpCoolTimeImage,
        GoldUpCoolTimeImage,
        CriticalUpCoolTimeImage
    }

    public enum Buttons
    {
        CloseButton,
        AttackUpButton,
        GoldUpButton,
        CriticalUpButton

    }
    #endregion

    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        GameObjectsType = typeof(GameObjects);
        TextsType = typeof(Texts);
        ImagesType = typeof(Images);
        ButtonsType = typeof(Buttons);

        BindObject(GameObjectsType);
        BindText(TextsType);
        BindImage(ImagesType);
        BindButton(ButtonsType);



        GetButton(ButtonsType, (int)Buttons.AttackUpButton).gameObject.BindEvent(() => OnClickUpButton(Define.BuffType.AttackUp));
        GetButton(ButtonsType, (int)Buttons.GoldUpButton).gameObject.BindEvent(() => OnClickUpButton(Define.BuffType.GoldUp));
        GetButton(ButtonsType, (int)Buttons.CriticalUpButton).gameObject.BindEvent(() => OnClickUpButton(Define.BuffType.CriticalUp));
        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        return true;
    }
    public override void SetInfo()
    {

        var data = Managers.GameM.gameData.BuffAds;
        int needCount = Managers.BuffM.GetNeedCount(data.level);
        GetText(TextsType, (int)Texts.Level_Text).text = $"Lv. {data.level}";
        GetText(TextsType, (int)Texts.Count_Text).text = $"{data.count} / {needCount}";
        float amount = (float)data.count / (float)needCount;
        GetImage(ImagesType, (int)Images.SliderFillImage).fillAmount = amount;


        RefreshUI();

        bool any = false;
        for (int i = 0; i < 3; i++)
        {
            if (Managers.BuffM.IsActive((Define.BuffType)i))
            {
                any = true;
                break;
            }
        }

        if (any) Managers.UpdateM.Register(_unscaledTickable: this);

    }


    void RefreshTimeObject(Define.BuffType _type, float _remainTime)
    {

        int min = Mathf.FloorToInt(_remainTime / 60f);
        int hour = Mathf.FloorToInt(_remainTime % 60f);
        float fillAmount = 1 - (_remainTime / 10f);
        string timeString = string.Format("{0:00} : {1:00}", min, hour);

        switch (_type)
        {
            case Define.BuffType.AttackUp:
                GetImage(ImagesType, (int)Images.AttackUpCoolTimeImage).fillAmount = fillAmount;
                GetText(TextsType, (int)Texts.AttackUpTimeText).text = timeString;
                break;

            case Define.BuffType.GoldUp:
                GetImage(ImagesType, (int)Images.GoldUpCoolTimeImage).fillAmount = fillAmount;
                GetText(TextsType, (int)Texts.GoldUpTimeText).text = timeString;
                break;

            case Define.BuffType.CriticalUp:
                GetImage(ImagesType, (int)Images.CriticalUpCoolTimeImage).fillAmount = fillAmount;
                GetText(TextsType, (int)Texts.CriticalUpTimeText).text = timeString;
                break;

        }
    }

    void OnClickUpButton(Define.BuffType _type)
    {
        Managers.SoundM.PlayButtonClick();
        Action rewardedAction = () =>
        {
            Managers.BuffM.OnWatchAd();
            Managers.BuffM.StartBuff(_type, 1800f);
            RefreshUI();
            Managers.UpdateM.Register(_unscaledTickable: this);
        };
        Managers.AdM.ShowRewardedAd(rewardedAction, null);
    }


    void RefreshUI()
    {

        var data = Managers.GameM.gameData.BuffAds;
        int needCount = Managers.BuffM.GetNeedCount(data.level);
        GetText(TextsType, (int)Texts.Level_Text).text = $"Lv. {data.level}";
        GetText(TextsType, (int)Texts.Count_Text).text = $"{data.count} / {needCount}";
        float amount = (float)data.count / (float)needCount;
        GetImage(ImagesType, (int)Images.SliderFillImage).fillAmount = amount;

        for (int i = 0; i < 3; i++)
        {
            var type = (Define.BuffType)i;
            bool active = Managers.BuffM.IsActive(type);

            GetObject(GameObjectsType, (int)GameObjects.AttackUpTimeObject + i).SetActive(active);
            GetObject(GameObjectsType, (int)GameObjects.AttackUpButtonLockObject + i).SetActive(active);
            GetObject(GameObjectsType, (int)GameObjects.AttackUpLockObject + i).SetActive(!active);
            GetObject(GameObjectsType, (int)GameObjects.AttackUpCoolTimeObject + i).SetActive(active);

            if (active)
            {
                float remain = Managers.BuffM.GetRemainTime(type);
                RefreshTimeObject(type, remain);
            }
        }
    }
    void OnClickCloseButton()
    {
        Managers.SoundM.PlayButtonClick();
        Managers.UIM.ClosePopup(this).Forget();
    }

    public void UnscaledTick(float _unscaledDeltaTime)
    {

        bool any = false;

        for (int i = 0; i < 3; i++)
        {
            var type = (Define.BuffType)i;
            if (Managers.BuffM.IsActive(type))
            {
                any = true;
                float remain = Managers.BuffM.GetRemainTime(type);
                RefreshTimeObject(type, remain);
            }
        }

        if (!any) Managers.UpdateM.UnRegister(_unscaledTickable: this);
    }

}
