using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;


public class CustomSceneManager
{
    public Define.SceneType CurrentScene { get; private set; }

    public async UniTask LoadSceneAsync(Define.SceneType _sceneType)
    {
        await UniTask.Yield();

        await SceneManager.LoadSceneAsync(GetScene(_sceneType));

        CurrentScene = _sceneType;


    }

    public string GetScene(Define.SceneType _sceneType)
    {
        return Enum.GetName(typeof(Define.SceneType), _sceneType);
    }

    public void Clear()
    {
        CurrentScene = Define.SceneType.None;
        Debug.Log("[CustomSceneManager] 씬 정보 초기화 완료");
    }
}
