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
    public bool IsLoading { get; private set; } = false;
    public async UniTask WriteData()
    {
        if (IsLoading || reference == null || currentUser == null) return;


        GameData data = Managers.GameM.gameData;
        if (data == null || data.Character_Holder.Count == 0)
        {
            Debug.LogWarning("데이터가 비어있어 저장을 취소합니다.");
            return;
        }
        try
        {
            data.EndDate = DateTime.Now.ToString();
            string userID = currentUser.UserId;

            string default_json = JsonUtility.ToJson(data);
            string character_json = JsonConvert.SerializeObject(data.Character_Holder);
            string item_json = JsonConvert.SerializeObject(data.Item_Holder);

            await UniTask.WhenAll(
                reference.Child("USER").Child(currentUser.UserId).Child("DATA").SetRawJsonValueAsync(default_json).AsUniTask(),
                reference.Child("USER").Child(currentUser.UserId).Child("CHARACTER").SetRawJsonValueAsync(character_json).AsUniTask(),
                reference.Child("USER").Child(currentUser.UserId).Child("ITEM").SetRawJsonValueAsync(item_json).AsUniTask());

            Debug.Log("모든 게임 데이터 통합 저장 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"데이터 저장중 오류 발생 : {e.Message}");
        }
    }

    public async UniTask ReadData()
    {
        try
        {
            IsLoading = true;
            var userId = currentUser.UserId;

            var (dataSnap, charSnap, itemSnap) = await UniTask.WhenAll(
            reference.Child("USER").Child(userId).Child("DATA").GetValueAsync().AsUniTask(),
            reference.Child("USER").Child(userId).Child("CHARACTER").GetValueAsync().AsUniTask(),
            reference.Child("USER").Child(userId).Child("ITEM").GetValueAsync().AsUniTask());

            if (dataSnap.Exists)
            {
                string json = dataSnap.GetRawJsonValue();
                JsonUtility.FromJsonOverwrite(json, Managers.GameM.gameData);
            }

            Managers.GameM.gameData.StartDate = DateTime.Now.ToString();

            if (charSnap.Exists)
            {
                var characterDic = JsonConvert.DeserializeObject<Dictionary<string, Holder>>(charSnap.GetRawJsonValue());
                Managers.GameM.gameData.Character_Holder = characterDic ?? new Dictionary<string, Holder>();
            }

            if (itemSnap.Exists)
            {
                string rawJson = itemSnap.GetRawJsonValue();
                Debug.Log($"[1. 서버 원본 데이터]: {rawJson}");
                var itemDic = JsonConvert.DeserializeObject<Dictionary<string, Holder>>(itemSnap.GetRawJsonValue());
                foreach (var key in itemDic.Keys)
                {
                    Debug.Log($"[2. 서버에 존재하는 키]: '{key}' (글자수: {key.Length})");
                }
                string targetKey = "Axe"; // 혹은 "Gold_Dice", "GoddessTears"
                if (itemDic.ContainsKey(targetKey))
                {
                    Debug.Log($"[3. 결과] {targetKey} 찾기 성공! 개수: {itemDic[targetKey].Count}");
                }
                else
                {
                    Debug.LogWarning($"[3. 결과] {targetKey} 찾기 실패! 서버 키 목록을 다시 확인하세요.");
                }
                Managers.GameM.gameData.Item_Holder = itemDic ?? new Dictionary<string, Holder>();

                if (itemDic != null && itemDic.TryGetValue("Axe", out var axe))
                    Debug.Log($"[로드 확인] Axe 개수: {axe.Count}");
            }

            Managers.GameM.gameData.Init();


            Debug.Log("모든 데이터 로드 및 초기화 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"데이터 로드 중 오류 발생: {e.Message}");
        }
        finally
        {
            IsLoading = false;
        }


    }
}
