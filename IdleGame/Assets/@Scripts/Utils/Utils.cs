using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{

    public static T GetOrAddComponent<T>(this GameObject _go) where T : Component
    {
        if (_go == null) return null;
        T component = _go.GetComponent<T>();
        if (component == null)
            component = _go.AddComponent<T>();

        return component;
    }


    //자식찾는 유틸 
    public static GameObject FindChild(GameObject _go, string _name = null, bool _recursive = false)
    {
        Transform tr = FindChild<Transform>(_go, _name, _recursive);
        if (tr = null) return null;

        return tr.gameObject;
    }

    public static T FindChild<T>(GameObject _go, string _name = null, bool _recursive = false) where T : UnityEngine.Object
    {
        if (_go == null) return null;

        if (_recursive == false)
        {
            for (int i = 0; i < _go.transform.childCount; i++)
            {
                Transform tr = _go.transform.GetChild(i);
                if (string.IsNullOrEmpty(_name) || tr.name == _name)
                {
                    T component = tr.GetComponent<T>();
                    if (component != null) return component;
                }
            }
        }
        else
        {
            foreach (T component in _go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(_name) || component.name == _name)
                {
                    return component;
                }
            }
        }

        return null;
    }

    //오브젝트가 있는지, 활성화중인지 확인하는 Util
    public static bool IsValid(this GameObject _go)
    {
        return _go != null && _go.activeSelf;
    }

    //지수증가 공식(레벨, 스테이지데이터)
    public static float CalculatedValue(float _baseValue, int _level, float _value)
    {
        return _baseValue * Mathf.Pow(_level, _value);
    }

}
