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
        CostumeButton
    }



    #endregion


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

    void OnClickStatButton()
    {
        Transform targetTr = GetObject(GameObjectsType, (int)GameObjects.BottomButtonBarObject).transform;
        Vector3 endPos = GetButton(ButtonsType, (int)Buttons.StatButton).transform.position;

        GetObject(GameObjectsType, (int)GameObjects.MasteryObject).SetActive(false);
        GetObject(GameObjectsType, (int)GameObjects.StatObject).SetActive(true);

        RefreshUI();
        targetTr.DOMove(endPos, 0.5f)
            .SetEase(Ease.OutQuad);
    }

    void OnClickMasteryButton()
    {
        Transform targetTr = GetObject(GameObjectsType, (int)GameObjects.BottomButtonBarObject).transform;
        Vector3 endPos = GetButton(ButtonsType, (int)Buttons.MasteryButton).transform.position;

        GetObject(GameObjectsType, (int)GameObjects.StatObject).SetActive(false);
        GetObject(GameObjectsType, (int)GameObjects.MasteryObject).SetActive(true);

        targetTr.DOMove(endPos, 0.5f)
            .SetEase(Ease.OutQuad);
    }

    void OnClickCostumeButton()
    {
        Transform targetTr = GetObject(GameObjectsType, (int)GameObjects.BottomButtonBarObject).transform;
        Vector3 endPos = GetButton(ButtonsType, (int)Buttons.CostumeButton).transform.position;

        GetObject(GameObjectsType, (int)GameObjects.StatObject).SetActive(false);
        GetObject(GameObjectsType, (int)GameObjects.MasteryObject).SetActive(false);
        

        targetTr.DOMove(endPos, 0.5f)
            .SetEase(Ease.OutQuad);
    }
}
