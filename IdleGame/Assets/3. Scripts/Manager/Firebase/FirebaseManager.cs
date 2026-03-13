using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Google;
using System;
using Newtonsoft.Json;
using way2tushar.NativeAlerts;


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
        public GameData ServerData;
        public bool HasConflict;
    }
    public async UniTask Init()
    {
        var status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
        if (status != DependencyStatus.Available)
        {
            Debug.LogError($"Firebase 초기화 실패: {status}");
            return;
        }

        Auth = FirebaseAuth.DefaultInstance;
        DB = FirebaseDatabase.DefaultInstance.RootReference;
        reference = DB;
        Auth.StateChanged += OnAuthStateChanged;
        SetupGoogleConfig();

        await UniTask.WaitUntil(() => Auth != null);

        await UniTask.Yield();

        Debug.Log($"[Init] 초기화 후 CurrentUser: {Auth.CurrentUser?.UserId ?? "null"}");
        Debug.Log("WebClientID : " + webClientId);


        CurrentUser = Auth.CurrentUser;

        isInitializing = false;
        Managers.UpdateM.isStartFirebase = true;
        Debug.Log("Firebase 초기화 성공");
    }

    private void OnAuthStateChanged(object _sender, EventArgs _eventArgs)
    {
        Debug.Log("OnAuthStateChanged Start");
        Debug.Log("isInitializing" + isInitializing);

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
        Debug.Log("[FirebaseManager] CurrentUser is : " + (CurrentUser != null));
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
            ServerData = server,
            HasConflict = hasConflict
        };
    }

    public async UniTask<bool> ForceUploadLocalDataToServer(SyncResult _syncInfo)
    {
        //TODO : 아이디 전환될때, 데이터가 덮어씌워져야하는데 안되는 이유 확인하기
        Debug.Log($"[Sync] 전환 전 UID: {Auth.CurrentUser?.UserId}");

        bool switchSuccess = await SwitchToGoogleAccount();
        if (!switchSuccess || Auth.CurrentUser == null)
        {
            Debug.LogError("[Sync] 계정 전환 실패 또는 유저 정보 없음");
            return false;
        }

        await UniTask.Delay(1000);

        Debug.Log($"[Sync] 전환 후 UID: {Auth.CurrentUser.UserId}");
        Managers.GameM.gameData = _syncInfo.LocalData;

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

    public async UniTask<bool> TryLinkGoogleAcount()
    {
        try
        {
            await LinkGoogleToCurrentUser();
            await ReadData();
            return true;
        }
        catch (FirebaseException e) when (e.ErrorCode == (int)AuthError.CredentialAlreadyInUse)
        {
            return false;
        }
    }

}

