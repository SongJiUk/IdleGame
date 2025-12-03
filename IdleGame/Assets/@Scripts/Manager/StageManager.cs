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
    public event Action OnChangeCount;

    //TODO: ��� 10��? 20��? �����غ���
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

    //TODO : ����� ���� ���� x
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
                //maxCount = Managers.DataM.StageDataDic[Managers.GameM.stage].StageClearMaxCount;
                maxCount = 200;
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
                Managers.GameM.stage++;
                isDead = false;
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
