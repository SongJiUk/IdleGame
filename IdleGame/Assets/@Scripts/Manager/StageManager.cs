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
    public OnReadyEvent readyEvent;
    public OnPlayEvent playEvent;
    public OnBossEvent bossEvent;
    public OnBossPlayEvent bossPlayEvent;
    public OnClearEvent clearEvent;
    public OnDeadEvent deadEvent;
    public OnDungeonEvent dungeonEvent;

    public void StateChange(StageState _state, int _value = 0)
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
                Managers.GameM.Stage++;
                isDead = false;
                break;
            case StageState.Dead:
                count = 0;
                deadEvent?.Invoke();
                isDead = true;
                break;

            case StageState.Dungeon:
                dungeonEvent?.Invoke(_value);
                //TODO : 이걸 해주면 UI_GameScene에서 초기화 돼서 안됌. 고치던가 수정해야됌
                //AsyncAction(() => StateChange(StageState.Play), 1f).Forget();
                break;
        }
    }


    async UniTask AsyncAction(Action _action, float _timer)
    {
        await UniTask.WaitForSeconds(_timer);
        _action?.Invoke();
    }
}
