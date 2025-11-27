using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    public override void Init()
    {
        base.Init();
        Managers.UIM.ShowScene<UI_GameScene>();

        //TODO : ���⼭ ������ �ٷ� �ؾ��ϳ�?
    }
}
