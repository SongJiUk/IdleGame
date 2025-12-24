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

    public double Damage() => Utils.CalculatedValue(Base_Damage, Managers.GameM.Level, Player_Damage);
    public double HP() => Utils.CalculatedValue(Base_Hp, Managers.GameM.Level, Player_Hp);
    public double Exp() => Utils.CalculatedValue(Base_Exp, Managers.GameM.Level, Player_Exp);
    public double MaxExp() => Utils.CalculatedValue(Base_MaxExp, Managers.GameM.Level, Player_MaxExp);
    public double Gold() => Utils.CalculatedValue(Base_Gold, Managers.GameM.Level, Player_Gold);
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

    public double Damage(double _baseDamage, int _stage) => Utils.CalculatedValue((float)_baseDamage, _stage, Monster_Damage);
    public double HP(double _baseHp, int _stage) => Utils.CalculatedValue((float)_baseHp, _stage, Monster_Hp);
    public double Gold(int _stage) => Utils.CalculatedValue(Base_Gold, _stage, Monster_Gold);
}
