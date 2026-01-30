using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class UI_AchievementItem : UI_Base
{
    #region enum
    enum GameObjects
    {
        Content,
        CollectObject,
    }

    enum Texts
    {
        AchievementNameText,
        AchievementEffectText,

    }

    enum Buttons
    {
        UpgradeButton,
    }
    #endregion
    Transform parent;
    Data.AchievementData data;
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        GameObjectsType = typeof(GameObjects);
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);

        BindObject(GameObjectsType);
        BindText(TextsType);
        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.UpgradeButton).gameObject.BindEvent(OnClickUpgradeButton);
        GetObject(GameObjectsType, (int)GameObjects.CollectObject).SetActive(false);
        parent = GetObject(GameObjectsType, (int)GameObjects.Content).transform;
        return true;
    }

    public void SetInfo(Data.AchievementData _data)
    {
        data = _data;
        GetText(TextsType, (int)Texts.AchievementNameText).text = data.Title;
        GetText(TextsType, (int)Texts.AchievementEffectText).text = data.RewardStatus.ToString();

        if (data.AchievementType == Define.AchievementType.Hero)
        {
            for (int i = 0; i < data.AchievementCharactersList.Count; i++)
            {
                var item = Managers.UIM.MakeSubItem<UI_AchievementIcon>(parent);
                item.Init().Forget();
                item.SetInfo(data.AchievementCharactersList[i], data.AchievementCharactersLevelList[i]);
            }
        }
        else
        {
            for (int i = 0; i < data.AchievementRelicList.Count; i++)
            {

                var item = Managers.UIM.MakeSubItem<UI_AchievementIcon>(parent);
                item.Init().Forget();
                item.SetInfo(data.AchievementRelicList[i]);
            }
        }
    }


    void OnClickUpgradeButton()
    {
    }
}
