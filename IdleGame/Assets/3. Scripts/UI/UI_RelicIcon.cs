using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RelicIcon : UI_Base
{
    #region Enum
    enum GameObjects
    {
        RelicLockObject,
        RelicUseObject
    }
    enum Buttons
    {
        UI_RelicIcon,
        RelicUseButton,

    }
    enum Images
    {
        UI_RelicIcon,
        RelicImage,
        CountFillImage,
        PlusImage,
        MinusImage

    }
    enum Texts
    {
        RelicCountText,
        RelicLevelText
    }
    #endregion

    Data.ItemData data;
    public Data.ItemData DATA
    {
        get { return data; }
    }
    UI_RelicsPopup parent;
    bool isUseRelic = false;
    public override async UniTask<bool> Init()
    {
        if (!await base.Init()) return false;
        GameObjectsType = typeof(GameObjects);
        ImagesType = typeof(Images);
        TextsType = typeof(Texts);
        ButtonsType = typeof(Buttons);

        BindObject(GameObjectsType);
        BindImage(ImagesType);
        BindText(TextsType);
        BindButton(ButtonsType);

        //TODO: 캐릭터 정보창
        GetButton(ButtonsType, (int)Buttons.UI_RelicIcon).gameObject.BindEvent(OnClickRelicInfo);
        GetButton(ButtonsType, (int)Buttons.RelicUseButton).gameObject.BindEvent(OnClickRelic);


        return true;
    }

    public void SetInfo(Data.ItemData _data, UI_RelicsPopup _parent)
    {
        data = _data;
        parent = _parent;
        parent.OnValueChange -= RefreshUI;
        parent.OnValueChange += RefreshUI;

        GetImage(ImagesType, (int)Images.UI_RelicIcon).sprite = Managers.ResourceM.GetAtlas(_data.ItemGrade.ToString());
        GetImage(ImagesType, (int)Images.RelicImage).sprite = Managers.ResourceM.GetAtlas(_data.Name);
        GetImage(ImagesType, (int)Images.RelicImage).SetNativeSize();

        GetImage(ImagesType, (int)Images.PlusImage).gameObject.SetActive(false);
        GetImage(ImagesType, (int)Images.MinusImage).gameObject.SetActive(false);

        GetObject(GameObjectsType, (int)GameObjects.RelicUseObject).SetActive(false);


        RefreshUI();
    }
    public void RefreshUI()
    {
        int levelCount = Managers.GameM.gameData.Item_Holder[data.Name].Level * 5;
        GetImage(ImagesType, (int)Images.CountFillImage).fillAmount = (float)Managers.GameM.gameData.Item_Holder[data.Name].Count / (float)levelCount;
        GetText(TextsType, (int)Texts.RelicCountText).text = Managers.GameM.gameData.Item_Holder[data.Name].Count.ToString() + " / " + levelCount.ToString();
        GetText(TextsType, (int)Texts.RelicLevelText).text = "Lv. " + Managers.GameM.gameData.Item_Holder[data.Name].Level.ToString();

        this.GetComponent<Outline>().enabled = false;
        GetObject(GameObjectsType, (int)GameObjects.RelicLockObject).SetActive(false);

        CheckUseRelic();
    }

    public void CheckUseRelic()
    {
        isUseRelic = false;

        for (int i = 0; i < Managers.GameM.gameData.relics.Length; i++)
        {
            if (Managers.GameM.gameData.relics[i] == null) continue;

            if (Managers.GameM.gameData.relics[i] == data)
            {
                isUseRelic = true;
            }
        }

        GetImage(ImagesType, (int)Images.MinusImage).gameObject.SetActive(isUseRelic);
        GetImage(ImagesType, (int)Images.PlusImage).gameObject.SetActive(!isUseRelic);

        GetObject(GameObjectsType, (int)GameObjects.RelicUseObject).SetActive(isUseRelic);
    }


    public void SetLockImage(bool _isLock)
    {
        GetObject(GameObjectsType, (int)GameObjects.RelicLockObject).SetActive(_isLock);
    }


    async void OnClickRelicInfo()
    {
        var popup = await Managers.UIM.ShowPopup<UI_RelicInfoPopup>();
        popup.SetInfo(data);
        popup.OnChangeRelicInfo -= CheckRelicInfo;
        popup.OnChangeRelicInfo += CheckRelicInfo;
    }

    public void CheckRelicInfo()
    {
        RefreshUI();
    }

    void OnClickRelic()
    {
        if (isUseRelic)
        {
            parent.SetClickIcon(this, true);
        }
        else
        {
            parent.SetClickIcon(this);
        }
    }
}
