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
    float spawnInterval = 2f;
    float spawnTime = 0f;

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






    #region 이벤트
    public void OnReady()
    {
        spawnMaxCount = Managers.DataM.StageDataDic[Managers.GameM.stage].SpawnMaxCount;
        spawnTime = Managers.DataM.StageDataDic[Managers.GameM.stage].SpawnTimer;

        if (scene != null) scene.CheckTexts();
    }

    public void OnPlay()
    {

        spawnTime = 0f;
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
        if (scene == null) scene = Managers.UIM.SceneUI as UI_GameScene;
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

        for (int i = Managers.ObjectM.mcList.Count - 1; i >= 0; i--)
        {
            //TODO : 여기서 해주는게 맞나 싶긴한데
            Managers.ObjectM.mcList[i].ClearChildVFXs();
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

        int value = spawnMaxCount - Managers.ObjectM.mcList.Count;
        for (int i = 0; i < value; i++)
        {
            //TODO: 여기 스테이지 마다 생성되는 몬스터가 달라진다면 하드코딩 없애기(10000) 이거 
            Managers.ObjectM.Spawn<MonsterController>(Utils.CreateMonsterSpawnPoint(), 10000);
        }
    }


    public void Tick(float _deltaTime)
    {
        spawnTime -= _deltaTime;
        if (spawnTime <= 0f)
        {
            spawnTime = spawnInterval;
            SpawnMonster();
        }
    }
}