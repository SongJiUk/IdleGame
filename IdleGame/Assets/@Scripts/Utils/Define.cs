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
        EndDrag,
        OnPointerExit
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
        Dead,
        Skill

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

    public enum CircleButtons
    {
        Circle1Button,
        Circle2Button,
        Circle3Button,
        Circle4Button,
        Circle5Button,
        Circle6Button
    }

    public enum CreatureType
    {
        None,
        Archer,
        Cleric,
        Assassin,
        Hammer,
        Knight,
        SpearMan,
        TwoHandSword,
        Mage_M,
        Mage_W,
        Monster,
        Boss,
    }

    public enum SkillEffectType
    {
        BuffEffect,
        DamageEffect
    }

    public enum BuffType
    {
        AttackUp,
        GoldUp,
        CriticalUp
    }

    public enum GachaType
    {
        HeroGacha,
        RelicGacha
    }
}
