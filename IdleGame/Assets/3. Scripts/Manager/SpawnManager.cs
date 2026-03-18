using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
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
    //public List<PlayerController> players = new List<PlayerController>();
    //List<MonsterController> monsters;
    UI_GameScene scene = null;
    int dungeonDataID;
    bool isDelay = true;
    bool isSpawning = false;


    //TODO : 처음 시작할땐 플레이어 스폰이 되있어야될거같긴함
    public void Init()
    {
        // 중복 구독 방지: 먼저 해제 후 재구독
        Managers.StageM.readyEvent -= OnReady;
        Managers.StageM.playEvent -= OnPlay;
        Managers.StageM.bossEvent -= OnBoss;
        Managers.StageM.clearEvent -= OnClear;
        Managers.StageM.deadEvent -= OnDead;
        Managers.StageM.dungeonEvent -= OnDungeon;
        Managers.StageM.dungeonClearEvent -= OnDungeonClear;
        Managers.StageM.dungeonFailEvent -= OnDungeonFail;
        Managers.StageM.dungeonOutEvent -= OnDungeonOut;

        Managers.StageM.readyEvent += OnReady;
        Managers.StageM.playEvent += OnPlay;
        Managers.StageM.bossEvent += OnBoss;
        Managers.StageM.clearEvent += OnClear;
        Managers.StageM.deadEvent += OnDead;
        Managers.StageM.dungeonEvent += OnDungeon;
        Managers.StageM.dungeonClearEvent += OnDungeonClear;
        Managers.StageM.dungeonFailEvent += OnDungeonFail;
        Managers.StageM.dungeonOutEvent += OnDungeonOut;

        scene = Managers.UIM.SceneUI as UI_GameScene;
    }

    void OnDestroy()
    {
        Clear();
    }

    #region 이벤트
    public void OnReady()
    {
        spawnMaxCount = Managers.StageM.maxCount;
        spawnInterval = Managers.StageM.spawnInterval;
        StopSpawn();
        DeSpawnMonster();
        if (scene != null) scene.CheckTexts();
    }

    public void OnPlay(Define.StageState _state)
    {
        StartSpawn();
    }

    public void OnBoss()
    {
        StopSpawn();
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

    public void OnDungeon(int _dungeonDataID)
    {
        // 이벤트 핸들러는 void 유지, 비동기 로직은 별도 메서드로 분리
        OnDungeonAsync(_dungeonDataID).Forget();
    }

    async UniTaskVoid OnDungeonAsync(int _dungeonDataID)
    {
        try
        {
            dungeonDataID = _dungeonDataID;
            StopSpawn();
            DeSpawnMonster();

            if (dungeonDataID == 70000)
            {
                spawnMaxCount = 30;
                spawnInterval = 30f;
            }
            else if (dungeonDataID == 70001)
            {
                OnBoss();
            }

            await scene.AsyncFadeInOut(true);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SpawnManager] 던전 진입 처리 실패: {e.Message}");
        }
    }

    public void OnDungeonClear()
    {
        dungeonDataID = 0;
    }

    public void OnDungeonFail()
    {
        dungeonDataID = 0;
    }

    public void OnDungeonOut()
    {
        isDelay = false;
        OnClear();
    }

    async UniTask ClearDelay()
    {
        Managers.StageM.StateChange(Define.StageState.Changing);
        StopSpawn();
        if (isDelay) await UniTask.WaitForSeconds(2.0f);

        await scene.AsyncFadeInOut(false);

        DeSpawnMonster();

        for (int i = Managers.ObjectM.pcList.Count - 1; i >= 0; i--)
        {
            Managers.ObjectM.DeSpawn(Managers.ObjectM.pcList[i]);
        }
        Managers.CharacterM.ClearAllPlayers();

        Managers.ObjectM.mcList.Clear();

        await UniTask.WaitForSeconds(1.0f);

        if (!isDelay) isDelay = true;
        Managers.StageM.StateChange(Define.StageState.Ready);
    }


    #endregion

    public void StartSpawn()
    {
        if (isSpawning) return;
        Managers.UpdateM.Register(this);
        spawnTime = 0f;
        isSpawning = true;
        //Debug.Log("Start Spawn");
    }
    public void StopSpawn()
    {
        if (!isSpawning) return;

        Managers.UpdateM.UnRegister(this);
        isSpawning = false;
        //Debug.Log("Spawn Stop");
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
        int bossID = _isDungeon ? 12000 : 11000;
        boss = Managers.ObjectM.Spawn<MonsterController>(Vector3.zero, bossID);
        if (scene == null) scene = Managers.UIM.SceneUI as UI_GameScene;
        boss.OnMonsterInfoUpdate += scene.UpdateBossInfo;

        ApplyKnockBackWithDelay(boss).Forget();

    }

    async UniTaskVoid ApplyKnockBackWithDelay(MonsterController _boss)
    {

        await UniTask.NextFrame();

        Vector3 bossPos = _boss.transform.position;

        foreach (var player in Managers.CharacterM.AlivePlayers)
        {

            Vector3 playerPos = player.transform.position;
            float dist = Vector3.Distance(bossPos, playerPos);

            if (dist <= 2.0f)
            {
                Vector3 knockBackDir = (playerPos - bossPos).normalized;
                if (knockBackDir == Vector3.zero) knockBackDir = Vector3.forward;

                player.transform.LookAt(bossPos);

                player.KnockBack(3.0f, 0.3f).Forget();
            }
        }
    }
    void DeSpawnPlayer()
    {
        for (int i = Managers.ObjectM.pcList.Count - 1; i >= 0; i--)
        {
            Managers.ObjectM.DeSpawn(Managers.ObjectM.pcList[i]);
        }
    }

    void DeSpawnMonster()
    {
        for (int i = Managers.ObjectM.mcList.Count - 1; i >= 0; i--)
        {
            var monster = Managers.ObjectM.mcList[i];
            monster.ClearAttachedVFXs();
            Managers.ObjectM.DeSpawn(monster);
        }
        Managers.ObjectM.mcList.Clear();
    }

    public void Tick(float _deltaTime)
    {

        if (Managers.StageM.stageState == Define.StageState.Boss || Managers.StageM.stageState == Define.StageState.BossPlay) return;


        spawnTime -= _deltaTime;
        if (spawnTime <= 0f)
        {
            spawnTime = spawnInterval;
            SpawnMonster(spawnMaxCount);
        }
    }

    public void Clear()
    {
        StopSpawn();

        if (Managers.StageM != null)
        {
            Managers.StageM.readyEvent -= OnReady;
            Managers.StageM.playEvent -= OnPlay;
            Managers.StageM.bossEvent -= OnBoss;
            Managers.StageM.clearEvent -= OnClear;
            Managers.StageM.deadEvent -= OnDead;
            Managers.StageM.dungeonEvent -= OnDungeon;
            Managers.StageM.dungeonClearEvent -= OnDungeonClear;
            Managers.StageM.dungeonFailEvent -= OnDungeonFail;
            Managers.StageM.dungeonOutEvent -= OnDungeonOut;
        }

        DeSpawnMonster();
        if (boss != null)
        {
            boss.OnMonsterInfoUpdate -= scene.UpdateBossInfo;
            boss = null;
        }

        scene = null;
        Debug.Log("[SpawnManager] 스폰 로직 및 이벤트 구독 해제 완료");
    }
}