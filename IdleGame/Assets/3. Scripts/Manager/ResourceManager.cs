using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System;
using Object = UnityEngine.Object;
using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.U2D;

public class ResourceManager
{

    Dictionary<string, Object> resourceDic = new();
    Dictionary<string, string> keyToLabelDic = new();


    public SpriteAtlas atlas;
    public Sprite GetAtlas(string _temp)
    {
        if (atlas == null) atlas = Managers.ResourceM.Load<SpriteAtlas>("Atlas");

        return atlas.GetSprite(_temp);
    }
    public T Load<T>(string _key) where T : Object
    {
        if (resourceDic.TryGetValue(_key, out Object resource))
        {
            return resource as T;
        }

        return null;
    }

    public GameObject Instantiate(string _key, Transform _parent = null, bool _pooling = false)
    {
        GameObject prefab = Load<GameObject>(_key);
        if (prefab == null)
        {
            Debug.LogError("키에 맞는 프리팹이 없음.");
            return null;
        }

        if (_pooling)
            return Managers.PoolM.Pop(prefab);

        GameObject go = GameObject.Instantiate(prefab, _parent);
        go.name = prefab.name;
        return go;
    }

    public void Destroy(GameObject _go)
    {
        if (_go == null) return;

        if (Managers.PoolM.Push(_go)) return;

        Object.Destroy(_go);
    }


    #region 비동기 로딩
    public async UniTask<T> LoadAsync<T>(string _key) where T : Object
    {
        if (resourceDic.TryGetValue(_key, out Object resource))
        {
            return resource as T;
        }

        UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle handle = default;
        Object resultAsset = null;

        try
        {
            if (_key.Contains(".sprite"))
            {
                var spriteHandle = Addressables.LoadAssetAsync<Sprite>(_key);
                resultAsset = await spriteHandle;
                handle = spriteHandle;
                //resultAsset = await Addressables.LoadAssetAsync<Sprite>(_key).ToUniTask();
            }
            else
            {
                var tHandle = Addressables.LoadAssetAsync<T>(_key);
                resultAsset = await tHandle;
                handle = tHandle;
                //resultAsset = await Addressables.LoadAssetAsync<T>(_key).ToUniTask();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"LoadAsync 실패 : {_key} : {e.Message}");
            return null;
        }


        if (resultAsset != null)
        {
            resourceDic.TryAdd(_key, resultAsset);

            //if (handle.IsValid())
            //    Addressables.Release(handle);

            return resultAsset as T;
        }
        else
        {
            Debug.LogError($"LoadAsync 실패 : 에셋에 키값이 없음 {_key}");

            if (handle.IsValid())
                Addressables.Release(handle);

            return null;
        }
    }

    public async UniTask LoadGroupAsync<T>(string _label, Action<string, int, int> _cb = null) where T : Object
    {
        var locationHandle = Addressables.LoadResourceLocationsAsync(_label, typeof(T));
        IList<UnityEngine.ResourceManagement.ResourceLocations.IResourceLocation> locations;

        try
        {
            locations = await locationHandle.ToUniTask();
        }
        catch (Exception e)
        {
            Debug.LogError($"LoadResourceLocationsAsync 실패 : {_label} : {e.Message}");
            Addressables.Release(locationHandle);
            return;
        }

        if (locations == null || locations.Count == 0)
        {
            _cb?.Invoke("", 0, 0);
            Addressables.Release(locationHandle);
            return;
        }

        int loadCount = 0;
        int maxCount = locations.Count;
        var loadTasks = new List<UniTask>();

        foreach (var reslut in locations)
        {
            string key = reslut.PrimaryKey;
            if (!keyToLabelDic.ContainsKey(key))
            {
                keyToLabelDic.TryAdd(key, _label);
            }

            UniTask loadTask;
            if (key.Contains(".sprite"))
            {
                loadTask = LoadAsync<Sprite>(key).AsUniTask();
            }
            else
            {
                loadTask = LoadAsync<T>(key).AsUniTask();
            }

            var completionTask = loadTask.ContinueWith(() =>
            {
                loadCount++;
                _cb?.Invoke(key, loadCount, maxCount);
            });

            loadTasks.Add(completionTask);
        }


        await UniTask.WhenAll(loadTasks);

        Addressables.Release(locationHandle);

    }

    public void UnLoadAll()
    {
        List<string> keys = resourceDic.Keys.ToList();
        foreach (var key in keys)
        {
            UnLoad(key);
        }

    }

    public async UniTask UnLoadGroup(string _label)
    {
        var keysToUnLoad = keyToLabelDic.
        Where(x => x.Value == _label).
        Select(x => x.Key).
        ToList();

        if (keysToUnLoad.Count == 0) return;

        foreach (var key in keysToUnLoad)
        {
            UnLoad(key);
        }

        await UniTask.Yield();
    }

    public void UnLoad(string _key)
    {
        if (resourceDic.TryGetValue(_key, out Object resource))
        {
            resourceDic.Remove(_key);
            keyToLabelDic.Remove(_key);
            Addressables.Release(resource);
        }
    }
    #endregion
}
