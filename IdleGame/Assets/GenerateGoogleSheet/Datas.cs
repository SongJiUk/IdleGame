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
	public string Type;
	public int AttackRange;
	public string CharacterGrade;
	public int BaseHp;
	public int BaseDamage;
	public int MaxMp;
	public int ProjectileDataID;
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

