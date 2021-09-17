using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ARPolis.Map;
using StaGeUnityTools;
using ARPolis.Info;
using ARPolis.Data;
using System.Linq;

namespace ARPolis.UI
{

    public class MenuPanel : Singleton<MenuPanel>
    {
        protected MenuPanel() { }

        public Animator animMenuPanel, animMapMaskHide;
        public GameObject menuPanel, btnPrevCity, btnNextCity, creditsExtraButtonsPanel;
        /// <summary>
        /// scroll snap town images (menu)
        /// </summary>
        public ScrollSnapCustom snapCustom;
        public ScrollRect scrollRect;
        public Text txtMenuAreaName;
        private string termAreaNameValue;

        public Button btnToggleSite, btnToggleSideMenu, btnCloseSideMenuBehind,
                      btnQuitApp, btnCredits, btnAthensMenu, btnNafpaktosMenu, btnHerakleionMenu,
                      btnLanguage, btnGamification, btnShowMapPois;

        public Image iconBtnLanguage, iconBtnMenu;

        public Sprite sprOnsite, sprOffSite, sprMenuOn, sprMenuOff, sprEng, sprGR;

        public delegate void ButtonAction();
        public static ButtonAction OnUserClickOnSiteModeNotAble, OnQuitApp;

        public PanelTransitionClass panelSideMenuTransition, panelTopBarTransition, panelBottomBarTour, panelCreditsPeople, panelMyPlaces;

        public Button btnBottomBarGame, btnBottomBarMap, btnBottomBarSavePoi, btnBottomBarDeletePoi, btnBottomBarAR;
        public GameObject btnARicon;


        public GameObject[] extraCreditsButtons;
        public Transform arrowCredits;
        public Button btnCreditsPeople;

        [Space]
        public Button btnMyPlaces;
        public Transform myPlacesContainer;
        public Transform myPlacePrefab;

        public void CreateListOnUI(List<UserPlaceItem> userPlaces)
        {
            foreach(UserPlaceItem item in userPlaces)
            {
                Transform pl = Instantiate(myPlacePrefab, myPlacesContainer);
                MyPlaceButton placeButton = pl.GetComponent<MyPlaceButton>();
                if (placeButton == null) continue;
                placeButton.Init(item);
            }
        }

        public void DestroyListFromUI(List<UserPlaceItem> userPlaces)
        {
            List<MyPlaceButton> placeButtons = myPlacesContainer.GetComponentsInChildren<MyPlaceButton>().ToList();
            placeButtons.ForEach(b => b.DestroyItem());
        }

        void ShowMyPlacesPanel()
        {
            panelSideMenuTransition.HidePanel();
            panelMyPlaces.ShowPanel();
        }

        private void OnMyPlaceSelected(PoiEntity poi)
        {
            panelMyPlaces.HidePanel();
            //instantly go to map panel
            //we need all data in order to return back properly
            InfoManager.Instance.SetInstantlyMyPlaceData(poi);
            SetBottomBarButtonsForPoi();
            panelBottomBarTour.ShowPanel();
            ShowMap();
        }

        void OnInfoPoiShow()
        {
            if (OnSiteManager.Instance.siteMode == OnSiteManager.SiteMode.NEAR) return;
            if (UserPlacesManager.Instance.IsThisPoiSaved(InfoManager.Instance.poiNowID))
            {
                btnBottomBarSavePoi.gameObject.SetActive(false);
                btnBottomBarDeletePoi.gameObject.SetActive(true);
            }
            else
            {
                btnBottomBarSavePoi.gameObject.SetActive(true);
                btnBottomBarDeletePoi.gameObject.SetActive(false);
            }
        }

        void ShowCreditsPeople()
        {
            panelSideMenuTransition.HidePanel();
            panelCreditsPeople.ShowPanel();
        }

        void ShowTopicTours() { SetBottomBarButtonsForTour(); panelBottomBarTour.ShowPanel(); }
        void HideTopicTours() { panelBottomBarTour.HidePanel(); }

