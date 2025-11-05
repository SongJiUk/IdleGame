using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.EventSystems;

public class UI_Base : MonoBehaviour
{
    #region 초기화
    protected bool isInit = false;

    protected Type GameObjectsType;
    protected Type ImagesType;
    protected Type TextsType;
    protected Type ButtonsType;
    protected Type TogglesType;
    protected Type SlidersType;


    private void Start()
    {
        Init();
    }

    public virtual bool Init()
    {
        if (isInit) return false;

        isInit = true;
        return true;
    }
    #endregion


    #region 바인딩
    protected Dictionary<Type, UnityEngine.Object[]> objs_Dic = new();

    protected void Bind<T>(Type _type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(_type);
        UnityEngine.Object[] objs = new UnityEngine.Object[names.Length];
        objs_Dic.Add(_type, objs);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
            {
                objs[i] = Utils.FindChild(gameObject, names[i], true);
            }
            else
            {
                objs[i] = Utils.FindChild<T>(gameObject, names[i], true);
            }

            if (objs[i] == null) Debug.LogError($"Failed Bind {names[i]}");
        }
    }

    protected void BindObject(Type _type) { Bind<GameObject>(_type); }
    protected void BindImage(Type _type) { Bind<Image>(_type); }
    protected void BindText(Type _type) { Bind<TextMeshProUGUI>(_type); }
    protected void BindButton(Type _type) { Bind<Button>(_type); }
    protected void BindToggle(Type _type) { Bind<Toggle>(_type); }
    protected void BindSlider(Type _type) { Bind<Slider>(_type); }
    #endregion


    #region  Get
    protected T Get<T>(Type _type, int _index) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objs = null;
        if (objs_Dic.TryGetValue(_type, out objs) == false) return null;

        return objs[_index] as T;
    }

    protected GameObject GetObject(Type _type, int _index) { return Get<GameObject>(_type, _index); }
    protected Image GetImage(Type _type, int _index) { return Get<Image>(_type, _index); }
    protected TextMeshProUGUI GetText(Type _type, int _index) { return Get<TextMeshProUGUI>(_type, _index); }
    protected Button GetButton(Type _type, int _index) { return Get<Button>(_type, _index); }
    protected Toggle GetToggle(Type _type, int _index) { return Get<Toggle>(_type, _index); }
    protected Slider GetSlider(Type _type, int _index) { return Get<Slider>(_type, _index); }
    #endregion


    #region 이벤트 바인딩
    public static void BindEvent(GameObject _go, Action _action = null, Action<BaseEventData> _dragAction = null, Define.UIEvent _type = Define.UIEvent.Click)
    {
        UI_EventHandler eh = Utils.GetOrAddComponent<UI_EventHandler>(_go);

        switch (_type)
        {
            case Define.UIEvent.Click:
                eh.OnClickHandler -= _action;
                eh.OnClickHandler += _action;
                break;

            case Define.UIEvent.Pressed:
                eh.OnPressHandler -= _action;
                eh.OnPressHandler += _action;
                break;

            case Define.UIEvent.PointerDown:
                eh.OnPointerDownHandler -= _action;
                eh.OnPointerDownHandler += _action;
                break;

            case Define.UIEvent.PointerUp:
                eh.OnPointerUpHandler -= _action;
                eh.OnPointerUpHandler += _action;
                break;
            case Define.UIEvent.Drag:
                eh.OnDragHandler -= _dragAction;
                eh.OnDragHandler += _dragAction;
                break;

            case Define.UIEvent.BeginDrag:
                eh.OnBeginDragHandler -= _dragAction;
                eh.OnBeginDragHandler += _dragAction;
                break;

            case Define.UIEvent.EndDrag:
                eh.OnEndDragHandler -= _dragAction;
                eh.OnEndDragHandler += _dragAction;
                break;
        }
    }

    public virtual void OnDrag(BaseEventData _eventData) { }
    public virtual void OnBeginDrag(BaseEventData _eventData) { }
    public virtual void OnEndDrag(BaseEventData _eventData) { }
    #endregion
}