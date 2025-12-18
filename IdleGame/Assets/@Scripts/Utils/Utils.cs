using System.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using System.Linq;

public static class Utils
{

    //TODO : 확률 이거 나중에 데이터로 관리하기
    public static float[] Gacha_Percentage = { 60.0f, 20.0f, 10.0f, 6.0f, 4.0f };

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






    //// DamageEffect.cs

    //public class DamageEffect : ISkillEffect
    //{
    //    private float damageMultiplier; // 피해 배율 (예: 1.0f = 100%)
    //    private float splashRadius = 0f; // 광역 피해 반경 (0이면 단일 타겟)
    //    private float splashMultiplier = 0f; // 광역 피해 배율

    //    public DamageEffect(float multiplier, float radius = 0f, float splashM = 0f)
    //    {
    //        this.damageMultiplier = multiplier;
    //        this.splashRadius = radius;
    //        this.splashMultiplier = splashM;
    //    }

    //    public void Excute(CreatureController _caster, CreatureController _target)
    //    {
    //        if (_target == null) return;

    //        // 1. 메인 타겟에게 피해 적용
    //        float baseDamage = _caster.Damage * damageMultiplier;
    //        _target.TakeDamage(baseDamage); // CreatureController에 TakeDamage 함수가 있다고 가정

    //        // 2. 광역 피해 (Splash Damage) 처리
    //        if (splashRadius > 0 && splashMultiplier > 0)
    //        {
    //            ApplySplashDamage(_caster, _target.transform.position, splashRadius, splashMultiplier);
    //        }
    //    }

    //    private void ApplySplashDamage(CreatureController _caster, Vector3 center, float radius, float multiplier)
    //    {
    //        // ⭐️ Physics.OverlapSphere 등을 사용하여 주변 적들을 찾습니다.
    //        // (Managers.ObjectM 또는 유니티 Physics API 사용)
    //        Collider[] hitColliders = Physics.OverlapSphere(center, radius, LayerMask.GetMask("Enemy"));

    //        float splashDamage = _caster.Damage * multiplier;

    //        foreach (var hit in hitColliders)
    //        {
    //            CreatureController target = hit.GetComponent<CreatureController>();
    //            // 메인 타겟에게는 광역 피해를 중복으로 주지 않도록 제외 로직 필요
    //            if (target != null && target != _caster)
    //            {
    //                target.TakeDamage(splashDamage);
    //            }
    //        }
    //    }
    //}


    //// WizardMale_Meteor.cs

    //public class WizardMale_Meteor : SkillBase
    //{
    //    public WizardMale_Meteor() { SetUpEffect(); }

    //    protected override void SetUpEffect()
    //    {
    //        // 피해량 = 150% (main) + 60% (splash)
    //        // DamageEffect(배율, 광역반경, 광역배율)
    //        // 광역반경: 5.0f (가정)
    //        effects.Add(new DamageEffect(1.5f, 5.0f, 0.6f));
    //    }

    //    // ⭐️ 광역 스킬이므로 UseSkill을 오버라이드하여 타겟 위치를 활용합니다.
    //    public override void UseSkill(CreatureController _caster, CreatureController _target)
    //    {
    //        // 메테오는 단일 타겟(_target)을 메인 타겟으로 삼아 그 주변에 광역 피해를 줍니다.
    //        if (_target != null)
    //        {
    //            foreach (var effect in effects)
    //            {
    //                // DamageEffect.Excute()가 호출되어 메인 타겟 및 주변에 피해를 줍니다.
    //                effect.Excute(_caster, _target);
    //            }
    //        }
    //        // ... (쿨타임 처리)
    //    }
    //}

    //// WizardFemale_RandomBuff.cs

    //public class WizardFemale_RandomBuff : SkillBase
    //{
    //    public WizardFemale_RandomBuff() { SetUpEffect(); }

    //    // 공격력 버프와 방어력 버프를 미리 준비합니다.
    //    // (AttackBuff와 DefenseBuff 클래스가 있다고 가정)
    //    private BuffEffect attackBuffEffect;
    //    private BuffEffect defenseBuffEffect;

