using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager
{
    readonly Stack<UI_Base> popupStack = new();

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
            }

            return root;
        }
    }

    public T MakeSubItem<T>(Transform _parent = null, string _name = null, bool _pooling = true) where T : UI_Base
    {
        if (string.IsNullOrEmpty(_name)) _name = typeof(T).Name;
        GameObject go = Managers.ResourceM.Instantiate($"{_name}", _parent, _pooling);
        if (_parent != null)
            go.transform.SetParent(_parent);

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

        return ui;
    }
    #region  Popup

    public T ShowPopup<T>(string _name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(_name))
            _name = typeof(T).Name;

        GameObject go = Managers.ResourceM.Instantiate($"{_name}");
        T popup = go.GetOrAddComponent<T>();
        popupStack.Push(popup);
        go.transform.SetParent(Root.transform);

        //팝업 띄워져있을때 게임 일시정지 할지 말지
        //RefreshTimeScale();

        return popup;
    }

    public void ClosePopup(UI_Base _popup)
    {
        if (popupStack.Count == 0) return;

        if (popupStack.Peek() != _popup)
        {
            Debug.Log("창 닫기 실패. 팝업이 맞지 않음");
            return;
        }

        //사운드 넣기


        ClosePopup();
    }

    public void ClosePopup()
    {
        if (popupStack.Count == 0) return;

        UI_Base popup = popupStack.Pop();
        Managers.ResourceM.Destory(popup.gameObject);
        popup = null;
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
        toast.SetInfo(_detail);
        go.transform.SetParent(Root.transform);

        return toast;
    }

    public void CloseToast(UI_Toast _toast)
    {
        Managers.ResourceM.Destory(_toast.gameObject);
    }
    #endregion
}
