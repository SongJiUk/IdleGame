using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDesignData", menuName = "LevelDesign/Level Design Data")]
public class LevelDesign : ScriptableObject
{
    public LevelData levelData;

}

[Serializable]
public class LevelData
{
    public int currentLevel;

    [Range(0f, 10.0f)]
    public float Player_Attack, Player_Hp, Player_Exp, Player_MaxExp, Player_Money;

    [Space(20f)]
    [Header("Base Value")]
    public float Base_Attack;
    public float Base_Hp;
    public float Base_Exp;
    public float Base_MaxExp;
    public float Base_Money;

}
