using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class B
{
    public static bool isEditor;
    public static bool isMobile;
    public static bool isDesctop;
    public static bool isAndroid;
    public static bool isWindows;
    public static bool isMac;

    public static bool isMobileHaveGyro;


    static B()
    {
        isEditor = Application.platform == RuntimePlatform.WindowsEditor ? true : false;
    }

    public static void Init()
    {
        isEditor = Application.platform == RuntimePlatform.WindowsEditor ? true : false;
        isWindows = Application.platform == RuntimePlatform.WindowsPlayer ? true : false;
        isMac = Application.platform == RuntimePlatform.OSXPlayer ? true : false;
        isAndroid = Application.platform == RuntimePlatform.Android ? true : false;
        isMobile = Application.platform == RuntimePlatform.Android ? true : false;
        if(!isMobile) isMobile = Application.platform == RuntimePlatform.IPhonePlayer ? true : false;
        if (isMobile) { isMobileHaveGyro = SystemInfo.supportsGyroscope; }
        if (isMac || isWindows) { isDesctop = true; } else { isDesctop = false; }
    } 
}
