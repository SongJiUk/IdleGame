using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers instance;
    static bool init = false;


    UpdateManager updateManager = null;
    SpawnManager spawnManager = null;
    RenderManager rederManager = null;

    readonly PoolManager poolManager = new();
    readonly ResourceManager resourceManager = new();
    readonly UIManager uIManager = new();
    readonly ObjectManager objectManager = new();
    readonly GameManager gameManager = new();
    readonly DataManager dataManager = new();
    readonly CustomSceneManager customSceneManager = new();
    readonly PlayerManager playerManager = new();
    readonly StageManager stageManager = new();
    readonly CameraManager cameraManager = new();
    readonly ItemManager itemManager = new();
    readonly CharacterManager characterManager = new();
    readonly InventoryManager inventoryManager = new();
    readonly ADManager adManager = new();
    readonly FirebaseManager firebaseManager = new();
    readonly RelicManager relicManager = new();
    readonly QuestManager questManager = new();
    readonly IAPManager iapMamager = new();
    readonly SoundManager soundManager = new();
    readonly LocalizationManager localizationManager = new();
    readonly TimeManager timeManager = new();
    readonly GPGSManager gpgsManager = new();
    readonly BuffManager buffManager = new();

    public static PoolManager PoolM { get { return Instance?.poolManager; } }
    public static ResourceManager ResourceM { get { return Instance?.resourceManager; } }
    public static UIManager UIM { get { return Instance?.uIManager; } }
    public static UpdateManager UpdateM { get { return Instance?.updateManager; } }
    public static ObjectManager ObjectM { get { return Instance?.objectManager; } }
    public static SpawnManager SpawnM { get { return Instance?.spawnManager; } }
    public static GameManager GameM { get { return Instance?.gameManager; } }
    public static DataManager DataM { get { return Instance?.dataManager; } }
    public static CustomSceneManager SceneM { get { return Instance?.customSceneManager; } }
    public static PlayerManager PlayerM { get { return Instance?.playerManager; } }
    public static StageManager StageM { get { return Instance?.stageManager; } }
    public static CameraManager CameraM { get { return Instance?.cameraManager; } }
    public static ItemManager ItemM { get { return Instance?.itemManager; } }
    public static RenderManager RenderM { get { return Instance?.rederManager; } }
    public static CharacterManager CharacterM { get { return Instance?.characterManager; } }
    public static InventoryManager InventoryM { get { return Instance?.inventoryManager; } }
    public static ADManager AdM { get { return Instance?.adManager; } }
    public static FirebaseManager FirebaseM { get { return Instance?.firebaseManager; } }
    public static RelicManager RelicM { get { return Instance?.relicManager; } }
    public static QuestManager QuestM { get { return Instance?.questManager; } }
    public static IAPManager IAPM { get { return Instance?.iapMamager; } }
    public static SoundManager SoundM { get { return Instance?.soundManager; } }
    public static LocalizationManager LocalM { get { return Instance?.localizationManager; } }
    public static TimeManager TimeM { get { return Instance?.timeManager; } }
    public static GPGSManager GPGSM { get { return Instance?.gpgsManager; } }
    public static BuffManager BuffM { get { return Instance?.buffManager; } }


    public static bool isFast = false;
    public static float save_Timer = 0.0f;

    public static Managers Instance
    {
        get
        {
            if (init == false)
            {
                init = true;
                GameObject go = GameObject.Find("@Managers");

                if (go == null)
                {
                    go = new GameObject() { name = "@Managers" };
                    go.AddComponent<Managers>();
                }

                DontDestroyOnLoad(go);
                instance = go.GetComponent<Managers>();
                instance.updateManager = go.AddComponent<UpdateManager>();
                instance.spawnManager = go.AddComponent<SpawnManager>();
            }

            return instance;
        }
    }

    public void SetRenderManager(RenderManager _renderM)
    {
        if (this.rederManager == null)
            this.rederManager = _renderM;
        else
            Debug.LogError("RenderManager가 이미 할당되어있음.");
    }

    public static void Clear()
    {
        PoolM.Clear();
    }

    void OnApplicationPause(bool _pause)
    {
        GameM.OnApplicationPause(_pause);
    }

    void OnApplicationQuit()
    {
        GameM.OnApplicationPause(true);
    }

    public void TestManualSave()
    {
        Debug.Log("<color=yellow>[개발용] 수동 저장 테스트 시작</color>");
        OnApplicationPause(true); // 동일한 로직 실행
    }

    public void TestManualLoad()
    {
        Debug.Log("<color=cyan>[개발용] 수동 로드 테스트 시작</color>");
        OnApplicationPause(false); // 동일한 로직 실행
    }
    void Update()
    {
        // 에디터에서만 작동하게 제한
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F)) TestManualSave();
        if (Input.GetKeyDown(KeyCode.R)) TestManualLoad();
#endif
    }
}
