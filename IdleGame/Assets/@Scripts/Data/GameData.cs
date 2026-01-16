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

public class ItemHolder
{
    public Data.ItemData data;
    public Holder holder;
}

[Serializable]
public class Holder
{
    public int Level = 1;
    public int Count = 0;
}

public class GameData
{
    public double damage;
    public double hp;
    public double gold;
    public double dia;
    public int level = 1;
    public int upgradeCount;
    public int questCount;
    public double exp;
    public int stage = 1;

    public float[] buff_Timers = { 0.0f, 0.0f, 0.0f };
    public float fast_Timer = 0.0f;
    public int buff_Level, buff_count;

    public int heroSummonCount = 0;
    public int heroConfirmedLegendaryCount = 0;


    public int relicsSummonCount = 0;
    public int relicsConfirmedLegendaryCount = 0;
    public string startDate;
    public string endDate;

    public int[] DungeonKey = { 2, 2 };
    public int[] DungeonKeyAssets = { 0, 0 };
    public int[] DungeonClearLevel = { 0, 0 };
    //플레이어가 가지고 있는 데이터 저장
    public Dictionary<string, CharacterHolder> Characters_Data = new Dictionary<string, CharacterHolder>();
    public Dictionary<string, Holder> Character_Holder = new Dictionary<string, Holder>();

    public Dictionary<string, ItemHolder> Item_Data = new Dictionary<string, ItemHolder>();
    public Dictionary<string, Holder> Item_Holder = new Dictionary<string, Holder>();
    public Data.ItemData[] Items = new Data.ItemData[7];


    public void Init()
    {
        SetCharacter();
        SetItem();
    }

    private void SetCharacter()
    {
        var datas = Managers.DataM.CreatureDataDic.Values;

        foreach (var data in datas)
        {
            if (data.ObjectType != Define.ObjectType.Player) continue;

            var character = new CharacterHolder();
            character.data = data;

            Holder holder = new Holder();

            if (Character_Holder.ContainsKey(data.Name))
            {
                holder = Character_Holder[data.Name];
            }
            else
            {
                Character_Holder.Add(data.Name, holder);
            }
            character.holder = holder;
            Characters_Data[data.Name] = character;
        }
    }

    private void SetItem()
    {
        var datas = Managers.DataM.ItemDataDic.Values;

        foreach (var data in datas)
        {
            string targetName = data.Name.Trim();

            if (!Item_Holder.TryGetValue(targetName, out Holder holder))
            {
                holder = new Holder();
                Item_Holder.Add(data.Name, holder);
                Debug.Log($"[신규 아이템 생성] {targetName}");
            }
            else
            {
                Debug.Log($"[서버 데이터 유지] {targetName}: {holder.Count}개");
            }

            var item = new ItemHolder { data = data, holder = holder };
            Item_Data[targetName] = item;
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

    public Data.ItemData GetGradeRelic(Define.ItemGrade _grade)
    {
        List<Data.ItemData> holder = new List<Data.ItemData>();
        foreach (var data in Item_Data)
        {
            if (data.Value.data.ItemType == Define.ItemType.Equipment)
            {
                if (data.Value.data.ItemGrade == _grade)
                {
                    holder.Add(data.Value.data);
                }
            }
        }

        return holder[UnityEngine.Random.Range(0, holder.Count)];
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

    public void ChangeRelicInfo(Data.ItemData _data)
    {
        if (!Item_Holder.TryGetValue(_data.Name, out Holder holder))
        {
            holder = new Holder();
            Item_Holder.Add(_data.Name, holder);
        }


        var item = new ItemHolder();
        item.data = _data;
        item.holder = holder;
        Item_Data[_data.Name] = item;
    }
}
