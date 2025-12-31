using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private readonly List<ITickable> tickables = new();
    private readonly List<ITickable> toAdd = new();
    private readonly List<ITickable> toRemove = new();

    private readonly List<IUnScaledTickable> unScaledTickables = new();
    private readonly List<IUnScaledTickable> unScaledToAdd = new();
    private readonly List<IUnScaledTickable> unScaledToRemove = new();

    private bool isPaused = false;
    bool isWriting = false;
    public void PauseTicking(bool _pause)
    {
        isPaused = _pause;
    }

    public void Register(ITickable _tickable = null, IUnScaledTickable _unscaledTickable = null)
    {

        if (_tickable != null)
        {
            if (!tickables.Contains(_tickable)) toAdd.Add(_tickable);
        }


        if (_unscaledTickable != null)
        {
            if (!unScaledTickables.Contains(_unscaledTickable)) unScaledToAdd.Add(_unscaledTickable);
        }


    }


    public void UnRegister(ITickable _tickable = null, IUnScaledTickable _unscaledTickable = null)
    {
        if (_tickable != null) toRemove.Add(_tickable);

        if (_unscaledTickable != null)
        {
            if (!unScaledToRemove.Contains(_unscaledTickable)) unScaledToRemove.Add(_unscaledTickable);
        }
    }
    async void ExcuteWriteData()
    {
        isWriting = true;
        await Managers.firebaseM.WriteData();
        isWriting = false;
    }

    private void Update()
    {
        if (isPaused) return;


        if (isStartFirebase && !Managers.firebaseM.IsLoading && !isWriting)
        {
            Managers.save_Timer += Time.unscaledDeltaTime;
            if (Managers.save_Timer >= 10.0f)
            {
                Managers.save_Timer = 0.0f;
                ExcuteWriteData();
            }
        }


        float deltaTime = Time.deltaTime;
        float unscaledDeltaTime = Time.unscaledDeltaTime;


        foreach (var t in toAdd)
            if (!tickables.Contains(t)) tickables.Add(t);
        toAdd.Clear();
        foreach (var t in toRemove) tickables.Remove(t);
        toRemove.Clear();

        foreach (var t in unScaledToAdd)
            if (!unScaledTickables.Contains(t)) unScaledTickables.Add(t);
        unScaledToAdd.Clear();
        foreach (var t in unScaledToRemove) unScaledTickables.Remove(t);
        unScaledToRemove.Clear();

        for (int i = tickables.Count - 1; i >= 0; i--)
        {
            ITickable tick = tickables[i];
            if (tick is Component component)
            {
                if (component == null)
                {
                    tickables.RemoveAt(i);
                    continue;
                }
            }
            tick.Tick(deltaTime);
        }

        for (int i = unScaledTickables.Count - 1; i >= 0; i--)
        {
            IUnScaledTickable tick = unScaledTickables[i];
            if (tick is Component component)
            {
                if (component == null)
                {
                    unScaledTickables.RemoveAt(i);
                    continue;
                }
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
    }
}
