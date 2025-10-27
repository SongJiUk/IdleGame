using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class GoogleSheetManager : MonoBehaviour
{
    [Tooltip("true: google sheet, false: local json")]
    [SerializeField] bool isAccessGoogleSheet = true;
    [Tooltip("Google sheet appsscript webapp url")]
    [SerializeField] string googleSheetUrl;
    [Tooltip("Google sheet avail sheet tabs. seperate `/`. For example `Sheet1/Sheet2`")]
    [SerializeField] string availSheets = "Sheet1/Sheet2";
    [Tooltip("For example `/GenerateGoogleSheet`")]
    [SerializeField] string generateFolderPath = "/GenerateGoogleSheet";
    [Tooltip("You must approach through `GoogleSheetManager.SO<GoogleSheetSO>()`")]
    public ScriptableObject googleSheetSO;

    string JsonPath => $"{Application.dataPath}{generateFolderPath}/GoogleSheetJson.json";
    string ClassPath => $"{Application.dataPath}{generateFolderPath}/GoogleSheetSO.cs";
    string SOPath => $"Assets{generateFolderPath}/GoogleSheetSO.asset";

    string[] availSheetArray;
    string json;
    bool refeshTrigger;
    static GoogleSheetManager instance;



    public static T SO<T>() where T : ScriptableObject
    {
        if (GetInstance().googleSheetSO == null)
        {
            Debug.Log($"googleSheetSO is null");
            return null;
        }

        return GetInstance().googleSheetSO as T;
    }



#if UNITY_EDITOR
    [ContextMenu("FetchGoogleSheet")]
    async void FetchGoogleSheet()
    {
        //Init
        availSheetArray = availSheets.Split('/');

        if (isAccessGoogleSheet)
        {
            Debug.Log($"Loading from google sheet..");
            json = await LoadDataGoogleSheet(googleSheetUrl);
        }
        else
        {
            Debug.Log($"Loading from local json..");
            json = LoadDataLocalJson();
        }
        if (json == null) return;

        bool isJsonSaved = SaveFileOrSkip(JsonPath, json);
        string allClassCode = GenerateCSharpClass(json);
        bool isClassSaved = SaveFileOrSkip(ClassPath, allClassCode);

        if (isJsonSaved || isClassSaved)
        {
            refeshTrigger = true;
            UnityEditor.AssetDatabase.Refresh();
        }
        else
        {
            CreateGoogleSheetSO();
            Debug.Log($"Fetch done.");
        }
    }

    async Task<string> LoadDataGoogleSheet(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                byte[] dataBytes = await client.GetByteArrayAsync(url);
                return Encoding.UTF8.GetString(dataBytes);
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request error: {e.Message}");
                return null;
            }
        }
    }

    string LoadDataLocalJson()
    {
        if (File.Exists(JsonPath))
        {
            return File.ReadAllText(JsonPath);
        }

        Debug.Log($"File not exist.\n{JsonPath}");
        return null;
    }

    bool SaveFileOrSkip(string path, string contents)
    {
        string directoryPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (File.Exists(path) && File.ReadAllText(path).Equals(contents))
            return false;

        File.WriteAllText(path, contents);
        return true;
    }

    bool IsExistAvailSheets(string sheetName)
    {
        return Array.Exists(availSheetArray, x => x == sheetName);
    }

    string GenerateCSharpClass(string jsonInput)
    {
        JObject jsonObject = JObject.Parse(jsonInput);
        StringBuilder classCode = new();

        // Scriptable Object
        classCode.AppendLine("using System;\nusing System.Collections.Generic;\nusing UnityEngine;\n");
        classCode.AppendLine("/// <summary>You must approach through `GoogleSheetManager.SO<GoogleSheetSO>()`</summary>");
        classCode.AppendLine("public class GoogleSheetSO : ScriptableObject\n{");

        foreach (var sheet in jsonObject)
        {
            string className = sheet.Key;
            if (!IsExistAvailSheets(className))
                continue;

            classCode.AppendLine($"\tpublic List<{className}> {className}List;");
        }
        classCode.AppendLine("}\n");

        // Class
        foreach (var jObject in jsonObject)
        {
            string className = jObject.Key;

            if (!IsExistAvailSheets(className))
                continue;

            var items = (JArray)jObject.Value;
            var firstItem = (JObject)items[0];
            classCode.AppendLine($"[Serializable]\npublic class {className}\n{{");

            // int > float > bool > string
            int itemIndex = 0;
            int propertyCount = ((JObject)items[0]).Properties().Count();
            string[] propertyTypes = new string[propertyCount];

            foreach (JToken item in items)
            {
                itemIndex = 0;
                foreach (var property in ((JObject)item).Properties())
                {
                    string propertyType = GetCSharpType(property.Value.Type);
                    string oldPropertyType = propertyTypes[itemIndex];

                    if (oldPropertyType == null)
                    {
                        propertyTypes[itemIndex] = propertyType;
                    }
                    else if (oldPropertyType == "int")
                    {
                        if (propertyType == "int") propertyTypes[itemIndex] = "int";
                        else if (propertyType == "float") propertyTypes[itemIndex] = "float";
                        else if (propertyType == "bool") propertyTypes[itemIndex] = "string";
                        else if (propertyType == "string") propertyTypes[itemIndex] = "string";
                    }
                    else if (oldPropertyType == "float")
                    {
                        if (propertyType == "int") propertyTypes[itemIndex] = "float";
                        else if (propertyType == "float") propertyTypes[itemIndex] = "float";
                        else if (propertyType == "bool") propertyTypes[itemIndex] = "string";
                        else if (propertyType == "string") propertyTypes[itemIndex] = "string";
                    }
                    else if (oldPropertyType == "bool")
                    {
                        if (propertyType == "int") propertyTypes[itemIndex] = "string";
                        else if (propertyType == "float") propertyTypes[itemIndex] = "string";
                        else if (propertyType == "bool") propertyTypes[itemIndex] = "bool";
                        else if (propertyType == "string") propertyTypes[itemIndex] = "string";
                    }

                    itemIndex++;
                }
            }

            itemIndex = 0;
            foreach (var property in firstItem.Properties())
            {
                string propertyName = property.Name;
                string propertyType = propertyTypes[itemIndex];
                classCode.AppendLine($"\tpublic {propertyType} {propertyName};");
                itemIndex++;
            }

            classCode.AppendLine("}\n");
        }

        return classCode.ToString();
    }

    string GetCSharpType(JTokenType jsonType)
    {
        switch (jsonType)
        {
            case JTokenType.Integer:
                return "int";
            case JTokenType.Float:
                return "float";
            case JTokenType.Boolean:
                return "bool";
            default:
                return "string";
        }
    }

    bool CreateGoogleSheetSO()
    {
        if (Type.GetType("GoogleSheetSO") == null)
            return false;

        googleSheetSO = ScriptableObject.CreateInstance("GoogleSheetSO");
        JObject jsonObject = JObject.Parse(json);
        try
        {
            foreach (var jObject in jsonObject)
            {
                string className = jObject.Key;
                if (!IsExistAvailSheets(className))
                    continue;

                Type classType = Type.GetType(className);
                Type listType = typeof(List<>).MakeGenericType(classType);
                IList listInst = (IList)Activator.CreateInstance(listType);
                var items = (JArray)jObject.Value;

                foreach (var item in items)
                {
                    object classInst = Activator.CreateInstance(classType);

                    foreach (var property in ((JObject)item).Properties())
                    {
                        FieldInfo fieldInfo = classType.GetField(property.Name);
                        object value = Convert.ChangeType(property.Value.ToString(), fieldInfo.FieldType);
                        fieldInfo.SetValue(classInst, value);
                    }

                    listInst.Add(classInst);
                }

                googleSheetSO.GetType().GetField($"{className}List").SetValue(googleSheetSO, listInst);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"CreateGoogleSheetSO error: {e.Message}");
        }
        print("CreateGoogleSheetSO");
        UnityEditor.AssetDatabase.CreateAsset(googleSheetSO, SOPath);
        UnityEditor.AssetDatabase.SaveAssets();
        return true;
    }

    void OnValidate()
    {
        if (refeshTrigger)
        {
            bool isCompleted = CreateGoogleSheetSO();
            if (isCompleted)
            {
                refeshTrigger = false;
                Debug.Log($"Fetch done.");
            }
        }
    }
#endif

    static GoogleSheetManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindFirstObjectByType<GoogleSheetManager>();
        }
        return instance;
    }
}