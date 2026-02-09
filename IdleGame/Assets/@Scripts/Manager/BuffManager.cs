using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : IUnScaledTickable
{

    float[] Timers => Managers.GameM.Buff_Timers;

    float autoSaveTimer = 0f;
    const float AUTO_SAVE_INTERVAL = 30f;

    public void StartBuff(Define.BuffType _type, float _time)
    {
        int index = (int)_type;
        Timers[index] = _time;
        (Managers.UIM.SceneUI as UI_GameScene).SetBuffs(_type, true);
        Managers.UpdateM.Register(_unscaledTickable: this);
        Managers.FirebaseM.WriteData().Forget();
    }

    public void RestoreFromSave()
    {
        bool hasAny = false;

        for (int i = 0; i < Timers.Length; i++)
        {
            if (Timers[i] > 0f)
            {
                hasAny = true;
                (Managers.UIM.SceneUI as UI_GameScene).SetBuffs((Define.BuffType)i, true);
            }
        }

        if (hasAny) Managers.UpdateM.Register(_unscaledTickable: this);
    }
    public void UnscaledTick(float _unscaledDeltaTime)
    {
        autoSaveTimer += _unscaledDeltaTime;
        if (autoSaveTimer >= AUTO_SAVE_INTERVAL)
        {
            autoSaveTimer = 0f;
            SaveTimers();
        }

        bool hasAny = false;
        for (int i = 0; i < Timers.Length; i++)
        {
            if (Timers[i] > 0f)
            {
                Timers[i] -= _unscaledDeltaTime;

                if (Timers[i] <= 0f)
                {
                    Timers[i] = 0f;
                    (Managers.UIM.SceneUI as UI_GameScene).SetBuffs((Define.BuffType)i, false);
                }
                else
                {
                    hasAny = true;
                }
            }
        }

        if (!hasAny) Managers.UpdateM.UnRegister(_unscaledTickable: this);
    }

    void SaveTimers()
    {
        Managers.FirebaseM.WriteData().Forget();
    }

    public float GetRemainTime(Define.BuffType _type)
    {
        return Timers[(int)_type];
    }

    public bool IsActive(Define.BuffType _type)
    {
        return Timers[(int)_type] > 0f;
    }
}
