using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;


public class CustomSceneManager
{

    public async UniTask LoadSceneAsync(Define.SceneType _sceneType)
    {
        await UniTask.Yield();

        await SceneManager.LoadSceneAsync(GetScene(_sceneType));


    }

    public string GetScene(Define.SceneType _sceneType)
    {
        string sceneName = Enum.GetName(typeof(Define.SceneType), _sceneType);
        return sceneName;
    }
}
