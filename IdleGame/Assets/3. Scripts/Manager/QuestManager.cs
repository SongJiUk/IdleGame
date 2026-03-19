using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


public class DailyMission
{

}

public class Achievement_Status
{
    public double damage;
    public double hp;
    public double money;
    public double item;
    public double exp;
    public double attackSpeed;
    public double criticalP;
    public double criticalD;

    public void GetStatusData(Define.Status_Holder _holder, double _value)
    {
        switch (_holder)
        {
            case Define.Status_Holder.Damage: damage += _value; break;
            case Define.Status_Holder.HP: hp += _value; break;
            case Define.Status_Holder.Money: money += _value; break;
            case Define.Status_Holder.Item: item += _value; break;
            case Define.Status_Holder.Exp: exp += _value; break;
            case Define.Status_Holder.AttackSpeed: attackSpeed += _value; break;
            case Define.Status_Holder.CriticalP: criticalP += _value; break;
            case Define.Status_Holder.CriticalD: criticalD += _value; break;
        }

    }
}
public class QuestManager
{
    public event Action OnQuestDataChanged;
    int monsterIndex;
    public int MonsterIndex
    {
        get { return monsterIndex; }
        set
        {
            monsterIndex = value;
            OnQuestDataChanged?.Invoke();
        }
    }


    Data.QuestData quest;
    public Data.QuestData Quest
    {
        get { return quest; }
    }


    Define.QuestType questType;
    public bool isGetEnemy = false;
    public bool isReward = false;
    #region Daily Mission
    public Dictionary<string, MissionInfo> MissionDic
    {
        get { return Managers.GameM.gameData.MissionDic; }
        set { Managers.GameM.gameData.MissionDic = value; }
    }

    public MissionInfo GetMission(Define.MissionTarget _target)
    {
        string key = _target.ToString();
        if (!MissionDic.TryGetValue(key, out MissionInfo info))
        {
            info = new MissionInfo();
            MissionDic[key] = info;
        }
        return info;
    }

    #endregion

    #region Achievement 

    //public List<Data.AchievementData> AchievementList = new List<Data.AchievementData>();
    public Achievement_Status Achievement_Status_Data = new Achievement_Status();
    public Dictionary<int, bool> AchievementDic
    {
        get { return Managers.GameM.gameData.AchievementDic; }
        set { Managers.GameM.gameData.AchievementDic = value; }
    }

    public void FixAchievementDatas()
    {
        foreach (var data in Managers.DataM.AchievementDataDic)
        {
            int id = data.Key;

            if (!AchievementDic.ContainsKey(id))
            {
                AchievementDic[id] = false;
            }
        }
    }
    public void LoadAchievementDatas()
    {
        //Debug.Log($"[Debug] 로드 시점 AchievementDic 개수: {AchievementDic.Count}");
        Achievement_Status achievement_Status = new Achievement_Status();
        foreach (var data in Managers.DataM.AchievementDataDic)
        {
            int key = data.Key;
            bool isCompleted = false;
            if (AchievementDic.TryGetValue(key, out isCompleted))
            {
                if (isCompleted)
                {
                    achievement_Status.GetStatusData(data.Value.RewardStatus, data.Value.RewardValue);
                }
            }
            else
            {
                Debug.LogWarning($"[DataWarning] 업적 ID {key}가 세이브 데이터에 없습니다. 로딩 순서를 확인하세요!");
            }
        }

        Achievement_Status_Data = achievement_Status;
    }
    #endregion
    public void Init()
    {

        FixAchievementDatas();
        LoadAchievementDatas();
        NextQuest();
    }

    public void UpdateQuest()
    {
        isGetReward();
        OnQuestDataChanged?.Invoke();
    }
    public int Counting(Define.QuestType _questType)
    {
        switch (_questType)
        {
            case Define.QuestType.Monster: return monsterIndex;
            case Define.QuestType.Stage: return Managers.GameM.gameData.clearStage;
            case Define.QuestType.Gold: return Managers.GameM.gameData.DungeonClearLevel[1];
            case Define.QuestType.Dia: return Managers.GameM.gameData.DungeonClearLevel[0];
            case Define.QuestType.Upgrade: return Managers.GameM.UpgradeCount;
            case Define.QuestType.Hero: return Managers.GameM.Hero_Summon_Count;
            case Define.QuestType.Relic: return Managers.GameM.Relics_Summon_Count;
        }
        return 0;
    }

    public void NextQuest()
    {
        monsterIndex = 0;

        int totalQuestCount = Managers.DataM.QuestDataDic.Count;

        int questAllClearCount = Managers.GameM.QuestCount / totalQuestCount;

        Managers.GameM.QuestLevel = questAllClearCount + 1;
        int questCount = Managers.GameM.QuestCount % totalQuestCount;


        quest = Managers.DataM.QuestDataDic[questCount];
        questType = quest.QuestType;

        if (questType == Define.QuestType.Monster)
        {
            isGetEnemy = true;
        }
        OnQuestDataChanged?.Invoke();
    }

    public Color GetCountColor()
    {
        Color color = Counting(questType) >= quest.Value * Managers.GameM.QuestLevel ? Color.green : Color.red;


        return color;
    }

    public bool isGetReward()
    {
        isReward = Counting(questType) >= quest.Value * Managers.GameM.QuestLevel;

        return isReward;
    }

    public string Localization_Counting(Define.QuestType _questType)
    {
        return Managers.LocalizationM.Get($"UIQuest_{_questType}");
    }

    public Define.QuestType GetState()
    {
        int questCount = Managers.GameM.QuestCount % Managers.DataM.QuestDataDic.Count;
        quest = Managers.DataM.QuestDataDic[questCount];
        questType = quest.QuestType;

        return questType;
    }

    public (bool, int) GetQuestButton()
    {
        if (!isReward) return (false, 0);
        int count = quest.Reward * Managers.GameM.QuestLevel / 2;

        Managers.GameM.QuestCount++;
        NextQuest();

        return (true, count);

    }

    public int GetQuestValue()
    {
        return quest.Value * Managers.GameM.QuestLevel;
    }

    public int GetReward()
    {
        return quest.Reward * Managers.GameM.QuestLevel / 2;
    }

    public void Clear()
    {
        monsterIndex = 0;
        isGetEnemy = false;
        isReward = false;

        Achievement_Status_Data = new Achievement_Status();
        OnQuestDataChanged = null;
        Debug.Log("[QuestManager] 퀘스트 및 업적 상태 초기화 완료");
    }
}

