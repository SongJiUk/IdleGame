using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region Localization
    [Serializable]
    public class Localization
    {
        public string key;
        public string ko;
        public string en;
        public string ja;
    }
    [Serializable]
    public class LocalizationLoader : ILoader<string, Localization>
    {
        public LocalizationLoader() { }
        public List<Localization> dataList = new List<Localization>();
        public Dictionary<string, Localization> MakeDict()
        {
            Dictionary<string, Localization> dic = new Dictionary<string, Localization>();
            foreach (Localization data in dataList)
            {
                dic.Add(data.key, data);
            }

            return dic;
        }
    }

    #endregion


    #region CreatureData
    [Serializable]
    public class CreatureData
    {
        public int DataID;
        public string Name;
        public string NameKR;
        public string Description;
        public string PrefabName;
        public Define.CreatureType CreatureType;
        public Define.ObjectType ObjectType;
        public float AttackRange;
        public Define.Grade CharacterGrade;
        public double BaseHp;
        public double BaseDamage;
        public double BaseDefense;
        public int MaxMp;
        public float AttackSpeed;
        public int ProjectileDataID;
        public int SkillDataID;

    }

    [Serializable]
    public class CreatureDataLoader : ILoader<int, CreatureData>
    {
        public CreatureDataLoader() { }
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
        public Define.Grade Type;
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
        public string PrefabName;
        public Define.AttackType AttackType;
    }
    [Serializable]
    public class ProjectileDataLoader : ILoader<int, ProjectileData>
    {
        public ProjectileDataLoader() { }
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
        public string PrefabName;
        public string Description;
        public string RelicEffectDes;
        public Define.Grade ItemGrade;
        public float Probability;
        public Define.ItemType ItemType;
        public int MinStage;
        public string BaseValue;
        public float GrowValue;
        public int MaxLevel;
        public List<float> BaseValueList { get; private set; } = new List<float>();

        public void ParseRawData()
        {
            BaseValueList = ParseIdList(BaseValue);
        }

        List<float> ParseIdList(string _rawData)
        {
            List<float> result = new List<float>();
            if (string.IsNullOrEmpty(_rawData)) return result;
            string[] idStrings = _rawData.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string idString in idStrings)
            {
                string trimmedId = idString.Trim();

                if (int.TryParse(trimmedId, out int id))
                {
                    if (id == 0) continue;

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

    [Serializable]
    public class ItemDataLoader : ILoader<int, ItemData>
    {
        public ItemDataLoader() { }
        public List<ItemData> dataList = new List<ItemData>();

        public Dictionary<int, ItemData> MakeDict()
        {
            Dictionary<int, ItemData> dic = new Dictionary<int, ItemData>();
            foreach (var data in dataList)
            {
                data.ParseRawData();
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

    [Serializable]
    public class StageSpawnDataLoader : ILoader<int, StageSpawnData>
    {
        public StageSpawnDataLoader() { }
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
        public string Description;
        public int SkillAttackCount;
        public float SkillDuration;
        public float SkillInterval;
        public float SkillDamageMul;
        public float SkillCoolTime;
        public float SkillRadius;
        public float SkillLength;
        public float SkillWidth;
        public float AnimDuration;
        public string SkillEffect;
        public bool IsSplash;
        public string CastingVFX_ID_Raw;
        public string TargetVFX_ID_Raw;
        public string BuffList_ID_Raw;
        public int SkillProjectileID;
        public List<int> CastingVFX_ID { get; private set; } = new List<int>();
        public List<int> TargetVFX_ID { get; private set; } = new List<int>();
        public List<int> BuffList_ID { get; private set; } = new List<int>();

        public void ParseRawData()
        {
            CastingVFX_ID = ParseIdList(CastingVFX_ID_Raw);
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
    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public SkillDataLoader() { }
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

    #region BuffData
    [Serializable]
    public class BuffData
    {
        public int DataID;
        public string Description;
        public string SkillEffectType;
        public float BuffDuration;
        public float ValueRatio;
        public float Interval;
        public int BuffTypeID;
    }
    [Serializable]
    public class BuffDataLoader : ILoader<int, BuffData>
    {
        public BuffDataLoader() { }
        public List<BuffData> dataList = new List<BuffData>();

        public Dictionary<int, BuffData> MakeDict()
        {
            Dictionary<int, BuffData> dic = new Dictionary<int, BuffData>();
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
    [Serializable]
    public class BuffTypeDataLoader : ILoader<int, BuffTypeData>
    {
        public BuffTypeDataLoader() { }
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
    [Serializable]
    public class VFXDataLoader : ILoader<int, VFXData>
    {
        public VFXDataLoader() { }
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

    #region GachaData
    [Serializable]
    public class GachaData
    {
        public int Level;
        public float Common;
        public float UnCommon;
        public float Rare;
        public float Unique;
        public float Legendary;
        public int SummonCount;
        public float[] GetRates() => new float[] { Common, UnCommon, Rare, Unique, Legendary };
    }
    [Serializable]
    public class GachaDataLoader : ILoader<int, GachaData>
    {
        public GachaDataLoader() { }
        public List<GachaData> dataList = new List<GachaData>();

        public Dictionary<int, GachaData> MakeDict()
        {
            Dictionary<int, GachaData> dic = new Dictionary<int, GachaData>();
            foreach (var data in dataList)
            {
                dic.Add(data.Level, data);
            }

            return dic;
        }
    }

    // public class GachaTableData
    // {
    //     public Define.GachaType Type;
    //     public List<GachaRateData> GachaRateTable = new List<GachaRateData>();
    // }

    // public class GachaDataLoader : ILoader<Define.GachaType, GachaTableData>
    // {
    //     public List<GachaTableData> list = new List<GachaTableData>();

    //     public Dictionary<Define.GachaType, GachaTableData> MakeDict()
    //     {
    //         Dictionary<Define.GachaType, GachaTableData> dic = new Dictionary<Define.GachaType, GachaTableData>();
    //         foreach (GachaTableData gacha in list)
    //             dic.Add(gacha.Type, gacha);

    //         return dic;
    //     }
    // }




    // public class GachaRateData
    // {
    //     public string EquipmentID;
    //     public float GachaRate;
    //     public Define.EquipmentGrade EquipGrade;
    // }

    #endregion

    #region DungeonData

    [Serializable]
    public class DungeonData
    {
        public int DataID;
        public string DungeonName;
        public string DungeonNameKR;
        public int ResultDataID;

    }
    [Serializable]
    public class DungeonDataLoader : ILoader<int, DungeonData>
    {
        public DungeonDataLoader() { }
        public List<DungeonData> dataList = new List<DungeonData>();

        public Dictionary<int, DungeonData> MakeDict()
        {
            Dictionary<int, DungeonData> dic = new Dictionary<int, DungeonData>();
            foreach (var data in dataList)
            {
                dic.Add(data.DataID, data);
            }

            return dic;
        }
    }
    #endregion

    #region MissionData
    [Serializable]
    public class MissionData
    {
        public int MissionID;
        public Define.MissionType MissionType;
        public string MissionName;
        public string Description;
        public Define.MissionTarget MissionTarget;
        public int MissionTargetValue;
        public int ClearRewardItemID;
        public int RewardCount;
    }
    [Serializable]
    public class MissionDataLoader : ILoader<int, MissionData>
    {
        public MissionDataLoader() { }
        public List<MissionData> dataList = new List<MissionData>();
        public Dictionary<int, MissionData> MakeDict()
        {
            Dictionary<int, MissionData> dic = new Dictionary<int, MissionData>();
            foreach (var data in dataList)
            {
                dic.Add(data.MissionID, data);
            }

            return dic;
        }
    }
    #endregion

    #region AchievementData
    [Serializable]
    public class AchievementData
    {
        public int AchievementID;
        public Define.AchievementType AchievementType;
        public string Title;
        public Define.Status_Holder RewardStatus;
        public double RewardValue;
        public string AchievementCharacters;
        public string AchievementCharactersLevel;
        public string AchievementRelic;

        public List<int> AchievementCharactersList { get; private set; } = new List<int>();
        public List<int> AchievementCharactersLevelList { get; private set; } = new List<int>();
        public List<int> AchievementRelicList { get; private set; } = new List<int>();

        public void ParseRawData()
        {
            if (AchievementType == Define.AchievementType.Hero)
            {
                AchievementCharactersList = ParseIdList(AchievementCharacters);
                AchievementCharactersLevelList = ParseIdList(AchievementCharactersLevel);
            }
            else
                AchievementRelicList = ParseIdList(AchievementRelic);
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
                    Debug.LogError($"[AchievementData {AchievementID}] ID 파싱 오류: '{trimmedId}'는 유효한 정수가 아닙니다.");
                }
            }
            return result;
        }
    }


    [Serializable]
    public class AchievementDataLoader : ILoader<int, AchievementData>
    {
        public AchievementDataLoader() { }
        public List<AchievementData> dataList = new List<AchievementData>();
        public Dictionary<int, AchievementData> MakeDict()
        {
            Dictionary<int, AchievementData> dic = new Dictionary<int, AchievementData>();
            foreach (var data in dataList)
            {
                data.ParseRawData();
                dic.Add(data.AchievementID, data);
            }

            return dic;
        }
    }
    #endregion

    #region QuestData
    [Serializable]
    public class QuestData
    {
        public int Level;
        public Define.QuestType QuestType;
        public int Value;
        public int Reward;
    }
    [Serializable]
    public class QuestDataLoader : ILoader<int, QuestData>
    {
        public QuestDataLoader() { }
        public List<QuestData> dataList = new List<QuestData>();
        public Dictionary<int, QuestData> MakeDict()
        {
            Dictionary<int, QuestData> dic = new Dictionary<int, QuestData>();

            foreach (var data in dataList)
            {
                dic.Add(data.Level, data);
            }
            return dic;
        }

    }
    #endregion

    #region NPCData
    [Serializable]
    public class NPCData
    {
        public int DataID;
        public string NPCName;
        public string NPCNameKR;
        public string Speech;

        public List<string> SpeechList { get; private set; } = new List<string>();


        public void ParseRawData()
        {
            SpeechList = ParseList(Speech);
        }

        List<string> ParseList(string _rawData)
        {
            List<string> result = new List<string>();
            if (string.IsNullOrEmpty(_rawData)) return result;
            string[] idStrings = _rawData.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string idString in idStrings)
            {
                result.Add(idString);
            }
            return result;
        }
    }
    [Serializable]
    public class NPCDataLoader : ILoader<int, NPCData>
    {
        public NPCDataLoader() { }
        public List<NPCData> dataList = new List<NPCData>();
        public Dictionary<int, NPCData> MakeDict()
        {
            Dictionary<int, NPCData> dic = new Dictionary<int, NPCData>();
            foreach (var data in dataList)
            {
                data.ParseRawData();
                dic.Add(data.DataID, data);
            }

            return dic;
        }
    }
    #endregion

    #region SmeltData
    [Serializable]
    public class SmeltData
    {
        public int DataID;
        public string Name;
        public Define.Grade Grade;
        public string Description;
        public float Min;
        public float Max;
        public float Probability;
    }
    [Serializable]
    public class SmeltDataLoader : ILoader<int, SmeltData>
    {
        public SmeltDataLoader() { }
        public List<SmeltData> dataList = new List<SmeltData>();

        public Dictionary<int, SmeltData> MakeDict()
        {
            Dictionary<int, SmeltData> dic = new Dictionary<int, SmeltData>();
            foreach (var data in dataList)
            {
                dic.Add(data.DataID, data);
            }
            return dic;
        }
    }
    #endregion


}