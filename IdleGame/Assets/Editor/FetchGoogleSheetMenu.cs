using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Tools > Fetch Google Sheet 메뉴 아이템.
/// DevScene을 임시로 열어 GoogleSheetManager.FetchGoogleSheet()를 실행하고,
/// 컴파일(도메인 리로드) 완료 후 DevScene을 자동으로 닫는다.
/// </summary>
[InitializeOnLoad]
public static class FetchGoogleSheetMenu
{
    const string SCENE_PATH = "Assets/2. Scnens/DevScene.unity";
    const string OPENED_KEY = "FetchGoogleSheetMenu_OpenedDevScene";

    // 도메인 리로드(컴파일) 후 자동 호출 - DevScene 자동 닫기
    static FetchGoogleSheetMenu()
    {
        if (!SessionState.GetBool(OPENED_KEY, false)) return;

        SessionState.EraseBool(OPENED_KEY);
        EditorApplication.delayCall += CloseDevScene;
    }

    [MenuItem("Tools/Fetch Google Sheet")]
    static void Execute()
    {
        bool alreadyOpen = IsDevSceneOpen();

        if (!alreadyOpen)
        {
            EditorSceneManager.OpenScene(SCENE_PATH, OpenSceneMode.Additive);
            SessionState.SetBool(OPENED_KEY, true);
        }

        var manager = Object.FindFirstObjectByType<GoogleSheetManager>();
        if (manager == null)
        {
            Debug.LogError("[FetchGoogleSheet] GoogleSheetManager를 찾을 수 없습니다. DevScene 경로를 확인하세요.");
            if (!alreadyOpen) CloseDevScene();
            return;
        }

        Debug.Log("[FetchGoogleSheet] Google Sheet 가져오는 중...");
        manager.FetchGoogleSheet();
    }

    static bool IsDevSceneOpen()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
            if (SceneManager.GetSceneAt(i).path == SCENE_PATH)
                return true;
        return false;
    }

    static void CloseDevScene()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.path == SCENE_PATH)
            {
                EditorSceneManager.CloseScene(scene, true);
                Debug.Log("[FetchGoogleSheet] Fetch 완료. DevScene 자동으로 닫힘.");
                return;
            }
        }
    }
}
