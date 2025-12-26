using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CharacterUpgradePopup : UI_Popup
{
    #region Enum

    enum GameObjects
    {
        Content
    }

    enum Buttons
    {
        CloseButton
    }

    #endregion

    Transform parent = null;
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        GameObjectsType = typeof(GameObjects);
        ButtonsType = typeof(Buttons);

        BindObject(GameObjectsType);
        BindButton(ButtonsType);

        parent = GetObject(GameObjectsType, (int)GameObjects.Content).transform;

        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        return true;
    }


    public void SetInfo(List<CharacterHolder> _characterList)
    {
        for (int i = 0; i < _characterList.Count; i++)
        {
            var characterHolder = _characterList[i];
            var icon = Managers.UIM.MakeSubItem<UI_UpgradeHeroIcon>(parent);

            icon.Init().Forget();
            icon.SetInfo(characterHolder);
        }
    }


    void OnClickCloseButton()
    {
        Managers.UIM.ClosePopup(this).Forget();
    }
}
