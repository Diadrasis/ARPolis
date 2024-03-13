using UnityEditor;

[InitializeOnLoad]
public class PreloadSigningAlias
{
    static PreloadSigningAlias()
    {
        PlayerSettings.Android.keystoreName = "Deploy/diadrasis.keystore";
        PlayerSettings.Android.keystorePass = "!diadrasis$";
        PlayerSettings.Android.keyaliasName = "arpolis";
        PlayerSettings.Android.keyaliasPass = PlayerSettings.Android.keystorePass;

    }
}
