using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUI : MonoBehaviour
{
    #region Variables
    [Space]
    [Header("UI References")]
    public Button btnLanguage;
    public Button btnPrintAnswers;
    public Button btnInitializeSurvey;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        // Subscribe Buttons
        SubscribeButtons();
    }
    #endregion

    #region Methods
    void SubscribeButtons()
    {
        btnLanguage.onClick.AddListener(() => ARPolis.AppManager.Instance.surveyManager.SetLanguage());
        btnPrintAnswers.onClick.AddListener(() => ARPolis.AppManager.Instance.surveyManager.PrintAnswers());
        btnInitializeSurvey.onClick.AddListener(() => ARPolis.AppManager.Instance.surveyManager.InitializeSurvey());
    }
    #endregion
}
