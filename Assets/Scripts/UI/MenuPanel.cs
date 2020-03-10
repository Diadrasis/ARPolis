using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ARPolis.Map;
using StaGeUnityTools;
using ARPolis.Info;

namespace ARPolis.UI
{

    public class MenuPanel : MonoBehaviour
    {
        public Animator animMenuPanel, animTownMenu;
        public GameObject menuPanel, btnPrevCity, btnNextCity, creditsExtraButtonsPanel;
        public ScrollSnapCustom snapCustom;
        public ScrollRect scrollRect;

        public Button btnToggleSite, btnToggleSideMenu, btnCloseSideMenuBehind,
                      btnQuitApp, btnCredits, btnAthensMenu, btnNafpaktosMenu, btnHerakleionMenu,
                      btnLanguage;

        public Image iconBtnLanguage;

        public Sprite sprOnsite, sprOffSite, sprMenuOn, sprMenuOff, sprEng, sprGR;

        public delegate void ButtonAction();
        public static ButtonAction OnUserClickOnSiteModeNotAble, OnQuitApp;

        public PanelTransitionClass panelSideMenuTransition;

        public GameObject[] extraCreditsButtons;
        public Transform arrowCredits;


        private void Awake()
        {

            if (!PlayerPrefs.HasKey("Lang"))
            {
                if (Application.systemLanguage == SystemLanguage.English)
                {
                    if (StaticData.lang == "gr") ChangeLanguage();
                }
                else
                {
                    if (StaticData.lang == "en") ChangeLanguage();
                }
            }
            else
            {
                StaticData.lang = PlayerPrefs.GetString("Lang");
                bool isEng = StaticData.lang == "en";
                //change icon
                iconBtnLanguage.sprite = isEng ? sprEng : sprGR;
            }

            UIController.OnMenuShow += ShowMenu;

            OnSiteManager.OnGpsOff += SetStatusOffSite;
            OnSiteManager.OnGpsFar += SetStatusOffSite;
            //OnSiteManager.OnGpsClose += SetStatusOnSite;

            OnSiteManager.OnGpsNearAthens += OnGpsNearAthens;
            OnSiteManager.OnGpsNearHerakleion += OnGpsNearHerakleion;
            OnSiteManager.OnGpsNearNafpaktos += OnGpsNearNafpaktos;

            btnToggleSite.onClick.AddListener(() => ToggleSiteMode());
            btnToggleSideMenu.onClick.AddListener(() => ToggleSideMenu());
            btnCloseSideMenuBehind.onClick.AddListener(() => ToggleSideMenu());
            btnCredits.onClick.AddListener(() => ToggleExtraButtonsCredits());
            btnQuitApp.onClick.AddListener(() => OnQuitApp?.Invoke());

            btnAthensMenu.onClick.AddListener(() => ShowAthensMenu());
            btnNafpaktosMenu.onClick.AddListener(() => ShowNafpaktosMenu());
            btnHerakleionMenu.onClick.AddListener(() => ShowHerakleionMenu());

            btnLanguage.onClick.AddListener(() => ChangeLanguage());

            animTownMenu.gameObject.SetActive(false);

            arrowCredits.localEulerAngles = new Vector3(0f, 0f, -90f);

            menuPanel.SetActive(false);

        }

        void ChangeLanguage()
        {
            bool isEng = StaticData.lang == "en";

            //change icon
            iconBtnLanguage.sprite = isEng ? sprGR : sprEng;
            //change lang
            StaticData.lang = isEng ? "gr" : "en";
            PlayerPrefs.SetString("Lang", StaticData.lang);
            PlayerPrefs.Save();
            //get terms
            AppData.Init();
        }

        void ShowAthensMenu()
        {
            if (B.isRealEditor) Debug.Log("ShowAthensMenu");
            animTownMenu.gameObject.SetActive(true);
            animTownMenu.SetBool("show", true);
        }

        void ShowNafpaktosMenu()
        {
            if (B.isRealEditor) Debug.Log("ShowNafpaktosMenu");
            //animTownMenu.gameObject.SetActive(true);
            //animTownMenu.SetBool("show", true);
        }

        void ShowHerakleionMenu()
        {
            if (B.isRealEditor) Debug.Log("ShowHerakleionMenu");
            //animTownMenu.gameObject.SetActive(true);
            //animTownMenu.SetBool("show", true);
        }

