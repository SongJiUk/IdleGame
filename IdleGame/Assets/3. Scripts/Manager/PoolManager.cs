using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

class Pool
{
    GameObject prefab;
    IObjectPool<GameObject> pool;
    Transform root;
    Transform Root
    {
        get
        {
            if (root == null)
            {
                GameObject go = new GameObject() { name = $"{prefab.name}Root" };
                root = go.transform;
            }

            return root;
        }
    }

    public Pool(GameObject _prefab)
    {
        prefab = _prefab;
        pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }

    public GameObject Pop()
    {
        return pool.Get();
    }

    public void Push(GameObject _go)
    {
        pool.Release(_go);
    }

    GameObject OnCreate()
    {
        GameObject go = GameObject.Instantiate(prefab);
        go.name = prefab.name;
        go.transform.SetParent(Root);
        return go;
    }

    void OnGet(GameObject _go)
    {
        if (_go == null)
        {
            return;
        }
        _go.SetActive(true);
    }

    void OnRelease(GameObject _go)
    {
        _go.SetActive(false);
    }

    void OnDestroy(GameObject _go)
    {
        GameObject.Destroy(_go);
    }
}

public class PoolManager
{
    Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

    public GameObject Pop(GameObject _prefab)
    {
        //TODO : 여기인거같은데..
        if (_prefab.IsValid() == false)
            return null;

        if (!pools.ContainsKey(_prefab.name))
            CreatePool(_prefab);


        return pools[_prefab.name].Pop();
    }

    public bool Push(GameObject _prefab)
    {
        if (_prefab.IsValid() == false) return false;

        if (!pools.ContainsKey(_prefab.name)) return false;

        pools[_prefab.name].Push(_prefab);

        return true;
    }


    public void CreatePool(GameObject _prefab)
    {
        Pool pool = new Pool(_prefab);
        pools.Add(_prefab.name, pool);
    }

    public void Clear()
    {
        pools.Clear();
    }
}
