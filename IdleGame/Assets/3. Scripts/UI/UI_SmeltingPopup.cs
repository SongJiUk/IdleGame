using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class UI_SmeltingPopup : UI_Popup
{
    #region Enum

    const int SmeltingMaxCount = 5;
    const string UseItemName = "Monster_Energy";
    const int NeedCount = 100;
    enum GameObjects
    {
        Smelting_1_Object,
        Smelting_2_Object,
        Smelting_3_Object,
        Smelting_4_Object,
        Smelting_5_Object,
    }
    enum Texts
    {
        Smelting_1_ContentText,
        Smelting_2_ContentText,
        Smelting_3_ContentText,
        Smelting_4_ContentText,
        Smelting_5_ContentText,
        Smelting_1_ProbabilityText,
        Smelting_2_ProbabilityText,
        Smelting_3_ProbabilityText,
        Smelting_4_ProbabilityText,
        Smelting_5_ProbabilityText,
        NeedItemCountText,
    }

    enum Buttons
    {
        CloseButton,
        SmeltingButton,
    }

    enum Images
    {
        LockImage
    }

    #endregion

    readonly int statusCount = Enum.GetNames(typeof(Define.Status_Holder)).Length;

    Data.SmeltData data;
    public override async UniTask<bool> Init()
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
        GetButton(ButtonsType, (int)Buttons.SmeltingButton).gameObject.BindEvent(OnClickSmeltingButton);
        GetImage(ImagesType, (int)Images.LockImage).gameObject.SetActive(false);


        for (int i = 0; i < SmeltingMaxCount; i++)
        {
            GetObject(GameObjectsType, (int)GameObjects.Smelting_1_Object + i).SetActive(false);
        }

        if (Managers.GameM.gameData.EquippedSmelts.Count > 0)
        {
            for (int i = 0; i < Managers.GameM.gameData.EquippedSmelts.Count; i++)
            {
                var saveData = Managers.GameM.gameData.EquippedSmelts[i];
                RestoreSlot(saveData, i);
            }
        }

        RefreshUI();
        return true;
    }


    public void RestoreSlot(SmeltHolder _holder, int _count)
    {
        Managers.SoundM.MuteEffectVolume(true);
        GameObject slot = GetObject(GameObjectsType, (int)GameObjects.Smelting_1_Object + _count);
        slot.SetActive(true);

        string statusName = GetSmeltDataName(_holder.holder);

        var dataInSheet = Managers.DataM.SmeltDataDic.Values
            .FirstOrDefault(d => d.Name == statusName && d.Grade == _holder.grade);

        if (dataInSheet == null) return;

        string colorTag = Utils.StringToColorGrade(_holder.grade);


        GetText(TextsType, (int)Texts.Smelting_1_ContentText + _count).text = $"{colorTag}{Managers.LocalizationM.Get(dataInSheet.Name)}</color>";
        GetText(TextsType, (int)Texts.Smelting_1_ProbabilityText + _count).text = $"{colorTag}{_holder.value:F2}%</color>";

    }


    public void SetSmelt()
    {
        Managers.GameM.gameData.EquippedSmelts.Clear();
        for (int i = 0; i < SmeltingMaxCount; i++)
        {
            GetObject(GameObjectsType, (int)GameObjects.Smelting_1_Object + i).SetActive(false);
        }

        SetSmeltSlot(GetPercentage());

        Managers.FirebaseM.WriteData().Forget();
    }

    public void SetSmeltSlot(int _slotCount)
    {

        for (int i = 0; i < _slotCount; i++)
        {
            Define.Status_Holder statusNum = (Define.Status_Holder)UnityEngine.Random.Range(0, statusCount);

            SetSlot(statusNum, i);
        }
    }


    private string GetSmeltDataName(Define.Status_Holder _holder)
    {
        switch (_holder)
        {
            case Define.Status_Holder.Damage:       return "DamageUp";
            case Define.Status_Holder.HP:           return "HpUp";
            case Define.Status_Holder.Money:        return "GoldUp";
            case Define.Status_Holder.Item:         return "ItemDropUp";
            case Define.Status_Holder.Exp:          return "ExpUp";
            case Define.Status_Holder.AttackSpeed:  return "AttackSpeed";
            case Define.Status_Holder.CriticalP:    return "CriticalChanceUp";
            case Define.Status_Holder.CriticalD:    return "CriticalDamageUp";
            default: return _holder.ToString();
        }
    }

    public void SetSlot(Define.Status_Holder _statusName, int _count)
    {


        GameObject slot = GetObject(GameObjectsType, (int)GameObjects.Smelting_1_Object + _count);
        slot.transform.DOKill();
        slot.SetActive(true);

        string statusName = GetSmeltDataName(_statusName);
        var candidate = Managers.DataM.SmeltDataDic.Values
        .Where(d => d.Name == statusName)
        .ToList();

        Data.SmeltData localData = null;
        int randNum = UnityEngine.Random.Range(0, 100);
        float sum = 0;
        for (int i = 0; i < candidate.Count; i++)
        {
            sum += candidate[i].Probability;
            if (sum >= randNum)
            {
                localData = candidate[i];
                break;
            }
        }
        if (localData == null) return;

        float targetStat = UnityEngine.Random.Range(localData.Min, localData.Max);
        targetStat = Mathf.Floor(targetStat * 100f) / 100f;

        string colorTag = Utils.StringToColorGrade(localData.Grade);

        slot.transform.localScale = Vector3.zero;
        slot.transform.DOScale(1.0f, 0.5f)
        .SetDelay(_count * 0.1f)
        .SetEase(Ease.OutBack);

        GetText(TextsType, (int)Texts.Smelting_1_ContentText + _count).text = $"{colorTag}{Managers.LocalizationM.Get(localData.Name)}</color>";

        float startValue = 0;

        DOTween.To(() => startValue, x =>
        {
            GetText(TextsType, (int)Texts.Smelting_1_ProbabilityText + _count).text =
            $"{colorTag}{x:F2}%</color>";
        }, targetStat, 0.5f)
        .SetDelay(_count * 0.1f)
        .OnComplete(() =>
        {
            if (localData.Grade >= Define.Grade.Unique)
            {
                slot.transform.DOShakePosition(0.4f, 10.0f, 20);
                slot.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.5f, 5);
            }
        });

        Managers.GameM.gameData.EquippedSmelts.Add(new SmeltHolder
        {
            holder = _statusName,
            grade = localData.Grade,
            value = targetStat
        });

        for (int i = 0; i < Managers.CharacterM.players.Length; i++)
        {
            if (Managers.CharacterM.players[i] != null)
                Managers.CharacterM.players[i].SetStat();
        }

    }


    public int GetPercentage()
    {
        int randNum = UnityEngine.Random.Range(0, 100);
        int slotCount = 1;
        int sum = 0;
        for (int i = 0; i < Utils.SmeltSlotCountWeights.Length; i++)
        {
            sum += Utils.SmeltSlotCountWeights[i];
            if (sum < randNum)
            {
                slotCount++;
            }
            else return slotCount;
        }

        return -1;
    }

    public void RefreshUI()
    {
        int currentCount = Managers.InventoryM.GetItemCount(UseItemName);
        var text = GetText(TextsType, (int)Texts.NeedItemCountText);
        text.text = $"({currentCount} / {NeedCount})";

        if (currentCount >= NeedCount)
        {
            text.color = Color.green;
            GetImage(ImagesType, (int)Images.LockImage).gameObject.SetActive(false);
        }
        else
        {
            text.color = Color.red;
            GetImage(ImagesType, (int)Images.LockImage).gameObject.SetActive(true);
        }
    }

    async void OnClickCloseButton()
    {
        Managers.SoundM.PlayButtonClick();
        await TriggerClose(this, false);
    }

    void OnClickSmeltingButton()
    {
        Managers.SoundM.PlayButtonClick();

        if (Managers.InventoryM.UseItem(UseItemName, NeedCount))
        {
            SetSmelt();
            RefreshUI();
            Managers.QuestM.GetMission(Define.MissionTarget.Smelting).Progress++;
        }
    }
}