        #region select town page from gps coordiness

        void OnGpsNearAthens() {  GoOnsite(0); }
        void OnGpsNearNafpaktos() { GoOnsite(1); }
        void OnGpsNearHerakleion() { GoOnsite(2); }

        void GoOnsite(int val) { StartCoroutine(DelaySetOnsite(val)); }
        IEnumerator DelaySetOnsite(int val)
        {
            //wait panel to be enabled
            while(!menuPanel.activeSelf) yield return new WaitForEndOfFrame();
            snapCustom.Init();
            snapCustom.enabled = true;
            snapCustom.SetCustomPage(val);
            SetStatusOnSite();
        }

        #endregion

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha1)) { snapCustom.SetCustomPage(0); }
        //    if (Input.GetKeyDown(KeyCode.Alpha2)) { snapCustom.SetCustomPage(1); }
        //    if (Input.GetKeyDown(KeyCode.Alpha3)) { snapCustom.SetCustomPage(2); }
        //}

        void ToggleExtraButtonsCredits()
        {
            foreach (GameObject gb in extraCreditsButtons) gb.SetActive(false);

            if (!creditsExtraButtonsPanel.activeSelf)
            {
                creditsExtraButtonsPanel.SetActive(true);
                StartCoroutine(DelayShowButtons(extraCreditsButtons, true));
                arrowCredits.localEulerAngles = new Vector3(0f, 0f, 90f);
            }
            else
            {
                StartCoroutine(DelayShowButtons(extraCreditsButtons, false));
                arrowCredits.localEulerAngles = new Vector3(0f, 0f, -90f);
            }
        }

        IEnumerator DelayShowButtons(GameObject[] buttons, bool isOn)
        {
            foreach (GameObject gb in extraCreditsButtons)
            {
                gb.SetActive(isOn);
                yield return new WaitForEndOfFrame();
            }
            creditsExtraButtonsPanel.SetActive(isOn);
            yield break;
        }

        void ToggleSideMenu()
        {
            if (B.isRealEditor) Debug.Log("ToggleSideMenu");

            if (btnToggleSideMenu.image.sprite == sprMenuOff)
            {
                //hide panel
                btnToggleSideMenu.image.sprite = sprMenuOn;
                panelSideMenuTransition.HidePanel();
                btnCloseSideMenuBehind.gameObject.SetActive(false);
            }
            else
            {
                //show panel
                btnToggleSideMenu.image.sprite = sprMenuOff;
                panelSideMenuTransition.ShowPanel();
                btnCloseSideMenuBehind.gameObject.SetActive(true);
            }
        }

        void ToggleSiteMode()
        {
            //if (B.isRealEditor) Debug.Log("ToggleSiteMode");

            if(btnToggleSite.image.sprite == sprOffSite)
            {
                if (isAbleToChangeToOnSiteMode)
                {
                    //go on-site
                    btnToggleSite.image.sprite = sprOnsite;
                    ShowButtons(false);
                }
                else
                {
                    OnUserClickOnSiteModeNotAble?.Invoke();
                }
            }
            else
            {
                //go off-site
                btnToggleSite.image.sprite = sprOffSite;
                ShowButtons(true);
            }
        }

        void ShowButtons(bool val)
        {
            scrollRect.enabled = val;
            snapCustom.Init();
            //snapCustom.SetFirstPage();
            snapCustom.enabled = val;
            if (val == false)
            {
                btnNextCity.SetActive(val);
                btnPrevCity.SetActive(val);
            }
        }

        bool isAbleToChangeToOnSiteMode;

        void SetStatusOffSite()
        {
            if (B.isRealEditor) Debug.Log("SetStatusOffSite");
            btnToggleSite.image.sprite = sprOffSite;
            isAbleToChangeToOnSiteMode = false;
            ShowButtons(true);
        }

        void SetStatusOnSite()
        {
            if (B.isRealEditor) Debug.Log("SetStatusOnSite");
            btnToggleSite.image.sprite = sprOnsite;
            isAbleToChangeToOnSiteMode = true;
            ShowButtons(false);
        }

        void ShowMenu()
        {
            menuPanel.SetActive(true);
            animMenuPanel.SetBool("show", true);
        }
    }

}
