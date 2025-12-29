using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_RelicsPopup : UI_Popup
{
    #region Enum
    enum GameObjects
    {
        Content,
        RelicLockObject12,
        RelicLockObject10,
        RelicLockObject2,
        RelicLockObject5,
        RelicLockObject7,
        CenterRelicLockObject
    }

    enum Buttons
    {
        CloseButton,
        GachaButton,
        EnforceButton,
        RelicButton12,
        RelicButton10,
        RelicButton2,
        RelicButton5,
        RelicButton7,
        CenterRelicButton,
    }
    enum Images
    {
        RelicIconImage12,
        RelicIconImage10,
        RelicIconImage2,
        RelicIconImage5,
        RelicIconImage7,
        CenterRelicIconImage,
        Relic12BGImage,
        Relic10BGImage,
        Relic2BGImage,
        Relic5BGImage,
        Relic7BGImage,
        CenterRelicBGImage

    }
    #endregion
    public System.Action OnValueChange;
    Transform parent = null;
    UI_RelicIcon clickRelic;
    bool isRemoveRelic = false;

    public List<UI_RelicIcon> relics = new List<UI_RelicIcon>();
    Dictionary<Buttons, (Images bg, Images icon)> relicMaps;
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        GameObjectsType = typeof(GameObjects);
        ButtonsType = typeof(Buttons);
        ImagesType = typeof(Images);

        BindObject(GameObjectsType);
        BindButton(ButtonsType);
        BindImage(ImagesType);

        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton(ButtonsType, (int)Buttons.GachaButton).gameObject.BindEvent(OnClickGachaButton);
        GetButton(ButtonsType, (int)Buttons.EnforceButton).gameObject.BindEvent(OnClickEnforceButton);

        parent = GetObject(GameObjectsType, (int)GameObjects.Content).transform;


        foreach (Buttons buttonType in Enum.GetValues(typeof(Buttons)))
        {
            GetButton(ButtonsType, (int)buttonType).gameObject.BindEvent(() => OnClickRelicButton(buttonType));
        }

        for (int i = 0; i < 6; i++)
        {
            GetObject(GameObjectsType, (int)GameObjects.RelicLockObject12 + i).gameObject.SetActive(true);
        }


        relicMaps = new Dictionary<Buttons, (Images bg, Images icon)>
        {
        { Buttons.RelicButton12, (Images.Relic12BGImage, Images.RelicIconImage12) },
        { Buttons.RelicButton10, (Images.Relic10BGImage, Images.RelicIconImage10) },
        { Buttons.RelicButton2,  (Images.Relic2BGImage,  Images.RelicIconImage2) },
        { Buttons.RelicButton5,  (Images.Relic5BGImage,  Images.RelicIconImage5) },
        { Buttons.RelicButton7,  (Images.Relic7BGImage,  Images.RelicIconImage7) },
        { Buttons.CenterRelicButton, (Images.CenterRelicBGImage, Images.CenterRelicIconImage) }
        };
        //GetButton(ButtonsType, (int)Buttons.CenterRelicButton).image.color = Utils.StringToColorGrade()
        return true;
    }

    public override void SetInfo()
    {
        var datas = Managers.GameM.gameData.Item_Data;

        foreach (var data in datas)
        {
            if (data.Value.data.ItemType == Define.ItemType.Equipment)
            {
                if (data.Value.holder.Count == 0) continue;

                var go = Managers.UIM.MakeSubItem<UI_RelicIcon>();
                go.transform.SetParent(parent, false);
                go.transform.localScale = Vector3.one;

                go.Init().Forget();
                go.SetInfo(data.Value.data, this);
                relics.Add(go);
            }
        }

        RefreshUI();
    }

    void RefreshUI()
    {
        GetObject(GameObjectsType, (int)GameObjects.RelicLockObject12).gameObject.SetActive(false);
    }

    void OnClickRelicButton(Buttons _button)
    {
        if (clickRelic == null) return;

        if (relicMaps.TryGetValue(_button, out var map))
        {
            GetImage(ImagesType, (int)map.bg).color = Utils.HexToColor(Utils.StringToColorGradeImage(clickRelic.DATA.ItemGrade));
            GetImage(ImagesType, (int)map.icon).sprite = Managers.ResourceM.GetAtlas(clickRelic.DATA.Name);

            SetClick(null);
            OnValueChange?.Invoke();
            clickRelic = null;
        }
    }


    void OnClickCloseButton()
    {
        TriggerClose(this);

        for (int i = 0; i < relics.Count; i++)
        {
            Managers.ResourceM.Destroy(relics[i].gameObject);
        }
        relics.Clear();
    }

    void OnClickGachaButton()
    {

    }


    void OnClickEnforceButton()
    {

    }

    public void SetClick(UI_RelicIcon _clickRelic, bool _isMinusClick = false)
    {
        if (_clickRelic == null)
        {
            for (int i = 0; i < relics.Count; i++)
            {
                relics[i].SetLockImage(false);
                relics[i].GetComponent<Outline>().enabled = false;
            }
        }
        else
        {
            clickRelic = _clickRelic;
            if (_isMinusClick)
            {
                OnlyRemoveRelic();
            }
            else
            {
                for (int i = 0; i < relics.Count; i++)
                {
                    relics[i].SetLockImage(true);
                    relics[i].GetComponent<Outline>().enabled = false;
                }

                clickRelic.SetLockImage(false);
                clickRelic.GetComponent<Outline>().enabled = true;
            }

        }

    }

    void OnlyRemoveRelic()
    {
        SetClick(null);
        isRemoveRelic = true;

        OnValueChange?.Invoke();
        clickRelic = null;
    }

}
