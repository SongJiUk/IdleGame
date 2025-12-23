using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CharacterInfoPopup : UI_Popup
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
        CharacterInfoText,
        CharacterInfoGradeText,
        CharacterInfoDescriptionText,
        CombatPowerText,
        AttackPowerText,
        HelathPowerText,
        CharacterLevelText,
        CharacterLevelCountText,
        SkillEffectText,
        SkillDescriptionNameText,
        SkillDescriptionText
    }

    enum Images
    {
        CharacterInfoBackGround,
        CharacterInfoImage,
        CharacterLevelCountFill,
        SkillIDescriptionImage,
    }


    #endregion
    Data.CreatureData data;
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

    public void SetInfo(Data.CreatureData _data)
    {
        data = _data;
        RefreshUI();
    }

    public void RefreshUI()
    {
        GetText(TextsType, (int)Texts.CharacterInfoText).text = data.NameKR;
        GetText(TextsType, (int)Texts.CharacterInfoGradeText).text = data.CharacterGrade.ToString();
        GetText(TextsType, (int)Texts.CharacterInfoDescriptionText).text = data.Description;
        GetImage(ImagesType, (int)Images.CharacterInfoBackGround).sprite = Managers.ResourceM.GetAtlas(data.CharacterGrade.ToString());
        GetImage(ImagesType, (int)Images.CharacterInfoImage).sprite = Managers.ResourceM.GetAtlas(data.Name);
        

        GetText(TextsType, (int)Texts.CombatPowerText).text = data.Description;
        GetText(TextsType, (int)Texts.AttackPowerText).text = data.Description;
        GetText(TextsType, (int)Texts.HelathPowerText).text = data.Description;
        GetImage(ImagesType, (int)Images.CharacterLevelCountFill).fillAmount = 0.3f;
      
        

        Managers.DataM.SkillDataDic.TryGetValue(data.SkillDataID, out var skillData);
        if(skillData != null)
        {
            //GetImage(ImagesType, (int)Images.SkillIDescriptionImage).sprite = Managers.ResourceM.GetAtlas(skillData.SkillName);

            GetText(TextsType, (int)Texts.CharacterLevelText).text = data.Description;
            GetText(TextsType, (int)Texts.CharacterLevelCountText).text = data.Description;
            GetText(TextsType, (int)Texts.SkillEffectText).text = data.Description;

            GetText(TextsType, (int)Texts.SkillDescriptionNameText).text = skillData.SkillNameKR;
            GetText(TextsType, (int)Texts.SkillDescriptionText).text = skillData.Description;
        }

    }

    void OnClickCloseButton()
    {
        Managers.UIM.ClosePopup(this).Forget();
    }

    void OnClickGachaButton()
    {

    }

    void OnClickEnforceButton()
    {

    }

}
