using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        ScrollView,
    }
    #endregion
    Transform parent = null;

    List<UI_MissionItem> itemPool = new List<UI_MissionItem>();
    ScrollRect scrollRect;
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        ButtonsType = typeof(Buttons);
        GameObjectsType = typeof(GameObjects);

        BindButton(ButtonsType);
        BindObject(GameObjectsType);

        parent = GetObject(GameObjectsType, (int)GameObjects.Content).transform;
        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        scrollRect = GetObject(GameObjectsType, (int)GameObjects.ScrollView).GetComponent<ScrollRect>();
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
            if(Managers.QuestM.MissionDic[itemPool[i].data.MissionTarget.ToString()].isRewarded)
            {
                itemPool[i].transform.SetAsLastSibling();
            }

        }


        for (int i = index; i < itemPool.Count; i++)
        {
            itemPool[i].gameObject.SetActive(false);
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }

    void OnClickCloseButton()
    {
        Managers.UIM.ClosePopup(this).Forget();
    }

}
