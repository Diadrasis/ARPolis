using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnSelectedOS : MonoBehaviour {

    public GameObject target;
    public enum SelectOS { ALL, NULL, ANDROID, IOS, WINDOWS, MAC, EDITOR}
    public SelectOS selectOS = SelectOS.NULL;
    public SelectOS thisOS = SelectOS.NULL;

    void OnEnable () {

        if (target == null)
        {
            Debug.Log("TARGET IS NULL!!!");
            target = this.gameObject;
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            thisOS = SelectOS.ANDROID;
        }
        else
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            thisOS = SelectOS.IOS;
        }
        else
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            thisOS = SelectOS.WINDOWS;
        }
        else
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            thisOS = SelectOS.EDITOR;
        }
        else
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            thisOS = SelectOS.EDITOR;
        }
        else
        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            thisOS = SelectOS.MAC;
        }

        target.SetActive(selectOS == thisOS);
    }
	
}
