using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDesignData", menuName = "LevelDesign/Level Design Data")]
public class LevelDesign : ScriptableObject
{
    public int currentLevel;
    public int currentStage;

    public LevelData levelData;
    [Space(20)]
    public StageData stageData;

}

[Serializable]
public class LevelData
{
    [Range(0f, 10.0f)]
    public float Player_Damage, Player_Hp, Player_Exp, Player_MaxExp, Player_Gold;

    [Space(20f)]
    [Header("Base Value")]
    public int Base_Damage;
    public int Base_Hp;
    public int Base_Exp;
    public int Base_MaxExp;
    public int Base_Gold;

    public double Damage(double _baseDamage) => Utils.CalculatedValue((float)_baseDamage, Managers.GameM.level, Player_Damage);
    public double HP(double _baseHp) => Utils.CalculatedValue((float)_baseHp, Managers.GameM.level, Player_Hp);
    public double Exp() => Utils.CalculatedValue(Base_Exp, Managers.GameM.level, Player_Exp);
    public double MaxExp() => Utils.CalculatedValue(Base_MaxExp, Managers.GameM.level, Player_MaxExp);
    public double Gold() => Utils.CalculatedValue(Base_Gold, Managers.GameM.level, Player_Gold);
}


[Serializable]
public class StageData
{
    [Range(0f, 10.0f)]
    public float Monster_Damage, Monster_Hp, Monster_Gold;

    [Space(20f)]
    [Header("Base Value")]
    public int Base_Damage;
    public int Base_Hp;
    public int Base_Gold;

    public double Damage(double _baseDamage) => Utils.CalculatedValue((float)_baseDamage, Managers.GameM.stage, Monster_Damage);
    public double HP(double _baseHp) => Utils.CalculatedValue((float)_baseHp, Managers.GameM.stage, Monster_Hp);
    public double Gold() => Utils.CalculatedValue(Base_Gold, Managers.GameM.stage, Monster_Gold);
}
