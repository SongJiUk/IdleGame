using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public static FirebaseManager firebaseM { get { return Instance?.firebaseManager; } }

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

}
