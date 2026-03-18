using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public interface ITickable
{
    void Tick(float _deltaTime);
}

public interface IUnScaledTickable
{
    void UnscaledTick(float _unscaledDeltaTime);
}

public class UpdateManager : MonoBehaviour
{
    public bool isStartFirebase = false;
    //private readonly List<ITickable> tickables = new();
    private readonly HashSet<ITickable> tickables = new();
    //private readonly List<ITickable> toAdd = new();
    private readonly HashSet<ITickable> toAdd = new();
    //private readonly List<ITickable> toRemove = new();
    private readonly HashSet<ITickable> toRemove = new();


    //private readonly List<IUnScaledTickable> unScaledTickables = new();
    //private readonly List<IUnScaledTickable> unScaledToAdd = new();
    //private readonly List<IUnScaledTickable> unScaledToRemove = new();

    private readonly HashSet<IUnScaledTickable> unScaledTickables = new();
    private readonly HashSet<IUnScaledTickable> unScaledToAdd = new();
    private readonly HashSet<IUnScaledTickable> unScaledToRemove = new();

    private bool isPaused = false;
    bool isWriting = false;
    public void PauseTicking(bool _pause)
    {
        isPaused = _pause;
    }

    public void Register(ITickable _tickable = null, IUnScaledTickable _unscaledTickable = null)
    {
        if(_tickable != null) toAdd.Add(_tickable);
        if (_unscaledTickable != null) unScaledToAdd.Add(_unscaledTickable);
    }


    public void UnRegister(ITickable _tickable = null, IUnScaledTickable _unscaledTickable = null)
    {
        if (_tickable != null) toRemove.Add(_tickable);
        if (_unscaledTickable != null)  unScaledToRemove.Add(_unscaledTickable);
    }

    async UniTaskVoid ExcuteWriteData()
    {
        isWriting = true;
        try
        {
            await Managers.FirebaseM.WriteData();
        }
        catch (Exception e)
        {
            Debug.LogError($"[UpdateManager] 자동 저장 실패: {e.Message}");
        }
        finally
        {
            // 성공/실패 관계없이 반드시 플래그 해제
            isWriting = false;
        }
    }

    private void Update()
    {
        if (isPaused) return;


        if (isStartFirebase && !Managers.FirebaseM.IsLoading && !isWriting && !Managers.FirebaseM.IsDeleting)
        {
            Managers.save_Timer += Time.unscaledDeltaTime;
            if (Managers.save_Timer >= 10.0f)
            {
                Managers.save_Timer = 0.0f;
                ExcuteWriteData().Forget();
            }
        }


        float deltaTime = Time.deltaTime;
        float unscaledDeltaTime = Time.unscaledDeltaTime;


        foreach (var t in toAdd)
            tickables.Add(t);
        toAdd.Clear();
        foreach (var t in toRemove) tickables.Remove(t);
        toRemove.Clear();

        foreach (var t in unScaledToAdd)
            unScaledTickables.Add(t);
        unScaledToAdd.Clear();
        foreach (var t in unScaledToRemove) unScaledTickables.Remove(t);
        unScaledToRemove.Clear();


        foreach (var tick in tickables)
        {
            if (tick is Component component && component == null)
            {
                toRemove.Add(tick);
                continue;
            }
            tick.Tick(deltaTime);
        }


        foreach (var tick in unScaledTickables)
        {
            if (tick is Component component && component == null)
            {
                unScaledToRemove.Add(tick);
                continue;
            }
            tick.UnscaledTick(unscaledDeltaTime);
        }
    }

    public void Clear()
    {
        PauseTicking(false);
        tickables.Clear();
        toAdd.Clear();
        toRemove.Clear();

        unScaledTickables.Clear();
        unScaledToAdd.Clear();
        unScaledToRemove.Clear();

        isStartFirebase = false;
        Managers.save_Timer = 0.0f;
        isWriting = false;

        Debug.Log("[UpdateManager] 틱 업데이트 및 저장 타이머 초기화 완료");
    }
}