        private void Awake()
        {
            menuPanel.SetActive(true);

            btnCreditsPeople.onClick.AddListener(ShowCreditsPeople);

            btnMyPlaces.onClick.AddListener(ShowMyPlacesPanel);

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

            GlobalActionsUI.OnShowTopicTours += ShowTopicTours;
            GlobalActionsUI.OnHideTopicTours += HideTopicTours;

            GlobalActionsUI.OnShowMenuAreas += ShowMenu;
            GlobalActionsUI.OnHideMenuAreas += HideMenu;
            GlobalActionsUI.OnLogoutUser += LogOutUser;

            GlobalActionsUI.OnHideAreaTopics += HideSideMenuIfVisible;
            GlobalActionsUI.OnHideTopicTours += HideSideMenuIfVisible;

            OnSiteManager.OnGpsOff += SetStatusOffSite;
            OnSiteManager.OnGpsFar += SetStatusOffSite;
            //OnSiteManager.OnGpsClose += SetStatusOnSite;

            OnSiteManager.OnGpsNearAthens += OnGpsNearAthens;
            OnSiteManager.OnGpsNearHerakleion += OnGpsNearHerakleion;
            OnSiteManager.OnGpsNearNafpaktos += OnGpsNearNafpaktos;

            btnToggleSite.onClick.AddListener(() => ToggleSiteMode());
            btnToggleSideMenu.onClick.AddListener(() => ToggleSideMenu());
            btnCloseSideMenuBehind.onClick.AddListener(() => AppManager.Instance.ReturnMode());
            btnCredits.onClick.AddListener(() => ToggleExtraButtonsCredits());
            btnQuitApp.onClick.AddListener(() => QuitApp());

            btnAthensMenu.onClick.AddListener(() => ShowAthensMenu());
            btnNafpaktosMenu.onClick.AddListener(() => ShowNafpaktosMenu());
            btnHerakleionMenu.onClick.AddListener(() => ShowHerakleionMenu());

            btnLanguage.onClick.AddListener(() => ChangeLanguage());

            arrowCredits.localEulerAngles = new Vector3(0f, 0f, -90f);

            menuPanel.SetActive(false);
            panelTopBarTransition.HidePanel();

            snapCustom.OnSelectionPageChangedEvent.AddListener(OnPageChangeEnd);


            btnShowMapPois.onClick.AddListener(HidePoiInfo);

            GlobalActionsUI.OnMyPlaceSelected += OnMyPlaceSelected;
            GlobalActionsUI.OnInfoPoiShow += OnInfoPoiShow;

            GlobalActionsUI.OnLangChanged += SetLanguageButtonIcon;
            SetLanguageButtonIcon();
        }

        

        void HidePoiInfo() {
            if(StaticData.isPoiInfoVisible == 0)
            {
                ShowMap();
            }
            else if (StaticData.isPoiInfoVisible == 1)
            {
                GlobalActionsUI.OnInfoPoiJustHide?.Invoke();
            }
            else if (StaticData.isPoiInfoVisible == 2)
            {
                GlobalActionsUI.OnInfoPoiJustHide?.Invoke();
            }
            else
            {
                ShowMap();
            }
        }

        void ShowMap()
        {
            InfoManager.Instance.ShowPois();
            GlobalActionsUI.OnShowPoisOnMap?.Invoke();
            AppManager.Instance.SetMode(AppManager.AppMode.MAP);
            OnShowMapHideMenuPanel();
            ShowBackgroundPanel(false);
            SetBottomBarButtonsForPoi();

            OnSiteManager.Instance.CheckPosition();
        }

        void SetBottomBarButtonsForTour() {
            btnBottomBarGame.gameObject.SetActive(true);
            btnBottomBarMap.gameObject.SetActive(true);
            btnBottomBarSavePoi.gameObject.SetActive(false);
            btnBottomBarDeletePoi.gameObject.SetActive(false);
            btnBottomBarAR.gameObject.SetActive(false);
            btnARicon.SetActive(false);
        }
        void SetBottomBarButtonsForPoi() {
            btnBottomBarGame.gameObject.SetActive(false);
            btnBottomBarMap.gameObject.SetActive(true);
            btnBottomBarDeletePoi.gameObject.SetActive(false);
            if (OnSiteManager.Instance.siteMode != OnSiteManager.SiteMode.NEAR)
            {
                btnBottomBarSavePoi.gameObject.SetActive(true);
                btnBottomBarAR.gameObject.SetActive(false);
                btnARicon.SetActive(false);
            }
            else
            {
                btnBottomBarSavePoi.gameObject.SetActive(false);
                btnBottomBarAR.gameObject.SetActive(true);
                btnARicon.SetActive(false);
                ARManager.Instance.EnableButtonAR(false);
            }
        }

        private void Start()
        {
            ShowBackgroundPanel(true);
            AppData.Instance.Init();
            //reset text to current languange
            termAreaNameValue = "athens";
            txtMenuAreaName.text = AppData.Instance.FindTermValue(termAreaNameValue);

            btnBottomBarSavePoi.onClick.AddListener(PoiSave);
            btnBottomBarDeletePoi.onClick.AddListener(PoiDelete);
        }

