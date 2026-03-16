using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class UIManager
{
    public readonly Stack<UI_Base> popupStack = new();

    private bool isGolbalLock = false;
    public Action<bool> OnDungeonPopupState;

    UI_Scene sceneUI = null;
    public UI_Scene SceneUI { get { return sceneUI; } }
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
            {
                root = new GameObject { name = "@UI_Root" };
                Canvas canvas = root.AddComponent<Canvas>();
                root.AddComponent<CanvasScaler>();
                root.AddComponent<GraphicRaycaster>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            return root;
        }
    }

    public T MakeSubItem<T>(Transform _parent = null, string _name = null, bool _pooling = true) where T : UI_Base
    {
        if (string.IsNullOrEmpty(_name)) _name = typeof(T).Name;
        GameObject go = Managers.ResourceM.Instantiate($"{_name}", _parent, _pooling);
        if (_parent != null)
            go.transform.SetParent(_parent, false);

        return Utils.GetOrAddComponent<T>(go);
    }



    public T ShowScene<T>(string _name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(_name))
            _name = typeof(T).Name;

        GameObject go = Managers.ResourceM.Instantiate(_name);
        T ui = go.GetOrAddComponent<T>();
        sceneUI = ui;
        go.transform.SetParent(Root.transform);

        RectTransform rect = go.gameObject.GetComponent<RectTransform>();
        if (rect != null)
        {

            rect.localPosition = Vector3.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

            //앵커 프리셋 효과 적용: Stretch-Stretch (꽉 채우기)
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        return ui;
    }
    #region  Popup

    public async UniTask<T> ShowPopup<T>(string _name = null, bool _isFade = false, Transform _parent = null) where T : UI_Base
    {
        Define.StageState prevState = Managers.StageM.stageState;

        if (_isFade) Managers.StageM.StateChange(Define.StageState.Changing);

        try
        {
            if (_isFade)
            {
                UI_GameScene scene = (sceneUI as UI_GameScene);
                await scene.AsyncFadeInOut(false, true);
            }

            T popup = GetPopupUI<T>(_name);
            if (popup == null)
            {
                Debug.LogError($"[UIManager] ShowPopup: '{_name ?? typeof(T).Name}' 팝업 표시 실패");
                return null;
            }

            if (_parent != null)
            {
                popup.gameObject.SetActive(false);
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
                popup.gameObject.transform.SetParent(_parent);
                await UniTask.Yield();
                popup.gameObject.SetActive(true);
            }
            popup.Init().Forget();
            popup.SetInfo();
            await UniTask.Yield();

            if (_isFade)
            {
                UI_GameScene scene = (sceneUI as UI_GameScene);
                await scene.AsyncFadeInOut(true, true);
            }

            return popup;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
        finally
        {
            if (_isFade)
            {
                Managers.StageM.StateChange(Define.StageState.Ready);
            }
        }
    }

    public T GetPopupUI<T>(string _name) where T : UI_Base
    {
        if (string.IsNullOrEmpty(_name))
            _name = typeof(T).Name;

        T popup = Managers.ObjectM.SpawnUI<T>(_name);
        if (popup == null)
        {
            Debug.LogError($"[UIManager] GetPopupUI: '{_name}' 팝업 생성 실패 (SpawnUI 반환값 null)");
            return null;
        }

        popup.gameObject.transform.SetParent(Root.transform);

        RectTransform rect = popup.gameObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localPosition = Vector3.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        popupStack.Push(popup);
        popup.gameObject.transform.SetAsLastSibling();

        return popup;
    }

    public async UniTask ClosePopup<T>(T _popup, bool _isFade = false) where T : UI_Base
    {
        if (_popup == null || !popupStack.Contains(_popup))
        {
            Debug.LogError("[UIManager] : 창 닫기 실패. 닫으려는 팝업이 스택에 없거나 유효하지 않습니다.");
            return;
        }

        if (popupStack.Peek() != _popup)
        {
            Debug.LogError("[UIManager] : 창 닫기 실패. 최상위 팝업만 닫을 수 있습니다.");
            return;
        }
        UI_GameScene scene = (sceneUI as UI_GameScene);

        if (_isFade)
        {
            await scene.AsyncFadeInOut(false, true);
        }

        ClosePopup();

        if (_isFade)
        {
            await scene.AsyncFadeInOut(true, true);
        }
    }
    public void ClosePopup()
    {
        UI_Base popup = popupStack.Pop();
        Managers.ResourceM.Destroy(popup.gameObject);

    }

    public void CloseAllPopup()
    {
        while (popupStack.Count > 0) ClosePopup();
    }

    public int GetPopupCount()
    {
        return popupStack.Count;
    }
    #endregion

    #region Toast
    public UI_Toast ShowToast(string _detail)
    {
        string name = typeof(UI_Toast).Name;
        GameObject go = Managers.ResourceM.Instantiate(name, _pooling: true);
        UI_Toast toast = go.GetOrAddComponent<UI_Toast>();

        toast.Init().Forget();
        toast.SetInfo(_detail);
        go.transform.SetParent(Root.transform);

        return toast;
    }

    public void CloseToast(UI_Toast _toast)
    {
        Managers.ResourceM.Destroy(_toast.gameObject);
    }
    #endregion

    public bool ClickLock(float _delay = 0.5f)
    {
        if (isGolbalLock) return false;

        isGolbalLock = true;
        ReleaseLockAfterDelay(_delay).Forget();
        return true;
    }

    private async UniTaskVoid ReleaseLockAfterDelay(float _delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_delay));
        isGolbalLock = false;
    }


    public void Clear()
    {
        CloseAllPopup();
        if (sceneUI != null)
        {
            Managers.ResourceM.Destroy(sceneUI.gameObject);
            sceneUI = null;
        }

        isGolbalLock = false;
        OnDungeonPopupState = null;

        Debug.Log("[UIManager] 모든 UI요소 및 팝업 정리 완료");
    }

}
