using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CharacterManager
{
    //TODO : 현재 착용중인 캐릭터(렌더텍스쳐에서만 보여지는것)
    public CharacterHolder[] Characters = new CharacterHolder[7];
    public PlayerController[] players = new PlayerController[7];
    //public Dictionary<string, CharacterHolder> CharacterDic = new Dictionary<string, CharacterHolder>();


    public event Action<PlayerController> OnCharacterAdd;
    //public event Action<PlayerController> OnCharacterRemove;

    public List<PlayerController> AlivePlayers => players.Where(p => p != null && !p.IsDead).ToList();
    public void AddPlayerReference(int _index, PlayerController _pc)
    {
        players[_index] = _pc;
        OnCharacterAdd?.Invoke(_pc);
    }

    public void ClearAllPlayers()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = null;
        }
    }
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

    public void LevelUpPlayer()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null) continue;
            players[i].SetStat();
            players[i].OnPlayerDataUpdate?.Invoke(players[i]);
        }
    }

    public void Clear()
    {
        ClearAllPlayers();
        OnCharacterAdd = null;
        Debug.Log("[CharacterManager] 캐릭터 및 플레이어 정보 초기화 완료");
    }
}
