using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static Define;
using System.Threading;

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
    bool isChangingState = false;
    public OnReadyEvent readyEvent;
    public OnPlayEvent playEvent;
    public OnBossEvent bossEvent;
    public OnBossPlayEvent bossPlayEvent;
    public OnClearEvent clearEvent;
    public OnDeadEvent deadEvent;
    public OnDungeonEvent dungeonEvent;
    public OnDungeonClearEvent dungeonClearEvent;
    public OnDungeonFailEvent dungeonFailEvent;
    public OnDungeonOutEvent dungeonOutEvent;

    private CancellationTokenSource stateCts;

    public void StateChange(StageState _state, int _dungeonDataID = 0, StageState _prevStage = StageState.Play)
    {
        if (isChangingState) return;
        isChangingState = true;

        try
        {
            stateCts?.Cancel();
            stateCts?.Dispose();
            stateCts = new CancellationTokenSource();
            var token = stateCts.Token;

            stageState = _state;
            switch (stageState)
            {
                case StageState.Ready:
                    maxCount = Managers.DataM.StageDataDic[Managers.GameM.Stage].StageClearMaxCount;
                    count = 0;
                    readyEvent?.Invoke();
                    AsyncAction(() => StateChange(StageState.Play), 1f, token).Forget();

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

                    Managers.QuestM.GetMission(Define.MissionTarget.StageClear).Progress++;

                    isDead = false;
                    break;
                case StageState.Dead:
                    count = 0;
                    deadEvent?.Invoke();
                    isDead = true;
                    break;

                case StageState.Dungeon:
                    isDungeon = true;
                    dungeonEvent?.Invoke(_dungeonDataID);
                    count = 0;
                    if (_dungeonDataID == 70000) AsyncAction(() => StateChange(StageState.Play, _prevStage: StageState.Dungeon), 1f, token).Forget();
                    break;

                case StageState.DungeonClear:
                    isDungeon = false;
                    count = 0;

                    Managers.QuestM.GetMission(Define.MissionTarget.DungeonClear).Progress++;
                    dungeonClearEvent?.Invoke();
                    break;

                case StageState.DungeonFail:
                    isDungeon = false;
                    count = 0;
                    dungeonFailEvent?.Invoke();
                    break;

                case StageState.DungeonOut:
                    dungeonOutEvent?.Invoke();
                    break;
            }
        }
        finally
        {
            isChangingState = false;
        }

    }


    async UniTask AsyncAction(Action _action, float _timer, CancellationToken _token)
    {
        try
        {
            await UniTask.WaitForSeconds(_timer, cancellationToken: _token);
            _action?.Invoke();
        }
        catch (OperationCanceledException) { }
        catch (Exception e) { }
    }

}

