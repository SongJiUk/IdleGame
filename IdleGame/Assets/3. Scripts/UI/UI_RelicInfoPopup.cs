using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;

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
        RelicLevelText,
        RelicLevelCountText,
        RelicEffectText,
    }

    enum Images
    {
        RelicInfoBackGroundImage,
        RelicInfoImage,
        RelicLevelCountFill,
        //RelicDescriptionImage,
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
        GetText(TextsType, (int)Texts.RelicInfoText).text = Utils.StringToColorGrade(data.ItemGrade) + Managers.LocalizationM.Get(data.Name) + "</color>";
        GetText(TextsType, (int)Texts.RelicInfoGradeText).text = Utils.StringToColorGrade(data.ItemGrade) + data.ItemGrade.ToString() + "</color>";
        GetText(TextsType, (int)Texts.RelicInfoDescriptionText).text = Managers.LocalizationM.Get($"{data.Name}_Description");
        GetImage(ImagesType, (int)Images.RelicInfoBackGroundImage).sprite = Managers.ResourceM.GetAtlas(data.ItemGrade.ToString());
        GetImage(ImagesType, (int)Images.RelicInfoImage).sprite = Managers.ResourceM.GetAtlas(data.Name);
        GetImage(ImagesType, (int)Images.RelicInfoImage).SetNativeSize();


        Managers.GameM.gameData.Item_Data.TryGetValue(data.Name, out var itemData);
        if (itemData != null)
        {
            int needCount = itemData.holder.Level * 5;
            GetText(TextsType, (int)Texts.RelicLevelText).text = $"Lv. {itemData.holder.Level}";
            GetText(TextsType, (int)Texts.RelicLevelCountText).text = $"({itemData.holder.Count} / {needCount})";
            GetImage(ImagesType, (int)Images.RelicLevelCountFill).fillAmount = (float)itemData.holder.Count / (float)(needCount);
        }

        var values = itemData.GetCurrentValues();
        object[] args = values.Select(val => (object)val.ToString("0.#")).ToArray();

        GetText(TextsType, (int)Texts.RelicEffectText).text = string.Format(Managers.LocalizationM.Get($"{data.Name}_Effect"), args);

    }

    void OnClickCloseButton()
    {
        Managers.SoundM.PlayButtonClick();
        Managers.UIM.ClosePopup(this).Forget();
    }

    void OnClickGachaButton()
    {
        Managers.SoundM.PlayButtonClick();
        var gameScene = Managers.UIM.SceneUI as UI_GameScene;
        if (gameScene != null)
        {
            Managers.UIM.ClosePopup(this).Forget();
            gameScene.OpenShopFormOtherUI().Forget();
        }
    }

    void OnClickEnforceButton()
    {
        Managers.SoundM.PlayButtonClick();
        if (Managers.GameM.gameData.Item_Data.TryGetValue(data.Name, out var itemData))
        {
            if (itemData.holder.Level < 20)
            {
                int needCount = itemData.holder.Level * 5;
                if (needCount <= itemData.holder.Count)
                {
                    itemData.holder.Count -= needCount;
                    itemData.holder.Level++;
                    OnChangeRelicInfo?.Invoke();
                    RefreshUI();
                }
                else
                    Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastNoRelic"));
            }
            else Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastRelicMaxLevel"));


        }
    }
}
