using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Relic
public delegate void MonsterDead(MonsterController _mc);
public delegate void PlayerAttack(PlayerController _pc, MonsterController _mc);
public delegate void PlayerHit(PlayerController _pc);
#endregion


#region Stage
public delegate void OnReadyEvent();
public delegate void OnPlayEvent(Define.StageState _state);
public delegate void OnBossEvent();
public delegate void OnBossPlayEvent();
public delegate void OnClearEvent();
public delegate void OnDeadEvent();
public delegate void OnDungeonEvent(int _value);
public delegate void OnDungeonClearEvent();
public delegate void OnDungeonFailEvent();
public delegate void OnDungeonOutEvent();
#endregion
public class DelegateHolder
{
    public static event MonsterDead MonsterDeadEvent;
    public static event PlayerAttack PlayerAttackEvent;
    public static event PlayerHit PlayerHitEvent;

    public static void Clear()
    {
        MonsterDeadEvent = null;
        PlayerAttackEvent = null;
        PlayerHitEvent = null;
    }

    public static void MonsterDead(MonsterController _mc)
    {
        MonsterDeadEvent?.Invoke(_mc);
    }

    public static void PlayerAttack(PlayerController _pc, MonsterController _mc)
    {
        PlayerAttackEvent?.Invoke(_pc, _mc);
    }

    public static void PlayerHit(PlayerController _pc)
    {
        PlayerHitEvent?.Invoke(_pc);
    }
}
