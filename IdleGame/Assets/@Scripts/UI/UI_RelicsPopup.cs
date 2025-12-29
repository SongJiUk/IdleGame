using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RelicsPopup : UI_Popup
{
    #region Enum
    #endregion


    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        return true;
    }
}
