using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;

public class User
{
    public string userName;
    public int stage;

}
public partial class FirebaseManager
{
    public void WirteData()
    {
        GameData data = new GameData();
        if (Managers.GameM.gameData != null)
        {
            data = Managers.GameM.gameData;
        }



        if (reference == null) return;
        #region 기본 캐릭터 데이터
        string default_json = JsonUtility.ToJson(data);
        reference.Child("USER").Child(currentUser.UserId).Child("DATA").SetRawJsonValueAsync(default_json)
            .ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompleted)
                {
                    Debug.LogError("데이터 쓰기 실패 : " + task.Exception.ToString());
                }

            });
        #endregion

        #region 배치하는 캐릭터 데이터
        string character_json = JsonConvert.SerializeObject(Managers.GameM.gameData.Character_Holder);
        reference.Child("USER").Child(currentUser.UserId).Child("CHARACTER").SetRawJsonValueAsync(character_json)
            .ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompleted)
                {
                    Debug.LogError("데이터 쓰기 실패 : " + task.Exception.ToString());
                }

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

                    Managers.GameM.gameData = data;
                    Managers.GameM.gameData.Init();
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
                }
                else
                {
                    Debug.LogError("데이터 읽기 실패 : " + task.Exception.ToString());
                }

            });
        #endregion
    }
}
