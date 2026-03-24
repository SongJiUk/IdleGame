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
    public float spawnInterval;
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
    //bool isChangingState = false;
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
        // if (isChangingState) return;
        // isChangingState = true;

        if (stageState == _state) return;

        if (stageState == Define.StageState.Changing && _state == Define.StageState.Changing)
        {
            Debug.LogError("이미 State Changing중 입니다.");
            return;
        }

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
                    int stageForward = ((Managers.GameM.Stage - 1) / 20) + 1;
                    int stageBack = ((Managers.GameM.Stage - 1) % 20) + 1;
                    int baseCount = Managers.DataM.StageDataDic[stageForward].StageClearMaxCount;
                    maxCount = baseCount + (stageBack - 1) * 15 / 19;
                    spawnInterval = Managers.DataM.StageDataDic[stageForward].SpawnTimer;
                    count = 0;
                    readyEvent?.Invoke();
                    AsyncAction(() => StateChange(StageState.Play), 1f, token).Forget();

                    break;
                case StageState.Play:
                    playEvent?.Invoke(_prevStage);
                    break;
                case StageState.Boss:
                    Managers.SoundM.Play(Sound.Effect, "BossStage");
                    count = 0;
                    bossEvent?.Invoke();
                    break;
                case StageState.BossPlay:
                    Managers.SoundM.Play(Sound.Effect, "BossSound");
                    bossPlayEvent?.Invoke();
                    break;
                case StageState.Clear:
                    count = 0;
                    clearEvent?.Invoke();
                    Managers.GameM.gameData.clearStage++;
                    Managers.GameM.Stage++;
                    Managers.CharacterM.aliveCount = 0;

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
                    isDead = false;
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
        catch (Exception e) { Debug.LogError(e); }

    }


    async UniTask AsyncAction(Action _action, float _timer, CancellationToken _token)
    {
        try
        {
            await UniTask.WaitForSeconds(_timer, cancellationToken: _token);
            _action?.Invoke();
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public bool CanEnterDungeon()
    {
        if (stageState == StageState.Dungeon) return false;
        if (stageState == StageState.Changing) return false;

        if (stageState == StageState.Dead || stageState == StageState.Clear || stageState == StageState.BossPlay) return false;

        return true;
    }


    public bool CanChangeToReady()
    {
        if (stageState == StageState.Boss || stageState == StageState.BossPlay)
        {
            Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastBoss"));
            return false;
        }

        if (stageState == StageState.Dungeon ||
            stageState == StageState.DungeonClear ||
            stageState == StageState.DungeonFail ||
            stageState == StageState.DungeonOut)
        {
            Managers.UIM.ShowToast(Managers.LocalizationM.Get("UIToastDugeon"));
            return false;
        }
        return true;
    }

    public void Clear()
    {
        stateCts?.Cancel();
        stateCts.Dispose();
        stateCts = null;

        stageState = StageState.None;
        count = 0;
        isDead = false;
        isDungeon = false;

        readyEvent = null;
        playEvent = null;
        bossEvent = null;
        bossPlayEvent = null;
        clearEvent = null;
        deadEvent = null;
        dungeonEvent = null;
        dungeonClearEvent = null;
        dungeonFailEvent = null;
        dungeonOutEvent = null;

        Debug.Log("[StageManager] 스테이지 상태 초기화 및 이벤트 구독 해제 완료");

    }
}

