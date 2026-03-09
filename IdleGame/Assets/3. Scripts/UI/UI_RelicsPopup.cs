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
        RelicButton12,
        RelicButton10,
        RelicButton2,
        RelicButton5,
        RelicButton7,
        CenterRelicButton,
        CloseButton,
        GachaButton,
        EnforceButton,
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

    enum Texts
    {
        Relic12NameText,
        Relic10NameText,
        Relic2NameText,
        Relic5NameText,
        Relic7NameText,
        RelicCenterNameText,

        Relic12LevelText,
        Relic10LevelText,
        Relic2LevelText,
        Relic5LevelText,
        Relic7LevelText,
        RelicCenterLevelText,
    }
    #endregion
    public System.Action OnValueChange;
    Transform parent = null;
    UI_RelicIcon clickRelic;

    List<ItemHolder> relicList = new List<ItemHolder>();
    public List<UI_RelicIcon> relics = new List<UI_RelicIcon>();
    Dictionary<Buttons, (Images bg, Images icon, Texts name, Texts level)> relicMaps;
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        GameObjectsType = typeof(GameObjects);
        ButtonsType = typeof(Buttons);
        ImagesType = typeof(Images);
        TextsType = typeof(Texts);

        BindObject(GameObjectsType);
        BindButton(ButtonsType);
        BindImage(ImagesType);
        BindText(TextsType);

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
            GetObject(GameObjectsType, (int)GameObjects.RelicLockObject12 + i).gameObject.SetActive(false);
            GetText(TextsType, (int)Texts.Relic12NameText + i).text = "";
            GetText(TextsType, (int)Texts.Relic12LevelText + i).text = "";
        }


        relicMaps = new Dictionary<Buttons, (Images bg, Images icon, Texts name, Texts level)>
        {
        { Buttons.RelicButton12, (Images.Relic12BGImage, Images.RelicIconImage12, Texts.Relic12NameText, Texts.Relic12LevelText) },
        { Buttons.RelicButton10, (Images.Relic10BGImage, Images.RelicIconImage10, Texts.Relic10NameText, Texts.Relic10LevelText) },
        { Buttons.RelicButton2,  (Images.Relic2BGImage,  Images.RelicIconImage2, Texts.Relic2NameText, Texts.Relic2LevelText) },
        { Buttons.RelicButton5,  (Images.Relic5BGImage,  Images.RelicIconImage5, Texts.Relic5NameText, Texts.Relic5LevelText) },
        { Buttons.RelicButton7,  (Images.Relic7BGImage,  Images.RelicIconImage7, Texts.Relic7NameText, Texts.Relic7LevelText) },
        { Buttons.CenterRelicButton, (Images.CenterRelicBGImage, Images.CenterRelicIconImage,Texts.RelicCenterNameText, Texts.RelicCenterLevelText ) }
        };
        //GetButton(ButtonsType, (int)Buttons.CenterRelicButton).image.color = Utils.StringToColorGrade()
        return true;
    }

    public override void SetInfo()
    {

        Managers.SoundM.MuteEffectVolume(true);
        RefreshUI();
        InitEquippedRelicsUI();
        InitRelicIcon();
    }

    void RefreshUI()
    {
        GetObject(GameObjectsType, (int)GameObjects.RelicLockObject12).gameObject.SetActive(false);
    }

    void InitRelicIcon()
    {
        var datas = Managers.GameM.gameData.Item_Data;

        if (relics.Count != 0) ClearIcons();

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
    }
    void InitEquippedRelicsUI()
    {
        var slots = Managers.GameM.gameData.EquippedRelics;

        for (int i = 0; i < slots.Length; i++)
        {
            var data = slots[i];
            Buttons btn = (Buttons)i;

            relicMaps.TryGetValue(btn, out var map);
            if (data != null)
            {
                Managers.ItemM.SetItem((int)btn, data.Name);

                GetImage(ImagesType, (int)map.bg).color = Utils.HexToColor(Utils.StringToColorGradeImage(data.ItemGrade));
                GetImage(ImagesType, (int)map.icon).gameObject.SetActive(true);
                GetImage(ImagesType, (int)map.icon).sprite = Managers.ResourceM.GetAtlas(data.Name);
                GetImage(ImagesType, (int)map.icon).SetNativeSize();

                GetText(TextsType, (int)map.name).text = data.NameKR;
                Managers.GameM.gameData.Item_Data.TryGetValue(data.Name, out var itemHolder);
                if (itemHolder != null) GetText(TextsType, (int)map.level).text = "Lv. " + itemHolder.holder.Level.ToString();
            }
        }
    }


    void OnClickRelicButton(Buttons _button)
    {
        if (clickRelic == null) return;

        Managers.SoundM.PlayButtonClick();
        InitRelic(_button);

    }


    void InitRelic(Buttons _button)
    {
        relicMaps.TryGetValue(_button, out var map);

        Managers.ItemM.SetItem((int)_button, clickRelic.DATA.Name);

        GetImage(ImagesType, (int)map.bg).color = Utils.HexToColor(Utils.StringToColorGradeImage(clickRelic.DATA.ItemGrade));
        GetImage(ImagesType, (int)map.icon).gameObject.SetActive(true);
        GetImage(ImagesType, (int)map.icon).sprite = Managers.ResourceM.GetAtlas(clickRelic.DATA.Name);
        GetImage(ImagesType, (int)map.icon).SetNativeSize();


        GetText(TextsType, (int)map.name).text = clickRelic.DATA.NameKR;
        Managers.GameM.gameData.Item_Data.TryGetValue(clickRelic.DATA.Name, out var itemHolder);
        if (itemHolder != null) GetText(TextsType, (int)map.level).text = "Lv. " + itemHolder.holder.Level.ToString();



        SetClickIcon(null);
        DelegateHolder.Clear();
        Managers.RelicM.Init();

        OnValueChange?.Invoke();
        clickRelic = null;
    }

    void RemoveRelic()
    {
        for (int i = 0; i < Managers.GameM.gameData.EquippedRelics.Length; i++)
        {
            if (Managers.GameM.gameData.EquippedRelics[i] == null)
            {
                Buttons btn = (Buttons)i;
                GetImage(ImagesType, (int)relicMaps[btn].bg).color = Utils.HexToColor("#FFFFFF");
                GetImage(ImagesType, (int)relicMaps[btn].icon).sprite = null;
                GetImage(ImagesType, (int)relicMaps[btn].icon).gameObject.SetActive(false);
                GetText(TextsType, (int)relicMaps[btn].name).text = "";
                GetText(TextsType, (int)relicMaps[btn].level).text = "";
            }
        }
    }

    async void OnClickCloseButton()
    {
        Managers.SoundM.PlayButtonClick();
        await TriggerClose(this, false);

        ClearIcons();
    }

    void ClearIcons()
    {
        for (int i = 0; i < relics.Count; i++)
        {
            Managers.ResourceM.Destroy(relics[i].gameObject);
        }
        relics.Clear();
    }


    async void OnClickGachaButton()
    {
        Managers.SoundM.PlayButtonClick();
        //if (!Managers.UIM.ClickLock(0.5f)) return;
        var gameScene = Managers.UIM.SceneUI as UI_GameScene;
        if (gameScene != null)
        {
            //await Managers.UIM.ClosePopup(this);
            gameScene.OpenShopFormOtherUI().Forget();
        }
    }


    async void OnClickEnforceButton()
    {
        Managers.SoundM.PlayButtonClick();
        if (CheckUpgradeRelic())
        {
            var popup = await Managers.UIM.ShowPopup<UI_UpgradePopup>();
            popup.SetInfo(relicList);
        }
        else
        {
            Managers.UIM.ShowToast("강화가 가능한 유물이 없습니다.");
        }
    }

    bool CheckUpgradeRelic()
    {
        relicList.Clear();
        foreach (var relic in Managers.GameM.gameData.Item_Data)
        {
            var data = relic.Value;
            if (data.data.ItemType == Define.ItemType.Equipment)
            {
                if (data.holder.Level >= data.data.MaxLevel) continue;
                if (data.holder.Level == 0) continue;

                int needCount = data.holder.Level * 5;
                if (needCount < data.holder.Count)
                {
                    data.holder.Count -= needCount;
                    data.holder.Level++;
                    OnValueChange?.Invoke();
                    relicList.Add(data);
                }
            }
        }
        if (relicList.Count == 0) return false;
        else return true;
    }


    public void SetClickIcon(UI_RelicIcon _clickRelic, bool _isMinusClick = false)
    {
        //TODO : 이거 체크하기
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

            for (int i = 0; i < Managers.GameM.gameData.EquippedRelics.Length; i++)
            {
                var data = Managers.GameM.gameData.EquippedRelics[i];
                if (data != null)
                {
                    if (data == clickRelic.DATA)
                    {
                        Managers.ItemM.DisableItem(i);
                        SetClickIcon(null);
                    }
                }
            }

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
        Managers.ItemM.RemoveItem(clickRelic.name);
        SetClickIcon(null);

        OnValueChange?.Invoke();
        RemoveRelic();
        clickRelic = null;
    }

}