        void PoiSave()
        {
            bool isSaved = UserPlacesManager.Instance.SaveCurrentPoi();
            btnBottomBarDeletePoi.gameObject.SetActive(isSaved);
            btnBottomBarSavePoi.gameObject.SetActive(!isSaved);
        }

        void PoiDelete()
        {
            bool isDeleted = UserPlacesManager.Instance.DeleteCurrentPoi();
            btnBottomBarSavePoi.gameObject.SetActive(isDeleted);
            btnBottomBarDeletePoi.gameObject.SetActive(!isDeleted);
        }

        /// <summary>
        /// this panel is covering map panel
        /// </summary>
        /// <param name="val"></param>
        void ShowBackgroundPanel(bool val) {
            if (!val)
            {
                StartCoroutine(DelayDisablePanel(animMapMaskHide.gameObject));
            }
            else
            {
                animMapMaskHide.gameObject.SetActive(true);
            }
            animMapMaskHide.SetBool("show", val);
        }

        IEnumerator DelayDisablePanel(GameObject gb)
        {
            yield return new WaitForSeconds(0.7f);
            gb.SetActive(false);
        }

        public void QuitApp()
        {
            OnQuitApp?.Invoke();
        }

        private void OnPageChangeEnd(int pageNo)
        {
            //if (B.isEditor) Debug.Log("thematic id = " + pageNo);
            if (pageNo == 0)
            {
                termAreaNameValue = "athens";
            }
            else if (pageNo == 1)
            {
                termAreaNameValue = "nafpaktos";
            }
            else if (pageNo == 2)
            {
                termAreaNameValue = "heracleion";
            }
            else
            {
                termAreaNameValue = "athens";
            }
            txtMenuAreaName.text = AppData.Instance.FindTermValue(termAreaNameValue);
        }

        void ChangeLanguage()
        {
            bool isEng = StaticData.lang == "en";

            //change icon
            //iconBtnLanguage.sprite = isEng ? sprGR : sprEng;

            //change lang
            StaticData.lang = isEng ? "gr" : "en";
            PlayerPrefs.SetString("Lang", StaticData.lang);
            PlayerPrefs.Save();

            //get terms
            AppData.Instance.Init();
            GlobalActionsUI.OnLangChanged?.Invoke();

            //change area names
            txtMenuAreaName.text = AppData.Instance.FindTermValue(termAreaNameValue);
        }

        void SetLanguageButtonIcon()
        {
            // Get current language
            bool isEng = StaticData.lang == "en";

            // Change icon
            iconBtnLanguage.sprite = isEng ? sprEng : sprGR;
        }

        void ShowAthensMenu()
        {
            if (B.isRealEditor) Debug.Log("ShowAthensMenu");
            btnLanguage.gameObject.SetActive(false);
            InfoManager.Instance.areaNowID = "1";
            GlobalActionsUI.OnShowAreaTopics?.Invoke();
        }

        void ShowNafpaktosMenu()
        {
            if (B.isRealEditor) Debug.Log("ShowNafpaktosMenu");
            InfoManager.Instance.areaNowID = "2";
            //animTownMenu.gameObject.SetActive(true);
            //animTownMenu.SetBool("show", true);
        }

        void ShowHerakleionMenu()
        {
            if (B.isRealEditor) Debug.Log("ShowHerakleionMenu");
            InfoManager.Instance.areaNowID = "3";
            //animTownMenu.gameObject.SetActive(true);
            //animTownMenu.SetBool("show", true);
        }

        #region select town page from gps coordiness

        void OnGpsNearAthens() {  GoOnsite(0); }
        void OnGpsNearNafpaktos() { GoOnsite(1); }
        void OnGpsNearHerakleion() { GoOnsite(2); }

        void GoOnsite(int val) {

            //check user preferences
            if (AppManager.Instance.isUserPrefersOffSiteMode) return;

            StartCoroutine(DelaySetOnsite(val));
        }

        IEnumerator DelaySetOnsite(int val)
        {
            //wait panel to be enabled
            while(!menuPanel.activeSelf) yield return new WaitForEndOfFrame();
            snapCustom.Init();
            snapCustom.enabled = true;
            SetStatusOnSite();
        }

