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
    public System.Action OnChangeCharacterInfo;
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
        GetText(TextsType, (int)Texts.CharacterInfoText).text = Utils.StringToColorGrade(data.CharacterGrade) + data.NameKR + "</color>";
        GetText(TextsType, (int)Texts.CharacterInfoGradeText).text = Utils.StringToColorGrade(data.CharacterGrade) + data.CharacterGrade.ToString() + "</color>";
        GetText(TextsType, (int)Texts.CharacterInfoDescriptionText).text = data.Description;
        GetImage(ImagesType, (int)Images.CharacterInfoBackGround).sprite = Managers.ResourceM.GetAtlas(data.CharacterGrade.ToString());
        GetImage(ImagesType, (int)Images.CharacterInfoImage).sprite = Managers.ResourceM.GetAtlas(data.Name);

        var damage = Managers.PlayerM.GetAttack(data.CharacterGrade, Managers.GameM.gameData.Characters_Data[data.Name]);
        var hp = Managers.PlayerM.GetHP(data.CharacterGrade, Managers.GameM.gameData.Characters_Data[data.Name]);
        GetText(TextsType, (int)Texts.CombatPowerText).text = Utils.ToCurrencyString(damage + hp);

        GetText(TextsType, (int)Texts.AttackPowerText).text = $"+ {Utils.ToCurrencyString(damage)}";
        GetText(TextsType, (int)Texts.HelathPowerText).text = $"+ {Utils.ToCurrencyString(hp)}";



        Managers.GameM.gameData.Characters_Data.TryGetValue(data.Name, out var characterData);
        if (characterData != null)
        {
            int needCount = characterData.holder.Level * 5;
            GetText(TextsType, (int)Texts.CharacterLevelText).text = $"Lv. {characterData.holder.Level}";
            GetText(TextsType, (int)Texts.CharacterLevelCountText).text = $"({characterData.holder.Count} / {needCount})";
            GetImage(ImagesType, (int)Images.CharacterLevelCountFill).fillAmount = (float)characterData.holder.Count / (float)(needCount);
        }

        Managers.DataM.SkillDataDic.TryGetValue(data.SkillDataID, out var skillData);
        if (skillData != null)
        {
            //TODO : 스킬 이미지 뽑아서 하기
            //GetImage(ImagesType, (int)Images.SkillIDescriptionImage).sprite = Managers.ResourceM.GetAtlas(skillData.SkillName);
            GetText(TextsType, (int)Texts.SkillEffectText).text = "짱 쎔";
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
        if (Managers.GameM.gameData.Characters_Data.TryGetValue(data.Name, out var characterData))
        {
            int needCount = characterData.holder.Level * 5;
            if (needCount <= characterData.holder.Count)
            {
                characterData.holder.Count -= needCount;
                characterData.holder.Level++;
                OnChangeCharacterInfo?.Invoke();
                RefreshUI();
            }
        }
    }

}
