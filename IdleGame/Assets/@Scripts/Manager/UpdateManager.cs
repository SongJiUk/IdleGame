using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface ITickable
{
    void Tick(float _deltaTime);
}

public class UpdateManager : MonoBehaviour
{
    private readonly List<ITickable> tickables = new();
    private readonly List<ITickable> toAdd = new();
    private readonly List<ITickable> toRemove = new();

    private bool isPaused = false;
    public void PauseTicking(bool _pause)
    {
        isPaused = _pause;
    }

    public void Register(ITickable _tickable)
    {
        if (!tickables.Contains(_tickable)) toAdd.Add(_tickable);
    }

    public void UnRegister(ITickable _tickable)
    {
        toRemove.Add(_tickable);
    }


    private void Update()
    {
        if (isPaused) return;
        float deltaTime = Time.deltaTime;

        foreach (var t in toAdd)
            if (!tickables.Contains(t)) tickables.Add(t);
        toAdd.Clear();
        foreach (var t in toRemove) tickables.Remove(t);
        toRemove.Clear();

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
    }

    public void Clear()
    {
        PauseTicking(false);
        tickables.Clear();
        toAdd.Clear();
        toRemove.Clear();
    }
}
