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
        public Define.CreatureType CreatureType;
        public float AttackRange;
        public Define.CharacterGrade CharacterGrade;
        public double BaseHp;
        public double BaseDamage;
        public int MaxMp;
        public int ProjectileDataID;
        public int SkillDataID;
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

    #region ItemData
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

    #region SkillData

    [Serializable]
    public class SkillData
    {
        public int DataID;
        public string SkillName;
        public string SkillNameKR;
        public string TargetVFX_ID_Raw;
        public string BuffList_ID_Raw;
        public int CastingVFX_ID;
        public List<int> TargetVFX_ID { get; private set; } = new List<int>();
        public List<int> BuffList_ID { get; private set; } = new List<int>();

        public void ParseRawData()
        {
            TargetVFX_ID = ParseIdList(TargetVFX_ID_Raw);
            BuffList_ID = ParseIdList(BuffList_ID_Raw);
        }

        List<int> ParseIdList(string _rawData)
        {
            List<int> result = new List<int>();
            if (string.IsNullOrEmpty(_rawData)) return result;
            string[] idStrings = _rawData.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string idString in idStrings)
            {
                string trimmedId = idString.Trim();

                if (int.TryParse(trimmedId, out int id))
                {
                    result.Add(id);
                }
                else
                {
                    Debug.LogError($"[SkillData {DataID}] ID 파싱 오류: '{trimmedId}'는 유효한 정수가 아닙니다.");
                }
            }
            return result;
        }
    }

    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> dataList = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            Dictionary<int, SkillData> dic = new Dictionary<int, SkillData>();

            foreach (var data in dataList)
            {
                data.ParseRawData();
                dic.Add(data.DataID, data);
            }
            return dic;
        }
    }
    #endregion

    #region SkillEffectData
    [Serializable]
    public class SkillEffectData
    {
        public int DataID;
        public string Description;
        public string SkillEffectType;
        public float SkillDuration;
        public float AnimDuration;
        public float ValueRatio;
        public float Radius;
        public float Length;
        public float Width;
        public float Interval;
        public int BuffTypeID;
    }

    public class SkillEffectDataLoader : ILoader<int, SkillEffectData>
    {
        public List<SkillEffectData> dataList = new List<SkillEffectData>();

        public Dictionary<int, SkillEffectData> MakeDict()
        {
            Dictionary<int, SkillEffectData> dic = new Dictionary<int, SkillEffectData>();
            foreach (var data in dataList)
            {
                dic.Add(data.DataID, data);
            }
            return dic;
        }
    }

    #endregion

    #region BuffTypeData
    [Serializable]
    public class BuffTypeData
    {
        public int DataID;
        public string BuffName;
    }

    public class BuffTypeDataLoader : ILoader<int, BuffTypeData>
    {
        public List<BuffTypeData> dataList = new List<BuffTypeData>();
        public Dictionary<int, BuffTypeData> MakeDict()
        {
            Dictionary<int, BuffTypeData> dic = new Dictionary<int, BuffTypeData>();
            foreach (var data in dataList)
            {
                dic.Add(data.DataID, data);
            }
            return dic;
        }
    }
    #endregion

    #region VFXData
    [Serializable]
    public class VFXData
    {
        public int DataID;
        public string PrefabName;
    }

    public class VFXDataLoader : ILoader<int, VFXData>
    {
        public List<VFXData> dataList = new List<VFXData>();
        public Dictionary<int, VFXData> MakeDict()
        {
            Dictionary<int, VFXData> dic = new Dictionary<int, VFXData>();
            foreach (var data in dataList)
            {
                dic.Add(data.DataID, data);
            }
            return dic;
        }
    }
    #endregion
}