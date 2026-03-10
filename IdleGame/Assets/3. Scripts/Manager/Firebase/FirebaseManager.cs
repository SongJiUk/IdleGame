using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Google;
using System;


//NOTE : partial class는 이 이름으로 사용하게 해줌
public partial class FirebaseManager
{
    public FirebaseAuth Auth { get; private set; }
    public DatabaseReference DB { get; private set; }
    public FirebaseUser CurrentUser { get; private set; }
    public DatabaseReference reference;

    GoogleSignInConfiguration configuration;
    bool isInitializing = true;
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

            //Managers.GameM.gameData.ResetAllData();
            Managers.GameM.gameData.isGuest = true;
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
}

