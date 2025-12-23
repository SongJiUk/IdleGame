using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Newtonsoft.Json;

public class CharacterHolder
{
    public Data.CreatureData data;
    public Holder holder;
}

[Serializable]
public class Holder
{
    public int level;
    public int count;

    [JsonIgnore]
    public int Level { get => level; set => level = value; }
    public int Count { get => count; set => count = value; }
   
}

public class GameData
{
    public GameData()
    {
        Debug.Log($"[GameData 생성됨] 호출 스택: {StackTraceUtility.ExtractStackTrace()}");
    }

    public double gold;
    public double dia;
    public int level = 1;
    public double exp;
    public int stage = 1;

    public float[] buff_Timers = { 0.0f, 0.0f, 0.0f };
    public float fast_Timer = 0.0f;
    public int buff_Level, buff_count;

    public int summonCount = 0;
    public int confirmedLegendaryCount = 0;
    //플레이어가 가지고 있는 데이터 저장
    public Dictionary<string, CharacterHolder> Characters_Data = new Dictionary<string, CharacterHolder>();
    public Dictionary<string, Holder> Character_Holder = new Dictionary<string, Holder>();


    public void Init()
    {
        SetCharacter();
    }
   
    public void ChangeCharacterInfo(Data.CreatureData _data)
    {
        if (!Character_Holder.TryGetValue(_data.Name, out Holder holder))
        {
            holder = new Holder();
            Character_Holder.Add(_data.Name, holder);
        }


        var character = new CharacterHolder();
        character.data = _data;
        character.holder = holder;
        Characters_Data[_data.Name] = character;
    }

    private void SetCharacter()
    {
        var datas = Managers.DataM.CreatureDataDic.Values;
        foreach (var data in datas)
        {
            if (data.Type != Define.ObjectType.Player) continue;

            if (Character_Holder.TryGetValue(data.Name, out Holder currentHolder))
            {
                Debug.Log($"[로드 확인] {data.Name} : 개수 {currentHolder.Count}");
            }
            else
            {
                currentHolder = new Holder();
                Character_Holder.Add(data.Name, currentHolder);
                Debug.Log($"[신규 생성] {data.Name} 데이터가 없어 새로 생성합니다.");
            }

            var character = new CharacterHolder();
            character.data = data;
            character.holder = currentHolder;
            Characters_Data[data.Name] = character;
        }
    }

    public Data.CreatureData GetGradeCharacter(Define.CharacterGrade _grade)
    {
        List<Data.CreatureData> holder = new List<Data.CreatureData>();
        foreach (var data in Characters_Data)
        {
            if (data.Value.data.CharacterGrade == _grade)
            {
                holder.Add(data.Value.data);
            }
        }

        return holder[UnityEngine.Random.Range(0, holder.Count)];
    }
}
