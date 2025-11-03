using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour, ITickable
{

    float spawnInterval = 5f;
    float spawnTimer = 0f;

    public bool isStop { get; set; } = false;

    public void StartSpawn()
    {   //TODO : 이거 여기서 생성할지 생각해보자
        for (int i = 1; i <= 1; i++)
        {
            Managers.ObjectM.Spawn<PlayerController>(Vector3.zero, i);
        }

        Managers.UpdateM.Register(this);
    }

    public void StopSpawn()
    {
        Managers.UpdateM.UnRegister(this);
    }
    void SpawnMonster()
    {
        //TODO : 몬스터 몇마리 생성할지 정하고 하드코딩 없애기(데이터도 어떻게불러올지 생각해보고 하드코딩 수정)
        for (int i = 0; i < 5; i++)
        {
            Managers.ObjectM.Spawn<MonsterController>(Utils.CreateMonsterSpawnPoint(), 10000);
        }
    }
    public void Tick(float _deltaTime)
    {
        spawnTimer -= _deltaTime;

        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnInterval;
            //TODO : spawnStart
            SpawnMonster();
        }

    }
}