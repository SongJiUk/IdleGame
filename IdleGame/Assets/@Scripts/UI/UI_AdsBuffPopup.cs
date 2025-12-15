using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AdsBuffPopup : UI_Popup
{
    #region Enum
    #endregion


    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        return true;
    }
}
