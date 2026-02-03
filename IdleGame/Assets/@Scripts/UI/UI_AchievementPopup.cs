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
    List<UI_AchievementItem> itemPool = new List<UI_AchievementItem>();
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
        int index = 0;
        foreach (var data in Managers.DataM.AchievementDataDic)
        {
            UI_AchievementItem item;
            if (index < itemPool.Count)
            {
                item = itemPool[index];
                item.gameObject.SetActive(true);
            }
            else
            {
                item = Managers.UIM.MakeSubItem<UI_AchievementItem>(parent);
                item.Init().Forget();
                itemPool.Add(item);
            }

            item.SetInfo(data.Value);
            index++;
        }


        for (int i = 0; i < itemPool.Count; i++)
        {
            if(Managers.QuestM.AchievementDic[itemPool[i].data.AchievementID])
            {
                itemPool[i].transform.SetAsLastSibling();
            }
        }
    }

    void OnClickCloseButton()
    {
        Managers.UIM.ClosePopup(this).Forget();

    }
}
