using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class UI_Popup : UI_Base
{
    public bool isOpen = false;
    public Action OnThisPopupClosed;

    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        return true;
    }

    //하단 버튼 애니메이션
    protected void TriggerClose(UI_Popup _popup)
    {
        Managers.UIM.ClosePopup(_popup, true).Forget();
        OnThisPopupClosed?.Invoke();
        OnThisPopupClosed = null;
    }

    public Vector2 PivotPoint(Vector2 _pos)
    {
        float xPos = _pos.x > Screen.width / 2 ? 1.0f : 0.0f;
        float yPos = _pos.y > Screen.height / 2 ? 1.0f : 0.0f;

        return new Vector2(xPos, yPos);
    }
}
