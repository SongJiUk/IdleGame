using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers instance;
    static bool init = false;

    readonly PoolManager poolManager = new();
    readonly ResourceManager resourceManager = new();

    readonly UIManager uIManager = new();

    public static PoolManager PoolM { get { return Instance?.poolManager; } }
    public static ResourceManager ResourceM { get { return Instance?.resourceManager; } }
    public static UIManager UIM { get { return Instance?.uIManager; } }


    public static Managers Instance
    {
        get
        {
            if (init == false)
            {
                init = true;
                GameObject go = GameObject.Find("@Managers");

                if (go = null)
                {
                    go = new GameObject() { name = "@Managers" };
                    go.AddComponent<Managers>();
                }

                DontDestroyOnLoad(go);
                instance = go.GetComponent<Managers>();
            }

            return instance;
        }
    }



    public static void Clear()
    {
        PoolM.Clear();
    }
}
