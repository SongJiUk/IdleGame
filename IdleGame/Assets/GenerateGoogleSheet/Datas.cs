using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>You must approach through `GoogleSheetManager.SO<Datas>()`</summary>
public class Datas : ScriptableObject
{
	public List<WeaponData> WeaponDataList;
	public List<CreatureData> CreatureDataList;
	public List<ProjectileData> ProjectileDataList;
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

