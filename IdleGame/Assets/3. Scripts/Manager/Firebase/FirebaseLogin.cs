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


    public async UniTask<bool> GoogleLogin()
    {
#if UNITY_EDITOR
        Debug.LogWarning("Google Login은 에디터에서 동작하지 않습니다. 디바이스에서 테스트하세요.");
        return false;
#endif

#if UNITY_ANDROID || UNITY_IOS
        try
        {
            SetupGoogleConfig();
            GoogleSignIn.Configuration = configuration;

            GoogleSignInUser googleUser = await GoogleSignIn.DefaultInstance.SignIn();

            if (googleUser == null || string.IsNullOrEmpty(googleUser.IdToken))
            {
                Debug.LogError("[Firebase] Google SignIn 실패: 토큰 없음");
                return false;
            }

            Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);
            var result = await Auth.SignInWithCredentialAsync(credential);

            CurrentUser = result;
            ApplyUserToGameData(CurrentUser);

            PlayerPrefs.SetInt("HasSeenLogin", 1);
            PlayerPrefs.Save();

            Debug.Log("[FirebaseManager] : 파이어베이스 로그인 성공!");
            Debug.Log($"[FirebaseManager] : UserID : {result.UserId}");

            await ReadData();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FirebaseManager] : 구글 로그인 실패 : {e}");
            return false;
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
        if (Auth == null) return;


        try
        {
            FirebaseUser user = await Auth.SignInAnonymouslyAsync();
            if (user == null) return;


            CurrentUser = user;
            ApplyUserToGameData(CurrentUser);

            PlayerPrefs.SetInt("HasSeenLogin", 1);
            PlayerPrefs.Save();

            Debug.Log("게스트 로그인 성공 ! 사용자 ID : " + user.UserId);

            await ReadData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"로그인 실패 : {e.Message}");
        }
    }

    public void SetupGoogleConfig()
    {
        if (configuration == null)
        {
            configuration = new GoogleSignInConfiguration()
            {
                WebClientId = webClientId,
                RequestIdToken = true,
                RequestEmail = true
            };
            GoogleSignIn.Configuration = configuration;
        }
    }

    public async UniTask LinkGoogleToCurrentUser()
    {
        if (Auth.CurrentUser == null || !Auth.CurrentUser.IsAnonymous)
        {
            Debug.LogWarning("게스트가 아니거나 유저가 없음. Link 불가");
            return;
        }

        try
        {
            SetupGoogleConfig();
            GoogleSignIn.Configuration = configuration;
            GoogleSignInUser gUser = await GoogleSignIn.DefaultInstance.SignIn();


            if (gUser == null || string.IsNullOrEmpty(gUser.IdToken)) return;

            Credential credential = GoogleAuthProvider.GetCredential(gUser.IdToken, null);
            var result = await Auth.CurrentUser.LinkWithCredentialAsync(credential);

            CurrentUser = result;
            ApplyUserToGameData(CurrentUser);

            await ReadData();

            Debug.Log("[LOGIN] 구글계정 연동 성공");
        }
        catch (FirebaseException e)
        {
            throw e;

        }
    }


    void ApplyUserToGameData(FirebaseUser _user)
    {
        if (_user == null) return;

        bool isGuest = _user.IsAnonymous;
        Debug.Log("[확인하려는곳] " + isGuest);
        Managers.GameM.gameData.isGuest = isGuest;

        if (isGuest)
        {
            Managers.GameM.gameData.playerName = "Guest";
        }
        else
        {
            string name = !string.IsNullOrEmpty(_user.DisplayName) ? _user.DisplayName :
                     (!string.IsNullOrEmpty(_user.Email) ? _user.Email : "Player");

            Managers.GameM.gameData.playerName = name;
        }
    }

    public async UniTask CheckAndApplyCurrentUser()
    {
        if (Auth.CurrentUser != null)
        {
            CurrentUser = Auth.CurrentUser;
            ApplyUserToGameData(CurrentUser);
            Debug.Log("자동 로그인 적용 : " + CurrentUser.UserId);
            await ReadData();
        }
    }

}
