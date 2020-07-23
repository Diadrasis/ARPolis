using UnityEngine;
using UnityEditor;
using System;
using System.IO;

/// <summary>
/// ISL 02/07/2018 Sta.Ge.
/// </summary>

public class ScreenShot
{
    static string saveFolder = Application.persistentDataPath + "/screenshots/";
    static string todayFolder;

    [MenuItem("Screenshot/Description/Usefull for publishing to app store.")]
    [MenuItem("Screenshot/Description/")]
    [MenuItem("Screenshot/Description/Take screenshots from game view window.")]

    [MenuItem("Screenshot/Zoom/x1")]
    public static void ScreenShotA() { TakeScreenShot(1); }
    
    [MenuItem("Screenshot/Zoom/x2")]
    public static void ScreenShotB() { TakeScreenShot(2); }

    // [MenuItem("Screenshot/Zoom/x3")]
    public static void ScreenShotC() { TakeScreenShot(3); }

    [MenuItem("Screenshot/Zoom/x4")]
    public static void ScreenShotD() { TakeScreenShot(4); }

    //[MenuItem("Screenshot/Zoom/x5")]
    public static void ScreenShotE() { TakeScreenShot(5); }

    //[MenuItem("Screenshot/Zoom/x6")]
    public static void ScreenShotF() { TakeScreenShot(6); }

    static void TakeScreenShot(int val)
    {
        Vector2 size = GetMainGameViewSize();
        size *= val;
        if (!Directory.Exists(saveFolder)) { Directory.CreateDirectory(saveFolder); }
        todayFolder = DateTime.Today.Year.ToString() + "_" + DateTime.Today.DayOfYear.ToString() + "/";
        if (!Directory.Exists(saveFolder + todayFolder)) { Directory.CreateDirectory(saveFolder + todayFolder); }
        string folderSize = saveFolder + todayFolder + "size x" + val.ToString() +"/";
        if (!Directory.Exists(folderSize)) { Directory.CreateDirectory(folderSize); }
        string folderDimensions = folderSize + size.x.ToString("F0") + "x" + size.y.ToString("F0") + "/";
        if (!Directory.Exists(folderDimensions)) { Directory.CreateDirectory(folderDimensions); }
        string url = folderDimensions + TimeUtilities.GetUTCUnixTimestamp().ToString()+".png";
        ScreenCapture.CaptureScreenshot(url, val);
        Debug.LogWarning("Screenshot with size x"+val);
        Debug.LogWarning("File folder: " + url);
    }

    static Vector2 GetMainGameViewSize()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
        return (Vector2)Res;
    }


    [MenuItem("Screenshot/Open Folder")]
    public static void OpenSaveFolder()
    {
        bool openInsidesOfFolder = false;
        string folderPath = saveFolder.Replace(@"/", @"\");
        if (Directory.Exists(folderPath)) { openInsidesOfFolder = true;}
        try{  System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + folderPath);  }
        catch (System.ComponentModel.Win32Exception e) { e.HelpLink = ""; }
    }

}
