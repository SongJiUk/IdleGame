using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class UI_AchievementIcon : UI_Base
{
    #region enum
    enum GameObjects
    {
        BlackObject
    }
    enum Images
    {
        AchievementImage,

    }

    enum Texts
    {
        AchievementCountText
    }
    #endregion

    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        GameObjectsType = typeof(GameObjects);
        ImagesType = typeof(Images);
        TextsType = typeof(Texts);

        BindObject(GameObjectsType);
        BindImage(ImagesType);
        BindText(TextsType);

        GetObject(GameObjectsType, (int)GameObjects.BlackObject).SetActive(false);
        return true;
    }

    public void SetInfo(int _dataID, int _level = 0)
    {
        if (_level != 0)
        {
            if (Managers.DataM.CreatureDataDic.TryGetValue(_dataID, out var data))
            {
                GetImage(ImagesType, (int)Images.AchievementImage).sprite = Managers.ResourceM.GetAtlas(data.Name);


                GetText(TextsType, (int)Texts.AchievementCountText).gameObject.SetActive(true);

                int characterLevel = Managers.GameM.gameData.Character_Holder[data.Name].Level;
                if (characterLevel >= _level)
                {
                    GetObject(GameObjectsType, (int)GameObjects.BlackObject).SetActive(false);
                    GetText(TextsType, (int)Texts.AchievementCountText).color = Color.green;
                }
                else
                {
                    GetObject(GameObjectsType, (int)GameObjects.BlackObject).SetActive(true);
                    GetText(TextsType, (int)Texts.AchievementCountText).color = Color.red;
                }
                GetText(TextsType, (int)Texts.AchievementCountText).text = $"({Managers.GameM.gameData.Character_Holder[data.Name].Level} / {_level})";


            }
        }
        else
        {
            if (Managers.DataM.ItemDataDic.TryGetValue(_dataID, out var data))
            {
                GetImage(ImagesType, (int)Images.AchievementImage).sprite = Managers.ResourceM.GetAtlas(data.Name);
                GetText(TextsType, (int)Texts.AchievementCountText).gameObject.SetActive(false);

                var itemData = Managers.GameM.gameData.Item_Data[data.Name];


                if (itemData.holder.Count > 0)
                {
                    GetObject(GameObjectsType, (int)GameObjects.BlackObject).SetActive(false);
                }
                else
                    GetObject(GameObjectsType, (int)GameObjects.BlackObject).SetActive(true);
            }
        }
    }
}
