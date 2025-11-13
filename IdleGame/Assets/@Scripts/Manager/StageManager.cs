using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static Define;


public delegate void OnReadyEvent();
public delegate void OnPlayEvent();
public delegate void OnBossEvent();
public delegate void OnBossPlayEvent();
public delegate void OnClearEvent();
public delegate void OnDeadEvent();

//State Pattern
public class StageManager
{
    public StageState stageState;

    public int maxCount = 5;
    public int count = 0;
    public int stage;

    public OnReadyEvent readyEvent;
    public OnPlayEvent playEvent;
    public OnBossEvent bossEvent;
    public OnBossPlayEvent bossPlayEvent;
    public OnClearEvent clearEvent;
    public OnDeadEvent deadEvent;
    
    public void StateChange(StageState _state)
    {
        stageState = _state;
        switch (stageState)
        {
            case StageState.Ready:
                readyEvent?.Invoke();
                AsyncAction(() => StateChange(StageState.Play), 2.0f).Forget();

                break;
            case StageState.Play:
                playEvent?.Invoke();
                break;
            case StageState.Boss:
                count = 0;
                bossEvent?.Invoke();
                break;
            case StageState.BossPlay:
                bossPlayEvent?.Invoke();
                break;
            case StageState.Clear:
                clearEvent?.Invoke();
                stage++;
                break;
            case StageState.Dead:
                deadEvent?.Invoke();
                break;
        }
    }


    async UniTask AsyncAction(Action _action, float _timer)
    {
        await UniTask.WaitForSeconds(_timer);
        _action?.Invoke();
    }
}
