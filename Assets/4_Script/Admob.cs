using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class Admob : MonoBehaviour
{
    string adUnitId = "ca-app-pub-3940256099942544/5354046379";
    public MapButton RewardGameObject;
    public GameObject AdLoadedStatus;
    private RewardedInterstitialAd riAd;

    public void LoadAd()
    {
        if (riAd != null)
        {
            DestroyAd();
        }

        var adRequest = new AdRequest();
        RewardedInterstitialAd.Load(adUnitId, adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                if (error != null) return;
                if (ad == null) return;
                riAd = ad;
                RegisterEventHandlers(ad);
                AdLoadedStatus?.SetActive(true);
                RewardGameObject.SetNumberAlpha();
            });
    }

    public void ShowAd()
    {
        if (riAd != null && riAd.CanShowAd())
        {
            riAd.Show((Reward reward) => 
            {
                RewardGameObject.RewardAd();
            });
        }
        AdLoadedStatus?.SetActive(false);
    }

    public void DestroyAd()
    {
        if (riAd != null)
        {
            riAd.Destroy();
            riAd = null;
        }
        AdLoadedStatus?.SetActive(false);
    }

    public void LogResponseInfo()
    {
        if (riAd != null)
        {
            var responseInfo = riAd.GetResponseInfo();
            UnityEngine.Debug.Log(responseInfo);
        }
    }

    protected void RegisterEventHandlers(RewardedInterstitialAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) => {};
        ad.OnAdImpressionRecorded += () => {};
        ad.OnAdClicked += () => {};
        ad.OnAdFullScreenContentOpened += () => {};
        ad.OnAdFullScreenContentClosed += () => {};
        ad.OnAdFullScreenContentFailed += (AdError error) => {};
    }
}
