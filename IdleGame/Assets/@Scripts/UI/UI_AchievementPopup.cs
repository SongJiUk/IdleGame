using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class UI_AchievementPopup : UI_Popup
{
    #region enum
    enum GameObjects
    {
        Content,

    }

    enum Texts
    {
        AchievementAbilityText
    }

    enum Buttons
    {
        CloseButton,

    }
    #endregion
    Transform parent;
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        GameObjectsType = typeof(GameObjects);
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);

        BindObject(GameObjectsType);
        BindText(TextsType);
        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        parent = GetObject(GameObjectsType, (int)GameObjects.Content).transform;
        return true;
    }


    public override void SetInfo()
    {
        RefreshUI();
    }
    void RefreshUI()
    {
        foreach (var data in Managers.DataM.AchievementDataDic)
        {
            var item = Managers.UIM.MakeSubItem<UI_AchievementItem>(parent);
            item.Init().Forget();
            item.SetInfo(data.Value);
        }
    }

    void OnClickCloseButton()
    {
        Managers.UIM.ClosePopup(this);

    }
}
