using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
public class GPGSManager
{
    private bool isInitialized = false;
    bool isLoggedIn = false;
    public void Init()
    {

        if (isInitialized || isLoggedIn) return;
#if UNITY_ANDROID
        SignIn(Managers.GameM.Level);
#else
        Debug.Log("이 플랫폼은 google Play Games를 지원하지 않습니다.");
#endif

        isInitialized = true;
    }

    public void SignIn(int _score)
    {
#if UNITY_ANDROID
        if (isLoggedIn) return;

        PlayGamesPlatform.Instance.Authenticate((result) =>
        {
            if (result == SignInStatus.Success)
            {
                Debug.Log("로그인 성공 ");
                SaveScore(_score);
            }
            else
            {
                Debug.Log("로그인 실패 : " + result);
            }
        });
#endif
    }

    public void ShowLeaderboardUI()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_combatpower);
#endif
    }

    public void SaveScore(int _damage)
    {
#if UNITY_ANDROID
        if(!isLoggedIn)
        {
            Debug.LogError("[GPGSManager] 로그인 되지 않아 저장을 건너뜁니다.");
        }
        PlayGamesPlatform.Instance.ReportScore(_damage, GPGSIds.leaderboard_combatpower, (bool _isCompleted) =>
        {
            if (_isCompleted)
            {

                Debug.Log("리더보드 저장에 성공");
            }
        });
#endif
    }

    public void Clear()
    {
#if UNITY_ANDROID
        isLoggedIn = false;
        Debug.Log("[GPGSManager] 구글 플레이 게임 로그아웃 완료");
#endif
        isInitialized = false;
    }
}
