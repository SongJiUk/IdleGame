using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UI_CharacterIcon : UI_Base
{
    enum Images
    {
        CharacterBGImge,
        CharacterImage,
        CountFillImage,

    }
    enum Texts
    {
        CharacterCountText,
        CharacterLevelText
    }

    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        ImagesType = typeof(Images);
        TextsType = typeof(Texts);

        BindImage(ImagesType);
        BindText(TextsType);

        return true;
    }

    public void SetInfo(Data.CreatureData _data)
    {
        GetImage(ImagesType, (int)Images.CharacterBGImge).sprite = Managers.ResourceM.GetAtlas(_data.CharacterGrade.ToString());
        GetImage(ImagesType, (int)Images.CharacterImage).sprite = Managers.ResourceM.GetAtlas(_data.prefabName);
        //GetImage(ImagesType, (int)Images.CountFillImage).fillAmount = Utils.GetAtlas("Common");
    }
}
