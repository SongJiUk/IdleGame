using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum UIEvent
    {
        None,
        Click,
        Pressed,
        PointerDown,
        PointerUp,
        Drag,
        BeginDrag,
        EndDrag
    }
    public enum CurrencyType
    {
        Default,
        SI,
    }

    public enum CreatureState
    {
        Idle,
        Move,
        Attack,
        Hit,
        Dead
    }

    public enum ObjectType
    {
        None,
        Player,
        Monster,
        Boss
    }

    public enum WeaponAbilityType
    {
        None,
        Good,
        Rare,
        Unique,
        Legendary
    }

    public enum SceneType
    {
        None,
        TitleScene,
        GameScene
    }
}
