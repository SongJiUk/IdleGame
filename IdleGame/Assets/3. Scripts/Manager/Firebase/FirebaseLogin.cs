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
    private FirebaseUser user;
    private string webClientId = "1007607179174-nikhlhh6i3pirp3du71c24jegjv2enf8.apps.googleusercontent.com";

    public async UniTask<bool> GoogleLogin()
    {
#if UNITY_EDITOR
        Debug.LogWarning("Google Login은 에디터에서 동작하지 않습니다. 디바이스에서 테스트하세요.");
        return false;
        #elif UNITY_ANDROID || UNITY_IOS
        try
        {
            Debug.Log("Config WebClientID : " + GoogleSignIn.Configuration.WebClientId);
            Debug.Log("[FirebasLogin] 구글 로그인 시작...");
            GoogleSignIn.DefaultInstance.SignOut();
            GoogleSignIn.DefaultInstance.Disconnect();

            // 2. 로그인 시도
            Debug.Log("FirebasLogin SignIn() 전");
            GoogleSignIn.Configuration = configuration;
            GoogleSignInUser googleUser = await GoogleSignIn.DefaultInstance.SignIn();
            Debug.Log("FirebasLogin SignIn() 후");
            if (googleUser == null)
            {
                Debug.LogError("[Firebase] Google SignIn 실패: googleUser가 null입니다.");
                return false;
            }

            if (string.IsNullOrEmpty(googleUser.IdToken))
            {
                Debug.LogError("[Firebase] Google SignIn 실패: IdToken이 없습니다.");
                return false;
            }

            // 3. 파이어베이스 인증 수행
            Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);
            var result = await Auth.SignInWithCredentialAsync(credential);

            // 4. 성공 처리
            CurrentUser = result;
            ApplyUserToGameData(CurrentUser);

            PlayerPrefs.SetInt("HasSeenLogin", 1);
            PlayerPrefs.Save();

            Debug.Log($"[FirebaseManager] 로그인 성공! UserID: {result.UserId}");
            return true;
        }
        catch (Google.GoogleSignIn.SignInException e)
        {
            Debug.LogError($"[Google SignIn Error] Status: {e.Status}");
            Debug.LogError($"Message: {e.Message}");
            Debug.LogError($"StackTrace: {e.StackTrace}");
            return false;
        }
        catch (System.Exception e)
        {
            // 예외 상세 출력
            if (e is System.AggregateException agg)
            {
                foreach (var inner in agg.InnerExceptions)
                {
                    Debug.LogError($"[FirebaseManager] 내부 예외: {inner.Message}");
                }
            }
            else
            {
                Debug.LogError($"[FirebaseManager] 예외 발생: {e.Message}\n{e.StackTrace}");
            }
            return false;
        }
#endif
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

            //await ReadData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"로그인 실패 : {e.Message}");
        }
    }

    public void SetupGoogleConfig()
    {
        if (configuration != null && GoogleSignIn.Configuration != null) return;
        Debug.Log("FirebasLogin new GoogleSignInConfiguration  전");
        configuration = new GoogleSignInConfiguration()
        {
            WebClientId = webClientId,
            RequestIdToken = true,
            RequestEmail = true
        };
        GoogleSignIn.Configuration = configuration;
        Debug.Log("[FirebaseManager] GoogleSignInConfiguration 설정 완료: " + webClientId);
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
            GoogleSignInUser gUser = await GoogleSignIn.DefaultInstance.SignIn();

            if (gUser == null || string.IsNullOrEmpty(gUser.IdToken)) return;

            Credential credential = GoogleAuthProvider.GetCredential(gUser.IdToken, null);
            var result = await Auth.CurrentUser.LinkWithCredentialAsync(credential);

            CurrentUser = result;
            ApplyUserToGameData(CurrentUser);

            //await ReadData();

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
            //await ReadData();
        }
    }

}
