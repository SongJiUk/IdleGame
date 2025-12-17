using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

//NOTE : partial class는 이 이름으로 사용하게 해줌
public partial class FirebaseManager
{
    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    private DatabaseReference reference;
    public void Init()
    {
        //NOTE : 초기화 작업
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if(task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                currentUser = auth.CurrentUser;
                //NOTE : realTimeDB 참조 얻는거
                reference = FirebaseDatabase.DefaultInstance.RootReference;

                GuestLogin();
                Managers.UpdateM.isStartFirebase = true;
                Debug.Log("Firebase 초기화 성공");
            }
            else
            {
                Debug.Log("Firebase 초기화 실패");
            }
        });
    }
}
