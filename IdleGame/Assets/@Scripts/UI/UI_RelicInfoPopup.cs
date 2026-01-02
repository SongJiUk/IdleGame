using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class UI_RelicInfoPopup : UI_Popup
{

    #region Enum
    enum Buttons
    {
        CloseButton,
        GachaButton,
        EnforceButton

    }
    enum Texts
    {
        RelicInfoText,
        RelicInfoGradeText,
        RelicInfoDescriptionText,
        CombatPowerText,
        AttackPowerText,
        HelathPowerText,
        RelicLevelText,
        RelicLevelCountText,
        RelicEffectText,
        RelicDescriptionNameText,
        RelicDescriptionText
    }

    enum Images
    {
        RelicInfoBackGroundImage,
        RelicInfoImage,
        RelicLevelCountFill,
        RelicDescriptionImage,
    }


    #endregion
    Data.ItemData data;
    public System.Action OnChangeRelicInfo;



    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        ButtonsType = typeof(Buttons);
        TextsType = typeof(Texts);
        ImagesType = typeof(Images);

        BindButton(ButtonsType);
        BindText(TextsType);
        BindImage(ImagesType);

        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton(ButtonsType, (int)Buttons.GachaButton).gameObject.BindEvent(OnClickGachaButton);
        GetButton(ButtonsType, (int)Buttons.EnforceButton).gameObject.BindEvent(OnClickEnforceButton);
        return true;
    }

    public void SetInfo(Data.ItemData _data)
    {
        data = _data;
        RefreshUI();
    }

    public void RefreshUI()
    {
        GetText(TextsType, (int)Texts.RelicInfoText).text = Utils.StringToColorGrade(data.ItemGrade) + data.NameKR + "</color>";
        GetText(TextsType, (int)Texts.RelicInfoGradeText).text = Utils.StringToColorGrade(data.ItemGrade) + data.ItemGrade.ToString() + "</color>";
        GetText(TextsType, (int)Texts.RelicInfoDescriptionText).text = data.Description;
        GetImage(ImagesType, (int)Images.RelicInfoBackGroundImage).sprite = Managers.ResourceM.GetAtlas(data.ItemGrade.ToString());
        GetImage(ImagesType, (int)Images.RelicInfoImage).sprite = Managers.ResourceM.GetAtlas(data.Name);
        GetImage(ImagesType, (int)Images.RelicInfoImage).SetNativeSize();


        // var damage = Managers.PlayerM.GetAttack(data.CharacterGrade, Managers.GameM.gameData.Characters_Data[data.Name]);
        // var hp = Managers.PlayerM.GetHP(data.CharacterGrade, Managers.GameM.gameData.Characters_Data[data.Name]);
        //GetText(TextsType, (int)Texts.CombatPowerText).text = Utils.ToCurrencyString(damage + hp);

        //GetText(TextsType, (int)Texts.AttackPowerText).text = $"+ {Utils.ToCurrencyString(damage)}";
        //GetText(TextsType, (int)Texts.HelathPowerText).text = $"+ {Utils.ToCurrencyString(hp)}";



        Managers.GameM.gameData.Item_Data.TryGetValue(data.Name, out var itemData);
        if (itemData != null)
        {
            int needCount = itemData.holder.Level * 5;
            GetText(TextsType, (int)Texts.RelicLevelText).text = $"Lv. {itemData.holder.Level}";
            GetText(TextsType, (int)Texts.RelicLevelCountText).text = $"({itemData.holder.Count} / {needCount})";
            GetImage(ImagesType, (int)Images.RelicLevelCountFill).fillAmount = (float)itemData.holder.Count / (float)(needCount);
        }

        // Managers.DataM.SkillDataDic.TryGetValue(data.SkillDataID, out var skillData);
        // if (skillData != null)
        // {
        //     //TODO : 스킬 이미지 뽑아서 하기
        //     //GetImage(ImagesType, (int)Images.SkillIDescriptionImage).sprite = Managers.ResourceM.GetAtlas(skillData.SkillName);
        //     GetText(TextsType, (int)Texts.SkillEffectText).text = "짱 쎔";
        //     GetText(TextsType, (int)Texts.SkillDescriptionNameText).text = skillData.SkillNameKR;
        //     GetText(TextsType, (int)Texts.SkillDescriptionText).text = skillData.Description;
        // }

    }

    void OnClickCloseButton()
    {
        Managers.UIM.ClosePopup(this).Forget();
    }

    void OnClickGachaButton()
    {
        Managers.UIM.CloseAllPopup();
        Managers.UIM.ShowPopup<UI_ShopPopup>().Forget();
    }

    void OnClickEnforceButton()
    {
        if (Managers.GameM.gameData.Characters_Data.TryGetValue(data.Name, out var characterData))
        {
            int needCount = characterData.holder.Level * 5;
            if (needCount <= characterData.holder.Count)
            {
                characterData.holder.Count -= needCount;
                characterData.holder.Level++;
                OnChangeRelicInfo?.Invoke();
                RefreshUI();
            }
            else
                Managers.UIM.ShowToast("캐릭터의 개수가 부족합니다.");
        }
    }
}
