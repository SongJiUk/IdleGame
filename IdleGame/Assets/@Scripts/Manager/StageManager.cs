using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static Define;

//State Pattern
public class StageManager
{
    public StageState stageState;
    public event Action OnChangeCount;


    public int maxCount;
    int count = 0;
    public int COUNT
    {
        get { return count; }
        set
        {
            count = value;
            OnChangeCount?.Invoke();
        }
    }


    public bool isDead = false;
    public bool isDungeon = false;
    public OnReadyEvent readyEvent;
    public OnPlayEvent playEvent;
    public OnBossEvent bossEvent;
    public OnBossPlayEvent bossPlayEvent;
    public OnClearEvent clearEvent;
    public OnDeadEvent deadEvent;
    public OnDungeonEvent dungeonEvent;
    public OnDungeonClearEvent dungeonClearEvent;
    public OnDungeonFailEvent dungeonFailEvent;

    public void StateChange(StageState _state, int _value = 0, StageState _prevStage = StageState.Play)
    {
        stageState = _state;
        switch (stageState)
        {
            case StageState.Ready:
                maxCount = Managers.DataM.StageDataDic[Managers.GameM.Stage].StageClearMaxCount;
                readyEvent?.Invoke();
                AsyncAction(() => StateChange(StageState.Play), 1f).Forget();

                break;
            case StageState.Play:
                playEvent?.Invoke(_prevStage);
                break;
            case StageState.Boss:
                count = 0;
                bossEvent?.Invoke();
                break;
            case StageState.BossPlay:
                bossPlayEvent?.Invoke();
                break;
            case StageState.Clear:
                count = 0;
                clearEvent?.Invoke();
                Managers.GameM.Stage++;
                isDead = false;
                break;
            case StageState.Dead:
                count = 0;
                deadEvent?.Invoke();
                isDead = true;
                break;

            case StageState.Dungeon:
                isDungeon = true;
                dungeonEvent?.Invoke(_value);
                count = 0;
                if (_value == 0) AsyncAction(() => StateChange(StageState.Play, _prevStage: StageState.Dungeon), 1f).Forget();
                break;

            case StageState.DungeonClear:
                isDungeon = false;
                count = 0;
                dungeonClearEvent?.Invoke(_value);
                break;

            case StageState.DungeonFail:
                isDungeon = false;
                count = 0;
                dungeonFailEvent?.Invoke(_value);
                break;
        }
    }


    async UniTask AsyncAction(Action _action, float _timer)
    {
        await UniTask.WaitForSeconds(_timer);
        _action?.Invoke();
    }
}
