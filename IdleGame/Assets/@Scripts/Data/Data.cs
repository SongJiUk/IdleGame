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
        public Define.WeaponAbilityType Type;
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
}