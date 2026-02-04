using Firebase.Auth;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Google;
using Firebase.Extensions;
using System.Threading.Tasks;

public partial class FirebaseManager
{
    public async void OnGoogleLoginClicked()
    {
        DebugConsole.Instance.Log("구글 로그인 버튼 눌림 - 함수 진입");
      
        if (googleConfiguration == null)
        {
            DebugConsole.Instance.Log("googleConfiguration == NULL");
            return;
        }
        try
        {
            DebugConsole.Instance.Log("Configuration 설정 시도");
            GoogleSignIn.Configuration = googleConfiguration;
            DebugConsole.Instance.Log("Configuration 설정 완료");

            DebugConsole.Instance.Log("GoogleSignIn.SignIn() 호출 직전");
            var googleUser = await GoogleSignIn.DefaultInstance.SignIn();
            DebugConsole.Instance.Log("GoogleSignIn.SignIn() 리턴 받음");

            await OnGoogleAuthFinished(googleUser);
            DebugConsole.Instance.Log("OnGoogleAuthFinished 호출 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Google Login Failed" + e.Message);
            DebugConsole.Instance.Log("Google Login Failed: " + e.ToString());
        }
    }

    private async UniTask OnGoogleAuthFinished(GoogleSignInUser _googleUser)
    {
        DebugConsole.Instance.Log("OnGoogleAuthFinished 진입");

        if (_googleUser== null)
        {
            Debug.LogError("Google Login Failed");
            DebugConsole.Instance.Log("_googleUser == NULL");
            return;
        }
        try
        {
            DebugConsole.Instance.Log("Firebase Credential 생성");
            var credential = GoogleAuthProvider.GetCredential(_googleUser.IdToken, null);
            DebugConsole.Instance.Log("Firebase SignInWithCredentialAsync 호출");
            FirebaseUser user = await auth.SignInWithCredentialAsync(credential);
            DebugConsole.Instance.Log("Firebase 로그인 성공");
            currentUser = user;
            PlayerPrefs.SetFloat("BGM", 1.0f);
            PlayerPrefs.SetFloat("EFFECT", 1.0f);
            DebugConsole.Instance.Log("ReadData 호출");
            await ReadData();
            Debug.Log("구글 로그인 성공 ! 사용자 ID : " + user.UserId);
            DebugConsole.Instance.Log("LoginComplete 호출");
            LoadingLogin.instance.LoginComplete();
            loginCTS?.TrySetResult(true);
            
        }
        catch(System.Exception e)
        {
            Debug.LogError("Firebase Login Failed" + e.Message);
            DebugConsole.Instance.Log("Firebase Login Failed: " + e.ToString());
        }
        
        
    }

    public async UniTask GuestLogin()
    {
        DebugConsole.Instance.Log("GuestLogin: 함수 진입");
        if (auth == null)
        {
            DebugConsole.Instance.Log("GuestLogin: auth == NULL");
            return;
        }
        try
        {
            DebugConsole.Instance.Log("GuestLogin: SignInAnonymouslyAsync 호출 직전");
            FirebaseUser user = await auth.SignInAnonymouslyAsync();
            DebugConsole.Instance.Log("GuestLogin: SignInAnonymouslyAsync 완료");
            if (user == null)
            {
                DebugConsole.Instance.Log("GuestLogin: user == NULL");
                return;
            }
            currentUser = user;
            DebugConsole.Instance.Log("GuestLogin: currentUser 설정 완료");
            PlayerPrefs.SetFloat("BGM", 1.0f);
            PlayerPrefs.SetFloat("EFFECT", 1.0f);


            DebugConsole.Instance.Log("GuestLogin: ReadData 호출");
            await ReadData();
            DebugConsole.Instance.Log("GuestLogin: ReadData 완료");
            Debug.Log("게스트 로그인 성공 ! 사용자 ID : " + user.UserId);
            DebugConsole.Instance.Log("GuestLogin: LoginComplete 호출");
            LoadingLogin.instance.LoginComplete();
            DebugConsole.Instance.Log("GuestLogin: loginCTS TrySetResult");
            loginCTS?.TrySetResult(true);
            DebugConsole.Instance.Log("GuestLogin: 전체 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"로그인 실패 : {e.Message}");
            DebugConsole.Instance.Log("GuestLogin Exception: " + e.ToString());
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
