using System.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

public static class Utils
{
    //NOTE: 레벨 데이터 값
    private static LevelDesign datas;
    public static LevelDesign Datas
    {
        get
        {
            if(datas == null)
            {
                if(Managers.ResourceM != null)
                {
                    datas = Managers.ResourceM.Load<LevelDesign>("LevelDesignData");
                }
                else
                {
                    Debug.LogError("[Utils] 아직 매니저 초기화 전이라 안됌");
                }
            }
            return datas;
        }
    }

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
        if (tr == null) return null;

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
    public static bool IsValid(this BaseController _cc)
    {
        return _cc != null && _cc.isActiveAndEnabled;
    }
    public static bool IsValid(this UI_Base _ui)
    {
        return _ui != null && _ui.isActiveAndEnabled;
    }

    //몬스터 소환 위치
    public static Vector3 CreateMonsterSpawnPoint()
    {

        Vector3 spawnPos = Vector3.zero + UnityEngine.Random.insideUnitSphere * 5f;
        spawnPos.y = 0.0f;

        return spawnPos;
    }

    //지수증가 공식(레벨, 스테이지데이터)
    public static double CalculatedValue(float _baseValue, int _level, float _value)
    {
        return _baseValue * Mathf.Pow(_level + 1, _value);
    }
    public static bool CoinCheck(double _gold)
    {
        if (Managers.GameM.Gold >= _gold) return true;
        else return false;
    }

    #region UI관련
    public static void BindEvent(this GameObject _go, Action _action = null, Action<BaseEventData> _dragAction = null, Define.UIEvent _type = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(_go, _action, _dragAction, _type);
    }
    #endregion


    #region 천문학적인 숫자알파벳으로 바꾸는 코드
    const string Zero = "0";
    static readonly string[] CurrencyUnits = new string[]
    {
         "",
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "AA",
            "AB",
            "AC",
            "AD",
            "AE",
            "AF",
            "AG",
            "AH",
            "AI",
            "AJ",
            "AK",
            "AL",
            "AM",
            "AN",
            "AO",
            "AP",
            "AQ",
            "AR",
            "AS",
            "AT",
            "AU",
            "AV",
            "AW",
            "AX",
            "AY",
            "AZ",
            "BA",
            "BB",
            "BC",
            "BD",
            "BE",
            "BF",
            "BG",
            "BH",
            "BI",
            "BJ",
            "BK",
            "BL",
            "BM",
            "BN",
            "BO",
            "BP",
            "BQ",
            "BR",
            "BS",
            "BT",
            "BU",
            "BV",
            "BW",
            "BX",
            "BY",
            "BZ",
            "CA",
            "CB",
            "CC",
            "CD",
            "CE",
            "CF",
            "CG",
            "CH",
            "CI",
            "CJ",
            "CK",
            "CL",
            "CM",
            "CN",
            "CO",
            "CP",
            "CQ",
            "CR",
            "CS",
            "CT",
            "CU",
            "CV",
            "CW",
            "CX",
    };
    public static string ToCurrencyString(this double _number, CurrencyType _currencyType = CurrencyType.Default)
    {
        if (-1d < _number && _number < 1d)
        {
            return Zero;
        }

        if (true == double.IsInfinity(_number))
        {
            return "Infinity";
        }

        string significant = (_number < 0) ? "-" : string.Empty;

        string showNumber = string.Empty;
        string unitString = string.Empty;

        string[] partsSplit = _number.ToString("E").Split('+');

        if (partsSplit.Length < 2)
        {
            Debug.LogWarning(string.Format("Failed - ToCurrencyString({0}) partsSplit[1] = {1}", _number));
            return Zero;
        }

        if (false == int.TryParse(partsSplit[1], out int exponent))
        {
            Debug.LogWarning(string.Format("Failed - ToCurrencyString({0}) : partsSplit[1] = {1}", _number, partsSplit[1]));
            return Zero;
        }

        int quotient = exponent / 3;
        int remainder = exponent % 3;

        if (exponent < 3)
        {
            showNumber = System.Math.Truncate(_number).ToString();
        }
        else
        {
            var temp = double.Parse(partsSplit[0].Replace("E", "")) * Math.Pow(10, remainder);

            showNumber = temp.ToString("F").Replace(".00", "");
        }

        if (_currencyType == CurrencyType.Default)
        {
            unitString = CurrencyUnits[quotient];
        }

        return string.Format("{0}{1}{2}", significant, showNumber, unitString);
    }

    public static double ToCurrencyDouble(this string _currencyString, CurrencyType _stringType = CurrencyType.Default)
    {
        double result = 0;
        bool isNumber = double.TryParse(_currencyString, out result);

        if (true == isNumber)
        {
            return result;
        }
        else
        {
            int length = _currencyString.Length;
            int lastNumberIndex = -1;

            for (int i = length - 1; i <= i; --i)
            {
                if (true == char.IsNumber(_currencyString, i))
                {
                    lastNumberIndex = i;
                    break;
                }
            }

            if (lastNumberIndex < 0)
            {
                throw new Exception("Failed currency string");
            }

            string number = _currencyString.Substring(0, lastNumberIndex + 1);
            string unit = _currencyString.Substring(lastNumberIndex + 1);

            int index = Array.FindIndex(CurrencyUnits, p => p == unit);
            if (-1 == index)
            {
                throw new Exception("Failed currency string");
            }

            string exponentNumber = string.Format("{0}E+{1]}", number, index * 3);

            return double.Parse(exponentNumber);
        }
    }
    #endregion

    #region Color
    public static Color HexToColor(string _color)
    {
        Color parsedColor;
        ColorUtility.TryParseHtmlString("#" + _color, out parsedColor);

        return parsedColor;
    }

    #endregion

    public static string StringToColorGrade(Define.ItemGrade _grade)
    {
        switch (_grade)
        {
            case Define.ItemGrade.Common: return "<color=#FFFFFF>";
            case Define.ItemGrade.UnCommon: return "<color=#00FF00>";
            case Define.ItemGrade.Rare: return "<color=#0000FF>";
            case Define.ItemGrade.Unique: return "<color=#BB45FF>";
            case Define.ItemGrade.Legendary: return "<color=#FF9A45>";
        }

        return "<color=#FFFFFF>";
    }

    public static void Shuffle<T>(this List<T> _list)
    {
        int count = _list.Count;

        while (count > 1)
        {
            count--;
            int randNum = UnityEngine.Random.Range(0, count + 1);
            T value = _list[randNum];
            _list[randNum] = _list[count];
            _list[count] = value;
        }
    }


}
