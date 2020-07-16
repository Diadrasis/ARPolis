using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.Save
{

    public class SaveLoad : MonoBehaviour
    {
        public static void SaveUserCredentials(string name, string pass)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass)) return;
            PlayerPrefsX.SetStringArray("credentials", new string[] { name, pass });
        }

        public static bool LoadUserCredentials(out string name, out string pass)
        {
            bool exists = PlayerPrefs.HasKey("credentials");

            string[] vals = PlayerPrefsX.GetStringArray("credentials");
            if (vals.Length > 0) { name = vals[0]; pass = vals[1]; }
            else { name = pass = string.Empty; }

            return exists;
        }

        public static void SaveRememberCredentialsState(bool val)
        {
            PlayerPrefsX.SetBool("remState", val);
        }

        public static bool GetRememberCredentialsState()
        {
            return PlayerPrefsX.GetBool("remState");
        }

        public static int GetSetAppEntrances()
        {
            int val = 0;
            if (PlayerPrefs.HasKey("entrances"))
            {
                val = PlayerPrefs.GetInt("entrances");
                val++;
            }
            PlayerPrefs.SetInt("entrances", val); 
            return val;
        }
    }

}
