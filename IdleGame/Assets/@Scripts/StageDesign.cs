using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDesignData", menuName = "StageDesign/Stage Design Data")]
public class StageDesign : ScriptableObject
{
    public StageData stageData;

    public float CalculatedValue(float _baseValue, int _level, float _value)
    {
        return _baseValue * Mathf.Pow(_level, _value);
    }
}

[Serializable]
public class StageData
{
    public int currentStage;
    [Range(0f, 10.0f)]
    public float Monster_Attack, Monster_Hp, Monster_Gold;

    [Space(20f)]
    [Header("Base Value")]
    public float Base_Attack;
    public float Base_Hp;
    public float Base_Gold;
}
