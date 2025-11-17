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

    //TODO: 계속 10개? 20개? 고민해보자
    public int maxCount = 20;
    public int count = 0;
    public int stage;
    public bool isDead = false;
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
                AsyncAction(() => StateChange(StageState.Play), 1f).Forget();

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
                count = 0;
                clearEvent?.Invoke();
                stage++;
                break;
            case StageState.Dead:
                count = 0;
                deadEvent?.Invoke();
                isDead = true;
                break;
        }
    }


    async UniTask AsyncAction(Action _action, float _timer)
    {
        await UniTask.WaitForSeconds(_timer);
        _action?.Invoke();
    }
}
