using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;

//TODO : Spawn함수를 여기서 사용할지 고민해보자
public class SpawnManager : MonoBehaviour, ITickable
{

    float spawnInterval = 2f;
    float spawnTimer = 0f;

    public bool isStop { get; set; } = false;
    MonsterController boss = null;
    public List<PlayerController> players;
    List<MonsterController> monsters;
    UI_GameScene scene = null;


    //TODO : 처음 시작할땐 플레이어 스폰이 되있어야될거같긴함
    public void Init()
    {
        Managers.StageM.readyEvent += OnReady;
        Managers.StageM.playEvent += OnPlay;
        Managers.StageM.bossEvent += OnBoss;
        Managers.StageM.clearEvent += OnClear;
        Managers.StageM.deadEvent += OnDead;

        scene = Managers.UIM.SceneUI as UI_GameScene;
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
    }

    #region 이벤트
    public void OnReady()
    {
        PlayerSpawn();
    }

    public void OnPlay()
    {
        players = Managers.ObjectM.pcList.ToList();
        spawnTimer = 0f;
        Managers.UpdateM.Register(this);
    }

    public void OnBoss()
    {
        StopSpawn();

        //보스전 잔몹들 삭제
        List<MonsterController> monsetrs = Managers.ObjectM.mcList.ToList();
        foreach (var monster in monsetrs)
        {
            Managers.ObjectM.DeSpawn(monster);
        }

        //TODO : 수정
        BossSet().Forget();
    }

    public async UniTask BossSet()
    {
        await UniTask.WaitForSeconds(2f);

        //TODO : 하드코딩 삭제
        boss = Managers.ObjectM.Spawn<MonsterController>(Vector3.zero, 10001);
        if(scene == null) scene = Managers.UIM.SceneUI as UI_GameScene;
        boss.OnMonsterInfoUpdate += scene.UpdateBossInfo;

        //players = Managers.ObjectM.pcList.ToList();

        Vector3 Pos = boss.transform.position;
        foreach (var player in players)
        {
            if (player.IsDead) continue;

            if (Vector3.Distance(Pos, player.transform.position) <= 2.0f)
            {
                player.transform.LookAt(Pos);
                player.KnockBack(3.0f, 0.3f).Forget();
            }
        }

        await UniTask.WaitForSeconds(1.5f);


        Managers.StageM.StateChange(Define.StageState.BossPlay);
    }

    //TODO : 사용하지 않으면 지우기
    public void OnClear()
    {
        ClearDelay().Forget();
        if (boss != null)
        {
            boss.OnMonsterInfoUpdate -= scene.UpdateBossInfo;
            boss = null;
        }
    }

    public void OnDead()
    {
        ClearDelay().Forget();

        if (boss != null)
        {
            boss.AnimatorChange(Define.CreatureState.Idle);
            boss.OnMonsterInfoUpdate -= scene.UpdateBossInfo;
            boss = null;
        }
    }

    async UniTask ClearDelay()
    {
        StopSpawn();
        await UniTask.WaitForSeconds(2.0f);

        await scene.AsyncFadeInOut(false);

        //List<MonsterController> monsetrs = Managers.ObjectM.mcList.ToList();
        //foreach (var monster in monsetrs)
        //{
        //    Managers.ObjectM.DeSpawn(monster);
        //}

        for (int i = Managers.ObjectM.mcList.Count -1; i >= 0; i--)
        {
            Managers.ObjectM.DeSpawn(Managers.ObjectM.mcList[i]);
        }

        for (int i = Managers.ObjectM.pcList.Count - 1; i >= 0; i--)
        {
            Managers.ObjectM.DeSpawn(Managers.ObjectM.pcList[i]);
        }

        players.Clear();
        Managers.ObjectM.mcList.Clear();
        await UniTask.WaitForSeconds(1.0f);

        Managers.StageM.StateChange(Define.StageState.Ready);
    }


    #endregion
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