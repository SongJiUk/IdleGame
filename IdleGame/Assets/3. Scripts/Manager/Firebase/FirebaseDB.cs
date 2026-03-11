using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;

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
        if (reference == null || CurrentUser == null) return;
        if (IsLoading) return;


        GameData data = Managers.GameM.gameData;
        if (data == null || data.Character_Holder.Count == 0)
        {
            Debug.LogWarning("데이터가 비어있어 저장을 취소합니다.");
            return;
        }
        //data.SyncToSave();
        data.LastSaveTimeTicks = TimerNTP.NowTime.Ticks;

        if (IsNewDay(data.LastSaveTimeTicks, TimerNTP.NowTime))
        {
            Managers.GameM.gameData.DungeonKey[0] = 2;
            Managers.GameM.gameData.DungeonKey[1] = 2;
            Managers.GameM.gameData.ResetDailyMission();
        }


        try
        {
            string default_json = JsonConvert.SerializeObject(data);
            string character_json = JsonConvert.SerializeObject(data.Character_Holder);
            string item_json = JsonConvert.SerializeObject(data.Item_Holder);
            string smelt_json = JsonConvert.SerializeObject(data.EquippedSmelts);

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
        if (IsLoading) return;
        try
        {
            IsLoading = true;
            if (CurrentUser == null) return;

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

                MigrateTimeData(Managers.GameM.gameData);
            }
            else
            {
                Managers.GameM.gameData.LastSaveTimeTicks = TimerNTP.NowTime.Ticks;
            }


            if (IsNewDay(Managers.GameM.gameData.LastSaveTimeTicks, TimerNTP.NowTime))
            {
                Managers.GameM.gameData.DungeonKey[0] = 2;
                Managers.GameM.gameData.DungeonKey[1] = 2;


                Managers.GameM.gameData.ResetDailyMission();
            }


            if (charSnap.Exists)
            {
                var characterDic = JsonConvert.DeserializeObject<Dictionary<string, Holder>>(charSnap.GetRawJsonValue());
                Managers.GameM.gameData.Character_Holder = characterDic ?? new Dictionary<string, Holder>();
            }

            if (itemSnap.Exists)
            {
                string rawJson = itemSnap.GetRawJsonValue();
                //Debug.Log($"[1. 서버 원본 데이터]: {rawJson}");
                var itemDic = JsonConvert.DeserializeObject<Dictionary<string, Holder>>(itemSnap.GetRawJsonValue());
                // foreach (var key in itemDic.Keys)
                // {
                //     Debug.Log($"[2. 서버에 존재하는 키]: '{key}' (글자수: {key.Length})");
                // }
                // string targetKey = "Axe";
                // if (itemDic.ContainsKey(targetKey))
                // {
                //     //Debug.Log($"[3. 결과] {targetKey} 찾기 성공! 개수: {itemDic[targetKey].Count}");
                // }
                // else
                // {
                //     Debug.LogWarning($"[3. 결과] {targetKey} 찾기 실패! 서버 키 목록을 다시 확인하세요.");
                // }
                Managers.GameM.gameData.Item_Holder = itemDic ?? new Dictionary<string, Holder>();

                // if (itemDic != null && itemDic.TryGetValue("Axe", out var axe))
                //     Debug.Log($"[로드 확인] Axe 개수: {axe.Count}");
            }

            if (smeltSnap.Exists)
            {
                string rawJson = smeltSnap.GetRawJsonValue();
                var smeltList = JsonConvert.DeserializeObject<List<SmeltHolder>>(rawJson);
                Managers.GameM.gameData.EquippedSmelts = smeltList ?? new List<SmeltHolder>();
            }

            Managers.GameM.gameData.Init();
            Managers.GPGSM.Init();
            //Managers.GameM.gameData.SyncFromSave();
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

    public async UniTask<bool> SyncDataOnly()
    {
        if (IsDeleting || CurrentUser == null || reference == null)
        {
            Debug.LogWarning("Firebase 동기화 불가: 삭제 중이거나 유저가 로그인되어 있지 않습니다.");
            return false;
        }

        try
        {
            var dataSnap = await reference.Child("users").Child(CurrentUser.UserId).Child("DATA").GetValueAsync().AsUniTask();

            if (dataSnap.Exists)
            {
                string json = dataSnap.GetRawJsonValue();
                if (!string.IsNullOrEmpty(json))
                {
                    JsonConvert.PopulateObject(json, Managers.GameM.gameData);
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"SyncDataOnly 도중 에러: {e.Message}");
        }
        return false;

    }

    bool IsNewDay(long _lastTicks, DateTime _now)
    {
        if (_lastTicks == 0) return true;
        DateTime lastDate = new DateTime(_lastTicks);
        return lastDate.Date != _now.Date;
    }
    public void MigrateTimeData(GameData _data)
    {
        if (_data.LastSaveTimeTicks != 0) return;

        if (!string.IsNullOrEmpty(_data.endDate))
        {
            DateTime oldTime = DateTime.Parse(_data.endDate);
            _data.LastSaveTimeTicks = oldTime.Ticks;

            Debug.Log("데이터 마이그레이션 완료: string -> long(Ticks)");
        }
    }


    public bool IsDeleting { get; private set; } = false;
    public async UniTask DeleteUserData()
    {
        if (CurrentUser == null) { Debug.LogError("[ERROR] 삭제 실패: 로그인된 유저 없음"); return; }

        Debug.Log($"[DEBUG] 서버 데이터 삭제 요청: {CurrentUser.UserId}");
        IsDeleting = true;

        try
        {
            Auth.StateChanged -= OnAuthStateChanged;

            await reference.Child("users").Child(CurrentUser.UserId).RemoveValueAsync().AsUniTask();


            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Managers.ClearAll();
            Debug.Log("서버 및 로컬 데이터 삭제 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ERROR] 삭제 프로세스 중 오류: {e.Message}");
        }
        finally
        {
            IsDeleting = false;
        }
    }
}
