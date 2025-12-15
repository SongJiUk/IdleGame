using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    bool isAttackBuffing = false;
    bool isGoldBuffing = false;
    bool isCriticalBuffing = false;

    public class BuffData
    {
        public Define.BuffType BuffType;
        public float remainTime;
    }
    List<BuffData> BuffTimers = new List<BuffData>();

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

        //TODO : 바꾸기
        GetText(TextsType, (int)Texts.Level_Text).text = Managers.GameM.level.ToString();
        GetText(TextsType, (int)Texts.Count_Text).text = $"{1} / {3}";
        //GetImage(ImagesType, (int)Images.SliderFillImage).fillAmount = 

        RefreshPopup();

    }

    public void RefreshPopup()
    {
        //TODO : 최적화 하기.
        GetObject(GameObjectsType, (int)GameObjects.AttackUpTimeObject).SetActive(isAttackBuffing);
        GetObject(GameObjectsType, (int)GameObjects.AttackUpButtonLockObject).SetActive(isAttackBuffing);
        GetObject(GameObjectsType, (int)GameObjects.AttackUpLockObject).SetActive(!isAttackBuffing);
        GetObject(GameObjectsType, (int)GameObjects.AttackUpCoolTimeObject).SetActive(isAttackBuffing);


        GetObject(GameObjectsType, (int)GameObjects.GoldUpTimeObject).SetActive(isGoldBuffing);
        GetObject(GameObjectsType, (int)GameObjects.GoldUpButtonLockObject).SetActive(isGoldBuffing);
        GetObject(GameObjectsType, (int)GameObjects.GoldUpLockObject).SetActive(!isGoldBuffing);
        GetObject(GameObjectsType, (int)GameObjects.GoldUpCoolTimeObject).SetActive(isGoldBuffing);

        GetObject(GameObjectsType, (int)GameObjects.CriticalUpTimeObject).SetActive(isCriticalBuffing);
        GetObject(GameObjectsType, (int)GameObjects.CriticalUpButtonLockObject).SetActive(isCriticalBuffing);
        GetObject(GameObjectsType, (int)GameObjects.CriticalUpLockObject).SetActive(!isCriticalBuffing);
        GetObject(GameObjectsType, (int)GameObjects.CriticalUpCoolTimeObject).SetActive(isCriticalBuffing);
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
        Managers.UpdateM.Register(_unscaledTickable: this);
        BuffData item = new BuffData { BuffType = _type, remainTime = 10f };
        BuffTimers.Add(item);

        switch (_type)
        {
            case Define.BuffType.AttackUp:

                isAttackBuffing = true;
                (Managers.UIM.SceneUI as UI_GameScene).SetBuffs(_type, isAttackBuffing);
                RefreshPopup();
                break;

            case Define.BuffType.GoldUp:
                isGoldBuffing = true;
                (Managers.UIM.SceneUI as UI_GameScene).SetBuffs(_type, isGoldBuffing);
                RefreshPopup();
                break;

            case Define.BuffType.CriticalUp:
                isCriticalBuffing = true;
                (Managers.UIM.SceneUI as UI_GameScene).SetBuffs(_type, isCriticalBuffing);
                RefreshPopup();
                break;
        }
    }

    void OnClickCloseButton()
    {
        Managers.UIM.ClosePopup(this).Forget();
    }

    public void UnscaledTick(float _unscaledDeltaTime)
    {

        for (int i = BuffTimers.Count - 1; i >= 0; i--)
        {
            BuffData buff = BuffTimers[i];
            buff.remainTime -= _unscaledDeltaTime;

            if (buff.remainTime < 0.0f)
            {
                switch (buff.BuffType)
                {
                    case Define.BuffType.AttackUp:

                        isAttackBuffing = false;
                        (Managers.UIM.SceneUI as UI_GameScene).SetBuffs(buff.BuffType, isAttackBuffing);
                        RefreshPopup();
                        break;

                    case Define.BuffType.GoldUp:
                        isGoldBuffing = false;
                        (Managers.UIM.SceneUI as UI_GameScene).SetBuffs(buff.BuffType, isGoldBuffing);
                        RefreshPopup();
                        break;

                    case Define.BuffType.CriticalUp:
                        isCriticalBuffing = false;
                        (Managers.UIM.SceneUI as UI_GameScene).SetBuffs(buff.BuffType, isCriticalBuffing);
                        RefreshPopup();
                        break;
                }
                BuffTimers.RemoveAt(i);

            }
            else
            {
                if (gameObject.activeSelf) RefreshTimeObject(buff.BuffType, buff.remainTime);
            }
        }

        if (BuffTimers.Count == 0)
            Managers.UpdateM.UnRegister(_unscaledTickable: this);
    }
}
