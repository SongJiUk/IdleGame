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
    public bool IsDeleting { get; private set; } = false;

    public async UniTask WriteData()
    {
        if (reference == null || CurrentUser == null) return;
        if (IsLoading) return;


        GameData data = Managers.GameM.gameData;
        if (data == null || data.Character_Holder == null || data.Character_Holder.Count == 0)
        {
            Debug.LogWarning("[WriteData] 저장할 유효 데이터가 없거나 캐릭터 정보가 비어있어 취소합니다.");
            return;
        }

        data.LastSaveTimeTicks = TimerNTP.NowTime.Ticks;

        if (IsNewDay(data.LastSaveTimeTicks, TimerNTP.NowTime))
        {
            Managers.GameM.gameData.DungeonKey[0] = 2;
            Managers.GameM.gameData.DungeonKey[1] = 2;
            Managers.GameM.gameData.ResetDailyMission();
        }

        string userId = CurrentUser.UserId;
        var userRef = reference.Child("users").Child(userId);

        string default_json = JsonConvert.SerializeObject(data);
        string character_json = JsonConvert.SerializeObject(data.Character_Holder);
        string item_json = JsonConvert.SerializeObject(data.Item_Holder);
        string smelt_json = JsonConvert.SerializeObject(data.EquippedSmelts);

        // 4개 노드를 단일 쓰기로 묶어 원자성 보장 (일부만 저장되는 불일치 방지)
        var savePayload = new Dictionary<string, object>
        {
            ["DATA"]      = JsonConvert.DeserializeObject<object>(default_json),
            ["CHARACTER"] = JsonConvert.DeserializeObject<object>(character_json),
            ["ITEM"]      = JsonConvert.DeserializeObject<object>(item_json),
            ["SMELT"]     = JsonConvert.DeserializeObject<object>(smelt_json)
        };
        string allJson = JsonConvert.SerializeObject(savePayload);

        // 네트워크 일시 오류 대비 최대 3회 재시도
        const int maxRetry = 3;
        for (int i = 0; i < maxRetry; i++)
        {
            try
            {
                await userRef.SetRawJsonValueAsync(allJson).AsUniTask();

                //Debug.Log($"[WriteData] Firebase 저장 완료 (User: {userId})");
                return;
            }
            catch (Exception e)
            {
                if (i == maxRetry - 1)
                {
                    Debug.LogError($"[WriteData] {maxRetry}회 재시도 모두 실패: {e.Message}\n{e.StackTrace}");
                }
                else
                {
                    Debug.LogWarning($"[WriteData] 저장 실패 ({i + 1}/{maxRetry}회), 2초 후 재시도...");
                    await UniTask.WaitForSeconds(2f, ignoreTimeScale: true);
                }
            }
        }
    }



    public async UniTask ReadData()
    {
        if (IsLoading) return;
        try
        {
            IsLoading = true;
            if (CurrentUser == null || reference == null)
            {
                Debug.LogWarning("[ReadData] CurrentUser 또는 reference가 null입니다.");
                return;
            }

            var userId = CurrentUser.UserId;
            var userRef = reference.Child("users").Child(userId);

            var (dataSnap, charSnap, itemSnap, smeltSnap) = await UniTask.WhenAll(
            userRef.Child("DATA").GetValueAsync().AsUniTask(),
            userRef.Child("CHARACTER").GetValueAsync().AsUniTask(),
            userRef.Child("ITEM").GetValueAsync().AsUniTask(),
            userRef.Child("SMELT").GetValueAsync().AsUniTask());

            if (dataSnap.Exists)
            {
                string json = dataSnap.GetRawJsonValue();
                JsonConvert.PopulateObject(json, Managers.GameM.gameData);
                MigrateTimeData(Managers.GameM.gameData);
            }
            else
            {
                Debug.Log("[Firebase] 서버 데이터가 없습니다. 신규 유저로 세팅합니다.");
                Managers.GameM.gameData.LastSaveTimeTicks = TimerNTP.NowTime.Ticks;
            }


            Managers.GameM.gameData.Character_Holder ??= new Dictionary<string, Holder>();
            Managers.GameM.gameData.Item_Holder ??= new Dictionary<string, Holder>();
            Managers.GameM.gameData.EquippedSmelts ??= new List<SmeltHolder>();
            Managers.GameM.gameData.Characters_Data ??= new Dictionary<string, CharacterHolder>();
            Managers.GameM.gameData.Item_Data ??= new Dictionary<string, ItemHolder>();
            Managers.GameM.gameData.MissionDic ??= new Dictionary<string, MissionInfo>();
            Managers.GameM.gameData.AchievementDic ??= new Dictionary<int, bool>();
            Managers.GameM.gameData.SaveEquippedRelics ??= new Dictionary<string, Data.ItemData>();
            Managers.GameM.gameData.DungeonKey ??= new int[] { 2, 2 };

            if (IsNewDay(Managers.GameM.gameData.LastSaveTimeTicks, TimerNTP.NowTime))
            {
                Managers.GameM.gameData.DungeonKey[0] = 2;
                Managers.GameM.gameData.DungeonKey[1] = 2;
                Managers.GameM.gameData.ResetDailyMission();
            }

            if (charSnap.Exists)
            {
                var characterDic = JsonConvert.DeserializeObject<Dictionary<string, Holder>>(charSnap.GetRawJsonValue());
                if (characterDic != null) Managers.GameM.gameData.Character_Holder = characterDic;
            }

            if (itemSnap.Exists)
            {
                var itemDic = JsonConvert.DeserializeObject<Dictionary<string, Holder>>(itemSnap.GetRawJsonValue());
                if (itemDic != null) Managers.GameM.gameData.Item_Holder = itemDic;
            }

            if (smeltSnap.Exists)
            {
                var smeltList = JsonConvert.DeserializeObject<List<SmeltHolder>>(smeltSnap.GetRawJsonValue());
                if (smeltList != null) Managers.GameM.gameData.EquippedSmelts = smeltList;
            }

            Managers.GameM.gameData.Init();
            Managers.GPGSM.Init();
            Managers.RelicM.Init();
            Managers.QuestM.Init();

            Debug.Log("모든 데이터 로드 및 초기화 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"데이터 로드 중 상세 오류: {e.Message}");
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
            Debug.LogWarning("[SyncDataOnly] Firebase 동기화 불가 (상태 확인 요망)");
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
        if (_lastTicks <= 0) return true;

        // DateTime 유효 범위를 벗어난 Ticks는 ArgumentOutOfRangeException 유발
        if (_lastTicks < DateTime.MinValue.Ticks || _lastTicks > DateTime.MaxValue.Ticks)
        {
            Debug.LogWarning($"[IsNewDay] 유효하지 않은 Ticks 값: {_lastTicks}");
            return true;
        }

        try
        {
            DateTime lastDate = new DateTime(_lastTicks);
            return lastDate.Date != _now.Date;
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.LogWarning($"[IsNewDay] Ticks 변환 실패: {e.Message}");
            return true;
        }
    }
    public void MigrateTimeData(GameData _data)
    {
        if (_data == null || _data.LastSaveTimeTicks != 0) return;

        if (!string.IsNullOrEmpty(_data.endDate) && DateTime.TryParse(_data.endDate, out DateTime oldTime))
        {
            _data.LastSaveTimeTicks = oldTime.Ticks;
            Debug.Log("데이터 마이그레이션 완료: string -> long(Ticks)");
        }
    }



    public async UniTask DeleteUserData()
    {
        if (CurrentUser == null) { Debug.LogError("[ERROR] 삭제 실패: 로그인된 유저 없음"); return; }

        Debug.Log($"[DEBUG] 서버 데이터 삭제 요청: {CurrentUser.UserId}");

        IsDeleting = true;
        string userId = CurrentUser.UserId;

        try
        {
            await reference.Child("users").Child(CurrentUser.UserId).RemoveValueAsync().AsUniTask();
            Debug.Log($"[DeleteUserData] 서버 데이터 삭제 완료: {userId}");

            Managers.ClearAll();
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

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
