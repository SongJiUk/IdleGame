using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterSpawnPoint : MonoBehaviour
{
    public List<Transform> SpawnTr = new List<Transform>();
    public PlayerController[] players = new PlayerController[7];
    private void Awake()
    {
        Managers.StageM.readyEvent += OnReady;
    }

    private void Start()
    {
        //TODO : 드래그앤 드랍인데 굳이 이게 필요한가..?
        if (SpawnTr == null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                SpawnTr[i] = transform.GetChild(i);
            }
        }

    }

    public void OnReady()
    {
        PlayerSpawn();
    }

    public void PlayerSpawn()
    {
        // TODO : 이거 초기값이 이건거고, 데이터 저장 되면 수정해야됌(메인캐릭터 바꾸려고 할거면)
        Vector3 pos = Vector3.zero;
        Managers.CharacterM.players[0] = Managers.ObjectM.Spawn<PlayerController>(pos, 6);
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
