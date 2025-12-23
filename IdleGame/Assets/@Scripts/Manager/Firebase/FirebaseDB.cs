using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

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
        Debug.Log("4. WriteData 실행됨");
        GameData data = Managers.GameM.gameData;
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
            Debug.Log("1. ReadDataAsync 시작");
            var userId = currentUser.UserId;
            var dataSnapShot = await reference.Child("USER").Child(userId).Child("DATA").GetValueAsync();
            if (dataSnapShot.Exists)
            {
                var json = dataSnapShot.GetRawJsonValue();
                Managers.GameM.gameData = JsonUtility.FromJson<GameData>(json);
            }

            var characterSnapShot = await reference.Child("USER").Child(userId).Child("CHARACTER").GetValueAsync();
            if (characterSnapShot.Exists)
            {
                var json = characterSnapShot.GetRawJsonValue();
                var charDic = JsonConvert.DeserializeObject<Dictionary<string, Holder>>(json);
                Managers.GameM.gameData.Character_Holder = charDic;
            }
            Debug.Log($"2. 로드 완료 - 캐릭터 수: {Managers.GameM.gameData.Character_Holder.Count}");
            Managers.GameM.gameData.Init();
            Debug.Log("3. Init(SetCharacter) 완료");
            Debug.Log("모든 데이터 로드 및 초기화 완료");
        }
        catch(System.Exception e)
        {
            Debug.LogError($"데이터 로드 중 오류 발생: {e.Message}");
        }
        

    }
}
