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
    public double skill;
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
            case Define.Status_Holder.Skill: skill += _value; break;
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

    public List<Data.AchievementData> AchievementList = new List<Data.AchievementData>();
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
        Achievement_Status achievement_Status = new Achievement_Status();
        foreach (var data in Managers.DataM.AchievementDataDic)
        {
            int key = data.Key;
            if (AchievementDic[key])
            {
                achievement_Status.GetStatusData(data.Value.RewardStatus, data.Value.RewardValue);
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
            case Define.QuestType.Stage: return Managers.GameM.Stage;
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
        switch (_questType)
        {
            case Define.QuestType.Monster: return "몬스터 처치";
            case Define.QuestType.Stage: return "스테이지 클리어";
            case Define.QuestType.Gold: return "골드 던전 클리어";
            case Define.QuestType.Dia: return "보물 던전 클리어";
            case Define.QuestType.Upgrade: return "경험치 획득";
            case Define.QuestType.Hero: return "영웅 소환";
            case Define.QuestType.Relic: return "유물 소환";
        }
        return null;
    }

    public Define.QuestType GetState()
    {
        //TODO ; 여기서 계속 높여가면 될거같음(레벨로 높이면 이거 스테이지나 뽑기 이런거 답 없기때문에, 그냥 데이터로 몇배 해줄지 정해놔야할거같음)
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
}

