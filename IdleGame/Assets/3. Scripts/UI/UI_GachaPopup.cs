using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GachaPopup : UI_Popup
{
    #region Enum
    enum GameObjects
    {
        Horizontal_1,
        Horizontal_2,
        Horizontal_3,
        RawImage,
        DiaImage,
        AdImage,
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
    bool isAd = false;
    List<UI_GachaHeroIcon> iconList = new List<UI_GachaHeroIcon>();
    bool isUsingButton = false;
    public Action OnGachaFinished;
    RawImage rawImage;
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
        rawImage = GetObject(GameObjectsType, (int)GameObjects.RawImage).GetComponent<RawImage>();
        for (int i = 0; i < horizontalCount; i++)
        {
            horizontals[i] = GetObject(GameObjectsType, (int)GameObjects.Horizontal_1 + i).transform;
        }

        return true;
    }


    public override void SetInfo()
    {
        Managers.RenderM.Show(RenderType.Gacha, rawImage);
    }
    public async UniTask GetGachaHero(int _count, bool _isAd = false)
    {
        isAd = _isAd;
        await GachaHeroes(_count);
    }


    async UniTask GachaHeroes(int _count)
    {
        isUsingButton = true;
        try
        {
            int horizontalCount = 0;
            value = _count;

            if (isAd)
            {
                GetText(TextsType, (int)Texts.OneMoreButtonText).text = "광고 소환";
                GetText(TextsType, (int)Texts.OneMoreButtonPriceText).text = $"({Managers.GameM.gameData.heroFreeGachaCount}/3)";
                GetObject(GameObjectsType, (int)GameObjects.DiaImage).SetActive(false);
                GetObject(GameObjectsType, (int)GameObjects.AdImage).SetActive(true);
            }
            else
            {
                GetObject(GameObjectsType, (int)GameObjects.DiaImage).SetActive(true);
                GetObject(GameObjectsType, (int)GameObjects.AdImage).SetActive(false);
                switch (_count)
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
            }

            for (int i = 0; i < _count; i++)
            {
                float percentage = UnityEngine.Random.Range(0.0f, 100.0f);
                float r_Percentage = 0.0f;

                Managers.GameM.Hero_Summon_Count++;
                Managers.GameM.Hero_Confirmed_Legendary_Count++;

                Define.Grade grade = Define.Grade.Common;


                if (i % horizontal_limit == 0 && i != 0)
                {
                    horizontalCount++;
                }

                var heroInfo = Managers.UIM.MakeSubItem<UI_GachaHeroIcon>(horizontals[horizontalCount]);


                if (Managers.GameM.Hero_Confirmed_Legendary_Count >= Managers.DataM.GachaDataDic[Utils.GachaMaxLevel].SummonCount)
                {
                    Managers.GameM.Hero_Confirmed_Legendary_Count = 0;
                    grade = Define.Grade.Legendary;
                }

                if (grade != Define.Grade.Legendary)
                {
                    for (int j = 0; j < Utils.GradeCount; j++)
                    {
                        r_Percentage += Utils.Gacha_Percentage(Define.GachaType.HeroGacha)[j];
                        if (percentage <= r_Percentage)
                        {
                            grade = (Define.Grade)j;
                            break;
                        }
                    }
                }

                Data.CreatureData data = Managers.GameM.gameData.GetGradeCharacter(grade);
                Managers.GameM.gameData.Character_Holder[data.Name].Count++;

                heroInfo.Init().Forget();
                heroInfo.SetHeroIcon(data);

                iconList.Add(heroInfo);

                if (_count == 11)
                    Managers.RenderM.renderGacha.GetHerosForEleven(i, data);
                else
                    Managers.RenderM.renderGacha.GetHero(data);

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
        Managers.SoundM.PlayButtonClick();
        if (isUsingButton) return;

        ResetIcon();
        Managers.RenderM.renderGacha.ClearList();
        Managers.UIM.ClosePopup(this).Forget();

    }

    void OnClickOneMoreButton()
    {
        Managers.SoundM.PlayButtonClick();
        if (isUsingButton) return;

        if (isAd)
        {
            if (Managers.GameM.gameData.heroFreeGachaCount <= 0)
            {
                Managers.UIM.ShowToast("오늘 무료 소환 횟수를 모두 사용했습니다.");
                return;
            }
            Action rewardedAction = async () =>
            {
                Managers.GameM.gameData.heroFreeGachaCount--;
                ResetIcon();
                Managers.RenderM.renderGacha.ClearList();
                await GetGachaHero(1, true);
                OnGachaFinished?.Invoke();
            };
            Managers.AdM.ShowRewardedAd(rewardedAction, null);
        }
        else
        {
            ResetIcon();
            Managers.RenderM.renderGacha.ClearList();
            GetGachaHero(value).Forget();
        }
    }

    private void OnDisable()
    {
        Managers.RenderM.Hide();
    }
}
