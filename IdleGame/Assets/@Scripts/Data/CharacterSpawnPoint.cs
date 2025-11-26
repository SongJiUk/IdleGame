using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterSpawnPoint : MonoBehaviour
{
    public List<Transform> SpawnTr = new List<Transform>();

    private void Awake()
    {
        Managers.StageM.readyEvent += OnReady;
    }

    private void Start()
    {
        //TODO : 드래그앤 드랍인데 굳이 이게 필요한가..?
        for(int i =0; i<transform.childCount; i++)
        {
            SpawnTr[i] = transform.GetChild(i);
        }
    }

    public void OnReady()
    {
        PlayerSpawn();
    }

    public void PlayerSpawn()
    {

        for (int i = 1; i <= 2; i++)
        {
            Vector3 spawnPos = Vector3.zero;
            if (i != 1)
            {
                spawnPos = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
            }
            Managers.ObjectM.Spawn<PlayerController>(spawnPos, i);
        }

        Managers.SpawnM.players = Managers.ObjectM.pcList.ToList();
    }
}
