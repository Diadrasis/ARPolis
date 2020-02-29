using System;
using UnityEngine;

namespace ARPolis.Android
{
    public static class AndroidBridge
    {
        public static void OpenIntent(string intentName)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                Debug.LogWarning("Editor: fake call "+intentName);
                return;
            }

            if (Application.platform != RuntimePlatform.Android) { return; }

            try
            {
                #if UNITY_ANDROID
                using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var intentObject = new AndroidJavaObject("android.content.Intent", intentName))
                {
                    currentActivityObject.Call("startActivity", intentObject);
                }
                #endif
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Debug.Log(ex.Message);
            }

        }


        public static bool CheckAppInstallation(string bundleId)
        {
            if (Application.platform != RuntimePlatform.Android) { return false; }
            
                bool installed = false;
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = curActivity.Call<AndroidJavaObject>("getPackageManager");

            AndroidJavaObject launchIntent = null;
            try
            {
                launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
                if (launchIntent == null)
                {
                    installed = false;
                }
                else
                {
                    installed = true;
                }
            }

            catch (Exception)
            {
                installed = false;
            }

            unityPlayer.Dispose();
            curActivity.Dispose();
            packageManager.Dispose();
            launchIntent.Dispose();

            return installed;
        }


        public static void DetectAppInstallation(string bundleId)
        {
            if (Application.platform != RuntimePlatform.Android) { return; }

            bool installed = false;
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = curActivity.Call<AndroidJavaObject>("getPackageManager");

            AndroidJavaObject launchIntent = null;
            try
            {
                launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
                if (launchIntent == null)
                {
                    installed = false;
                }
                else
                {
                    installed = true;
                }
            }

            catch (System.Exception)
            {
                installed = false;
            }

            if (!installed)
            { //open app in store
                Application.OpenURL("https://play.google.com/store/apps/details?id="+ bundleId);
            }
            else
            { //open the app
                curActivity.Call("startActivity", launchIntent);
            }

            unityPlayer.Dispose();
            curActivity.Dispose();
            packageManager.Dispose();
            launchIntent.Dispose();
            
        }


        public static void OpenMapsIntent(Vector2 destination)
        {
            string query = "google.navigation:q="+destination.y.ToString()+","+destination.x.ToString();

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                Debug.LogWarning("Editor: fake call " + destination);
                return;
            }

            if (Application.platform != RuntimePlatform.Android) { return; }

            try
            {
#if UNITY_ANDROID
                using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var intentObject = new AndroidJavaObject("android.content.Intent", new string[] { IntentNames.MAPS, query }))
                {
                    currentActivityObject.Call("startActivity", intentObject);
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Debug.Log(ex.Message);
            }

        }

    }

}
