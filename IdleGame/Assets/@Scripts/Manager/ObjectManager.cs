using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public PlayerController mPlayer { get; private set; }
    public HashSet<PlayerController> pcSet { get; } = new HashSet<PlayerController>();
    public HashSet<MonsterController> mcSet { get; } = new HashSet<MonsterController>();
    public HashSet<ProjectileController> pjSet { get; } = new HashSet<ProjectileController>();
    public HashSet<ObjectController> ocSet { get; } = new HashSet<ObjectController>();

    private Dictionary<Type, Action<BaseController, Vector3,int, CreatureController, CreatureController>> initActions;
    private Dictionary<Type, Action<BaseController>> removeActions;
    private string GetPrefabNames<T>(int _tempID) where T : BaseController
    {
        if(typeof(T).IsSubclassOf(typeof(CreatureController)) || typeof(T) == typeof(CreatureController))
        {
            Managers.DataM.CreatureDataDic.TryGetValue(_tempID, out var data);
            return data?.prefabName;
        }
        else if (typeof(T).IsSubclassOf(typeof(ProjectileController)) || typeof(T) == typeof(ProjectileController))
        {
            Managers.DataM.ProjectileDataDic.TryGetValue(_tempID, out var data);
            return data?.prefabName;
        }
        else if (typeof(T).IsSubclassOf(typeof(ObjectController)) || typeof(T) == typeof(ObjectController))
        {
            //TODO : 데이터 추가해서 수정하기
            return "Smoke";
        }

        Debug.LogError("ID에 맞는 오브젝트 타입이 없습니다.");
        return null;

    }
    public void Init()
    {
        #region 딕셔너리로 관리 스폰시 초기화 관리
        initActions = new Dictionary<Type, Action<BaseController, Vector3, int, CreatureController, CreatureController>>();
        initActions.Add(typeof(PlayerController), (baseController, pos, tempId, owner, target) =>
        {
            var player = baseController as PlayerController;
            Managers.DataM.CreatureDataDic.TryGetValue(tempId, out var data);

            if (tempId == 1) mPlayer = player;
            player.Init();
            pcSet.Add(player);
            if (data != null) player.SetInfo(data);
        });

        initActions.Add(typeof(MonsterController), (baseController, pos, tempId, owner, target) =>
        {
            var monster = baseController as MonsterController;

            monster.Init();
            mcSet.Add(monster);
        });

        initActions.Add(typeof(RangeAttackController), (baseController, pos, tempId, owner, target) =>
        {
            var rangeAttack = baseController as RangeAttackController;
            rangeAttack.AttackInit(target as MonsterController, 10, owner);
        });

        initActions.Add(typeof(MeleeAttackController), (baseController, pos, tempId, owner, target) =>
        {
            var meleeAttack = baseController as MeleeAttackController;
            meleeAttack.AttackInit(target, 10, owner);
        });

        initActions.Add(typeof(ObjectController), (baseController, pos, tempId, owner, target) =>
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
            pcSet.Remove(pc);
        });

        removeActions.Add(typeof(MonsterController), (baseController) =>
        {
            var mc = baseController as MonsterController;
            mcSet.Remove(mc);
        });
        #endregion
        //TODO : 오브젝트들은 찾을 일이 없으니까 hashset에 안넣어도 될거같긴함
        //removeActions.Add(typeof(RangeAttackController), (baseController) =>
        //{
        //    var rac = baseController as RangeAttackController;
        //    pjSet.Remove(rac);
        //});
        //removeActions.Add(typeof(MeleeAttackController), (baseController) =>
        //{
        //    var mac = baseController as MeleeAttackController;
        //    pjSet.Remove(mac);
        //});
        //removeActions.Add(typeof(ObjectController), (baseController) =>
        //{
        //    var oc = baseController as ObjectController;
        //    ocSet.Remove(oc);
        //});


    }

    public T Spawn<T>(Vector3 _pos, int _tempId = 0, CreatureController _owner = null, CreatureController _target = null) where T : BaseController
    {

        string prefabName = GetPrefabNames<T>(_tempId);
        if(string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("[ObjectManager] ID에 맞는 오브젝트가 없음");
            return null;
        }
        GameObject go = Managers.ResourceM.Instantiate(prefabName, _pooling: true);

        if(go == null)
        {
            Debug.LogError("[ObjectManager] Instantiate 실패");
            return null;
        }
        go.transform.position = _pos;

        T controller = go.GetOrAddComponent<T>();
        if(controller == null)
        {
            Debug.LogError($"[ObjectManager] Controller 불러오기 실패 {typeof(T).Name}");
            return null;
        }

        Type type = controller.GetType();
        if(initActions.TryGetValue(type, out var initAction))
        {
            initAction(controller, _pos, _tempId, _owner, _target);
        }
        else
        {
            controller.Init();
        }

        return controller;
    }

  
    public void DeSpawn<T>(T _obj) where T : BaseController
    {
        if (_obj == null || !_obj.IsValid()) return;

        if(removeActions.TryGetValue(typeof(T), out var removeAction))
        {
            removeAction(_obj);
        }

        Managers.Destroy(_obj.gameObject);
    }

    public void ShowDamageFont(Vector3 _pos, double _dmg, bool _isMonster = false, bool _isCritical = false)
    {
        //TODO : 하드코딩 제거
        //Spawn<DamageFont>(_pos, );
        string prefabName = "DamageFont";

        GameObject go = Managers.ResourceM.Instantiate(prefabName, _pooling: true);
        DamageFont damageFont = go.GetOrAddComponent<DamageFont>();
        damageFont.Init(_pos, _dmg, _isMonster,_isCritical);

    }
}
