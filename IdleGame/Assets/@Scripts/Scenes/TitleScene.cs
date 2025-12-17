using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{

    public override void Init() 
    {
        base.Init();
        Managers.UIM.ShowScene<UI_TitleScene>();

    }
}
