using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeOpenOtherApp : MonoBehaviour
{
    //    private void Awake()
    //    {
    //#if !UNITY_ANDROID || UNITY_EDITOR
    //        gameObject.SetActive(false);
    //#endif
    //    }

    readonly string arGameBundleID = "net.diadrasis.arpolis_game";// "net.Diadrasis.ARFrame"; //"net.diadrasis.arpolis_game"; //"com.Diadrasis.ARPolis";
    readonly string arGameStoreURL = "https://play.google.com/store/apps/details?id=net.Diadrasis.ARPolis";

    //SystemInfo.operatingSystem
    void Start()
    {
        // Prints "Windows 7 (6.1.7601) 64bit" on 64 bit Windows 7
        // Prints "Mac OS X 10.10.4" on Mac OS X Yosemite
        // Prints "iPhone OS 8.4" on iOS 8.4
        // Prints "Android OS API-22" on Android 5.1
        //Debug.Log(SystemInfo.operatingSystem);
    }


    public void OpenApp()
    {
        //Debug.Log("open app "+arGameBundleID);
#if UNITY_ANDROID && !UNITY_EDITOR
        if (IsAndroidAppInstalled(arGameBundleID)) { /*Debug.Log("############### installed");*/ } 
        else { Application.OpenURL(arGameStoreURL); }
#elif UNITY_IOS
        
#else
        Application.OpenURL(arGameStoreURL); 
#endif
    }

    void OpenNewApp(string bundleId, string storeURL)// your target bundle id, google play store url
    {
#if UNITY_ANDROID
        bool fail = false;
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

        AndroidJavaObject launchIntent = null;
        try
        {
            launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
        }
        catch (Exception e)
        {
            fail = true;
        }

        if (fail)
        { //open app in store
            Application.OpenURL(storeURL);
        }
        else //open the app
            ca.Call("startActivity", launchIntent);

        up.Dispose();
        ca.Dispose();
        packageManager.Dispose();
        launchIntent.Dispose();
#endif


        
    }

    bool IsAndroidAppInstalled(string bundleID)
    {
        bool isInstalled = false;

        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

            //Debug.Log(" ********LaunchOtherApp ");

            AndroidJavaObject launchIntent = null;

            //if the app is installed, no errors. Else, doesn't get past next line
            try
            {
                launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", arGameBundleID);// "net.diadrasis.arpolis_game");// bundleID); //getLaunchIntentForPackage

                ca.Call("startActivity", launchIntent);
            }
            catch (Exception ex)
            {
                Debug.Log("exception" + ex.Message);
            }

            if (launchIntent == null)
            {
                isInstalled = false;
            }
            else
            {
                launchIntent.Dispose();

                isInstalled = true;
            }

            if (up != null)
            {
                up.Dispose();
            }
            if (ca != null)
            {
                ca.Dispose();
            }
            if (packageManager != null)
            {
                packageManager.Dispose();
            }


            return isInstalled;
        }
        else
        {
            return false;
        }
    }


}