        #endregion

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha1)) { snapCustom.SetCustomPage(0); }
        //    if (Input.GetKeyDown(KeyCode.Alpha2)) { snapCustom.SetCustomPage(1); }
        //    if (Input.GetKeyDown(KeyCode.Alpha3)) { snapCustom.SetCustomPage(2); }
        //}

        public void HideSideMenu()
        {
            iconBtnMenu.rectTransform.sizeDelta = new Vector2(100f, 100f);
            iconBtnMenu.sprite = sprMenuOn;
            panelSideMenuTransition.HidePanel();
            btnCloseSideMenuBehind.gameObject.SetActive(false);
            AppManager.Instance.isSideMenuOpen = false;
            StartCoroutine(DelayShowButtons(extraCreditsButtons, false));
            arrowCredits.localEulerAngles = new Vector3(0f, 0f, -90f);
        }

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

            if (panelCreditsPeople.isVisible)
            {
                panelCreditsPeople.HidePanel();
                panelSideMenuTransition.ShowPanel();
                return;
            }

            if (panelMyPlaces.isVisible)
            {
                panelMyPlaces.HidePanel();
                iconBtnMenu.rectTransform.sizeDelta = new Vector2(100f, 100f);
                iconBtnMenu.sprite = sprMenuOn;
                btnCloseSideMenuBehind.gameObject.SetActive(false);
                AppManager.Instance.isSideMenuOpen = false;
               // panelSideMenuTransition.ShowPanel();
                return;
            }

            if (iconBtnMenu.sprite == sprMenuOff)
            {
                Debug.Log("1111");
                //hide panel
                //btnToggleSideMenu.image.sprite = sprMenuOn;
                iconBtnMenu.rectTransform.sizeDelta = new Vector2(100f, 100f);
                iconBtnMenu.sprite = sprMenuOn;
                panelSideMenuTransition.HidePanel();
                btnCloseSideMenuBehind.gameObject.SetActive(false);
                AppManager.Instance.isSideMenuOpen = false;
                StartCoroutine(DelayShowButtons(extraCreditsButtons, false));
                arrowCredits.localEulerAngles = new Vector3(0f, 0f, -90f);
            }
            else
            {
                Debug.Log("2222");
                //show panel
                //btnToggleSideMenu.image.sprite = sprMenuOff;
                iconBtnMenu.rectTransform.sizeDelta = new Vector2(80f, 80f);
                iconBtnMenu.sprite = sprMenuOff;
                panelSideMenuTransition.ShowPanel();
                btnCloseSideMenuBehind.gameObject.SetActive(true);
                AppManager.Instance.isSideMenuOpen = true;

                InfoPoiPanel.Instance.HideInfo();
            }
        }

        void ToggleSiteMode()
        {
            //if (B.isRealEditor) Debug.Log("ToggleSiteMode");

            if(btnToggleSite.image.sprite == sprOffSite)//go on-site
            {
                if (isAbleToChangeToOnSiteMode)
                {
                    AppManager.Instance.isUserPrefersOffSiteMode = false;
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
                AppManager.Instance.isUserPrefersOffSiteMode = true;
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
            if (B.isEditor) Debug.Log("ShowMenu");
            ShowBackgroundPanel(true);
            btnLanguage.gameObject.SetActive(true);
            menuPanel.SetActive(true);
            animMenuPanel.SetBool("show", true);
            panelTopBarTransition.ShowPanel();
            AppManager.Instance.SetMode(AppManager.AppMode.AREA_SELECTION);
        }

        void HideMenu()
        {
            if (B.isEditor) Debug.Log("HideMenu");

            if (AppManager.Instance.isSideMenuOpen)
            {
                ToggleSideMenu();
                return;
            }
            GlobalActionsUI.OnLoginShow?.Invoke();
            panelTopBarTransition.HidePanel();
            HideMenuPanel();
        }

        void OnShowMapHideMenuPanel()
        {
            HideSideMenuIfVisible();
            HideMenuPanel();
        }

        void HideMenuPanel()
        {
            animMenuPanel.SetBool("show", false);
            StartCoroutine(DelayHide());
        }

        void LogOutUser()
        {
            if (B.isEditor) Debug.Log("LogOutUser");
            GlobalActionsUI.OnToggleHideAll?.Invoke();
            animMapMaskHide.gameObject.SetActive(true);
            animMapMaskHide.SetBool("show", true);
            HideSideMenuIfVisible();
            panelTopBarTransition.HidePanel();
            panelBottomBarTour.HidePanel();
            HideMenuPanel();
            GlobalActionsUI.OnLoginShow?.Invoke();

        }

        void HideSideMenuIfVisible()
        {
            if (AppManager.Instance.isSideMenuOpen)
            {
                ToggleSideMenu();
            }
        }

        IEnumerator DelayHide()
        {
            yield return new WaitForSeconds(0.75f);
            menuPanel.SetActive(false);
            btnLanguage.gameObject.SetActive(false);
        }

        
    }

}
