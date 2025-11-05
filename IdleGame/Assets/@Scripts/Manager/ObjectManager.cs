using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public HashSet<PlayerController> pcSet { get; } = new HashSet<PlayerController>();
    public HashSet<MonsterController> mcSet { get; } = new HashSet<MonsterController>();
    public HashSet<ProjectileController> pjSet { get; } = new HashSet<ProjectileController>();
    public HashSet<ObjectController> ocSet { get; } = new HashSet<ObjectController>();


    public T Spawn<T>(Vector3 _pos, int _tempId = 0, string _ownerName = "", CreatureController _target = null) where T : BaseController
    {
        Type type = typeof(T);

        //TODO : 화이팅..
        if (type == typeof(PlayerController))
        {
            GameObject go = Managers.ResourceM.Instantiate(Managers.DataM.CreatureDataDic[1].prefabName, _pooling: true);
            go.transform.position = _pos;
            T pc = go.GetOrAddComponent<T>();
            
            if(pc is PlayerController player)
            {
                player.Init();
                pcSet.Add(pc as PlayerController);
            }
            return pc as T;
        }

        if (type == typeof(MonsterController))
        {
            GameObject go = Managers.ResourceM.Instantiate(Managers.DataM.CreatureDataDic[10000].prefabName, _pooling: true);
            go.transform.position = _pos;
            T mc = go.GetOrAddComponent<T>();

            if (mc is MonsterController monster)
            {
                monster.Init();
                mcSet.Add(monster);
            }
            return mc as T;
        }

        if(type == typeof(ProjectileController))
        {
            GameObject go = Managers.ResourceM.Instantiate(Managers.DataM.ProjectileDataDic[_tempId].prefabName, _pooling: true);
            go.transform.position = _pos;
            T pj = go.GetOrAddComponent<T>();


            if(pj is ProjectileController projectile)
            {
                projectile.Init(_target as MonsterController, 10, _ownerName);
                pjSet.Add(projectile);
            }

            return pj as T;
        }

        //TODO : 맞을때 이펙트 나오는거 고치기
        if(type == typeof(ObjectController))
        {
            GameObject go = Managers.ResourceM.Instantiate("Smoke", _pooling: true);
            go.transform.position = _pos;
            T oc = go.GetOrAddComponent<T>();

            if(oc is ObjectController objects)
            {
                objects.Init();
                ocSet.Add(objects);

            }

            return oc as T;
        }

        return null;
    }

    public void DeSpawn<T>(T _obj) where T : BaseController
    {
        if (_obj == null || !_obj.IsValid()) return;

        Type type = typeof(T);

        if (type == typeof(MonsterController))
        {
            mcSet.Remove(_obj as MonsterController);
            Managers.ResourceM.Destory(_obj.gameObject);
            return;
        }

        if(type == typeof(ProjectileController))
        {
            Managers.ResourceM.Destory(_obj.gameObject);
            return;
        }

        if(type == typeof(ObjectController))
        {
            Managers.ResourceM.Destory(_obj.gameObject);
            return;
        }
    }

    public void ShowDamageFont(Vector3 _pos, double _dmg, Transform _parent, bool _isCritical = false)
    {
        string prefabName = "DamageFont";

        GameObject go = Managers.ResourceM.Instantiate(prefabName, _pooling: true);
        DamageFont damageFont = go.GetOrAddComponent<DamageFont>();
        damageFont.Init(_pos, _dmg, true);
        
    }
}
