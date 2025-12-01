using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region CreatureData
    [Serializable]
    public class CreatureData
    {
        public int DataID;
        public string Name;
        public string NameKR;
        public string Description;
        public string prefabName;
        public Define.ObjectType Type;
        public float AttackRange;
        public Define.CharacterGrade CharacterGrade;
        public double BaseHp;
        public double BaseDamage;
        public int MaxMp;
        public int ProjectileDataID;
    }

    public class CreatureDataLoader : ILoader<int, CreatureData>
    {
        public List<CreatureData> dataList = new List<CreatureData>();

        public Dictionary<int, CreatureData> MakeDict()
        {
            Dictionary<int, CreatureData> dic = new Dictionary<int, CreatureData>();
            foreach (CreatureData data in dataList)
            {
                dic.Add(data.DataID, data);
            }

            return dic;
        }
    }
    #endregion

    #region WeaponData
    [Serializable]
    public class WeaponData
    {
        public int DataID;
        public string Name;
        public float Damage;
        public Define.ItemGrade Type;
    }

    public class WeaponDataLoader : ILoader<int, WeaponData>
    {
        public List<WeaponData> dataList = new List<WeaponData>();

        public Dictionary<int, WeaponData> MakeDict()
        {
            Dictionary<int, WeaponData> dic = new Dictionary<int, WeaponData>();
            foreach (WeaponData data in dataList)
            {
                dic.Add(data.DataID, data);
            }

            return dic;
        }
    }
    #endregion

    #region ProjectileData
    [Serializable]
    public class ProjectileData
    {
        public int DataID;
        public string Name;
        public string NameKR;
        public string Description;
        public string prefabName;
        public Define.AttackType AttackType;
    }

    public class ProjectileDataLoader : ILoader<int, ProjectileData>
    {
        public List<ProjectileData> dataList = new List<ProjectileData>();

        public Dictionary<int, ProjectileData> MakeDict()
        {
            Dictionary<int, ProjectileData> dic = new Dictionary<int, ProjectileData>();
            foreach (ProjectileData data in dataList)
            {
                dic.Add(data.DataID, data);
            }

            return dic;
        }
    }

    #endregion

    #region
    [Serializable]
    public class ItemData
    {
        public int DataID;
        public string Name;
        public string NameKR;
        public string Description;
        public Define.ItemGrade ItemGrade;
        public int Probability;
    }

    public class ItemDataLoader : ILoader<int, ItemData>
    {
        public List<ItemData> dataList = new List<ItemData>();

        public Dictionary<int, ItemData> MakeDict()
        {
            Dictionary<int, ItemData> dic = new Dictionary<int, ItemData>();
            foreach (var data in dataList)
            {
                dic.Add(data.DataID, data);
            }

            return dic;
        }
    }
    #endregion

    #region StageSpawnData
    [Serializable]
    public class StageSpawnData
    {
        public int Level;
        public int SpawnMaxCount;
        public float SpawnTimer;
        public int StageClearMaxCount;
    }

    public class StageSpawnDataLoader : ILoader<int, StageSpawnData>
    {
        public List<StageSpawnData> dataList = new List<StageSpawnData>();

        public Dictionary<int, StageSpawnData> MakeDict()
        {
            Dictionary<int, StageSpawnData> dic = new Dictionary<int, StageSpawnData>();
            foreach (var data in dataList)
            {
                dic.Add(data.Level, data);
            }

            return dic;
        }
    }
    #endregion
}