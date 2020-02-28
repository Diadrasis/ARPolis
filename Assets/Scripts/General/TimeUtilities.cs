using System;
using UnityEngine;

public static class TimeUtilities
{

    /// <summary>
    /// Returns current UTC Timestamp in milliseconds since the epoch
    /// </summary>
    /// <returns></returns>
    public static long GetUTCUnixTimestamp()
    {
        return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
    }
    /// <summary>
    /// Returns current UTC Timestamp in string format yyyy-MM-dd HH:mm:ss.fff
    /// </summary>
    /// <returns></returns>
    public static string GetUTCDateTimeString()
    {
        return DateTime.UtcNow.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff");
    }

    public static float GetLastTimestampDifferenceInHours()
    {
        DateTime currentDay = DateTime.UtcNow;

        long temp = Convert.ToInt64(PlayerPrefs.GetString("lastQuitSession"));

        DateTime lastSaveDay = DateTime.FromBinary(temp);

        TimeSpan diff = currentDay.Subtract(lastSaveDay);

        return (float)diff.TotalHours;
    }


    public static float GetSessionWaitingIdleSeconds()
    {
        return (GetLastTimestampDifferenceInHours() * 60f * 60f) + 10f;
    }

    public static float GetRemainSecondsToMidnight()
    {
        DateTime current = DateTime.Now; // current time
        DateTime tomorrow = current.AddDays(1).Date; //"next" midnight

        double secondsUntilMidnight = (tomorrow - current).TotalSeconds;

        return (float)secondsUntilMidnight;
    }


    //public static bool IsSameDay()
    //{
    //    long temp = Convert.ToInt64(PlayerPrefs.GetString("todayDate"));

    //    DateTime todayDate = DateTime.FromBinary(temp);

    //    return todayDate.IsToday();
    //}

    public static string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
}
