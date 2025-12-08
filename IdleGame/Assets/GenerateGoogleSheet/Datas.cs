using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>You must approach through `GoogleSheetManager.SO<Datas>()`</summary>
public class Datas : ScriptableObject
{
	public List<WeaponData> WeaponDataList;
	public List<CreatureData> CreatureDataList;
	public List<ProjectileData> ProjectileDataList;
	public List<ItemData> ItemDataList;
	public List<StageSpawnData> StageSpawnDataList;
	public List<SkillData> SkillDataList;
	public List<SkillEffectData> SkillEffectDataList;
	public List<BuffTypeData> BuffTypeDataList;
	public List<VFXData> VFXDataList;
}

[Serializable]
public class WeaponData
{
	public int DataID;
	public string Name;
	public int Damage;
	public string Type;
}

[Serializable]
public class CreatureData
{
	public int DataID;
	public string Name;
	public string NameKR;
	public string Description;
	public string prefabName;
	public string CreatureType;
	public string Type;
	public int AttackRange;
	public string CharacterGrade;
	public int BaseHp;
	public int BaseDamage;
	public int MaxMp;
	public int ProjectileDataID;
	public int SkillDataID;
}

[Serializable]
public class ProjectileData
{
	public int DataID;
	public string Name;
	public string NameKR;
	public string Description;
	public string prefabName;
	public string AttackType;
}

[Serializable]
public class ItemData
{
	public int DataID;
	public string Name;
	public string NameKR;
	public string Description;
	public string ItemGrade;
	public int Probability;
}

[Serializable]
public class StageSpawnData
{
	public int Level;
	public int SpawnMaxCount;
	public int SpawnTimer;
	public int StageClearMaxCount;
}

[Serializable]
public class SkillData
{
	public int DataID;
	public string SkillName;
	public string SkillNameKR;
	public int CastingVFX_ID_Raw;
	public string TargetVFX_ID_Raw;
	public string BuffList_ID_Raw;
	public int SkillProjectileID;
}

[Serializable]
public class SkillEffectData
{
	public int DataID;
	public string Description;
	public string SkillEffectType;
	public float SkillDuration;
	public float AnimDuration;
	public float ValueRatio;
	public int Radius;
	public int Length;
	public int Width;
	public float Interval;
	public int BuffTypeID;
}

[Serializable]
public class BuffTypeData
{
	public int DataID;
	public string BuffName;
}

[Serializable]
public class VFXData
{
	public int DataID;
	public string PrefabName;
}

