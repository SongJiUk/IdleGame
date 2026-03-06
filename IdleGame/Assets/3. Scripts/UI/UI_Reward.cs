using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Reward : UI_Base
{
    #region enum
    enum Texts
    {
        RewardCountText
    }
    enum Images
    {
        RewardImage
    }

    enum Buttons
    {
        CloseButton
    }

    #endregion


    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        TextsType = typeof(Texts);
        ImagesType = typeof(Images);
        ButtonsType = typeof(Buttons);

        BindText(TextsType);
        BindImage(ImagesType);
        BindButton(ButtonsType);

        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        return true;
    }

    public void SetInfo(int _count)
    {   //Sound : reward Sound
        GetRewardInit("Dia", _count);
    }

    public void GetIAPReward(Define.IAP _iap)
    {
        switch (_iap)
        {
            case Define.IAP.removeads:
                GetRewardInit("ADS", 0);
                break;
            case Define.IAP.dia300:
                GetRewardInit("Dia", 300);
                break;
        }
    }

    public void GetRewardInit(string _itemName, int _count)
    {
        GetImage(ImagesType, (int)Images.RewardImage).sprite = Managers.ResourceM.GetAtlas(_itemName);
        GetText(TextsType, (int)Texts.RewardCountText).text = _count <= 1 ? "" : $"x{_count}";

        switch (_itemName)
        {
            case "Dia": Managers.GameM.Dia += _count; break;
            case "ADS": Managers.GameM.gameData.ADS_Remove = true; break;
        }

    }

    void OnClickCloseButton()
    {
        Managers.SoundM.PlayButtonClick();

        Managers.IAPM.NotifyPurchaseSucces();
        Managers.UIM.ClosePopup(this).Forget();
    }
}
