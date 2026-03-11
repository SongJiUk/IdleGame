using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterSpawnPoint : MonoBehaviour
{
    public List<Transform> SpawnTr = new List<Transform>();
    //public PlayerController[] players = new PlayerController[7];

    public GameObject[] Maps;
    private void Awake()
    {
        Managers.StageM.readyEvent += OnReady;
        Managers.StageM.dungeonEvent += OnDungeon;
    }

    public void OnReady()
    {
        for (int i = 0; i < Maps.Length; i++) Maps[i].SetActive(false);

        //Managers.CharacterM.ClearAllPlayers();
        PlayerSpawn();
    }

    public void OnDungeon(int _dungeonData)
    {
        int value = _dungeonData == 70001 ? 1 : 0;

        Maps[value].SetActive(true);
    }
    public void PlayerSpawn()
    {

        for (int i = 0; i < Managers.CharacterM.Characters.Length; i++)
        {
            var charData = Managers.CharacterM.Characters[i];
            if (charData != null)
            {
                Debug.Log($"[DEBUG_SPAWN] 스폰 전 데이터 확인: 인덱스 {i} - 영웅 ID: {charData.data.DataID}");
            }
        }

        int mainIndex = 0;

        if (Managers.CharacterM.players[mainIndex] == null)
        {
            Vector3 mainPos = SpawnTr[mainIndex].position;
            PlayerController mainPC = Managers.ObjectM.Spawn<PlayerController>(mainPos, 1);
            mainPC.index = mainIndex;
            Managers.CharacterM.AddPlayerReference(mainIndex, mainPC);

        }

        for (int i = 1; i < Managers.CharacterM.Characters.Length; i++)
        {
            if (Managers.CharacterM.Characters[i] == null) continue;

            if (Managers.CharacterM.players[i] != null)
            {
                Managers.CharacterM.players[i].transform.position = SpawnTr[i].position;
                continue;
            }

            int dataID = Managers.CharacterM.Characters[i].data.DataID;
            Vector3 spawnPos = SpawnTr[i].position;
            PlayerController pc = Managers.ObjectM.Spawn<PlayerController>(spawnPos, dataID);
            pc.index = i;
            Managers.CharacterM.AddPlayerReference(i, pc);
        }
    }
    private void OnDestroy()
    {
        if (Managers.StageM != null)
        {
            Managers.StageM.readyEvent -= OnReady;
            Managers.StageM.dungeonEvent -= OnDungeon;
        }
    }


    //TODO: 메인캐릭터는 클레릭 고정임(지금은 테스트한다고 바꿔놓은거.)
    //Managers.CharacterM.players[Managers.CharacterM.Characters.Length - 1] = Managers.ObjectM.Spawn<PlayerController>(pos, 3);
    //for (int i = 0; i < Managers.CharacterM.Characters.Length; i++)
    //{

    //    if (Managers.CharacterM.Characters[i] == null) continue;
    //    int dataID = Managers.CharacterM.Characters[i].data.DataID;

    //    Vector3 spawnPos = SpawnTr[i].position;
    //    PlayerController pc = Managers.ObjectM.Spawn<PlayerController>(spawnPos, dataID);
    //    pc.index = i;
    //    Managers.CharacterM.players[i] = pc;
    //    if (pc != null)
    //    {
    //        Managers.CharacterM.OnNotifyCharacter(pc);
    //    }
    //}

    //Managers.SpawnM.players = Managers.ObjectM.pcList.ToList();
}