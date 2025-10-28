using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Globalization;

[CustomEditor(typeof(LevelDesign))]
public class LevelDesignEditor : Editor
{
    LevelDesign levelDesign = null;
    public override void OnInspectorGUI()
    {
        levelDesign = (LevelDesign)target;

        EditorGUILayout.LabelField("Level Design", EditorStyles.boldLabel);
        LevelData data = levelDesign.levelData;

        EditorGUILayout.LabelField("캐릭터 레벨 그래프", EditorStyles.boldLabel);

        DrawGraph(data);
        EditorGUILayout.Space();

        DrawDefaultInspector();
    }


    private void DrawGraph(LevelData _data)
    {
        Rect rect = GUILayoutUtility.GetRect(200, 100);
        Handles.DrawSolidRectangleWithOutline(rect, Color.black, Color.white);

        Vector3[] curvePoint_Attack = GraphDisign(rect, _data.Player_Attack);
        Handles.color = Color.green;
        Handles.DrawAAPolyLine(3, curvePoint_Attack);

        Vector3[] curvePoint_Hp = GraphDisign(rect, _data.Player_Hp);
        Handles.color = Color.red;
        Handles.DrawAAPolyLine(3, curvePoint_Hp);

        Vector3[] curvePoint_MaxExp = GraphDisign(rect, _data.Player_MaxExp);
        Handles.color = Color.blue;
        Handles.DrawAAPolyLine(3, curvePoint_MaxExp);

        Vector3[] curvePoint_Money = GraphDisign(rect, _data.Player_Money);
        Handles.color = Color.yellow;
        Handles.DrawAAPolyLine(3, curvePoint_Money);

        EditorGUILayout.Space(20);

        GetColorGUI("Attack", Utils.ToCurrencyString(Utils.CalculatedValue(_data.Base_Attack, _data.currentLevel, _data.Player_Attack)), Color.green);
        EditorGUILayout.LabelField("HP : " + Utils.CalculatedValue(_data.Base_Hp, _data.currentLevel, _data.Player_Hp));
        GetColorGUI("HP", Utils.ToCurrencyString(Utils.CalculatedValue(_data.Base_Hp, _data.currentLevel, _data.Player_Hp)), Color.red);
        GetColorGUI("MaxExp", Utils.ToCurrencyString(Utils.CalculatedValue(_data.Base_MaxExp, _data.currentLevel, _data.Player_MaxExp)), Color.blue);
        GetColorGUI("Gold", Utils.ToCurrencyString(Utils.CalculatedValue(_data.Base_Money, _data.currentLevel, _data.Player_Money)), Color.yellow);
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
