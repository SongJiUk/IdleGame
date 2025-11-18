using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Globalization;

[CustomEditor(typeof(LevelDesign))]
public class LevelDesignEditor : Editor
{
    LevelDesign design = null;
    public override void OnInspectorGUI()
    {
        design = (LevelDesign)target;

        EditorGUILayout.LabelField("Level Design", EditorStyles.boldLabel);
        LevelData data = design.levelData;
        StageData stageData = design.stageData;
        EditorGUILayout.LabelField("캐릭터 레벨 그래프", EditorStyles.boldLabel);

        DrawGraph(data, stageData);
        EditorGUILayout.Space();

        DrawDefaultInspector();
    }


    private void DrawGraph(LevelData _data, StageData _stageData)
    {
        Rect rect = GUILayoutUtility.GetRect(200, 100);
        Handles.DrawSolidRectangleWithOutline(rect, Color.black, Color.white);

        Vector3[] curvePoint_Attack = GraphDisign(rect, _data.Player_Damage);
        Handles.color = Color.green;
        Handles.DrawAAPolyLine(3, curvePoint_Attack);

        Vector3[] curvePoint_Hp = GraphDisign(rect, _data.Player_Hp);
        Handles.color = Color.red;
        Handles.DrawAAPolyLine(3, curvePoint_Hp);

        Vector3[] curvePoint_MaxExp = GraphDisign(rect, _data.Player_MaxExp);
        Handles.color = Color.blue;
        Handles.DrawAAPolyLine(3, curvePoint_MaxExp);

        Vector3[] curvePoint_Money = GraphDisign(rect, _data.Player_Gold);
        Handles.color = Color.yellow;
        Handles.DrawAAPolyLine(3, curvePoint_Money);

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("레벨 데이터", EditorStyles.boldLabel);

        GetColorGUI("Attack", Utils.ToCurrencyString(Utils.CalculatedValue(_data.Base_Damage, design.currentLevel, _data.Player_Damage)), Color.green);
        EditorGUILayout.LabelField("HP : " + Utils.CalculatedValue(_data.Base_Hp, design.currentLevel, _data.Player_Hp));
        GetColorGUI("HP", Utils.ToCurrencyString(Utils.CalculatedValue(_data.Base_Hp, design.currentLevel, _data.Player_Hp)), Color.red);
        GetColorGUI("MaxExp", Utils.ToCurrencyString(Utils.CalculatedValue(_data.Base_MaxExp, design.currentLevel, _data.Player_MaxExp)), Color.blue);
        GetColorGUI("Gold", Utils.ToCurrencyString(Utils.CalculatedValue(_data.Base_Gold, design.currentLevel, _data.Player_Gold)), Color.yellow);
        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("스테이지 데이터", EditorStyles.boldLabel);
        GetColorGUI("Monster Attack : ", Utils.ToCurrencyString(Utils.CalculatedValue(_stageData.Base_Damage, design.currentStage, _stageData.Monster_Damage)), Color.green);
        GetColorGUI("Monster HP : ", Utils.ToCurrencyString(Utils.CalculatedValue(_stageData.Base_Hp, design.currentStage, _stageData.Monster_Hp)), Color.green);
        GetColorGUI("Monster Gold : ", Utils.ToCurrencyString(Utils.CalculatedValue(_stageData.Base_Gold, design.currentStage, _stageData.Monster_Gold)), Color.green);
    }


    void GetColorGUI(string _baseTemp, string _dataTemp, Color _color)
    {
        GUIStyle colorLabel = new GUIStyle(EditorStyles.label);
        colorLabel.normal.textColor = _color;

        EditorGUILayout.LabelField(_baseTemp + " : " + _dataTemp, colorLabel);
    }

    private Vector3[] GraphDisign(Rect _rect, float _data)
    {
        Vector3[] curvePoint = new Vector3[100];
        for (int i = 0; i < 100; i++)
        {
            float t = i / 99.0f;
            float curveValue = Mathf.Pow(t, _data);
            curvePoint[i] = new Vector3(
                _rect.x + t * _rect.width,
                _rect.y + _rect.height - curveValue * _rect.height,
                0);
        }

        return curvePoint;
    }
}
