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

    public void DestroyPool()
    {
        if (root != null)
        {
            GameObject.Destroy(root.gameObject);
            root = null;
        }

        pool.Clear();
    }
}

public class PoolManager
{
    Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

    public GameObject Pop(GameObject _prefab)
    {
        if (_prefab.IsValid() == false)
            return null;

        if (!pools.TryGetValue(_prefab.name, out var pool))
        {
            CreatePool(_prefab);
            pool = pools[_prefab.name];
        }

        return pool.Pop();
    }

    public bool Push(GameObject _prefab)
    {
        if (_prefab.IsValid() == false) return false;

        if (!pools.TryGetValue(_prefab.name, out var pool)) return false;

        pool.Push(_prefab);

        return true;
    }


    public void CreatePool(GameObject _prefab)
    {
        Pool pool = new Pool(_prefab);
        pools.Add(_prefab.name, pool);
    }

    public void Clear()
    {
        foreach (var pool in pools.Values)
        {
            pool.DestroyPool();
        }

        pools.Clear();
        Debug.Log("[PoolManager] 모든 풀이 파괴되고 딕셔너리가 비워졌습니다.");
    }
}
