using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Google;
using System;
using Newtonsoft.Json;
using way2tushar.NativeAlerts;


//NOTE : partial class는 이 이름으로 사용하게 해줌
public partial class FirebaseManager
{
    public FirebaseAuth Auth { get; private set; }
    public DatabaseReference DB { get; private set; }
    public FirebaseUser CurrentUser { get; private set; }
    public DatabaseReference reference;

    GoogleSignInConfiguration configuration;
    bool isInitializing = true;

    public class SyncResult
    {
        public GameData LocalData;
        public GameData serverData;
        public bool HasConflict;
    }
    public async UniTask Init()
    {
        var status = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (status != DependencyStatus.Available)
        {
            Debug.Log("Firebase 초기화 실패");
            return;
        }

        Auth = FirebaseAuth.DefaultInstance;
        DB = FirebaseDatabase.DefaultInstance.RootReference;
        reference = DB;
        Auth.StateChanged += OnAuthStateChanged;

        isInitializing = false;

        Managers.UpdateM.isStartFirebase = true;
        Debug.Log("Firebase 초기화 성공");
    }

    private void OnAuthStateChanged(object _sender, EventArgs _eventArgs)
    {

        if (isInitializing) return;

        if (Auth.CurrentUser != CurrentUser)
        {
            CurrentUser = Auth.CurrentUser;
            if (CurrentUser != null)
                Debug.Log($"Signed In : {CurrentUser.UserId}");
            else
                Debug.Log("Signed Out");
        }
    }

    public bool IsLoggedIn()
    {
        Debug.Log("[FirebaseManager] : " + CurrentUser != null);
        return CurrentUser != null;
    }

    public void SignOutFM()
    {
        if (Auth != null)
        {
            Debug.Log($"[DEBUG] 로그아웃 시작. 유저 ID: {CurrentUser?.UserId}");
            Auth.StateChanged -= OnAuthStateChanged;
            Auth.SignOut();
            CurrentUser = null;

            Debug.Log("[DEBUG] 로그아웃 및 데이터 초기화 완료");
        }
    }

    public async UniTask<bool> SignInWithCredentialOnly(Credential _credential)
    {
        try
        {
            var result = await Auth.SignInWithCredentialAsync(_credential);
            CurrentUser = result;
            ApplyUserToGameData(CurrentUser);
            await ReadData();
            return true;
        }
        catch { return false; }
    }

    public async UniTask<bool> SwitchToGoogleAccount()
    {
        try
        {
            Auth.SignOut();

            bool success = await GoogleLogin();

            if (success)
            {
                Debug.Log("[Firebase] 구글 계정으로 성공적으로 전환되었습니다.");
                return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Firebase] 전환 실패: {e.Message}");
        }
        return false;
    }

    public async UniTask<GameData> FetchServerData()
    {
        try
        {
            if (CurrentUser == null) return null;
            var snapShot = await reference.Child("users").Child(CurrentUser.UserId).GetValueAsync().AsUniTask();

            if (!snapShot.Exists)
            {
                Debug.Log("[Firebase] 서버에 저장된 데이터가 없습니다. 새 데이터를 생성합니다.");
                return new GameData();
            }

            string json = snapShot.GetRawJsonValue();

            GameData serverData = JsonConvert.DeserializeObject<GameData>(json);

            Debug.Log("[Firebase] 서버 데이터 로드 성공");

            return serverData;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Firebase] 데이터 로드 실패 : {e.Message}");
            return null;
        }
    }

    public async UniTask<SyncResult> PrepareGoogleAccountSync()
    {
        GameData local = Managers.GameM.gameData;
        GameData server = await FetchServerData();

        bool hasConflict = false;

        if (server != null && local.stage > server.stage) hasConflict = true;
        return new SyncResult
        {
            LocalData = local,
            serverData = server,
            HasConflict = hasConflict
        };
    }

    public async UniTask<bool> ForceUploadLocalDataToServer(SyncResult _syncInfo)
    {
        //TODO : 아이디 전환될때, 데이터가 덮어씌워져야하는데 안되는 이유 확인하기
        Debug.Log($"[Sync] 전환 전 UID: {Auth.CurrentUser?.UserId}");

        bool switchSuccess = await SwitchToGoogleAccount();
        if (!switchSuccess) return false;

        // 2. [중요] 세션이 완전히 바뀔 때까지 짧은 대기 (인증 지연 방지)
        await UniTask.Delay(1000);
        Managers.GameM.gameData = _syncInfo.LocalData;
        Managers.GameM.gameData.Character_Holder = _syncInfo.LocalData.Character_Holder;
        Managers.GameM.gameData.Item_Holder = _syncInfo.LocalData.Item_Holder;
        Managers.GameM.gameData.EquippedSmelts = _syncInfo.LocalData.EquippedSmelts;

        Managers.GameM.gameData.LastSaveTimeTicks = _syncInfo.LocalData.LastSaveTimeTicks;

        Debug.Log($"[Firebase] {Auth.CurrentUser?.UserId} 계정으로 데이터 업로드 시작...");

        // 4. 저장 실행
        await WriteData();

        Debug.Log("[Firebase] 데이터 업로드 성공");
        return true;
    }

    public async UniTask<bool> LoadServerDataOnly()
    {
        return await SwitchToGoogleAccount();
    }
}

