using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO : 이거 UI_GameScene에서 관리하게 바꿔줘야함
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


    public void Init()
    {
        NextQuest();
    }
    public void UpdateQuest()
    {
        GetReward();
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

        quest = Managers.DataM.QuestDataDic[Managers.GameM.QuestCount];
        questType = quest.QuestType;

        if(questType == Define.QuestType.Monster)
        {
            isGetEnemy = true;
        }
        OnQuestDataChanged?.Invoke();
    }

    public Color GetCountColor()
    {
        Color color = Counting(questType) >= quest.Value ? Color.green : Color.red;

        
        return color;
    }

    public bool GetReward()
    {
        isReward = Counting(questType) >= quest.Value ? true : false;

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
        quest = Managers.DataM.QuestDataDic[Managers.GameM.QuestCount];
        questType = quest.QuestType;

        return questType;
    }

    public (bool, int) GetQuestButton()
    {
        if (!isReward) return (false, 0);
        int count = quest.Reward;

        Managers.GameM.QuestCount++;
        NextQuest();

        return (true,count);

    }
}   
