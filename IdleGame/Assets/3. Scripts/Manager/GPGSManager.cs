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

        if (isInitialized) return;
#if UNITY_ANDROID
        PlayGamesPlatform.Activate();
        SignIn(0);
#else
        Debug.Log("이 플랫폼은 google Play Games를 지원하지 않습니다.");
#endif

        isInitialized = true;
    }

    public void SignIn(long _score)
    {
#if UNITY_ANDROID
        if (isLoggedIn) return;

        PlayGamesPlatform.Instance.Authenticate((result) =>
        {
            if (result == SignInStatus.Success)
            {
                isLoggedIn = true;
                Debug.Log("[GPGSManager] 로그인 성공 ");
                SaveScore(_score);
            }
            else if(result == SignInStatus.Canceled)
            {
                // 자동 로그인 취소는 정상 케이스 (유저 미동의 또는 에디터 DummyClient)
                Debug.LogWarning("[GPGSManager] 자동 로그인 취소됨. 리더보드 버튼 클릭 시 재시도합니다.");
            }
            else
            {
                Debug.LogError("[GPGSManager] 로그인 실패 결과값: " + result);
            }
        });
#endif
    }

    public void ShowLeaderboardUI()
    {
#if UNITY_ANDROID
        Managers.GameM.isInternalPause = true;
        if(PlayGamesPlatform.Instance.IsAuthenticated())
        {
            SaveScore((long)Managers.PlayerM.AverageCombatPower(), () =>
            {
                PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_combatpower);
            });
        }
        else
        {
            Debug.Log("[GPGSManager] 인증되지 않음. 재인증 시도 중...");
            PlayGamesPlatform.Instance.ManuallyAuthenticate((result) =>
            {
                if (result == SignInStatus.Success)
                {
                    isLoggedIn = true;
                    SaveScore((long)Managers.PlayerM.AverageCombatPower(), () =>
                    {
                        PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_combatpower);
                    });
                }
                else
                {
                    Debug.LogError("[GPGSManager] 로그인 실패: 리더보드를 열 수 없습니다. " + result);
                }
            });
        }

#endif
    }

    public void SaveScore(long _damage, System.Action _onComplete = null)
    {
#if UNITY_ANDROID
        if(!isLoggedIn)
        {
            Debug.LogError("[GPGSManager] 로그인 되지 않아 저장을 건너뜁니다.");
            return;
        }
        Debug.Log($"[GPGSManager] 리더보드 점수 저장 시도: {_damage}");
        PlayGamesPlatform.Instance.ReportScore(_damage, GPGSIds.leaderboard_combatpower, (bool _isCompleted) =>
        {
            if (_isCompleted)
            {
                Debug.Log("[GPGSManager] 리더보드 저장에 성공");
                _onComplete?.Invoke();
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
