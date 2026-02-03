using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEditor;
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
        CollectButton,
    }
    #endregion
    Transform parent;
    public Data.AchievementData data;

    List<UI_AchievementIcon> iconPool = new List<UI_AchievementIcon>();
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        GameObjectsType = typeof(GameObjects);
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);

        BindObject(GameObjectsType);
        BindText(TextsType);
        BindButton(ButtonsType);

        GetObject(GameObjectsType, (int)GameObjects.CollectObject).SetActive(false);
        parent = GetObject(GameObjectsType, (int)GameObjects.Content).transform;
        return true;
    }

    public void SetInfo(Data.AchievementData _data)
    {
        data = _data;
        GetText(TextsType, (int)Texts.AchievementNameText).text = data.Title;
        GetText(TextsType, (int)Texts.AchievementEffectText).text = Managers.LocalM.localData[data.RewardStatus.ToString()].GetData() + " + " + data.RewardValue + "%";

        GetButton(ButtonsType, (int)Buttons.CollectButton).onClick.RemoveAllListeners();

        if (Managers.QuestM.AchievementDic[data.AchievementID])
        {
            GetObject(GameObjectsType, (int)GameObjects.CollectObject).SetActive(true);
        }

        int index = 0;
        int achievementID = data.AchievementID;
        switch (data.AchievementType)
        {
            case Define.AchievementType.Hero:
                bool isCanCollectHero = true;
                
                for (int i = 0; i < data.AchievementCharactersList.Count; i++)
                {
                    UI_AchievementIcon icon;
                    if (index < iconPool.Count)
                    {
                        icon = iconPool[index];
                        icon.gameObject.SetActive(true);
                    }
                    else
                    {
                        icon = Managers.UIM.MakeSubItem<UI_AchievementIcon>(parent);
                        icon.Init().Forget();
                        iconPool.Add(icon);

                    }

                    int dataID = data.AchievementCharactersList[i];
                    string cName = Managers.DataM.CreatureDataDic[dataID].Name;
                    bool levelCompleted =
                        Managers.GameM.gameData.Character_Holder[cName].Level >= data.AchievementCharactersLevelList[i];

                    icon.SetInfo(data.AchievementCharactersList[i], data.AchievementCharactersLevelList[i]);
                    index++;

                    if (!levelCompleted)
                    {
                        isCanCollectHero = false;
                    }
                }

                for (int i = index; i < iconPool.Count; i++)
                {
                    iconPool[i].gameObject.SetActive(false);
                }

                GetButton(ButtonsType, (int)Buttons.CollectButton).gameObject.BindEvent(() => OnClickCollectButton(achievementID, isCanCollectHero));
                break;

            case Define.AchievementType.Relic:
                bool isCanCollectRelic = true;
                int relicNum = -1;
                for (int i = 0; i < data.AchievementRelicList.Count; i++)
                {
                    UI_AchievementIcon icon;
                    if (index < iconPool.Count)
                    {
                        icon = iconPool[index];
                        icon.gameObject.SetActive(true);
                    }
                    else
                    {
                        icon = Managers.UIM.MakeSubItem<UI_AchievementIcon>(parent);
                        icon.Init().Forget();
                        iconPool.Add(icon);
                    }

                    int dataID = data.AchievementRelicList[i];
                    var itemData = Managers.DataM.ItemDataDic[dataID];

                    var item = Managers.GameM.gameData.Item_Data[itemData.Name];
                    if (item.holder.Count == 0)
                    {
                        isCanCollectRelic = false;
                    }

                    icon.SetInfo(data.AchievementRelicList[i]);
                    index++;
                }

                GetButton(ButtonsType, (int)Buttons.CollectButton).gameObject.BindEvent(() => OnClickCollectButton(relicNum, isCanCollectRelic));
                break;
        }

    }


    void OnClickCollectButton(int _achievementID, bool _isCollect)
    {
        if (_isCollect)
        {
            Managers.QuestM.AchievementDic[_achievementID] = true;
            Managers.QuestM.LoadAchievementDatas();
            SetInfo(data);
        }
        else
        {
            Managers.UIM.ShowToast(Managers.LocalM.localData["NoMaterial"].GetData());
        }

    }
}
