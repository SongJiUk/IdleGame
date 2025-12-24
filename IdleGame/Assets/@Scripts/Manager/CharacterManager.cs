using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CharacterManager
{
    //TODO : 현재 착용중인 캐릭터.
    public CharacterHolder[] Characters = new CharacterHolder[7];
    public PlayerController[] players = new PlayerController[7];
    public Dictionary<string, CharacterHolder> CharacterDic = new Dictionary<string, CharacterHolder>();


    public event Action<PlayerController> OnCharacterAdd;
    public event Action<PlayerController> OnCharacterRemove;

    public void SetCharacter(int _value, string _name)
    {
        Characters[_value] = Managers.GameM.gameData.Characters_Data[_name];
    }

    public void GetCharacter(string _name)
    {
        for (int i = 0; i < Characters.Length; i++)
        {
            if (Characters[i] == null) continue;

            if (Characters[i].data.Name == _name) Characters[i] = null;
        }
    }

    public void OnNotifyCharacter(PlayerController _pc)
    {
        OnCharacterAdd?.Invoke(_pc);
    }
}
