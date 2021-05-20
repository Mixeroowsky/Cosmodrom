using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
#if UNITY_ANDROID
    static readonly string storeID = "3960727";
#elif UNITY_IOS
    static readonly string storeID = "3960726";
#endif
    static readonly string videoID = "video";
    static readonly string rewardedID = "rewardedVideo";
    static readonly string bannerID = "bannerAd";
    Action adSuccess;
    Action adSkipped;
    Action adFailed;
#if UNITY_EDITOR
    static bool testMode = true;
#else
    static bool testMode = false;
#endif
    public static AdManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            Advertisement.AddListener(this);
            Advertisement.Initialize(storeID, testMode);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static void ShowStandardAd()
    {
        if (Advertisement.IsReady(videoID))
        {
            Advertisement.Show(videoID);
        }
    }
    public static void ShowBanner()
    {
        instance.StartCoroutine(ShowBannerWhenReady());
    }
    public static void HideBanner()
    {
        Advertisement.Banner.Hide();
    }
    public static void ShowRewardedAd(Action success, Action skipped, Action failed)
    {
        instance.adSuccess = success;
        instance.adSkipped = skipped;
        instance.adFailed = failed;
        if (Advertisement.IsReady(rewardedID))
        {
            Advertisement.Show(rewardedID);
        }
    }
    static IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(bannerID))
        {
            yield return new WaitForSeconds(.5f);
        }            
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(bannerID);

    }
    public void OnUnityAdsDidError(string message)
    {
        
    }
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if(placementId == rewardedID)
        {
            switch(showResult)
            {
                case ShowResult.Finished:
                    adSuccess();
                    break;
                case ShowResult.Skipped:
                    adSkipped();
                    break;
                case ShowResult.Failed:
                    adFailed();
                    break;
            }
        }
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        
    }

    public void OnUnityAdsReady(string placementId)
    {
        
    }
    void OnDestroy()
    {
        Debug.Log("DestroyAdController");
        Advertisement.RemoveListener(this);
    }

}
