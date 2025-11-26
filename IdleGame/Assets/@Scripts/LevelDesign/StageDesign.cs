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

