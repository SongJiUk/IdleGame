using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageDesign))]
public class StageDesignEditor : Editor
{
    StageDesign stageDesign = null;
    public override void OnInspectorGUI()
    {

        stageDesign = (StageDesign)target;

        EditorGUILayout.LabelField("Stage Design", EditorStyles.boldLabel);
        StageData data = stageDesign.stageData;

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("Monster Attack : " + Utils.CalculatedValue(data.Base_Attack, data.currentStage, data.Monster_Attack));
        EditorGUILayout.LabelField("Monster HP : " + Utils.CalculatedValue(data.Base_Hp, data.currentStage, data.Monster_Hp));
        EditorGUILayout.LabelField("Monster Gold : " + Utils.CalculatedValue(data.Base_Gold, data.currentStage, data.Monster_Gold));
        EditorGUILayout.Space(20);
        DrawDefaultInspector();
    }
}
