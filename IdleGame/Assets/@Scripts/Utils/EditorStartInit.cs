using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


[InitializeOnLoad]
public class EditorStartInit
{

    [MenuItem("MyEditor/시작 씬부터 시작")]
    public static void SetupFromStartScene()
    {
        var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
        EditorSceneManager.playModeStartScene = sceneAsset;
        UnityEditor.EditorApplication.isPlaying = true;
    }

    [MenuItem("MyEditor/현재 씬부터 시작")]
    public static void StartFromThisScene()
    {
        EditorSceneManager.playModeStartScene = null;
        UnityEditor.EditorApplication.isPlaying = true;
    }
}
#endif
