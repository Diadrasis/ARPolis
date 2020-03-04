using ARPolis.Android;
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

    public class MessagesManager : MonoBehaviour
    {

        public Animator animPanel;
        GameObject panelMessage;
        public Button btnOK, btnAction1, btnAction2;
        public Text txtTitle, txtDesc;
        Text txtBtnOK, txtBtnAction1, txtBtnAction2;
        public Image iconImage;
        public Sprite sprOffSite, sprOnSite;
        RectTransform rectPanel;

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

            OnSiteManager.OnGpsOff += GpsIsOff;
            OnSiteManager.OnGpsOn += GpsIsOn;
            OnSiteManager.OnGpsFar += GpsIsFar;
            OnSiteManager.OnGpsClose += GpsIsNear;
            UIController.OnIntroFinished += ShowGpsMessageOnIntro;
            MenuPanel.OnUserClickOnSiteModeNotAble += ShowGpsMessageOnUser;
        }

        void GpsIsOff() { SetGpsMessageMode(GpsMessageMode.OFF); }
        void GpsIsOn() { SetGpsMessageMode(GpsMessageMode.ON); }
        void GpsIsFar() { SetGpsMessageMode(GpsMessageMode.FAR); }
        void GpsIsNear() { SetGpsMessageMode(GpsMessageMode.NEAR); }

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

        void ShowMessageGpsOff()
        {
            panelMessage.SetActive(true);

            iconImage.sprite = sprOffSite;
            iconImage.gameObject.SetActive(true);
            txtTitle.text = AppData.FindTermValue(StaticData.termGpsOffTitle);
            txtDesc.text = AppData.FindTermValue(StaticData.termGpsOffDesc);

            btnOK.onClick.AddListener(() => HidePanel());
            txtBtnOK.text = AppData.FindTermValue(StaticData.termBtnOK);
            btnOK.gameObject.SetActive(true);

            if (B.isAndroid || B.isEditor)
            {
                btnAction2.onClick.AddListener(() => AndroidBridge.OpenIntent(IntentNames.GPS_SETTINGS));
                txtBtnAction2.text = AppData.FindTermValue(StaticData.termBtnEnableGps);
                btnAction2.gameObject.SetActive(true);
            }

            btnAction1.gameObject.SetActive(false);

            ForceRebuildLayout();

            ShowPanel();
        }

        void ShowMessageGpsFar()
        {
            panelMessage.SetActive(true);

            iconImage.sprite = sprOffSite;
            iconImage.gameObject.SetActive(true);
            txtTitle.text = AppData.FindTermValue(StaticData.termGpsFarTitle);
            txtDesc.text = AppData.FindTermValue(StaticData.termGpsFarDesc);

            btnOK.onClick.AddListener(() => HidePanel());
            txtBtnOK.text = AppData.FindTermValue(StaticData.termBtnOK);
            btnOK.gameObject.SetActive(true);

            btnAction2.gameObject.SetActive(false);
            btnAction1.gameObject.SetActive(false);

            ForceRebuildLayout();

            ShowPanel();
        }

        void ShowMessageGpsInsideArea()
        {
            panelMessage.SetActive(true);

            iconImage.sprite = sprOffSite;
            iconImage.gameObject.SetActive(true);
            txtTitle.text = AppData.FindTermValue(StaticData.termGpsNearTitle);
            txtDesc.text = AppData.FindTermValue(StaticData.termGpsNearDesc);

            btnOK.onClick.AddListener(() => HidePanel());
            txtBtnOK.text = AppData.FindTermValue(StaticData.termBtnOK);
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
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectPanel);
        }

    }


}