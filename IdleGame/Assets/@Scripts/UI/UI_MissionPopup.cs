using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MissionPopup : UI_Popup
{
    #region enum

    enum Buttons
    {
        CloseButton,

    }

    enum GameObjects
    {
        Content,
    }
    #endregion
    Transform parent = null;

    List<UI_MissionItem> itemPool = new List<UI_MissionItem>();
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        ButtonsType = typeof(Buttons);
        GameObjectsType = typeof(GameObjects);

        BindButton(ButtonsType);
        BindObject(GameObjectsType);

        parent = GetObject(GameObjectsType, (int)GameObjects.Content).transform;
        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        return true;
    }

    public override void SetInfo()
    {
        int index = 0;
        foreach (var data in Managers.DataM.MissionDataDic)
        {
            if (data.Value.MissionType != Define.MissionType.Daily) continue;

            UI_MissionItem item;

            if (index < itemPool.Count)
            {
                item = itemPool[index];
                item.gameObject.SetActive(true);
            }
            else
            {
                item = Managers.UIM.MakeSubItem<UI_MissionItem>(parent);
                item.Init().Forget();
                itemPool.Add(item);
            }

            item.SetInfo(data.Value, index);
            index++;
        }

        for (int i = 0; i < itemPool.Count; i++)
        {
            if (Managers.GameM.gameData.IsDailyMissions[i])
            {
                itemPool[i].transform.SetAsLastSibling();
            }
        }


        for (int i = index; i < itemPool.Count; i++)
        {
            itemPool[i].gameObject.SetActive(false);
        }
    }

    void OnClickCloseButton()
    {
        Managers.UIM.ClosePopup(this).Forget();
    }

}
