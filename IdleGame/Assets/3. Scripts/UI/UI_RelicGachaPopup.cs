using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RelicGachaPopup : UI_Popup
{
    #region Enum
    enum GameObjects
    {
        Horizontal_1,
        Horizontal_2,
        Horizontal_3,
        DiaImage,
        AdImage
    }

    enum Buttons
    {
        CloseButton,
        OneMoreButton
    }

    enum Texts
    {
        OneMoreButtonText,
        OneMoreButtonPriceText,
    }

    #endregion

    const int horizontal_limit = 4;
    const int horizontalCount = 3;
    Transform[] horizontals = new Transform[horizontalCount];
    bool isUsingButton = false;
    bool isAd = false;
    int gachaCount = 0;
    List<UI_RelicGachaIcon> iconList = new List<UI_RelicGachaIcon>();
    public Action OnGachaFinished;
    public async override UniTask<bool> Init()
    {
        if (!await base.Init()) return false;

        GameObjectsType = typeof(GameObjects);
        ButtonsType = typeof(Buttons);
        TextsType = typeof(Texts);

        BindObject(GameObjectsType);
        BindButton(ButtonsType);
        BindText(TextsType);


        GetButton(ButtonsType, (int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton(ButtonsType, (int)Buttons.OneMoreButton).gameObject.BindEvent(OnClickOneMoreButton);

        for (int i = 0; i < horizontalCount; i++)
        {
            horizontals[i] = GetObject(GameObjectsType, (int)GameObjects.Horizontal_1 + i).transform;
        }
        return true;
    }

    public async UniTask GetGachaRelic(int _count, bool _isAd = false)
    {
        isAd = _isAd;
        await GachaRelics(_count);
    }

    async UniTask GachaRelics(int _count)
    {
        isUsingButton = true;
        try
        {
            int horizontalCount = 0;
            gachaCount = _count;

            if (!isAd)
            {
                int cost = _count == 11 ? 3000 : 300;
                if (Managers.GameM.Dia < cost)
                {
                    Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastNoDia"));
                    return;
                }
                Managers.GameM.Dia -= cost;
            }

            if (isAd)
            {
                GetText(TextsType, (int)Texts.OneMoreButtonText).text = Managers.LocalizationM.Get("UIShop_1Summon");
                GetText(TextsType, (int)Texts.OneMoreButtonPriceText).text = $"({Managers.GameM.gameData.relicFreeGachaCount}/3)";
                GetObject(GameObjectsType, (int)GameObjects.DiaImage).SetActive(false);
                GetObject(GameObjectsType, (int)GameObjects.AdImage).SetActive(true);
            }
            else
            {
                GetObject(GameObjectsType, (int)GameObjects.DiaImage).SetActive(true);
                GetObject(GameObjectsType, (int)GameObjects.AdImage).SetActive(false);
                switch (gachaCount)
                {
                    case 11:
                        GetText(TextsType, (int)Texts.OneMoreButtonText).text = Managers.LocalizationM.Get("UIShop_11Summon");
                        GetText(TextsType, (int)Texts.OneMoreButtonPriceText).text = "3000";
                        break;
                    case 1:
                        GetText(TextsType, (int)Texts.OneMoreButtonText).text = Managers.LocalizationM.Get("UIShop_1Summon");
                        GetText(TextsType, (int)Texts.OneMoreButtonPriceText).text = "300";
                        break;
                }
            }

            for (int i = 0; i < gachaCount; i++)
            {
                float percentage = UnityEngine.Random.Range(0.0f, 100.0f);
                float r_percentage = 0.0f;

                Managers.GameM.Relics_Summon_Count++;
                Managers.GameM.Relics_Confirmed_Legendary_Count++;

                Define.Grade grade = Define.Grade.Common;

                if (i % horizontal_limit == 0 && i != 0) horizontalCount++;

                var relicInfo = Managers.UIM.MakeSubItem<UI_RelicGachaIcon>(horizontals[horizontalCount]);

                if (Managers.GameM.Relics_Confirmed_Legendary_Count >= Managers.DataM.GachaDataDic[Utils.GachaMaxLevel].SummonCount)
                {
                    Managers.GameM.Relics_Confirmed_Legendary_Count = 0;
                    grade = Define.Grade.Legendary;
                }

                if (grade != Define.Grade.Legendary)
                {
                    for (int j = 0; j < Utils.GradeCount; j++)
                    {
                        r_percentage += Utils.Gacha_Percentage(Define.GachaType.RelicGacha)[j];
                        if (percentage <= r_percentage)
                        {
                            grade = (Define.Grade)j;
                            break;
                        }
                    }
                }

                Data.ItemData data = Managers.GameM.gameData.GetGradeRelic(grade);
                if (Managers.GameM.gameData.Item_Data.TryGetValue(data.Name, out var itemData)) itemData.holder.Count++;

                relicInfo.Init().Forget();
                relicInfo.SetRelicIcon(data);

                iconList.Add(relicInfo);

                Managers.GameM.gameData.ChangeRelicInfo(data);

                await UniTask.Delay(120);
            }

            await Managers.FirebaseM.WriteData();

            OnGachaFinished?.Invoke();

        }
        catch (Exception e) { }
        finally
        {
            isUsingButton = false;
        }


    }


    void ResetIcon()
    {
        for (int i = iconList.Count - 1; i >= 0; i--)
        {
            Managers.ResourceM.Destroy(iconList[i].gameObject);
        }
        iconList.Clear();
    }

    void OnClickCloseButton()
    {
        Managers.SoundM.PlayButtonClick();
        if (isUsingButton) return;

        ResetIcon();
        Managers.UIM.ClosePopup(this).Forget();
    }

    void OnClickOneMoreButton()
    {
        Managers.SoundM.PlayButtonClick();
        if (isUsingButton) return;

        if (isAd)
        {
            if (Managers.GameM.gameData.relicFreeGachaCount <= 0)
            {
                Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIShop_AdSummonEnd"));
                return;
            }
            Action rewardedAction = async () =>
            {
                Managers.GameM.gameData.relicFreeGachaCount--;
                ResetIcon();
                await GetGachaRelic(1, true);
                OnGachaFinished?.Invoke();
            };
            Managers.AdM.ShowRewardedAd(rewardedAction, null);
        }
        else
        {
            ResetIcon();
            GetGachaRelic(gachaCount).Forget();
        }
    }
}
