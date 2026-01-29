using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager
{
    public class StringData
    {
        public string ko;
        public string en;
        public string ja;

        public string GetData(string _language = "")
        {
            if (_language != "")
            {
                switch (_language)
                {
                    case "ko": return ko;
                    case "en": return en;
                    case "ja": return ja;
                }
            }

            switch (LocalAccess)
            {
                case "ko": return ko;
                case "en": return en;
                case "ja": return ja;
            }
            return "";
        }
    }


    public static string LocalAccess = "ko";
    public Dictionary<string, StringData> localData;

    public void Init()
    {
        localData = CreateDictionary();
    }
    private Dictionary<string, StringData> CreateDictionary()
    {
        Dictionary<string, StringData> value = new Dictionary<string, StringData>();

        if(Managers.DataM == null || Managers.DataM.LocalizationDic == null)
        {
            Debug.Log("Localization 초기화 확인하기");
            return value;
        }

        var localization = Managers.DataM.LocalizationDic;
        SetLanguage();


        //for (int i = 0; i < localization.Count; i++)
        //{
        //    StringData data = new StringData();
        //    data.ko = localization["ko"].ToString();
        //    data.en = localization["en"].ToString();
        //    data.ja = localization["ja"].ToString();

        //    string temp = localization["Key"].ToString();

        //    value.Add(temp, data);
        //}

        foreach (var pair in Managers.DataM.LocalizationDic)
        {
            string key = pair.Key;
            Data.Localization src = pair.Value;

            StringData data = new StringData
            {
                ko = src.ko,
                en = src.en,
                ja = src.ja
            };

            if (!value.ContainsKey(key))
                value.Add(key, data);
        }
        return value;
    }

    void SetLanguage()
    {
        string localCheck = PlayerPrefs.GetString("LOCAL");

        if (localCheck == "" || localCheck == null)
        {
            SystemLanguage language = Application.systemLanguage;

            switch (language)
            {
                case SystemLanguage.Korean: LocalAccess = "ko"; break;
                case SystemLanguage.Japanese: LocalAccess = "ja"; break;
                default: LocalAccess = "en"; break;
            }

        }
        else
        {
            LocalAccess = localCheck;
        }

        //TODO : 영어 테스트 
        //LocalAccess = "en";
    }

    public string Get(string _key)
    {
        if (localData == null)
            return "";

        if(!localData.ContainsKey(_key))
        {
            Debug.Log($"Localization key not found : {_key}");
            return _key;
        }

        return localData[_key].GetData();
    }
}

