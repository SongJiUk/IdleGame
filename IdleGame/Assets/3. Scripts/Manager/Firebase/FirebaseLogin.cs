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
            configuration = GetGoogleConfig();
            GoogleSignIn.Configuration = configuration;

            GoogleSignInUser googleUser = await GoogleSignIn.DefaultInstance.SignIn();

            if (googleUser == null || string.IsNullOrEmpty(googleUser.IdToken))
            {
                Debug.LogError("[FirebaseManager] : Google SignIn failed : user is null");
                return;
            }

            Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);
            var result = await Auth.SignInWithCredentialAsync(credential);

            CurrentUser = result;
            ApplyUserToGameData(CurrentUser);

            PlayerPrefs.SetInt("HasSeenLogin", 1);
            PlayerPrefs.Save();

            Debug.Log("[FirebaseManager] : Firebase Google Login Success!");
            Debug.Log($"[FirebaseManager] : UserID : {result.UserId}");

            await ReadData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FirebaseManager] : 구글 로그인 실패 : {e}");
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
                    if (!string.IsNullOrEmpty(CurrentUser.Email))
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


    public async UniTask LinkGoogleToCurrentUser()
    {
        Debug.Log("[LOGIN] LinkGoogleToCurrentUser START");
        if (Auth.CurrentUser == null || !Auth.CurrentUser.IsAnonymous)
        {

            Debug.LogWarning("게스트가 아니거나 유저가 없음. Link 불가");
            return;
        }

        Debug.Log($"[LOGIN] IsAnonymous: {Auth.CurrentUser.IsAnonymous}");

        try
        {
            // configuration = GetGoogleConfig();
            // GoogleSignIn.Configuration = configuration; ;

            // GoogleSignInUser googleUser = await GoogleSignIn.DefaultInstance.SignIn();
            // if (googleUser == null || string.IsNullOrEmpty(googleUser.IdToken))
            // {
            //     Debug.LogError("google SignIn 실패");
            //     return;
            // }

            // Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);
            // var result = await Auth.CurrentUser.LinkWithCredentialAsync(credential);

            // CurrentUser = result;
            // ApplyUserToGameData(CurrentUser);

            // PlayerPrefs.SetInt("HasSeenLogin", 1);
            // PlayerPrefs.Save();

            // Debug.Log("게스트 -> 구글 계정 연동 성공 : UID : " + result.UserId);

            // await ReadData();
            Debug.Log("[LOGIN] Set Google Configuration");
            configuration = GetGoogleConfig();
            GoogleSignIn.Configuration = configuration;

            Debug.Log("[LOGIN] Call GoogleSignIn.SignIn()");
            GoogleSignInUser gUser = await GoogleSignIn.DefaultInstance.SignIn();

            Debug.Log("[LOGIN] GoogleSignIn returned");

            if (gUser == null)
            {
                Debug.LogError("[LOGIN] GoogleSignInUser is NULL");
                return;
            }

            Debug.Log("[LOGIN] Got IdToken");
            if (string.IsNullOrEmpty(gUser.IdToken))
            {
                Debug.LogError("[LOGIN] IdToken is NULL or EMPTY");
                return;
            }

            Credential credential = GoogleAuthProvider.GetCredential(gUser.IdToken, null);

            Debug.Log("[LOGIN] Call LinkWithCredentialAsync()");
            var result = await Auth.CurrentUser.LinkWithCredentialAsync(credential);

            Debug.Log("[LOGIN] LinkWithCredentialAsync SUCCESS");

            CurrentUser = result;
            ApplyUserToGameData(CurrentUser);

            Debug.Log("[LOGIN] ApplyUserToGameData DONE");

            PlayerPrefs.SetInt("HasSeenLogin", 1);
            PlayerPrefs.Save();

            Debug.Log("[LOGIN] PlayerPrefs Saved");

            await ReadData();

            Debug.Log("[LOGIN] ReadData DONE");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[LOGIN] EXCEPTION: " + e);
            //Debug.Log("계정 연동 실패 : " + e);
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
    void ApplyUserToGameData(FirebaseUser _user)
    {

        if (_user == null || _user.IsAnonymous)
        {
            Managers.GameM.gameData.isGuest = true;
            Managers.GameM.gameData.playerName = "Guest";
        }
        else
        {
            Managers.GameM.gameData.isGuest = false;

            string name = user.DisplayName;

            if (string.IsNullOrEmpty(name))
            {
                if (!string.IsNullOrEmpty(user.Email))
                    name = user.Email;
                else
                    name = "Player";
            }

            Managers.GameM.gameData.playerName = name;
        }
    }

    GoogleSignInConfiguration GetGoogleConfig()
    {
        return new GoogleSignInConfiguration()
        {
            WebClientId = webClientId,
            RequestIdToken = true,
            RequestEmail = true
        };
    }

}
