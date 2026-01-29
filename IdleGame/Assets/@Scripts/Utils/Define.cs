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
        DungeonBoss
    }


    public enum Grade
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

    public enum GoodsType
    {
        Gold,
        Dia
    }
    public enum UILayerIndex
    {
        Goods,
        DamageFont,
        ItemRect,
        Speech
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
        Dead,
        Dungeon,
        DungeonClear,
        DungeonFail,
        DungeonOut
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
        Enchantress,
        Monster,
        Boss,
        DungeonBoss,
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

    public enum ItemType
    {
        Currency,
        Consumable,
        Equipment,
        Other
    }

    public enum QuestType
    {
        Monster,
        Stage,
        Gold,
        Dia,
        Upgrade,
        Hero,
        Relic
    }

    public enum DungeonType
    {
        TreasureTrove,
        GoldDungeon
    }

    public enum Status_Holder
    {
        Damage, //공격력 
        Hp, //체력 
        Money, //골드 획득량
        Item, //아이템 드랍률
        Skill,// 스킬 쿨타임
        AttackSpeed, //공격속도
        CriticalP, //크리티컬 확률
        CriticalD // 크리티컬 데미지
    }

    public enum IAP
    {
        removeads,
        dia300,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        Max
    }
    public enum MissionType
    {
        Weekly,
        Daily
    }

    public enum MissionTarget
    {
        DailyAttendance,
        StageClear,
        LevelUp,
        HeroGacha,
        RelicGacha,
        Dungeon,
        DungeonClear,
        WatchingAD,
        Smelting
    }
}
