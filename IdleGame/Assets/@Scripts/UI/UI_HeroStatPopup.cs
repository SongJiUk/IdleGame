using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_HeroStatPopup : UI_Popup
{
    #region Enum
    enum Texts
    {
        LevelText,
        CombatPowerText,
        AttackPowerText,
        HealthText,
        GoldDropRateText,
        ItemDropRateText,
        AttackSpeedText,
        CriticalChanceText,
        CriticalDamageText,

    }

    enum GameObjects
    {
        StatObject,
        MasteryObject,
        BottomButtonBarObject,
    }

    enum Buttons
    {
        StatButton,
        MasteryButton,
        CostumeButton,
        CloseButton
    }



    #endregion


    RectTransform targetRect;
    RectTransform statBtnRect;
    RectTransform masteryBtnBect;
    RectTransform costumeBtnRect;

    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        TextsType = typeof(Texts);
        GameObjectsType = typeof(GameObjects);
        ButtonsType = typeof(Buttons);

        BindText(TextsType);
        BindObject(GameObjectsType);
        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.StatButton).gameObject.BindEvent(OnClickStatButton);
        GetButton(ButtonsType, (int)Buttons.MasteryButton).gameObject.BindEvent(OnClickMasteryButton);
        GetButton(ButtonsType, (int)Buttons.CostumeButton).gameObject.BindEvent(OnClickCostumeButton);
        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);



        targetRect = GetObject(GameObjectsType, (int)GameObjects.BottomButtonBarObject).GetComponent<RectTransform>();
        statBtnRect = GetButton(ButtonsType, (int)Buttons.StatButton).GetComponent<RectTransform>();
        masteryBtnBect = GetButton(ButtonsType, (int)Buttons.MasteryButton).GetComponent<RectTransform>();
        costumeBtnRect = GetButton(ButtonsType, (int)Buttons.CostumeButton).GetComponent<RectTransform>();

        MoveTarget(statBtnRect, false);

        GetObject(GameObjectsType, (int)GameObjects.MasteryObject).SetActive(false);
        return true;
    }


    public override void SetInfo()
    {
        RefreshUI();

    }


    void RefreshUI()
    {
        GetText(TextsType, (int)Texts.CombatPowerText).text = Utils.ToCurrencyString(Managers.PlayerM.AverageCombatPower());
        GetText(TextsType, (int)Texts.AttackPowerText).text = Utils.ToCurrencyString(Managers.PlayerM.MainAttack());
        GetText(TextsType, (int)Texts.HealthText).text = Utils.ToCurrencyString(Managers.PlayerM.MainHP());

        GetText(TextsType, (int)Texts.GoldDropRateText).text = string.Format("{0:0}%", Managers.PlayerM.GoldDrop() * 100f);
        GetText(TextsType, (int)Texts.ItemDropRateText).text = string.Format("{0:0}%", Managers.PlayerM.ItemDrop());
        GetText(TextsType, (int)Texts.AttackSpeedText).text = string.Format("{0:0}%", Managers.PlayerM.AttackSpeed());
        GetText(TextsType, (int)Texts.CriticalChanceText).text = string.Format("{0:0}%", Managers.PlayerM.CriticalChance());
        GetText(TextsType, (int)Texts.CriticalDamageText).text = string.Format("{0:0}%", Managers.PlayerM.CriticalDamage());
    }


    void MoveTarget(RectTransform _target, bool _animate = true)
    {

        Vector3 worldPos = targetRect.position;


        targetRect.SetParent(_target, false);
        targetRect.SetAsFirstSibling();

        targetRect.anchorMin = new Vector2(0.5f, 0.5f);
        targetRect.anchorMax = new Vector2(0.5f, 0.5f);
        targetRect.pivot = new Vector2(0.5f, 0.5f);
        targetRect.position = worldPos;




        if (_animate)
        {
            targetRect.localScale = Vector3.one * 0.9f;
            targetRect.DOScale(1f, 0.15f).SetEase(Ease.OutBack);
            targetRect.DOAnchorPos(Vector2.zero, 0.25f).SetEase(Ease.OutQuad);
        }
        else targetRect.anchoredPosition = Vector2.zero;

    }
    void OnClickStatButton()
    {
        GetObject(GameObjectsType, (int)GameObjects.StatObject).SetActive(true);
        GetObject(GameObjectsType, (int)GameObjects.MasteryObject).SetActive(false);

        RefreshUI();
        MoveTarget(statBtnRect, true);
    }

    void OnClickMasteryButton()
    {
        GetObject(GameObjectsType, (int)GameObjects.StatObject).SetActive(false);
        GetObject(GameObjectsType, (int)GameObjects.MasteryObject).SetActive(true);

        MoveTarget(masteryBtnBect, true);
    }

    void OnClickCostumeButton()
    {
        GetObject(GameObjectsType, (int)GameObjects.StatObject).SetActive(false);
        GetObject(GameObjectsType, (int)GameObjects.MasteryObject).SetActive(false);


        MoveTarget(costumeBtnRect, true);
    }

    async void OnClickCloseButton()
    {
        await TriggerClose(this, true);
    }
}
