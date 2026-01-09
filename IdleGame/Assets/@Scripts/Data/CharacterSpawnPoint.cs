using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterSpawnPoint : MonoBehaviour
{
    public List<Transform> SpawnTr = new List<Transform>();
    public PlayerController[] players = new PlayerController[7];

    public GameObject[] Maps;
    private void Awake()
    {
        Managers.StageM.readyEvent += OnReady;
        Managers.StageM.dungeonEvent += OnDungeon;
    }

    private void Start()
    {
        //TODO : 드래그앤 드랍인데 굳이 이게 필요한가..?
        // if (SpawnTr == null)
        // {
        //     for (int i = 0; i < transform.childCount; i++)
        //     {
        //         SpawnTr[i] = transform.GetChild(i);
        //     }
        // }

    }

    public void OnReady()
    {
        for (int i = 0; i < Maps.Length; i++) Maps[i].SetActive(false);
        PlayerSpawn();
    }

    public void OnDungeon(int _dungeonData)
    {
        int value = 0;
        if (_dungeonData == 70001) value = 1;

        Maps[value].SetActive(true);
    }
    public void PlayerSpawn()
    {
        Vector3 pos = Vector3.zero;
        //TODO: 메인캐릭터는 클레릭 고정임(지금은 테스트한다고 바꿔놓은거.)
        Managers.CharacterM.players[0] = Managers.ObjectM.Spawn<PlayerController>(pos, 3);
        for (int i = 1; i < Managers.CharacterM.Characters.Length; i++)
        {

            if (Managers.CharacterM.Characters[i] == null) continue;
            int dataID = Managers.CharacterM.Characters[i].data.DataID;

            Vector3 spawnPos = SpawnTr[i].position;
            PlayerController pc = Managers.ObjectM.Spawn<PlayerController>(spawnPos, dataID);
            pc.index = i;
            Managers.CharacterM.players[i] = pc;
            if (pc != null)
            {
                Managers.CharacterM.OnNotifyCharacter(pc);
            }
        }

        Managers.SpawnM.players = Managers.ObjectM.pcList.ToList();
    }
}
