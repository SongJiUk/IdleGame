using System.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using System.Linq;

/* NOTE : 데이터 번호
1 ~10 플레이어
100 ~ buffTypeData 
10000 ~ 몬스터 
20000 ~ projectile
30000 ~ itemData
40000 ~ skillData
50000 ~ buffData
60000 ~ VfxData
70000 ~ DungeonData
80000 ~ MissionData
90000~ Achievement
90000 ~ QuestData
100000 ~ NPCData
110000 ~ SmeltData
*/


public static class Utils
{

    public const int GoodsDirectingDataID = 69101;
    public const int DamageFontDataID = 69102;
    public const int SpeechBubbleDataID = 69103;
    public const int DropItemDataID = 69104;


    public const int GachaMaxLevel = 10;
    public const int GradeCount = 5;
    //NOTE: 레벨 데이터 값
    private static LevelDesign datas;
    public static LevelDesign Datas
    {
        get
        {
            if (datas == null)
            {
                if (Managers.ResourceM != null)
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

    #region 스킬 헬퍼함수들
    //체력 가장 낮은 아군 찾는 함수
    public static CreatureController FindLowestHpPlayer()
    {
        List<PlayerController> pList = Managers.ObjectM.pcList;
        CreatureController lowestHpPlayer = null;

        foreach (PlayerController player in pList)
        {
            if (lowestHpPlayer == null)
            {
                lowestHpPlayer = player;
            }
            else
            {
                if (lowestHpPlayer.HP > player.HP)
                {
                    lowestHpPlayer = player;
                }
            }
        }

        if (lowestHpPlayer != null) return lowestHpPlayer;
        else return null;
    }

    //아군 중 랜덤한 캐릭터 찾기
    public static CreatureController FindRandomPlayer(CreatureController _caster)
    {
        List<PlayerController> pList = Managers.ObjectM.pcList;
        List<PlayerController> randomPlayer = new List<PlayerController>();
        CreatureController player = null;

        foreach (PlayerController pc in pList)
        {
            if (!pc.gameObject.activeSelf) continue;
            //if (pc.gameObject == _caster.gameObject) continue;
            randomPlayer.Add(pc);
        }

        if (randomPlayer.Count > 0)
        {
            int randNum = UnityEngine.Random.Range(0, randomPlayer.Count);
            return randomPlayer[randNum];
        }

        return null;
    }

    //가장 가까이 있는 적 찾기
    public static CreatureController FindNearEnemy(CreatureController _caster)
    {
        CreatureController nearEnemy = null;
        List<MonsterController> mcList = Managers.ObjectM.mcList;

        float attackRange = _caster.AttackRange;
        float nearRange = attackRange;
        foreach (MonsterController monster in mcList)
        {
            if (monster.gameObject.activeSelf == false) continue;
            float distance = Vector3.Distance(_caster.transform.position, monster.transform.position);
            if (distance < nearRange)
            {
                nearRange = distance;
                nearEnemy = monster;
            }
        }

        if (nearEnemy != null) return nearEnemy;
        else return null;
    }

    //주변 사정거리 내에 랜덤한 적 찾기
    public static CreatureController FindRandomEnemyInRange(CreatureController _casster, float _range)
    {
        CreatureController randomEnemy = null;
        List<MonsterController> mclist = Managers.ObjectM.mcList;
        List<MonsterController> enemyInRange = new List<MonsterController>();

        foreach (MonsterController monster in mclist)
        {
            if (monster.gameObject.activeSelf == false) continue;
            float distance = Vector3.Distance(_casster.transform.position, monster.transform.position);
            if (distance < _range)
            {
                enemyInRange.Add(monster);
            }
        }

        if (enemyInRange.Count > 0)
        {
            int randNum = UnityEngine.Random.Range(0, enemyInRange.Count);

            return enemyInRange[randNum];
        }

        return null;
    }


    //주변 원형적들 찾는 함수
    public static List<CreatureController> FindEnemyInSphereArea(CreatureController _caster, float _radius)
    {
        List<CreatureController> hitEnemies = new List<CreatureController>();
        Vector3 center = _caster.transform.position;

        List<MonsterController> mclist = Managers.ObjectM.mcList;

        foreach (MonsterController monster in mclist)
        {
            if (monster.gameObject == _caster.gameObject) continue;

            float distance = Vector3.Distance(center, monster.transform.position);

            if (distance <= _radius)
            {
                hitEnemies.Add(monster);
            }
        }

        return hitEnemies;
    }


    //전방의 적들 찾는 함수
    public static List<CreatureController> FindEnemyForwardArea(CreatureController _caster, float _length, float _width)
    {
        List<CreatureController> hitEnemies = new List<CreatureController>();

        List<MonsterController> mclist = Managers.ObjectM.mcList;

        foreach (MonsterController monster in mclist)
        {
            /// 이렇게 하면 시전자가 어느 방향을 보고 있든 전방(Z축), 좌우(X축) 기준으로 체크할 수 있음 
            Vector3 localTargetPos = _caster.transform.InverseTransformPoint(monster.transform.position);

            // Z축 (앞) 확인: 0 < Z < Length (시전자 바로 앞부터 사거리 이내)
            if (localTargetPos.z > 0 && localTargetPos.z <= _length)
            {
                // X축 (좌우 폭) 확인: -Width/2 < X < Width/2 (공격 폭 이내)
                if (Mathf.Abs(localTargetPos.x) <= _width / 2f)
                {
                    hitEnemies.Add(monster);
                }
            }
        }

        return hitEnemies;
    }


    #endregion


    #region 이펙트 헬퍼함수
    public static string GetVfxPrefabName(int _vfxId)
    {
        if (Managers.DataM.VFXDataDic.TryGetValue(_vfxId, out var vfxData))
        {
            return vfxData.PrefabName;
        }

        Debug.LogError($"VFX ID {_vfxId}에 해당하는 PrefabName을 찾을 수 없습니다.");
        return null;
    }
    #endregion
    //몬스터 소환 위치
    public static Vector3 CreateMonsterSpawnPoint()
    {

        Vector3 spawnPos = Vector3.zero + UnityEngine.Random.insideUnitSphere * 5f;
        spawnPos.y = 0.0f;

        return spawnPos;
    }

    #region 지수증가 공식(레벨, 스테이지데이터)
    public static double Money()
    {
        return Utils.CalculatedValue(
            Utils.Datas.stageData.Base_Gold,
            Managers.GameM.Stage,
            Utils.Datas.stageData.Monster_Gold);
    }
    public static double CalculatedValue(float _baseValue, int _level, float _value)
    {
        return _baseValue * Mathf.Pow(_level, _value);
    }
    #endregion
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
            Debug.LogWarning(string.Format("Failed - ToCurrencyString({0}) : No Exponent found", _number));
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

            if (quotient < CurrencyUnits.Length)
                unitString = CurrencyUnits[quotient];
            else
                unitString = "Infinity";
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
        ColorUtility.TryParseHtmlString(_color, out parsedColor);

        return parsedColor;
    }

    public static string StringToColorGrade(Define.Grade _grade)
    {
        switch (_grade)
        {
            case Define.Grade.Common: return "<color=#FFFFFF>";
            case Define.Grade.UnCommon: return "<color=#00FF00>";
            case Define.Grade.Rare: return "<color=#0000FF>";
            case Define.Grade.Unique: return "<color=#BB45FF>";
            case Define.Grade.Legendary: return "<color=#FF9A45>";
        }

        return "<color=#FFFFFF>";
    }

    public static string StringToColorGradeImage(Define.Grade _grade)
    {
        switch (_grade)
        {
            case Define.Grade.Common: return "#FFFFFF";
            case Define.Grade.UnCommon: return "#00FF00";
            case Define.Grade.Rare: return "#0000FF";
            case Define.Grade.Unique: return "#BB45FF";
            case Define.Grade.Legendary: return "#FF9A45";
        }

        return "#FFFFFF";
    }

    #endregion

    #region Time
    public static double TimerCheck()
    {
        if (Managers.GameM.EndDate == "" || Managers.GameM.StartDate == "")
        {
            return 0.0d;
        }

        DateTime startDate = DateTime.Parse(Managers.GameM.StartDate);
        if (Managers.GameM.EndDate != null && Managers.GameM.EndDate != "")
        {
            DateTime endDate = DateTime.Parse(Managers.GameM.EndDate);

            TimeSpan timer = startDate - endDate;
            double timeCount = timer.TotalSeconds;

            return timeCount;

        }
        else return 0;
       
    }

    public static string NextDayTimer()
    {
        DateTime nowDate = DateTime.Now;
        DateTime nextDate = nowDate.AddDays(1);
        nextDate = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, 0, 0, 0);
        TimeSpan timer = nextDate - nowDate;

        return timer.ToString(@"hh\ \:\ mm\ \:\ ss");

    }
    #endregion



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

    public static int Summon_Level(int _count)
    {
        if (_count >= Managers.DataM.GachaDataDic[GachaMaxLevel].SummonCount)
        {
            return GachaMaxLevel;
        }

        for (int i = 1; i <= GachaMaxLevel; i++)
        {
            if (_count < Managers.DataM.GachaDataDic[i].SummonCount)
            {
                return i;
            }
        }

        return -1;
    }
    public static float[] Gacha_Percentage(Define.GachaType _type)
    {
        float[] valueCount = new float[GradeCount];
        switch (_type)
        {
            case Define.GachaType.HeroGacha:
                valueCount = Managers.DataM.GachaDataDic[Summon_Level(Managers.GameM.Hero_Summon_Count)].GetRates();
                break;

            case Define.GachaType.RelicGacha:
                valueCount = Managers.DataM.GachaDataDic[Summon_Level(Managers.GameM.Relics_Summon_Count)].GetRates();
                break;
        }


        return valueCount;
    }

    #region Smelt
    public static readonly int[] SmeltSlotCountWeights = { 50, 25, 15, 9, 1 };

    #endregion
}
