using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class MissionInfo
{
    public int Progress;
    public bool isRewarded;
}
public class GameManager
{
    public PlayerController mPlayer { get { return Managers.ObjectM?.mPlayer; } }
    public GameData gameData = new GameData();

    #region 재화 이벤트
    public event Action OnGoodsChanged;
    #endregion
    public double Damage
    {
        get { return gameData.damage; }
        set
        {
            gameData.damage = value;
        }
    }

    public double HP
    {
        get { return gameData.hp; }
        set
        {
            gameData.hp = value;
        }
    }

    public double Gold
    {
        get { return gameData.gold; }
        set
        {
            gameData.gold = value;
            OnGoodsChanged?.Invoke();
        }
    }

    public double Dia
    {
        get { return gameData.dia; }
        set
        {
            gameData.dia = value;
            OnGoodsChanged?.Invoke();
        }
    }
    public int Level
    {
        get { return gameData.level; }
        set
        {
            gameData.level = value;
        }
    }

    public int UpgradeCount
    {
        get { return gameData.upgradeCount; }
        set
        {
            gameData.upgradeCount = value;
            Managers.QuestM.UpdateQuest();
        }
    }

    public int QuestCount
    {
        get { return gameData.questCount; }
        set { gameData.questCount = value; }
    }
    public int QuestLevel
    {
        get { return gameData.questLevel; }
        set { gameData.questLevel = value; }
    }


    public string StartDate
    {
        get { return gameData.startDate; }
        set { gameData.startDate = value; }
    }
    public string EndDate
    {
        get { return gameData.endDate; }
        set { gameData.endDate = value; }
    }

    public double Exp
    {
        get
        {
            return gameData.exp;
        }
        set
        {
            gameData.exp = value;
        }
    }

    public int Stage
    {
        get
        {
            return gameData.stage;
        }
        set
        {
            gameData.stage = value;
            Managers.QuestM.UpdateQuest();
        }
    }

    public float[] Buff_Timers
    {
        get
        {
            return gameData.buff_Timers;
        }
        set
        {
            gameData.buff_Timers = value;
        }
    }

    public float Fast_Timer
    {
        get
        {
            return gameData.fast_Timer;
        }
        set
        {
            gameData.fast_Timer = value;
        }
    }

    public int Buff_Level
    {
        get
        {
            return gameData.buff_Level;
        }
        set
        {
            gameData.buff_Level = value;
        }
    }

    public int Buff_Count
    {
        get { return gameData.buff_count; }
        set { gameData.buff_count = value; }
    }

    public int Hero_Summon_Count
    {
        get { return gameData.heroSummonCount; }
        set
        {
            gameData.heroSummonCount = value;
            Managers.QuestM.UpdateQuest();
        }
    }

    public int Hero_Confirmed_Legendary_Count
    {
        get { return gameData.heroConfirmedLegendaryCount; }
        set { gameData.heroConfirmedLegendaryCount = value; }
    }

    public int Relics_Summon_Count
    {
        get { return gameData.relicsSummonCount; }
        set
        {
            gameData.relicsSummonCount = value;
            Managers.QuestM.UpdateQuest();
        }
    }

    public int Relics_Confirmed_Legendary_Count
    {
        get { return gameData.relicsConfirmedLegendaryCount; }
        set { gameData.relicsConfirmedLegendaryCount = value; }
    }

    public void SetDungeonClear(Define.DungeonType _type, int level)
    {
        gameData.DungeonClearLevel[(int)_type] = level;
        Managers.QuestM.UpdateQuest();
    }

    public Dictionary<Define.MissionTarget, MissionInfo> MissionDic
    {
        get { return gameData.MissionDic; }
        set { gameData.MissionDic = value; }
    }
}
