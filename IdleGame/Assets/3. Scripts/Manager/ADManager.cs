using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
public class ADManager
{
    private bool TestMode = true;
    public readonly string Test_Banner_ID = "ca-app-pub-3940256099942544/6300978111";
    public readonly string Test_Interstitial_ID = "ca-app-pub-3940256099942544/1033173712";
    public readonly string Test_Reward_ID = "ca-app-pub-3940256099942544/5224354917";

    string BannderID;
    string IntersititialID;
    string RewardID;

    BannerView bannerView;
    InterstitialAd interstitialAd;
    RewardedAd rewardedAd;

    Action OnRewardedCallback;
    Action OnReSumeCallback;

    public void Init()
    {
        SetAdID();


        MobileAds.Initialize(initStatus =>
        {
            LoadRewardedAd();
            LoadIntersititialAd();
            RequestBanner();

            //Debug.Log("광고 초기화 완료");
        });


    }

    public void SetAdID()
    {

        if (TestMode || Application.isEditor)
        {
            BannderID = Test_Banner_ID;
            IntersititialID = Test_Interstitial_ID;
            RewardID = Test_Reward_ID;
        }
        else
        {
#if UNITY_ANDROID
            BannderID = "ca-app-pub-7745197509342356/9199251409";
            IntersititialID = "ca-app-pub-7745197509342356/3363830166";
            RewardID = "ca-app-pub-7745197509342356/3691133226";
#elif UNITY_IOS
            BannderID = "ca-app-pub-7745197509342356/4090394957";
            IntersititialID = "ca-app-pub-7745197509342356/8132763505";
            RewardID = "ca-app-pub-7745197509342356/5506600169";
#else
            BannderID = Test_Banner_ID;
            IntersititialID = Test_Interstitial_ID;
            RewardID = Test_Reward_ID;
#endif
        }

    }

    public void OnAdRewardCallback(RewardedAd _ad, LoadAdError _error)
    {
        if (_error != null || _ad == null)
        {
            Debug.LogError($"보상형 광고 로드 실패 : {_error}");
            return;
        }

        rewardedAd = _ad;

        rewardedAd.OnAdFullScreenContentClosed += () =>
        {
            //Debug.Log("광고 닫힘, 다시 로드 시도");
            OnReSumeCallback?.Invoke();
            OnRewardedCallback = null;
            OnReSumeCallback = null;
            LoadRewardedAd();
        };

        rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"보상형 광고 로드 실패 : {error.GetMessage()}");
            LoadRewardedAd();
        };

        rewardedAd.OnAdPaid += (AdValue advalue) =>
        {
            if (OnRewardedCallback != null)
            {
                OnRewardedCallback?.Invoke();
                OnRewardedCallback = null;
            }
        };
    }

    public void LoadRewardedAd()
    {
        AdRequest request = new AdRequest();
        RewardedAd.Load(RewardID, request, OnAdRewardCallback);
    }

    public void ShowRewardedAd(Action _OnRewarded, Action _OnResume)
    {
        OnRewardedCallback = _OnRewarded;
        OnReSumeCallback = _OnResume;

        if (Managers.GameM.gameData.ADS_Remove)
        {
            if (OnRewardedCallback != null)
            {
                OnRewardedCallback?.Invoke();
                OnRewardedCallback = null;

                Managers.QuestM.GetMission(Define.MissionTarget.WatchingAD).Progress++;
            }
            return;
        }

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {

            AudioListener.pause = true;
            rewardedAd.Show((Reward reward) =>
            {
                //Debug.Log($"보상형 광고 보상 지급 : {reward.Type} / {reward.Amount}");
                Managers.QuestM.GetMission(Define.MissionTarget.WatchingAD).Progress++;
                if (OnRewardedCallback != null)
                {
                    OnRewardedCallback?.Invoke();
                    OnRewardedCallback = null;
                }

                AudioListener.pause = false;
            });
        }
        else
        {
            //Debug.Log("광고 준비 안됌, 로드 시작");
            LoadRewardedAd();
        }
    }

    public void LoadIntersititialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        AdRequest request = new AdRequest();
        InterstitialAd.Load(IntersititialID, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"전면광고 로드 실패 : {error.GetMessage()}");
                return;
            }
            interstitialAd = ad;

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                //Debug.Log("전면광고 닫힘. 다시 로드 시도");
                LoadIntersititialAd();
            };
        });
    }

    public void ShowIntersititialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            //Debug.Log("전면광고 준비 안됌. 로드 시작");
            LoadIntersititialAd();
        }
    }

    public void RequestBanner()
    {
        if (bannerView != null) bannerView.Destroy();
        AdSize size = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        bannerView = new BannerView(BannderID, size, AdPosition.Bottom);

        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
        HideBanner();
        bannerView.OnBannerAdLoaded += () =>
        {
            //Debug.Log("배너 광고 로드 완료");
        };

        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError($"배너 광고 로드 실패 : {error.GetMessage()}");
        };
    }

    public void HideBanner()
    {
        bannerView?.Hide();
    }

    public void ShowBanner()
    {
        bannerView?.Show();
    }

    public void DestoryBanner()
    {
        bannerView?.Destroy();
        bannerView = null;
    }

    public void Clear()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }

        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("[ADManger] 광고 객체들 정리 완료");
    }
}