    //    protected override void SetUpEffect()
    //    {
    //        // ⭐️ 효과는 여기서 정의하되, 적용은 UseSkill에서 랜덤으로 선택
    //        attackBuffEffect = new BuffEffect(duration => new AttackBuff(10f), 10f); // 10초 공격력 버프
    //        defenseBuffEffect = new BuffEffect(duration => new DefenseBuff(10f), 10f); // 10초 방어력 버프
    //    }

    //    public override void UseSkill(CreatureController _caster, CreatureController _target = null)
    //    {
    //        // 1. 체력이 가장 낮은 아군 찾는 로직 (Cleric과 유사)
    //        CreatureController randomAlly = GetRandomAlly(); // 아군 목록에서 랜덤 선택하는 함수

    //        if (randomAlly != null)
    //        {
    //            // 2. 랜덤 버프 선택 (0: 공격력, 1: 방어력)
    //            BuffEffect chosenEffect = (UnityEngine.Random.Range(0, 2) == 0) ? attackBuffEffect : defenseBuffEffect;

    //            // 3. 선택된 아군에게 버프 적용
    //            chosenEffect.Excute(_caster, randomAlly);
    //        }
    //    }

    //    private CreatureController GetRandomAlly()
    //    {
    //        // Managers.SpawnM.players 등 아군 목록에서 랜덤 플레이어를 찾아 반환
    //        // ...
    //        return null;
    //    }
    //}

    //// Spearman_ForwardAttack.cs

    //public class Spearman_ForwardAttack : SkillBase
    //{
    //    private const float RANGE = 5f; // 사거리 5m 가정

    //    public Spearman_ForwardAttack() { SetUpEffect(); }
    //    protected override void SetUpEffect()
    //    {
    //        // 단일 피해 (예: 120%)
    //        effects.Add(new DamageEffect(1.2f));
    //    }

    //    public override void UseSkill(CreatureController _caster, CreatureController _target = null)
    //    {
    //        // ⭐️ 전방 범위 내의 몬스터를 찾습니다.
    //        CreatureController nearestEnemy = FindForwardEnemy(_caster, RANGE); // 헬퍼 함수

    //        if (nearestEnemy != null)
    //        {
    //            // 찾은 타겟에게 효과 적용
    //            foreach (var effect in effects)
    //            {
    //                effect.Excute(_caster, nearestEnemy);
    //            }
    //        }
    //        // ...
    //    }

    //    // (헬퍼 함수) 시전자의 전방을 기준으로 가장 가까운 적을 찾는 함수
    //    private CreatureController FindForwardEnemy(CreatureController caster, float range)
    //    {
    //        // Physics.SphereCast 또는 Linecast를 사용하여 구현
    //        // ...
    //        return null;
    //    }
    //}


    //// Knight_ChargeAttack.cs

    //public class Knight_ChargeAttack : SkillBase
    //{
    //    private const float LENGTH = 8f; // 공격 길이 8m 가정
    //    private const float WIDTH = 2f;  // 공격 폭 2m 가정

    //    public Knight_ChargeAttack() { SetUpEffect(); }
    //    protected override void SetUpEffect()
    //    {
    //        // 단일 피해 (예: 150%)
    //        effects.Add(new DamageEffect(1.5f));
    //    }

    //    public override void UseSkill(CreatureController _caster, CreatureController _target = null)
    //    {
    //        // ⭐️ 특정 위치에 타겟팅하는 대신, 시전자의 전방 부채꼴/박스 범위 내의 모든 몬스터를 찾습니다.

    //        List<CreatureController> enemiesInArea = FindEnemiesInForwardArea(_caster, LENGTH, WIDTH);

    //        foreach (var enemy in enemiesInArea)
    //        {
    //            // 범위 내 모든 적에게 개별적으로 효과 적용
    //            foreach (var effect in effects)
    //            {
    //                effect.Excute(_caster, enemy);
    //            }
    //        }
    //        // ...
    //    }

    //    // (헬퍼 함수) 시전자의 전방 박스 범위 내의 모든 적을 찾는 함수
    //    private List<CreatureController> FindEnemiesInForwardArea(CreatureController caster, float length, float width)
    //    {
    //        // Physics.OverlapBox 등을 사용하여 구현
    //        // ...
    //        return new List<CreatureController>();
    //    }
    //}
}
