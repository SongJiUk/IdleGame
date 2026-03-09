using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;


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

    public void ResetGameData()
    {
        gameData = new GameData();
        gameData.isGuest = true;
        gameData.playerName = "Guest";

        //TODO : 이거 테스트 해야됌(로그아웃 되는지 확인해보기) 로그아웃이 된다하면 타이틀씬으로 이동되는데, 모든값들 초기화되는지 확인하세요
        gameData.Init();

        OnGoodsChanged?.Invoke();

    }
    /*
    "모바일 운영체제의 정책상 백그라운드 실시간 연산은 불가능합니다. 그래서 OnApplicationPause를 활용한 타임스탬프 기반 정산 시스템을 구현하여, 앱이 꺼져있는 동안의 보상을 논리적으로 계산했습니다."
    */

    public void OnApplicationPause(bool _pauseStatus)
    {
        if (_pauseStatus)
        {
            Time.timeScale = 0f;
            Managers.UpdateM.PauseTicking(true);

            SaveGameOnPause();
        }
        else
        {
            Time.timeScale = 1f;
            Managers.UpdateM.PauseTicking(false);

            ReloadAndCheckOfflineReward();
        }
    }

    private async void SaveGameOnPause()
    {
        Managers.GameM.gameData.LastSaveTimeTicks = TimerNTP.NowTime.Ticks;

        await Managers.FirebaseM.WriteData();

    }

    async void ReloadAndCheckOfflineReward()
    {
        await UniTask.WaitUntil(() => Managers.FirebaseM.CurrentUser != null);

        await Managers.FirebaseM.SyncDataOnly();

        double elapsedSeconds = GetOfflineSeconds();


        if (elapsedSeconds >= 10.0d)
        {
            var popup = await Managers.UIM.ShowPopup<UI_OfflinePopup>();
            popup.SetInfo();
        }

    }

    private double GetOfflineSeconds()
    {
        long lastTicks = Managers.GameM.gameData.LastSaveTimeTicks;
        long nowTicks = TimerNTP.NowTime.Ticks;

        if (lastTicks == 0) return 0;

        long elapsedTicks = nowTicks - lastTicks;
        double elapsedSeconds = (double)elapsedTicks / TimeSpan.TicksPerSecond;

        return elapsedSeconds < 0 ? 0 : elapsedSeconds;
    }


}
