using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ToggleObjects : MonoBehaviour
{

    public Button btn;
    public CanvasGroup canvasGroup;
    public Camera arCam, mainCam;
    public GameObject canvasAR, mapPanel;
   // UniversalAdditionalCameraData camData;

    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(EnterAR);
        //camData = arCam.GetUniversalAdditionalCameraData();
        
    }

    public void EnterAR()
    {
        canvasGroup.alpha = 0f;
        mainCam.enabled = false;
        mapPanel.SetActive(false);
        canvasAR.SetActive(true);
    }

    public void ExitAR()
    {
        canvasGroup.alpha = 1f;
        mainCam.enabled = true;
        mapPanel.SetActive(true);
        canvasAR.SetActive(false);
    }

}
