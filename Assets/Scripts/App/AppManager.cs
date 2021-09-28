using ARPolis.Data;
using ARPolis.Info;
using ARPolis.Map;
using ARPolis.UI;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis
{
    public class AppManager : Singleton<AppManager>
    {
        protected AppManager() { }

        public enum AppState { NULL, INTRO, LOGIN, TOPIC_SELECTION, AREA_SELECTION, TOUR_SELECTION, MAP, MAP_INFO_AREA, MAP_INFO_POI, MESSAGE, EXIT, CREDITS, SETTINGS }
        public AppState appState = AppState.NULL;
        public AppState appStateBefore = AppState.NULL;
        public AppState stateMessage = AppState.NULL;

        public enum NavigationMode { NULL, OFF_SITE, ON_SITE, ON_SITE_AR }
        public NavigationMode navigationMode = NavigationMode.NULL;

        public bool IsGpsNotInUse() { return navigationMode == NavigationMode.NULL || navigationMode == NavigationMode.OFF_SITE; }

        public enum NavigationAbilities { NULL, AR }
        public NavigationAbilities navigationAbilities = NavigationAbilities.NULL;

        public bool isSideMenuOpen, isUserPrefersOffSiteMode;

        public delegate void AppAction();
        public static AppAction OnInit, OnExit, OnMessage;

        private int areaEntrances = 0;

        #region Andrew Variables
        public SurveyManager surveyManager;
        [SerializeField]
        public User currentUser;
        #endregion

        public string GetUserSaveKey()
        {
            return !UserExists() ? "" : "_" + currentUser.username +"_"+ currentUser.password;
        }

        private bool UserExists() { return currentUser != null && !string.IsNullOrWhiteSpace(currentUser.password) && !string.IsNullOrWhiteSpace(currentUser.username); }

        private void Awake()
        {
            Debug.Log("APP MANAGER INIT");
            B.Init();
            AppData.Instance.Init();
            UserPlacesManager.Instance.Init();

            GlobalActionsUI.OnShowMenuAreas += SetModeMenu;

            ARManager.Instance.OnCheckFinished += OnDeviceCapabilitiesCheckFinished;
        }

        void Start()
        {
            SetMode(AppState.INTRO);
            GlobalActionsUI.OnLogoutUser += OnLogoutUser;
        }

        void OnLogoutUser() { currentUser = null; }

        
        void SetModeMenu()
        {
            SetMode(AppState.AREA_SELECTION);

            //check device capabilities
            if (areaEntrances == 0) CheckDeviceCapabilities();

            areaEntrances++;
        }

        void CheckDeviceCapabilities()
        {
            OnSiteManager.Instance.CheckUserDistance();
            ARManager.Instance.CheckARsupport(0f);
        }

        void OnDeviceCapabilitiesCheckFinished()
        {
            if(OnSiteManager.Instance.siteMode == OnSiteManager.SiteMode.OFF)
            {
                AppSettings.Instance.CheckDropDownNavigation(0);// calls >> SetNavigationMode(NavigationMode.OFF_SITE);
                MessagesManager.Instance.ShowMessageGpsOff();
            }
            else
            {
                if(ARManager.Instance.arMode == ARManager.ARMode.NOT_SUPPORTED)
                {
                    AppSettings.Instance.CheckDropDownNavigation(1);// calls >> SetNavigationMode(NavigationMode.ON_SITE);
                    MessagesManager.Instance.ShowMessageGpsInsideArea("\n\n"+AppData.Instance.FindTermValue(StaticData.msgARNotSupported));
                }
                else
                {
                    AppSettings.Instance.CheckDropDownNavigation(2);// calls >> SetNavigationMode(NavigationMode.ON_SITE_AR);
                    MessagesManager.Instance.ShowMessageGpsInsideArea("\n\n" + AppData.Instance.FindTermValue(StaticData.msgARSupported));
                }
                
            }
        }

        public void SetNavigationMode(NavigationMode mode)
        {
            navigationMode = mode;

            switch (mode)
            {
                case NavigationMode.NULL:
                    break;
                case NavigationMode.OFF_SITE:
                    OnSiteManager.Instance.ShowUserPanel(false);
                    break;
                case NavigationMode.ON_SITE:
                    //try to find location again (?)
                    OnSiteManager.Instance.ShowUserPanel(true);
                    break;
                case NavigationMode.ON_SITE_AR:
                    //check ar mode again (?)
                    OnSiteManager.Instance.ShowUserPanel(true);
                    break;
                default:
                    break;
            }
        }

        public void SetMode(AppState mode)
        {
            appStateBefore = appState;
            appState = mode;

            switch (mode)
            {
                case AppState.NULL:
                    OnSiteManager.Instance.ShowMarkerPanels(false);
                    break;
                case AppState.INTRO:
                    InfoManager.Instance.Init();
                    OnSiteManager.Instance.ShowMarkerPanels(false);
                    OnInit?.Invoke();
                    break;
                case AppState.TOPIC_SELECTION:
                    OnSiteManager.Instance.ShowMarkerPanels(false);
                    break;
                case AppState.MAP:
                    OnSiteManager.Instance.ShowMarkerPanels(true);
                    break;
                case AppState.MAP_INFO_AREA:
                    break;
                case AppState.MESSAGE:
                    break;
                case AppState.EXIT:
                    break;
                case AppState.AREA_SELECTION:
                    OnSiteManager.Instance.ShowMarkerPanels(false);
                    break;
                case AppState.MAP_INFO_POI:
                    break;
                case AppState.LOGIN:
                    areaEntrances = 0;
                    OnSiteManager.Instance.ShowMarkerPanels(false);
                    break;
                case AppState.TOUR_SELECTION:
                    OnSiteManager.Instance.ShowMarkerPanels(false);
                    break;
                case AppState.CREDITS:
                    break;
                default:
                    break;
            }
        }

        [ContextMenu("Return")]
        public void ReturnMode()
        {
            if(stateMessage == AppState.MESSAGE)
            {
                GlobalActionsUI.OnMessageHide?.Invoke();
                return;
            }

            switch (appState)
            {
                case AppState.NULL:
                    break;
                case AppState.INTRO:
                    break;
                case AppState.LOGIN:
                    break;
                case AppState.AREA_SELECTION:
                    //GlobalActionsUI.OnHideMenuAreas?.Invoke();
                    if (isSideMenuOpen)
                    {
                        MenuPanel.Instance.HideSideMenu();
                        return;
                    }
                    MenuPanel.OnQuitApp?.Invoke();
                    break;
                case AppState.TOPIC_SELECTION:
                    if (isSideMenuOpen)
                    {
                        MenuPanel.Instance.HideSideMenu();
                        return;
                    }
                    GlobalActionsUI.OnHideAreaTopics?.Invoke();
                    GlobalActionsUI.OnToggleHideAll?.Invoke();
                    break;
                case AppState.TOUR_SELECTION:
                    if (isSideMenuOpen)
                    {
                        MenuPanel.Instance.HideSideMenu();
                        return;
                    }
                    GlobalActionsUI.OnHideTopicTours?.Invoke();
                    GlobalActionsUI.OnToggleHideAll?.Invoke();
                    AudioManager.Instance.StopAudio();
                    break;
                case AppState.MAP:
                    if (isSideMenuOpen)
                    {
                        MenuPanel.Instance.HideSideMenu();
                        return;
                    }
                    // GlobalActionsUI.OnShowAreaTopics?.Invoke();
                    GlobalActionsUI.OnShowTopicTours?.Invoke();
                    break;
                case AppState.MAP_INFO_AREA:
                    break;
                case AppState.MAP_INFO_POI:
                    break;
                case AppState.MESSAGE:
                    break;
                case AppState.EXIT:
                    break;
                case AppState.CREDITS:
                    MenuPanel.Instance.ToggleSideMenu();
                    break;
                case AppState.SETTINGS:
                    MenuPanel.Instance.ToggleSideMenu();
                    break;
                default:
                    break;
            }
        }
    }
}