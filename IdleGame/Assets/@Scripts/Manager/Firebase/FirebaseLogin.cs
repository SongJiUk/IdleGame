using Firebase.Auth;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Google;
using Firebase.Extensions;
using System.Threading.Tasks;
using Firebase;
using UnityEngine.Purchasing;
using System.Net;

public partial class FirebaseManager
{
    private string webClientId = "1007607179174-eob21571uq2q6bku1i3l2s8nrmnmdkl3.apps.googleusercontent.com";
    private FirebaseUser user;
    private GoogleSignInConfiguration configuration;
    public async void GoogleLogin()
    {
        configuration = new GoogleSignInConfiguration()
        {
            WebClientId = webClientId,
            RequestIdToken = true,
            RequestEmail = true
        };

#if UNITY_EDITOR
        Debug.LogWarning("Google Login은 에디터에서 동작하지 않습니다. 디바이스에서 테스트하세요.");
        return;
#endif

#if UNITY_ANDROID || UNITY_IOS
        try
        {
            configuration = new GoogleSignInConfiguration()
            {
                WebClientId = webClientId,
                RequestIdToken = true,
                RequestEmail = true
            };

            GoogleSignIn.Configuration = configuration;

            GoogleSignInUser user = await GoogleSignIn.DefaultInstance.SignIn();

            if (user == null)
            {
                Debug.LogError("Google SignIn failed : user is null");
                return;
            }

            string idToken = user.IdToken;
            if (string.IsNullOrEmpty(idToken))
            {
                Debug.LogError("Google SignIn failed : IdToken is null or empty");
                return;
            }

            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
            var result = await Auth.SignInWithCredentialAsync(credential);

            CurrentUser = result;
            Managers.GameM.gameData.isGuest = false;
            string name = CurrentUser.DisplayName;

            if (string.IsNullOrEmpty(name))
            {
                if (!string.IsNullOrEmpty(CurrentUser.DisplayName))
                    name = CurrentUser.Email;
                else
                    name = "Player";
            }

            Managers.GameM.gameData.playerName = name;

            Debug.Log("Firebase Google Login Success!");
            Debug.Log($"UserID : {result.UserId}");

            await ReadData();
            LoadingLogin.instance.LoginComplete();
            loginCTS?.TrySetResult(true);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"구글 로그인 실패 : {e}");
        }
#endif
    }
    public void SignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        SignOutFM();
    }


    public async UniTask GuestLogin()
    {
        if (Auth == null)
        {
            return;
        }
        try
        {
            FirebaseUser user = await Auth.SignInAnonymouslyAsync();
            if (user == null)
            {
                return;
            }
            CurrentUser = user;
            PlayerPrefs.SetFloat("BGM", 1.0f);
            PlayerPrefs.SetFloat("EFFECT", 1.0f);


            await ReadData();
            Debug.Log("게스트 로그인 성공 ! 사용자 ID : " + user.UserId);

            Managers.GameM.gameData.isGuest = true;
            Managers.GameM.gameData.playerName = "Guest";

            LoadingLogin.instance.LoginComplete();
            loginCTS?.TrySetResult(true);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"로그인 실패 : {e.Message}");
        }
    }

    public async UniTask CheckOrLogin()
    {
        if (Auth.CurrentUser != null)
        {
            CurrentUser = Auth.CurrentUser;
            if (CurrentUser.IsAnonymous)
            {
                Managers.GameM.gameData.isGuest = true;
                Managers.GameM.gameData.playerName = "Guest";
            }
            else
            {
                Managers.GameM.gameData.isGuest = false;
                string name = CurrentUser.DisplayName;

                if (string.IsNullOrEmpty(name))
                {
                    if (!string.IsNullOrEmpty(CurrentUser.DisplayName))
                        name = CurrentUser.Email;
                    else
                        name = "Player";
                }
                Managers.GameM.gameData.playerName = name;
            }


            Debug.Log("자동 로그인 성공! 사용자 ID : " + CurrentUser.UserId);

            await ReadData();
        }
        else
        {
            await GuestLogin();
        }
    }

}
