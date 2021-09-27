﻿using ARPolis.Data;
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

        public enum AppState { NULL, INTRO, LOGIN, TOPIC_SELECTION, AREA_SELECTION, TOUR_SELECTION, MAP, MAP_INFO_AREA, MAP_INFO_POI, MESSAGE, EXIT, CREDITS }
        public AppState appState = AppState.NULL;
        public AppState stateMessage = AppState.NULL;

        public enum NavigationMode { NULL, OFF_SITE, ON_SITE, ON_SITE_AR }
        public NavigationMode navigationMode = NavigationMode.NULL;

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
                SetNavigationMode(NavigationMode.OFF_SITE);
                MessagesManager.Instance.ShowMessageGpsOff();
            }
            else
            {
                if(ARManager.Instance.arMode == ARManager.ARMode.NOT_SUPPORT)
                {
                    SetNavigationMode(NavigationMode.ON_SITE);
                    MessagesManager.Instance.ShowMessageGpsInsideArea("\n\n"+AppData.Instance.FindTermValue(StaticData.msgARNotSupported));
                }
                else
                {
                    SetNavigationMode(NavigationMode.ON_SITE_AR);
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
                    break;
                case NavigationMode.ON_SITE:
                    //try to find location again
                    break;
                case NavigationMode.ON_SITE_AR:
                    //check ar mode
                    break;
                default:
                    break;
            }
        }

        public void SetMode(AppState mode)
        {
            appState = mode;

            switch (mode)
            {
                case AppState.NULL:
                    break;
                case AppState.INTRO:
                    InfoManager.Instance.Init();
                    OnInit?.Invoke();
                    break;
                case AppState.TOPIC_SELECTION:
                    break;
                case AppState.MAP:
                    break;
                case AppState.MAP_INFO_AREA:
                    break;
                case AppState.MESSAGE:
                    break;
                case AppState.EXIT:
                    break;
                case AppState.AREA_SELECTION:
                    break;
                case AppState.MAP_INFO_POI:
                    break;
                case AppState.LOGIN:
                    areaEntrances = 0;
                    break;
                case AppState.TOUR_SELECTION:
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
                    break;
                default:
                    break;
            }
        }
    }
}