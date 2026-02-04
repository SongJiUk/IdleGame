using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Google;


//NOTE : partial class는 이 이름으로 사용하게 해줌
public partial class FirebaseManager
{
    private UniTaskCompletionSource<bool> loginCTS;
    private FirebaseAuth auth;
    private GoogleSignInConfiguration googleConfiguration;
    private FirebaseUser currentUser;
    public FirebaseUser CurrentUser
    {
        get => currentUser;
    }
    private DatabaseReference reference;
    public async UniTask Init()
    {
        DebugConsole.Instance.Log("Init: 시작");
        DebugConsole.Instance.Log("Init: FirebaseApp.CheckAndFixDependenciesAsync 호출");
        var status = await FirebaseApp.CheckAndFixDependenciesAsync();
        DebugConsole.Instance.Log("Init: CheckAndFixDependenciesAsync 완료, status = " + status);
        if (status != DependencyStatus.Available)
        {
            Debug.Log("Firebase 초기화 실패");
            DebugConsole.Instance.Log("Init: Firebase 초기화 실패");
            return;
        }
        DebugConsole.Instance.Log("Init: FirebaseAuth.DefaultInstance 가져오기");
        auth = FirebaseAuth.DefaultInstance;
        

        currentUser = auth.CurrentUser;
        DebugConsole.Instance.Log("Init: currentUser = " + (currentUser == null ? "NULL" : currentUser.UserId));
        DebugConsole.Instance.Log("Init: FirebaseDatabase RootReference 가져오기");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        DebugConsole.Instance.Log("Init: GoogleSignInConfiguration 생성");
        googleConfiguration = new GoogleSignInConfiguration
        {
            WebClientId = "689965818352-plir62446mohq2eq7vtptl4jca9lrldl.apps.googleusercontent.com",
            RequestIdToken = true
        };
        DebugConsole.Instance.Log("Init: loginCTS 생성");
        loginCTS = new UniTaskCompletionSource<bool>();
        DebugConsole.Instance.Log("Init: LoadingLogin.GetLoginInit 호출");
        LoadingLogin.instance.GetLoginInit();
        DebugConsole.Instance.Log("Init: loginCTS.Task 대기 시작");
        await loginCTS.Task;
        DebugConsole.Instance.Log("Init: loginCTS.Task 완료");
        Debug.Log("Firebase 초기화 성공");
        DebugConsole.Instance.Log("Init: Firebase 초기화 성공");
        Managers.UpdateM.isStartFirebase = true;
       


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
}
