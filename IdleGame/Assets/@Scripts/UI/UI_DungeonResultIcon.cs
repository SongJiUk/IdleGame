using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DungeonResultIcon : UI_Base
{
    #region Enum
    enum Images
    {
        ResultImage,
    }

    enum Texts
    {
        ResultCountText
    }

    #endregion

    Transform parent = null;
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;


        ImagesType = typeof(Images);
        TextsType = typeof(Texts);

        BindImage(ImagesType);
        BindText(TextsType);



        return true;
    }

    public void SetInfo(int _dungeonDataID)
    {
        int level = 0;

        int value = 0;
        if (_dungeonDataID == 70000) value = 0;
        else value = 1;
        Managers.DataM.DungeonDataDic.TryGetValue(_dungeonDataID, out var data);

        if (data != null)
        {
            Managers.DataM.ItemDataDic.TryGetValue(data.ResultDataID, out var item);

            GetImage(ImagesType, (int)Images.ResultImage).sprite = Managers.ResourceM.GetAtlas(item.Name);
            level = Managers.GameM.gameData.DungeonClearLevel[value];

            if (_dungeonDataID == 70000)
            {

                double Dia = level * 50;
                GetText(TextsType, (int)Texts.ResultCountText).text = $"x {Dia}";
            }
            else
            {
                double money = Utils.Money() * level * 10;
                GetText(TextsType, (int)Texts.ResultCountText).text = $"x {Utils.ToCurrencyString(money)}";
            }
        }
    }


}
