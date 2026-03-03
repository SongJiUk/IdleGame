using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DungeonResultInfoPopup : UI_Popup
{

    #region Enum

    enum GameObjects
    {
        Horizontal,

    }

    enum Texts
    {
        Level_Text,
        ResultText,
        TimerText

    }

    enum Buttons
    {
        CloseButton,
    }

    enum Images
    {
        TimerImage,
        DungeonFailImage
    }
    #endregion  
    Transform parent = null;
    int value = 0;
    int dungeonDataID = 0;
    bool isClickCloseButton = false;
    List<UI_DungeonResultIcon> iconPool = new List<UI_DungeonResultIcon>();
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        GameObjectsType = typeof(GameObjects);
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);
        ImagesType = typeof(Images);


        BindObject(GameObjectsType);
        BindText(TextsType);
        BindButton(ButtonsType);
        BindImage(ImagesType);

        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        parent = GetObject(GameObjectsType, (int)GameObjects.Horizontal).transform;
        return true;
    }

    public void SetInfo(int _dungeonDataID, bool _isClear)
    {

        GetImage(ImagesType, (int)Images.DungeonFailImage).gameObject.SetActive(false);

        dungeonDataID = _dungeonDataID;
        value = 0;
        if (_dungeonDataID == 70000) value = 0;
        else value = 1;


        GetText(TextsType, (int)Texts.Level_Text).text = $"Lv. {Managers.GameM.gameData.DungeonClearLevel[value] + 1}";
        if (_isClear)
        {
            GetText(TextsType, (int)Texts.ResultText).text = "클리어";
            int index = 0;
            UI_DungeonResultIcon icon;

            if (index < iconPool.Count)
            {
                icon = iconPool[index];
                icon.gameObject.SetActive(true);
                icon.SetInfo(dungeonDataID);
            }
            else
            {
                icon = Managers.UIM.MakeSubItem<UI_DungeonResultIcon>(parent);
                icon.transform.SetParent(parent);
                icon.Init().Forget();
                icon.SetInfo(dungeonDataID);
                iconPool.Add(icon);
            }

        }
        else
        {
            GetText(TextsType, (int)Texts.ResultText).text = "실패";
            GetImage(ImagesType, (int)Images.DungeonFailImage).gameObject.SetActive(true);
        }

        UpdateResultInfoPopupClose().Forget();
    }


    public async UniTaskVoid UpdateResultInfoPopupClose()
    {
        float totalTime = 5f;
        float currentTime = totalTime;

        var cts = this.GetCancellationTokenOnDestroy();
        try
        {

            while (currentTime >= 0.0f)
            {
                if (isClickCloseButton) break;
                currentTime -= Time.deltaTime;
                float ratio = currentTime / totalTime;

                int seconds = Mathf.CeilToInt(currentTime);

                GetImage(ImagesType, (int)Images.TimerImage).fillAmount = ratio;
                GetText(TextsType, (int)Texts.TimerText).text = seconds.ToString() + "/s";

                await UniTask.Yield(PlayerLoopTiming.Update, cts);
            }

            if (!isClickCloseButton) OnClickCloseButton();
            isClickCloseButton = false;

        }
        catch (System.Exception e) { Debug.LogError(e.Message); }
    }

    void OnClickCloseButton()
    {
        isClickCloseButton = true;
        Managers.StageM.StateChange(Define.StageState.DungeonOut);
        Managers.UIM.ClosePopup(this).Forget();

    }


}
