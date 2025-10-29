using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Item : UI_Base
{

    enum Images
    {
        UI_ItemBGImage,
        ItemIconImage,


    }
    enum Buttons
    {

    }


    enum Texts
    {
        ItemCountText
    }

    public override bool Init()
    {
        if (!base.Init()) return false;

        ImagesType = typeof(Images);
        TextsType = typeof(Texts);

        BindImage(ImagesType);
        BindText(TextsType);

        return true;
    }

}
