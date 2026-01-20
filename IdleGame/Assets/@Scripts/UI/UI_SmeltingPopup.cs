using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SmeltingPopup : UI_Popup
{
    #region Enum


    enum GameObjects
    {
        Horizontal,
    }
    enum Texts
    {
        Smelting_1_ContentText,
        Smelting_2_ContentText,
        Smelting_3_ContentText,
        Smelting_4_ContentText,
        Smelting_5_ContentText,
        Smelting_1_ProbabilityText,
        Smelting_2_ProbabilityText,
        Smelting_3_ProbabilityText,
        Smelting_4_ProbabilityText,
        Smelting_5_ProbabilityText,
    }

    enum Buttons
    {
        CloseButton,
        SmeltingButton,
    }

    enum Images
    {
        LockImage
    }

    #endregion

    Transform parent;

    Data.SmeltData data;
    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        GameObjectsType = typeof(GameObjects);
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);
        ImagesType = typeof(Images);

        BindObject(GameObjectsType);
        BindText(TextsType);
        BindButton(ButtonsType);
        BindImage(ImagesType);

        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton(ButtonsType, (int)Buttons.SmeltingButton).gameObject.BindEvent(OnClickSmeltingButton);

        parent = GetObject(GameObjectsType, (int)GameObjects.Horizontal).transform;

        return true;
    }

    public void SetInfo()
    {
    }


    async void OnClickCloseButton()
    {
        await TriggerClose(this, false);
    }

    void OnClickSmeltingButton()
    {
    }
}
