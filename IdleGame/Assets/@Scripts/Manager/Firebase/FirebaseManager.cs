using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using Cysharp.Threading.Tasks;


//NOTE : partial class는 이 이름으로 사용하게 해줌
public partial class FirebaseManager
{
    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    private DatabaseReference reference;
    public async UniTask Init()
    {

        var status = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (status != DependencyStatus.Available)
        {
            Debug.Log("Firebase 초기화 실패");
            return;
        }

        auth = FirebaseAuth.DefaultInstance;
        currentUser = auth.CurrentUser;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("Firebase 초기화 성공");

        await GuestLogin();
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
