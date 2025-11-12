using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public static string AnimState = "State";

    public enum CharacterType
    {
        Cleric = 1,
        Barbarian,
        Berserker,
        Elementalist_B,
        Elementalist_W,
        Hunter,
        Spearman,
    }
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
        Boss,
    }

    public enum ItemGrade
    {
        Common,
        UnCommon,
        Rare,
        Unique,
        Legendary
    }

    public enum CharacterGrade
    {
        Common,
        UnCommon,
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
    public enum UILayerIndex
    {
        Coin,
        DamageFont,
        ItemRect,
    }

    public enum AttackType
    {
        Melee,
        Range
    }

    public enum StageState
    {
        Ready,
        Play,
        Boss,
        BossPlay,
        Clear,
        Dead
    }
}
