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

    public async UniTask GetGachaRelic(int _count)
    {
        await GachaRelics(_count);
    }

    async UniTask GachaRelics(int _count)
    {
        isUsingButton = true;
        try
        {
            int horizontalCount = 0;
            gachaCount = _count;

            for (int i = 0; i < gachaCount; i++)
            {
                float percentage = UnityEngine.Random.Range(0.0f, 100.0f);
                float r_percentage = 0.0f;

                Managers.GameM.Relics_Summon_Count++;
                Managers.GameM.Relics_Confirmed_Legendary_Count++;

                Define.ItemGrade grade = Define.ItemGrade.Common;

                if (i % horizontal_limit == 0 && i != 0) horizontalCount++;

                var relicInfo = Managers.UIM.MakeSubItem<UI_RelicGachaIcon>(horizontals[horizontalCount]);

                if (Managers.GameM.Relics_Confirmed_Legendary_Count >= Managers.DataM.GachaDataDic[Utils.GachaMaxLevel].SummonCount)
                {
                    Managers.GameM.Relics_Confirmed_Legendary_Count = 0;
                    grade = Define.ItemGrade.Legendary;
                }

                if (grade != Define.ItemGrade.Legendary)
                {
                    for (int j = 0; j < Utils.GradeCount; j++)
                    {
                        r_percentage += Utils.Gacha_Percentage(Define.GachaType.RelicGacha)[j];
                        if (percentage <= r_percentage)
                        {
                            grade = (Define.ItemGrade)j;
                            break;
                        }
                    }
                }

                Data.ItemData data = Managers.GameM.gameData.GetGradeRelic(grade);
                Managers.GameM.gameData.Item_Holder[data.Name].Count++;

                relicInfo.Init().Forget();
                relicInfo.SetRelicIcon(data);

                iconList.Add(relicInfo);

                switch (gachaCount)
                {
                    case 11:
                        GetText(TextsType, (int)Texts.OneMoreButtonText).text = "11회 소환";
                        GetText(TextsType, (int)Texts.OneMoreButtonPriceText).text = "3000";
                        break;

                    case 1:
                        GetText(TextsType, (int)Texts.OneMoreButtonText).text = "1회 소환";
                        GetText(TextsType, (int)Texts.OneMoreButtonPriceText).text = "300";
                        break;

                }

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
        if (isUsingButton) return;

        ResetIcon();
        Managers.UIM.ClosePopup(this).Forget();
    }

    void OnClickOneMoreButton()
    {
        if (isUsingButton) return;

        ResetIcon();
        GetGachaRelic(gachaCount).Forget();

    }
}
