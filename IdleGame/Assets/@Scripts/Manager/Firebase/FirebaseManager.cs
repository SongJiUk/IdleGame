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
    private UniTaskCompletionSource<bool> loginCTS;
    public FirebaseAuth Auth { get; private set; }
    public DatabaseReference DB { get; private set; }
    public FirebaseUser CurrentUser { get; private set; }
    public DatabaseReference reference;
    public async UniTask Init()
    {
        var status = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (status != DependencyStatus.Available)
        {
            Debug.Log("Firebase 초기화 실패");
            return;
        }
        loginCTS = new UniTaskCompletionSource<bool>();

        Auth = FirebaseAuth.DefaultInstance;
        DB = FirebaseDatabase.DefaultInstance.RootReference;
        reference = DB;
        Auth.StateChanged += OnAuthStateChanged;
        OnAuthStateChanged(this, null);
        LoadingLogin.instance.GetLoginInit();

        await loginCTS.Task;
        Managers.UpdateM.isStartFirebase = true;
        Debug.Log("Firebase 초기화 성공");

        // //NOTE : 초기화 작업
        // await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        // {
        //     if (task.Result == DependencyStatus.Available)
        //     {
        //         auth = FirebaseAuth.DefaultInstance;
        //         currentUser = auth.CurrentUser;
        //         //NOTE : realTimeDB 참조 얻는거
        //         reference = FirebaseDatabase.DefaultInstance.RootReference;

        //         GuestLogin();
        //         Managers.UpdateM.isStartFirebase = true;
        //         Debug.Log("Firebase 초기화 성공");
        //     }
        //     else
        //     {
        //         Debug.Log("Firebase 초기화 실패");
        //     }
        // });
    }

    private void OnAuthStateChanged(object _sender, EventArgs _eventArgs)
    {
        if (Auth.CurrentUser != CurrentUser)
        {
            bool signedIn = Auth.CurrentUser != null;
            if (!signedIn && CurrentUser != null)
            {
                Debug.Log("Signed Out");
            }

            CurrentUser = Auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log($"Sign in : {CurrentUser.UserId}");
            }
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
