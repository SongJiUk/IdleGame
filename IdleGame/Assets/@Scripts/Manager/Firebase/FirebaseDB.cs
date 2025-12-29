using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System;

public class User
{
    public string userName;
    public int stage;

}
public partial class FirebaseManager
{
    public void WirteData()
    {
        if (Managers.GameM.gameData.Character_Holder.Count == 0)
        {
            Debug.LogWarning("[위험] 데이터가 비어있는데 저장을 시도함! 실행 차단.");
            return;
        }
        GameData data = new GameData();
        if(Managers.GameM.gameData != null)
        {
            data = Managers.GameM.gameData;
            data.EndDate = DateTime.Now.ToString();
        }

        if (data == null || reference == null) return;

        #region 기본 캐릭터 데이터
        string default_json = JsonUtility.ToJson(data);
        reference.Child("USER").Child(currentUser.UserId).Child("DATA").SetRawJsonValueAsync(default_json)
            .ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompleted)
                {
                    Debug.LogError("데이터 쓰기 실패 : " + task.Exception.ToString());
                }
                else
                    Debug.Log("기본 캐릭터 데이터 저장 완료");

            });
        #endregion

        #region 배치하는 캐릭터 데이터
        string character_json = JsonConvert.SerializeObject(data.Character_Holder);
        reference.Child("USER").Child(currentUser.UserId).Child("CHARACTER").SetRawJsonValueAsync(character_json)
            .ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompleted)
                {
                    Debug.LogError("데이터 쓰기 실패 : " + task.Exception.ToString());
                }
                else
                    Debug.Log("배치하는 캐릭터 데이터 저장 완료");

            });
        #endregion

        #region 아이템 데이터
        string item_json = JsonConvert.SerializeObject(data.Item_Holder);
        reference.Child("USER").Child(currentUser.UserId).Child("ITEM").SetRawJsonValueAsync(item_json)
            .ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompleted)
                {
                    Debug.LogError("데이터 쓰기 실패 : " + task.Exception.ToString());
                }
                else
                    Debug.Log("배치하는 캐릭터 데이터 저장 완료");

            });
        #endregion
    }

    public void ReadData()
    {
        #region 기본 캐릭터 데이터
        reference.Child("USER").Child(currentUser.UserId).Child("DATA").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    //NOTE : FromJson => json -> class
                    var defaultdata = JsonUtility.FromJson<GameData>(snapshot.GetRawJsonValue());
                    GameData data = new GameData();
                    if (defaultdata != null)
                    {
                        data = defaultdata;
                    }
                    data.StartDate = DateTime.Now.ToString();
                    Managers.GameM.gameData = data;
                    Managers.GameM.gameData.Init();
                    Debug.Log("기본 캐릭터 데이터 로드 성공 ");
                }
                else
                {
                    Debug.LogError("데이터 읽기 실패 : " + task.Exception.ToString());
                }

            });
        #endregion

        #region 배치하는 캐릭터 데이터
        reference.Child("USER").Child(currentUser.UserId).Child("CHARACTER").GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    //NOTE : FromJson => json -> class
                    var data = JsonConvert.DeserializeObject<Dictionary<string, Holder>>(snapshot.GetRawJsonValue());
                    Managers.GameM.gameData.Character_Holder = data;
                    Managers.GameM.gameData.Init();
                    Debug.Log("배치하는 캐릭터 데이터 로드 성공 ");
                }
                else
                {
                    Debug.LogError("데이터 읽기 실패 : " + task.Exception.ToString());
                }

            });
        #endregion
    }

    public async UniTask ReadDataAsync()
    {
        try
        {
            var userId = currentUser.UserId;
            var dataSnapShot = await reference.Child("USER").Child(userId).Child("DATA").GetValueAsync();
            if (dataSnapShot.Exists)
            {
                var json = dataSnapShot.GetRawJsonValue();
                var defaultdata = JsonUtility.FromJson<GameData>(json);
                GameData data = new GameData();
                if (defaultdata != null)
                {
                    data = defaultdata;
                }
                data.StartDate = DateTime.Now.ToString();
                Managers.GameM.gameData = data;

            }

            var characterSnapShot = await reference.Child("USER").Child(userId).Child("CHARACTER").GetValueAsync();
            if (characterSnapShot.Exists)
            {
                var json = characterSnapShot.GetRawJsonValue();
                var charDic = JsonConvert.DeserializeObject<Dictionary<string, Holder>>(json);
                Managers.GameM.gameData.Character_Holder = charDic;
            }

            var ItemSnapShot = await reference.Child("USER").Child(userId).Child("ITEM").GetValueAsync();
            if (ItemSnapShot.Exists)
            {
                var json = ItemSnapShot.GetRawJsonValue();
                var itemDic = JsonConvert.DeserializeObject<Dictionary<string, Holder>>(json);
                Managers.GameM.gameData.Item_Holder = itemDic;
            }

            Managers.GameM.gameData.Init();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"데이터 로드 중 오류 발생: {e.Message}");
        }


    }
}
