using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RecallHeroIcon : UI_Base
{
    enum Images
    {
        HeroBGImage,
        HeroImage,
    }

    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        ImagesType = typeof(Images);

        BindImage(ImagesType);


        return true;
    }


    public void SetHeroIcon(Data.CreatureData _data)
    {
        GetImage(ImagesType, (int)Images.HeroBGImage).sprite = Managers.ResourceM.GetAtlas(_data.CharacterGrade.ToString());
        GetImage(ImagesType, (int)Images.HeroImage).sprite = Managers.ResourceM.GetAtlas(_data.Name);

    }
}
