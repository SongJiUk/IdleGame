using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public HashSet<PlayerController> pcSet { get; } = new HashSet<PlayerController>();
    public HashSet<MonsterController> mcSet { get; } = new HashSet<MonsterController>();


    public T Spawn<T>(Vector3 _pos, int _tempId = 0, string _prefabName = "") where T : CreatureController
    {
        Type type = typeof(T);

        //TODO : 화이팅..
        if (type == typeof(PlayerController))
        {
            GameObject go = Managers.ResourceM.Instantiate(Managers.DataM.CreatureDataDic[1].prefabName, _pooling: true);
            go.transform.position = _pos;
            T pc = go.GetOrAddComponent<T>();
            pcSet.Add(pc as PlayerController);
            pc.SetInfo();

            return pc as T;
        }

        if (type == typeof(MonsterController))
        {
            //TODO : 스폰
            GameObject go = Managers.ResourceM.Instantiate(Managers.DataM.CreatureDataDic[10000].prefabName, _pooling: true);
            T mc = go.GetOrAddComponent<T>();
            go.transform.position = _pos;

            if (mc is MonsterController monster)
            {
                monster.SetInfo();
                mc.name = "";
                mcSet.Add(monster);
            }

            return mc as T;

        }

        return null;
    }

    public void DeSpawn<T>(T _obj) where T : CreatureController
    {
        if (_obj == null || !_obj.IsValid()) return;

        Type type = typeof(T);

        if (type == typeof(MonsterController))
        {
            mcSet.Remove(_obj as MonsterController);
            Managers.ResourceM.Destory(_obj.gameObject);
        }
    }
}
