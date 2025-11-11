using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class UI_Toast : UI_Base
{
    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        return true;
    }


    public void SetInfo(string _detail)
    {

        DestoryToast().Forget();
    }

    async UniTask DestoryToast()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1f), ignoreTimeScale: false);

        Managers.UIM.CloseToast(this);
    }
}
