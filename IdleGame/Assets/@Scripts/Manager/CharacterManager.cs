using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterManager
{
    //TODO : 현재 착용중인 캐릭터.
    public CharacterHolder[] Characters = new CharacterHolder[7];
    public PlayerController[] players = new PlayerController[7];
    public Dictionary<string, CharacterHolder> CharacterDic = new Dictionary<string, CharacterHolder>();


    public event Action<PlayerController> OnCharacterAdd;
    public event Action<PlayerController> OnCharacterRemove;

    public void GetCharacter(int _value, string _name)
    {
        Characters[_value] = Managers.GameM.gameData.Characters_Data[_name];
    }

    public void OnNotifyCharacter(PlayerController _pc)
    {
        OnCharacterAdd?.Invoke(_pc);
    }
}
