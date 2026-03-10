using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AccountConflictPopup : UI_Popup
{
    #region enum
    enum Buttons
    {
        ConfirmButton,
        CancelButton
    }

    #endregion
    Action OnConfirm;
    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        ButtonsType = typeof(Buttons);

        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.ConfirmButton).gameObject.BindEvent(OnClickConfirm);
        GetButton(ButtonsType, (int)Buttons.CancelButton).gameObject.BindEvent(OnClickCancel);

        return true;
    }

    public void SetCallBack(Action _confirmAction)
    {
        OnConfirm = _confirmAction;
    }


    void OnClickConfirm()
    {
        Managers.SoundM.PlayButtonClick();
        OnConfirm?.Invoke();
        Managers.UIM.ClosePopup(this).Forget();
    }

    void OnClickCancel()
    {
        //TODO : 이거 해줬을때 다시 눌러도 안됌(이유 확인해보기)
        Managers.SoundM.PlayButtonClick();
        Managers.UIM.ClosePopup(this).Forget();

    }
}
