using ARPolis.Android;
using ARPolis.Data;
using ARPolis.Info;
using ARPolis.Map;
using ARPolis.UI;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis
{

    public class MessagesManager : Singleton<MessagesManager>
    {

        protected MessagesManager() { }

        public Animator animPanel;
        GameObject panelMessage;
        public Button btnOK, btnAction1, btnAction2;
        public Text txtTitle, txtDesc;
        Text txtBtnOK, txtBtnAction1, txtBtnAction2;
        public Image iconImage;
        public Sprite sprOffSite, sprOnSite, sprQuitApp, sprAR;
        public RectTransform rectPanel, rectContainer, rectButtons;

        public bool isIntroFinished;

        public enum GpsMessageMode { OFF, ON, FAR, NEAR}
        public GpsMessageMode gpsMessageMode = GpsMessageMode.OFF;

        void SetGpsMessageMode(GpsMessageMode mode) {
            if (gpsMessageMode == mode) return;
            gpsMessageMode = mode;
            if(isIntroFinished) StartCoroutine(DelayShowMessage(0.2f));
        }

        private void Awake()
        {
            panelMessage = animPanel.gameObject;
            rectPanel = panelMessage.GetComponent<RectTransform>();

            txtBtnOK = btnOK.GetComponentInChildren<Text>();
            txtBtnAction1 = btnAction1.GetComponentInChildren<Text>();
            txtBtnAction2 = btnAction2.GetComponentInChildren<Text>();

            //OnSiteManager.OnGpsOff += GpsIsOff;
            //OnSiteManager.OnGpsOn += GpsIsOn;
            //OnSiteManager.OnGpsFar += GpsIsFar;
            //OnSiteManager.OnGpsClose += GpsIsNear;
            //GlobalActionsUI.OnShowMenuAreas += ShowGpsMessageOnIntro;
            MenuPanel.OnUserClickOnSiteModeNotAble += ShowGpsMessageOnUser;
            MenuPanel.OnQuitApp += ShowQuitAppWarning;

            ARManager.Instance.OnCheckMessage += ShowARWarning;

            GlobalActionsUI.OnMessageHide += HidePanel;
        }

        void GpsIsOff() { SetGpsMessageMode(GpsMessageMode.OFF); }
        void GpsIsOn() { SetGpsMessageMode(GpsMessageMode.ON); }
        void GpsIsFar() { SetGpsMessageMode(GpsMessageMode.FAR); }
        void GpsIsNear() { SetGpsMessageMode(GpsMessageMode.NEAR); }

        void ShowQuitAppWarning()
        {
            panelMessage.SetActive(true);
            AppManager.Instance.stateMessage = AppManager.AppState.MESSAGE;

            iconImage.sprite = sprQuitApp;
            iconImage.gameObject.SetActive(true);
            txtTitle.text = AppData.Instance.FindTermValue(StaticData.termLogoutTitle);
            txtDesc.text = AppData.Instance.FindTermValue(StaticData.termLogoutDesc);

            btnOK.onClick.AddListener(() => LogoutUser());// QuitApp());
            txtBtnOK.text = AppData.Instance.FindTermValue(StaticData.termBtnOK);
            btnOK.gameObject.SetActive(true);

            btnAction2.onClick.AddListener(() => HidePanel());
            txtBtnAction2.text = AppData.Instance.FindTermValue(StaticData.termBtnCancel);
            btnAction2.gameObject.SetActive(true);

            btnAction1.gameObject.SetActive(false);

            ForceRebuildLayout();

            ShowPanel();
        }

        void QuitApp()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        void LogoutUser()
        {
            GlobalActionsUI.OnLogoutUser?.Invoke();
            HidePanel();
        }

        void ShowGpsMessageOnUser()
        {
            StartCoroutine(DelayShowMessage(0.0f));
        }

        void ShowGpsMessageOnIntro()
        {
            isIntroFinished = true;
            StartCoroutine(DelayShowMessage(1f));
        }

        IEnumerator DelayShowMessage(float delayTime)
        {
            ButtonsRemoveListeners();

            yield return new WaitForSeconds(delayTime);

            switch (gpsMessageMode)
            {
                case GpsMessageMode.OFF:
                    ShowMessageGpsOff();
                    break;
                case GpsMessageMode.FAR:
                    ShowMessageGpsFar();
                    break;
                case GpsMessageMode.NEAR:
                    ShowMessageGpsInsideArea();
                    break;
                case GpsMessageMode.ON:
                    break;
                default:
                    ShowMessageGpsOff();
                    break;
            }

            yield break;
        }

        public void ShowCustomMessage(string title_ID, string desc_ID)
        {
            panelMessage.SetActive(true);
            AppManager.Instance.stateMessage = AppManager.AppState.MESSAGE;

            //iconImage.sprite = sprOffSite;
            iconImage.gameObject.SetActive(false);
            txtTitle.text = AppData.Instance.FindTermValue(title_ID);
            txtDesc.text = AppData.Instance.FindTermValue(desc_ID);

            btnOK.onClick.AddListener(() => HidePanel());
            txtBtnOK.text = AppData.Instance.FindTermValue(StaticData.termBtnOK);
            btnOK.gameObject.SetActive(true);

            btnAction2.gameObject.SetActive(false);
            btnAction1.gameObject.SetActive(false);

            ForceRebuildLayout();

            ShowPanel();
        }

        public void ShowMessageGpsOff()
        {
            panelMessage.SetActive(true);
            AppManager.Instance.stateMessage = AppManager.AppState.MESSAGE;

            iconImage.sprite = sprOffSite;
            iconImage.gameObject.SetActive(true);
            txtTitle.text = AppData.Instance.FindTermValue(StaticData.termGpsOffTitle);
            txtDesc.text = AppData.Instance.FindTermValue(StaticData.termGpsOffDesc);

            btnOK.onClick.AddListener(() => HidePanel());
            txtBtnOK.text = AppData.Instance.FindTermValue(StaticData.termBtnOK);
            btnOK.gameObject.SetActive(true);

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
            {
                btnAction2.onClick.AddListener(() => AndroidBridge.OpenIntent(IntentNames.GPS_SETTINGS));
                txtBtnAction2.text = AppData.Instance.FindTermValue(StaticData.termBtnEnableGps);
                btnAction2.gameObject.SetActive(true);
            }

            btnAction1.gameObject.SetActive(false);

            ForceRebuildLayout();

            ShowPanel();
        }

        void ShowMessageGpsFar()
        {
            panelMessage.SetActive(true);
            AppManager.Instance.stateMessage = AppManager.AppState.MESSAGE;

            iconImage.sprite = sprOffSite;
            iconImage.gameObject.SetActive(true);
            txtTitle.text = AppData.Instance.FindTermValue(StaticData.termGpsFarTitle);
            txtDesc.text = AppData.Instance.FindTermValue(StaticData.termGpsFarDesc);

            btnOK.onClick.AddListener(() => HidePanel());
            txtBtnOK.text = AppData.Instance.FindTermValue(StaticData.termBtnOK);
            btnOK.gameObject.SetActive(true);

            btnAction2.gameObject.SetActive(false);
            btnAction1.gameObject.SetActive(false);

            ForceRebuildLayout();

            ShowPanel();
        }

        public void ShowMessageGpsInsideArea(string descAppend = "")
        {
            panelMessage.SetActive(true);
            AppManager.Instance.stateMessage = AppManager.AppState.MESSAGE;

            iconImage.sprite = sprOnSite;
            iconImage.gameObject.SetActive(true);
            txtTitle.text = AppData.Instance.FindTermValue(StaticData.termGpsNearTitle);
            txtDesc.text = AppData.Instance.FindTermValue(StaticData.termGpsNearDesc)+descAppend;

            btnOK.onClick.AddListener(() => HidePanel());
            txtBtnOK.text = AppData.Instance.FindTermValue(StaticData.termBtnOK);
            btnOK.gameObject.SetActive(true);

            btnAction2.gameObject.SetActive(false);
            btnAction1.gameObject.SetActive(false);

            ForceRebuildLayout();

            ShowPanel();
        }

        void ShowARWarning(string msg, bool showInstallButton)
        {
            panelMessage.SetActive(true);
            AppManager.Instance.stateMessage = AppManager.AppState.MESSAGE;

            iconImage.sprite = sprAR;
            iconImage.gameObject.SetActive(true);
            txtTitle.text = "AR experience"; //AppData.Instance.FindTermValue(StaticData.termLogoutTitle);
            txtDesc.text = msg;// AppData.Instance.FindTermValue(StaticData.termLogoutDesc);

            btnOK.onClick.AddListener(() => HidePanel());
            txtBtnOK.text = AppData.Instance.FindTermValue(StaticData.termBtnOK);
            btnOK.gameObject.SetActive(true);

            btnAction2.gameObject.SetActive(false);
            btnAction1.gameObject.SetActive(false);

            ForceRebuildLayout();

            ShowPanel();
        }

        void ShowPanel() { animPanel.SetBool("show", true); }

        void HidePanel() {
            animPanel.SetBool("show", false);
            ButtonsRemoveListeners();
            StartCoroutine(DelayClosePanel());
        }

        IEnumerator DelayClosePanel()
        {
            yield return new WaitForSeconds(0.5f);
            panelMessage.SetActive(false);
            AppManager.Instance.stateMessage = AppManager.AppState.NULL;
            yield break;
        }

        void ButtonsRemoveListeners()
        {
            btnOK.onClick.RemoveAllListeners();
            btnAction1.onClick.RemoveAllListeners();
            btnAction2.onClick.RemoveAllListeners();
        }

        void ForceRebuildLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectButtons);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectContainer);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectPanel);
        }

    }


}