using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class UI_MissionItem : UI_Base
{
    #region enum
    enum Texts
    {
        RewardItemCountText,
        MissionNameText,
        MissionDescriptionText,
        MissionTargetValueText,

    }

    enum GameObjects
    {
        CollectObject,
    }

    enum Buttons
    {
        AcceptButton,
    }

    enum Images
    {
        MissionTargetValueImage,
        AcceptBlockImage,

    }

    enum MissionState
    {
        Progress,
        Complete,
        Rewarded
    }
    #endregion

    Data.MissionData data;
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        TextsType = typeof(Texts);
        GameObjectsType = typeof(GameObjects);
        ButtonsType = typeof(Buttons);
        ImagesType = typeof(Images);

        BindText(TextsType);
        BindObject(GameObjectsType);
        BindButton(ButtonsType);
        BindImage(ImagesType);

        GetButton(ButtonsType, (int)Buttons.AcceptButton).gameObject.BindEvent(OnClickAcceptButton);

        return true;
    }

    public void SetInfo(Data.MissionData _data)
    {
        data = _data;

        GetObject(GameObjectsType, (int)GameObjects.CollectObject).SetActive(false);
        GetImage(ImagesType, (int)Images.AcceptBlockImage).gameObject.SetActive(true);
        Refresh();




    }

    void Refresh()
    {
        if (data == null) return;
        GetText(TextsType, (int)Texts.RewardItemCountText).text = $"x{data.RewardCount}";
        GetText(TextsType, (int)Texts.MissionNameText).text = data.MissionName;
        GetText(TextsType, (int)Texts.MissionDescriptionText).text = data.Description;
        GetImage(ImagesType, (int)Images.MissionTargetValueImage).fillAmount = 0;
        GetText(TextsType, (int)Texts.MissionTargetValueText).text = $"({0} / {data.MissionTargetValue})";

        if (Managers.GameM.MissionDic.TryGetValue(data.MissionTarget, out MissionInfo missionInfo))
        {
            if (missionInfo.Progress > 0)
            {
                GetImage(ImagesType, (int)Images.MissionTargetValueImage).fillAmount = (float)(missionInfo.Progress / data.MissionTargetValue);
                GetText(TextsType, (int)Texts.MissionTargetValueText).text = $"({missionInfo.Progress} / {data.MissionTargetValue})";
            }

            if (missionInfo.Progress >= data.MissionTargetValue)
            {
                
                SetMission(MissionState.Complete);

                if (missionInfo.isRewarded)
                    SetMission(MissionState.Rewarded);
            }
            else
                SetMission(MissionState.Progress);

        }

    }


    void SetMission(MissionState _state)
    {

        switch (_state)
        {
            case MissionState.Progress:

                break;
            case MissionState.Complete:
                GetImage(ImagesType, (int)Images.AcceptBlockImage).gameObject.SetActive(false);
                break;
            case MissionState.Rewarded:
                GetObject(GameObjectsType, (int)GameObjects.CollectObject).SetActive(true);

                break;
        }
    }

    async void OnClickAcceptButton()
    {
        Managers.SoundM.PlayButtonClick();

        int count = data.RewardCount;
        UI_Reward reward = await Managers.UIM.ShowPopup<UI_Reward>();
        reward.SetInfo(count);

        if (Managers.GameM.MissionDic.TryGetValue(data.MissionTarget, out MissionInfo info))
        {
            info.isRewarded = true;
        }

        Refresh();
    }
}
