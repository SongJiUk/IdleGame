using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using UnityEditor.TextCore.Text;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;



public interface ILoader<key, value>
{
    Dictionary<key, value> MakeDict();
}
public class DataManager
{
    public Dictionary<int, Data.CreatureData> CreatureDataDic = new Dictionary<int, Data.CreatureData>();
    public Dictionary<int, Data.WeaponData> WeaponDataDic = new Dictionary<int, Data.WeaponData>();
    public Dictionary<int, Data.ProjectileData> ProjectileDataDic = new Dictionary<int, Data.ProjectileData>();
    public Dictionary<int, Data.ItemData> ItemDataDic = new Dictionary<int, Data.ItemData>();
    public Dictionary<int, Data.StageSpawnData> StageDataDic = new Dictionary<int, Data.StageSpawnData>();
    public Dictionary<int, Data.SkillData> SkillDataDic = new Dictionary<int, Data.SkillData>();
    public Dictionary<int, Data.BuffData> BuffDataDic = new Dictionary<int, Data.BuffData>();
    public Dictionary<int, Data.BuffTypeData> BuffTypeDataDic = new Dictionary<int, Data.BuffTypeData>();
    public Dictionary<int, Data.VFXData> VFXDataDic = new Dictionary<int, Data.VFXData>();
    public Dictionary<int, Data.GachaData> GachaDataDic = new Dictionary<int, Data.GachaData>();
    public void Init()
    {
        TextAsset jsonAsset = Managers.ResourceM.Load<TextAsset>("Datas.json");
        if (jsonAsset == null)
        {
            Debug.LogError("Datas.json is null");
            return;
        }

        JObject jsonObj = JObject.Parse(jsonAsset.text);

        CreatureDataDic = LoadJson<Data.CreatureDataLoader, int, Data.CreatureData>(jsonObj, "CreatureData").MakeDict();
        WeaponDataDic = LoadJson<Data.WeaponDataLoader, int, Data.WeaponData>(jsonObj, "WeaponData").MakeDict();
        ProjectileDataDic = LoadJson<Data.ProjectileDataLoader, int, Data.ProjectileData>(jsonObj, "ProjectileData").MakeDict();
        ItemDataDic = LoadJson<Data.ItemDataLoader, int, Data.ItemData>(jsonObj, "ItemData").MakeDict();
        StageDataDic = LoadJson<Data.StageSpawnDataLoader, int, Data.StageSpawnData>(jsonObj, "StageSpawnData").MakeDict();
        SkillDataDic = LoadJson<Data.SkillDataLoader, int, Data.SkillData>(jsonObj, "SkillData").MakeDict();
        BuffDataDic = LoadJson<Data.BuffDataLoader, int, Data.BuffData>(jsonObj, "BuffData").MakeDict();
        BuffTypeDataDic = LoadJson<Data.BuffTypeDataLoader, int, Data.BuffTypeData>(jsonObj, "BuffTypeData").MakeDict();
        VFXDataDic = LoadJson<Data.VFXDataLoader, int, Data.VFXData>(jsonObj, "VFXData").MakeDict();
        GachaDataDic = LoadJson<Data.GachaDataLoader, int, Data.GachaData>(jsonObj, "GachaData").MakeDict();
    }

    Loader LoadJson<Loader, Tkey, TValue>(JObject _jsonObj, string _dataName) where Loader : ILoader<Tkey, TValue> where TValue : class
    {
        if (_jsonObj.TryGetValue(_dataName, out JToken sheetToken))
        {
            string jsonArrayText = sheetToken.ToString(Newtonsoft.Json.Formatting.None);
            Type listType = typeof(List<>).MakeGenericType(typeof(TValue));
            System.Collections.IList dataList = JsonConvert.DeserializeObject(jsonArrayText, listType) as System.Collections.IList;

            if (dataList == null)
            {
                Debug.LogError($"'{_dataName}' 시트 데이터 역직렬화 실패! JSON 형식이나 TValue 타입을 확인하세요.");
                return default(Loader);
            }

            Loader loaderInstance = (Loader)Activator.CreateInstance(typeof(Loader));
            FieldInfo listField = typeof(Loader).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                                .FirstOrDefault(f => f.FieldType == listType);

            if (listField != null)
            {
                // 5. 찾은 필드에 역직렬화된 List<TValue>를 할당합니다.
                listField.SetValue(loaderInstance, dataList);
                return loaderInstance;
            }
            else
            {
                Debug.LogError($"Loader {typeof(Loader).Name}에서 List<{typeof(TValue).Name}> 타입의 필드를 찾을 수 없습니다.");
                return default(Loader);
            }
        }
        else
        {
            Debug.LogError($"JSON에 시트 키 '{_dataName}'가 존재하지 않습니다. Google Sheet 이름과 JSON 키를 확인하세요.");
            return default(Loader);
        }
    }
}

