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

    int spawnMaxCount;
    public int SpawnMaxCount
    {
        get => spawnMaxCount;
    }
    float spawnInterval;

    float spawnTime = 0f;

    public bool isStop { get; set; } = false;
    MonsterController boss = null;
    public List<PlayerController> players;
    List<MonsterController> monsters;
    UI_GameScene scene = null;
    int value = -1;


    //TODO : 처음 시작할땐 플레이어 스폰이 되있어야될거같긴함
    public void Init()
    {
        Managers.StageM.readyEvent += OnReady;
        Managers.StageM.playEvent += OnPlay;
        Managers.StageM.bossEvent += OnBoss;
        Managers.StageM.clearEvent += OnClear;
        Managers.StageM.deadEvent += OnDead;
        Managers.StageM.dungeonEvent += OnDungeon;
        Managers.StageM.dungeonClearEvent += OnDungeonClear;
        Managers.StageM.dungeonFailEvent += OnDungeonFail;

        scene = Managers.UIM.SceneUI as UI_GameScene;
    }






    #region 이벤트
    public void OnReady()
    {
        spawnMaxCount = Managers.DataM.StageDataDic[Managers.GameM.Stage].SpawnMaxCount;
        spawnInterval = Managers.DataM.StageDataDic[Managers.GameM.Stage].SpawnTimer;
        if (scene != null) scene.CheckTexts();
    }

    public void OnPlay(Define.StageState _state)
    {
        StartSpawn();
    }

    public void OnBoss()
    {
        StopSpawn();

        //보스전 잔몹들 삭제
        DeSpawnMonster();


        BossSet().Forget();
    }

    public async UniTask BossSet()
    {
        await UniTask.WaitForSeconds(2f);

        SpawnBoss(Managers.StageM.isDungeon);

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

    public async void OnDungeon(int _value)
    {
        value = _value;
        StopSpawn();


        if (value == 0)
        {
            spawnMaxCount = 30;
            spawnInterval = 3f;
        }
        else if (value == 1)
        {
            OnBoss();
        }

        await scene.AsyncFadeInOut(true);

    }

    public void OnDungeonClear(int _value)
    {
        value = -1;
        OnClear();
    }

    public void OnDungeonFail(int _value)
    {
        value = -1;
        OnDead();
    }


    async UniTask ClearDelay()
    {
        StopSpawn();
        await UniTask.WaitForSeconds(2.0f);

        await scene.AsyncFadeInOut(false);

        DeSpawnMonster();

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

    public void StartSpawn()
    {
        Managers.UpdateM.Register(this);
        spawnTime = 0f;
    }
    public void StopSpawn()
    {
        Managers.UpdateM.UnRegister(this);
    }

    void SpawnMonster(int _count)
    {

        int value = _count - Managers.ObjectM.mcList.Count;
        for (int i = 0; i < value; i++)
        {
            //TODO: 여기 스테이지 마다 생성되는 몬스터가 달라진다면 하드코딩 없애기(10000) 이거 
            Managers.ObjectM.Spawn<MonsterController>(Utils.CreateMonsterSpawnPoint(), 10000);
        }
    }

    void SpawnBoss(bool _isDungeon)
    {
        if (_isDungeon)
        {

            boss = Managers.ObjectM.Spawn<MonsterController>(Vector3.zero, 12000);
            if (scene == null) scene = Managers.UIM.SceneUI as UI_GameScene;
            boss.OnMonsterInfoUpdate += scene.UpdateBossInfo;
        }
        else
        {
            boss = Managers.ObjectM.Spawn<MonsterController>(Vector3.zero, 10001);
            if (scene == null) scene = Managers.UIM.SceneUI as UI_GameScene;
            boss.OnMonsterInfoUpdate += scene.UpdateBossInfo;
        }

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
    }

    void DeSpawnMonster()
    {
        var monsters = Managers.ObjectM.mcList.ToList();
        foreach (var monster in monsters)
        {
            monster.ClearChildVFXs();
            Managers.ObjectM.DeSpawn(monster);
        }
        Managers.ObjectM.mcList.Clear();
    }

    public void Tick(float _deltaTime)
    {
        spawnTime -= _deltaTime;
        if (spawnTime <= 0f)
        {
            spawnTime = spawnInterval;
            SpawnMonster(spawnMaxCount);
        }
    }
}