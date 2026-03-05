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
        if (reference == null || CurrentUser == null)
        {
            return;
        }
        if (IsLoading) return;


        GameData data = Managers.GameM.gameData;
        data.SyncToSave();
        if (data == null || data.Character_Holder.Count == 0)
        {
            Debug.LogWarning("데이터가 비어있어 저장을 취소합니다.");
            return;
        }
        try
        {
            Managers.GameM.EndDate = TimerNTP.NowTime.ToString();
            if (!DateTime.TryParse(Managers.GameM.EndDate, out DateTime endDate))
            {
                endDate = TimerNTP.NowTime;
            }


            if (GetDateItem(endDate, DateTime.Now))
            {
                Managers.GameM.gameData.DungeonKey[0] = 2;
                Managers.GameM.gameData.DungeonKey[1] = 2;

                //foreach (var mission in Managers.GameM.gameData.MissionDic)
                //{
                //    mission.Value.Progress = 0;
                //    mission.Value.isRewarded = false;
                //}
            }

            string default_json = JsonConvert.SerializeObject(data);
            string character_json = JsonConvert.SerializeObject(data.Character_Holder);
            string item_json = JsonConvert.SerializeObject(data.Item_Holder);
            string smelt_json = JsonConvert.SerializeObject(data.Smelts);

            await UniTask.WhenAll(
                reference.Child("users").Child(CurrentUser.UserId).Child("DATA").SetRawJsonValueAsync(default_json).AsUniTask(),
                reference.Child("users").Child(CurrentUser.UserId).Child("CHARACTER").SetRawJsonValueAsync(character_json).AsUniTask(),
                reference.Child("users").Child(CurrentUser.UserId).Child("ITEM").SetRawJsonValueAsync(item_json).AsUniTask(),
                reference.Child("users").Child(CurrentUser.UserId).Child("SMELT").SetRawJsonValueAsync(smelt_json).AsUniTask());

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
            var userId = CurrentUser.UserId;

            var (dataSnap, charSnap, itemSnap, smeltSnap) = await UniTask.WhenAll(
            reference.Child("users").Child(userId).Child("DATA").GetValueAsync().AsUniTask(),
            reference.Child("users").Child(userId).Child("CHARACTER").GetValueAsync().AsUniTask(),
            reference.Child("users").Child(userId).Child("ITEM").GetValueAsync().AsUniTask(),
            reference.Child("users").Child(userId).Child("SMELT").GetValueAsync().AsUniTask());

            if (dataSnap.Exists)
            {
                string json = dataSnap.GetRawJsonValue();
                JsonConvert.PopulateObject(json, Managers.GameM.gameData);
            }
            else
            {
                Managers.GameM.EndDate = TimerNTP.NowTime.ToString();
            }

            Managers.GameM.StartDate = TimerNTP.NowTime.ToString();

            if (string.IsNullOrEmpty(Managers.GameM.EndDate))
            {
                Managers.GameM.EndDate = TimerNTP.NowTime.ToString();
            }

            DateTime startDate = TimerNTP.NowTime;

            if (DateTime.TryParse(Managers.GameM.EndDate, out DateTime endDate))
            {
                if (GetDateItem(startDate, endDate))
                {
                    Managers.GameM.gameData.DungeonKey[0] = 2;
                    Managers.GameM.gameData.DungeonKey[1] = 2;


                    Managers.GameM.gameData.ResetDailyMission();
                }
            }


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
                string targetKey = "Axe";
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

            if (smeltSnap.Exists)
            {
                string rawJson = smeltSnap.GetRawJsonValue();
                var smeltList = JsonConvert.DeserializeObject<List<SmeltHolder>>(rawJson);
                Managers.GameM.gameData.Smelts = smeltList ?? new List<SmeltHolder>();
            }

            Managers.GameM.gameData.Init();
            Managers.GPGSM.Init();
            Managers.GameM.gameData.SyncFromSave();
            Managers.RelicM.Init();

            Managers.QuestM.Init();

            Debug.Log("모든 데이터 로드 및 초기화 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"데이터 로드 중 상세 오류: {e.Message}");

            if (e is Firebase.FirebaseException fe)
            {
                Debug.LogError($"에러 코드: {fe.ErrorCode}");
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool GetDateItem(DateTime _startTime, DateTime _endTime)
    {
        if (_startTime.Day != _endTime.Day)
        {
            return true;
        }
        return false;
    }
}
