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
            FirebaseUser user = await auth.SignInAnonymouslyAsync();
            currentUser = user;

            PlayerPrefs.SetFloat("BGM", 1.0f);
            PlayerPrefs.SetFloat("EFFECT", 1.0f);

            Debug.Log("게스트 로그인 성공 ! 사용자 ID : " + user.UserId);
            await ReadData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"로그인 실패 : {e.Message}");
        }
    }

    public async UniTask CheckOrLogin()
    {
        if(auth.CurrentUser != null)
        {
            currentUser = auth.CurrentUser;
            Debug.Log("자동 로그인 성공! 사용자 ID : " + currentUser.UserId);

            await ReadData();
        }
        else
        {
            await GuestLogin();
        }
    }

}
