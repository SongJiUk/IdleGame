using Firebase.Auth;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase.Extensions;
public partial class FirebaseManager
{
    //Guest
    public async UniTask GuestLogin()
    {
        try
        {
            Firebase.Auth.AuthResult authResult = await auth.SignInAnonymouslyAsync().AsUniTask();
            FirebaseUser user = authResult.User;
            Debug.Log("게스트 로그인 성공 ! 사용자 ID : " + user.UserId);
            await ReadDataAsync();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"로그인 실패 : {e.Message}");
        }

        // auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        // {

        //     if (task.IsCanceled || task.IsFaulted)
        //     {
        //         Debug.Log("게스트 로그인 실패");
        //         return;
        //     }

        //     FirebaseUser user = task.Result.User;
        //     Debug.Log("게스트 로그인 성공 ! 사용자 ID : " + user.UserId);
        //     ReadDataAsync().Forget();
        // });
    }

}
