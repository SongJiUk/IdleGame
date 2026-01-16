using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI
;

public class UI_GachaPopup : UI_Popup
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

    int value = 0;
    List<UI_GachaHeroIcon> iconList = new List<UI_GachaHeroIcon>();
    bool isUsingButton = false;
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

    public async UniTask GetGachaHero(int _count)
    {
        await GachaHeroes(_count);
    }


    async UniTask GachaHeroes(int _count)
    {
        isUsingButton = true;
        try
        {
            int horizontalCount = 0;
            value = _count;

            for (int i = 0; i < _count; i++)
            {
                float percentage = UnityEngine.Random.Range(0.0f, 100.0f);
                float r_Percentage = 0.0f;

                Managers.GameM.Hero_Summon_Count++;
                Managers.GameM.Hero_Confirmed_Legendary_Count++;

                Define.CharacterGrade grade = Define.CharacterGrade.Common;


                if (i % horizontal_limit == 0 && i != 0)
                {
                    horizontalCount++;
                }

                var heroInfo = Managers.UIM.MakeSubItem<UI_GachaHeroIcon>(horizontals[horizontalCount]);


                if (Managers.GameM.Hero_Confirmed_Legendary_Count >= Managers.DataM.GachaDataDic[Utils.GachaMaxLevel].SummonCount)
                {
                    Managers.GameM.Hero_Confirmed_Legendary_Count = 0;
                    grade = Define.CharacterGrade.Legendary;
                }

                if (grade != Define.CharacterGrade.Legendary)
                {
                    for (int j = 0; j < Utils.GradeCount; j++)
                    {
                        r_Percentage += Utils.Gacha_Percentage(Define.GachaType.HeroGacha)[j];
                        if (percentage <= r_Percentage)
                        {
                            grade = (Define.CharacterGrade)j;
                            break;
                        }
                    }
                }

                Data.CreatureData data = Managers.GameM.gameData.GetGradeCharacter(grade);
                Managers.GameM.gameData.Character_Holder[data.Name].Count++;


                heroInfo.Init().Forget();
                heroInfo.SetHeroIcon(data);

                iconList.Add(heroInfo);
                switch (_count)
                {
                    case 11:
                        Managers.RenderM.renderGacha.GetHerosForEleven(i, data);
                        GetText(TextsType, (int)Texts.OneMoreButtonText).text = "11회 소환";
                        GetText(TextsType, (int)Texts.OneMoreButtonPriceText).text = "3000";
                        break;
                    case 1:
                        Managers.RenderM.renderGacha.GetHero(data);
                        GetText(TextsType, (int)Texts.OneMoreButtonText).text = "1회 소환";
                        GetText(TextsType, (int)Texts.OneMoreButtonPriceText).text = "300";
                        break;
                }

                Managers.GameM.gameData.ChangeCharacterInfo(data);

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
        for (int i = 0; i < iconList.Count; i++)
        {
            Managers.ResourceM.Destroy(iconList[i].gameObject);
        }

        iconList.Clear();
    }
    void OnClickCloseButton()
    {
        if (isUsingButton) return;

        ResetIcon();
        Managers.RenderM.renderGacha.ClearList();
        Managers.UIM.ClosePopup(this).Forget();

    }

    void OnClickOneMoreButton()
    {
        if (isUsingButton) return;

        ResetIcon();
        Managers.RenderM.renderGacha.ClearList();
        GetGachaHero(value).Forget();

    }
}
