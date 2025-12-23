using Firebase.Auth;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase.Extensions;
public partial class FirebaseManager
{
    //Guest
    public void GuestLogin()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {

            if(task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("게스트 로그인 실패");
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log("게스트 로그인 성공 ! 사용자 ID : " + user.UserId);
            ReadDataAsync().Forget();
        });
    }
    
}
