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
        OnAuthStateChanged(this, null);

        Managers.UpdateM.isStartFirebase = true;
        Debug.Log("Firebase 초기화 성공");
    }

    private void OnAuthStateChanged(object _sender, EventArgs _eventArgs)
    {
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
        return CurrentUser != null;
    }

    public void SignOutFM()
    {
        if (Auth != null)
        {
            Auth.SignOut();
            CurrentUser = null;
        }
    }


}

