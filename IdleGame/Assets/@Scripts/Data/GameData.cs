using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;


public class CharacterHolder
{
    public Data.CreatureData data;
    public Holder holder;
}

public class Holder
{
    public int Level;
    public int Count;
}

public class GameData
{
    public double gold;
    public int level = 1;
    public double exp;
    public int stage = 1;

    public float[] buff_Timers = { 0.0f, 0.0f, 0.0f };
    public float fast_Timer = 0.0f;
    public int buff_Level, buff_count;

    //플레이어가 가지고 있는 데이터 저장
    public Dictionary<string, CharacterHolder> Characters_Data = new Dictionary<string, CharacterHolder>();
    public Dictionary<string, Holder> Character_Holder = new Dictionary<string, Holder>();

    public void Init()
    {
        SetCharacter();
    }

    private void SetCharacter()
    {
        var datas = Managers.DataM.CreatureDataDic.Values;
        foreach (var data in datas)
        {
            if (data.Type != Define.ObjectType.Player) continue;

            //TODO : 추후엔 파이어 베이스 이용
            var character = new CharacterHolder();
            character.data = data;
            Holder holder = new Holder();
            if (Character_Holder.ContainsKey(data.Name))
            {
                holder = Character_Holder[data.Name];
                Debug.Log($"{data.Name} :  {holder.Count} : {holder.Level}");
            }
            else
            {
                Character_Holder.Add(data.Name, holder);
            }
            
            character.holder = holder;
            Characters_Data.Add(data.Name, character);

        }
    }
}
