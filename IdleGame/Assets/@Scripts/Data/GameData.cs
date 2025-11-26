using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;


public class CharacterHolder
{
    public Data.CreatureData data;
    public bool isMplayer;
    public int Level;
    public int count;
}

public class GameData
{
    public double gold;
    public int level;
    public double exp;
    public int stage;
    //플레이어가 가지고 있는 데이터 저장
    public Dictionary<string, CharacterHolder> DataCharacter = new Dictionary<string, CharacterHolder>();

    public void Init()
    {
        SetCharacter();
    }

    private void SetCharacter()
    {
        var datas = Managers.DataM.CreatureDataDic.Values;
        foreach(var data in datas)
        {
            if (data.Type != Define.ObjectType.Player) continue;

            //TODO : 추후엔 파이어 베이스 이용
            var character = new CharacterHolder();
            character.data = data;
            character.Level = 1;
            character.count = 1;
            DataCharacter.Add(data.Name, character);

        }
    }
}
