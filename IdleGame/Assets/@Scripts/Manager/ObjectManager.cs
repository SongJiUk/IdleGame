using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectManager
{
    public PlayerController mPlayer { get; private set; }
    public List<PlayerController> pcList { get; } = new List<PlayerController>();
    public List<MonsterController> mcList { get; } = new List<MonsterController>();
    public List<ProjectileController> pjList { get; } = new List<ProjectileController>();
    public List<ObjectController> ocList { get; } = new List<ObjectController>();

    private Dictionary<Type, Action<BaseController, Vector3, int, CreatureController, CreatureController, bool>> initActions;
    private Dictionary<Type, Action<BaseController>> removeActions;
    private string GetPrefabNames<T>(int _tempID) where T : BaseController
    {
        if (typeof(T).IsSubclassOf(typeof(CreatureController)) || typeof(T) == typeof(CreatureController))
        {
            Managers.DataM.CreatureDataDic.TryGetValue(_tempID, out var data);
            return data?.PrefabName;
        }
        else if (typeof(T).IsSubclassOf(typeof(ProjectileController)) || typeof(T) == typeof(ProjectileController))
        {
            Managers.DataM.ProjectileDataDic.TryGetValue(_tempID, out var data);
            return data?.PrefabName;
        }
        else if (typeof(T).IsSubclassOf(typeof(UIDirecting)) || typeof(T) == typeof(UIDirecting))
        {
            Managers.DataM.VFXDataDic.TryGetValue(_tempID, out var data);
            return data?.PrefabName;
        }

        Debug.LogError("ID에 맞는 오브젝트 타입이 없습니다.");
        return null;

    }
    public void Init()
    {
        #region 딕셔너리로 관리 스폰시 초기화 관리
        initActions = new Dictionary<Type, Action<BaseController, Vector3, int, CreatureController, CreatureController, bool>>();
        initActions.Add(typeof(PlayerController), (baseController, pos, tempId, owner, target, isSkillProjectile) =>
        {
            var player = baseController as PlayerController;
            Managers.DataM.CreatureDataDic.TryGetValue(tempId, out var data);

            //TODO : 이거 수정해야됌 isMainCharacter 이런걸로 
            if (tempId == 9) mPlayer = player;
            player.Init();
            player.transform.position = pos;
            //player.transform.rotation = Quaternion.identity;
            pcList.Add(player);
            if (data != null) player.SetInfo(data);
        });

        initActions.Add(typeof(MonsterController), (baseController, pos, tempId, owner, target, isSkillProjectile) =>
        {
            var monster = baseController as MonsterController;
            Managers.DataM.CreatureDataDic.TryGetValue(tempId, out var data);
            monster.Init();
            monster.SetInfo(data);
            mcList.Add(monster);
        });

        initActions.Add(typeof(RangeAttackController), (baseController, pos, tempId, owner, target, isSkillProjectile) =>
        {
            var rangeAttack = baseController as RangeAttackController;
            rangeAttack.AttackInit(target, owner.Damage, owner, isSkillProjectile);
        });

        initActions.Add(typeof(MeleeAttackController), (baseController, pos, tempId, owner, target, isSkillProjectile) =>
        {
            var meleeAttack = baseController as MeleeAttackController;
            meleeAttack.AttackInit(target, owner.Damage, owner);
        });

        initActions.Add(typeof(ObjectController), (baseController, pos, tempId, owner, target, isSkillProjectile) =>
        {
            var obj = baseController as ObjectController;
            obj.Init();
        });

        //initActions.Add(typeof(CoinDirecting), (baseController, pos, tempId, owner, target) =>
        //{
        //    var cd = baseController as CoinDirecting;
        //    cd.Init(pos);
        //});

        //initActions.Add(typeof(DamageFont), (baseController, pos, tempId, owner, target) =>
        //{
        //    var df = baseController as DamageFont;
        //    //TODO : 방법 생각해보기
        //    df.Init();
        //});

        //initActions.Add(typeof(CoinDirecting), (baseController, pos, tempId, owner, target) =>
        //{
        //    var cd = baseController as CoinDirecting;
        //    cd.Init(pos);
        //});
        #endregion

        #region 제거

        removeActions = new Dictionary<Type, Action<BaseController>>();
        removeActions.Add(typeof(PlayerController), (baseController) =>
        {
            var pc = baseController as PlayerController;
            pcList.Remove(pc);
        });

        removeActions.Add(typeof(MonsterController), (baseController) =>
        {
            var mc = baseController as MonsterController;
            mcList.Remove(mc);
        });
        #endregion



    }

    public T Spawn<T>(Vector3 _pos, int _tempId = 0, CreatureController _owner = null, CreatureController _target = null, bool _isSkillProjectile = false) where T : BaseController
    {

        string prefabName = GetPrefabNames<T>(_tempId);
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("[ObjectManager] ID에 맞는 오브젝트가 없음");
            return null;
        }
        GameObject go = Managers.ResourceM.Instantiate(prefabName, _pooling: true);

        if (go == null)
        {
            Debug.LogError("[ObjectManager] Instantiate 실패");
            return null;
        }
        go.transform.position = _pos;

        T controller = go.GetOrAddComponent<T>();
        if (controller == null)
        {
            Debug.LogError($"[ObjectManager] Controller 불러오기 실패 {typeof(T).Name}");
            return null;
        }

        Type type = controller.GetType();
        if (initActions.TryGetValue(type, out var initAction))
        {
            initAction(controller, _pos, _tempId, _owner, _target, _isSkillProjectile);
        }
        else
        {
            controller.Init();
        }

        return controller;
    }

    public T SpawnUI<T>(string _prefabName) where T : UI_Base
    {
        if (string.IsNullOrEmpty(_prefabName))
        {
            Debug.LogError("[ObjectManager] UI 프리팹 이름이 비어있음 ");
            return null;
        }
        GameObject go = Managers.ResourceM.Instantiate(_prefabName, _pooling: true);
        if (go == null)
        {
            Debug.LogError("[ObjectManager] : UI Instantiate 실패! " + _prefabName);
            return null;
        }

        T uiController = go.GetOrAddComponent<T>();
        if (uiController == null)
        {
            Debug.LogError("[ObjectManager] : UI Controller호출 실패");
            return null;
        }

        return uiController;
    }

    public void DeSpawn<T>(T _obj) where T : BaseController
    {
        if (_obj == null || !_obj.IsValid()) return;

        if (removeActions.TryGetValue(typeof(T), out var removeAction))
        {
            removeAction(_obj);
        }

        Managers.ResourceM.Destroy(_obj.gameObject);
    }


    public void DeSpawnUI<T>(T _obj) where T : UI_Base
    {
        if (_obj == null || !_obj.IsValid()) return;
        Managers.ResourceM.Destroy(_obj.gameObject);
    }

    public void ShowDamageFont(Vector3 _pos, double _dmg, bool _isMonster = false, bool _isCritical = false, bool _isSkill = false)
    {
        //TODO : 하드코딩 제거
        //Spawn<DamageFont>(_pos, );
        string prefabName = "DamageFont";

        DamageFont damageFont = Spawn<DamageFont>(_pos, Utils.DamageFontDataID);
        // GameObject go = Managers.ResourceM.Instantiate(prefabName, _pooling: true);
        //DamageFont damageFont = go.GetOrAddComponent<DamageFont>();
        damageFont.Init(_pos, _dmg, _isMonster, _isCritical, _isSkill);

    }
}
