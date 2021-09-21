using ARPolis.Data;
using ARPolis.Info;
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

        public enum AppMode { NULL, INTRO, LOGIN, TOPIC_SELECTION, AREA_SELECTION, TOUR_SELECTION, MAP, MAP_INFO_AREA, MAP_INFO_POI, MESSAGE, EXIT, CREDITS }
        public AppMode appMode = AppMode.NULL;
        public AppMode modeMessage = AppMode.NULL;

        public bool isSideMenuOpen, isUserPrefersOffSiteMode;

        public delegate void AppAction();
        public static AppAction OnInit, OnExit, OnMessage;

        MenuPanel menuPanel;

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

            menuPanel = FindObjectOfType<MenuPanel>();
        }

        void Start()
        {
            SetMode(AppMode.INTRO);

            GlobalActionsUI.OnLogoutUser += OnLogoutUser;
        }

        void OnLogoutUser() { currentUser = null; }

        void SetModeMenu()
        {
            SetMode(AppMode.AREA_SELECTION);
        }

        public void SetMode(AppMode mode)
        {
            appMode = mode;

            switch (mode)
            {
                case AppMode.NULL:
                    break;
                case AppMode.INTRO:
                    InfoManager.Instance.Init();
                    OnInit?.Invoke();
                    break;
                case AppMode.TOPIC_SELECTION:
                    break;
                case AppMode.MAP:
                    break;
                case AppMode.MAP_INFO_AREA:
                    break;
                case AppMode.MESSAGE:
                    break;
                case AppMode.EXIT:
                    break;
                case AppMode.AREA_SELECTION:
                    break;
                case AppMode.MAP_INFO_POI:
                    break;
                case AppMode.LOGIN:
                    break;
                case AppMode.TOUR_SELECTION:
                    break;
                case AppMode.CREDITS:
                    break;
                default:
                    break;
            }
        }

        [ContextMenu("Return")]
        public void ReturnMode()
        {
            if(modeMessage == AppMode.MESSAGE)
            {
                GlobalActionsUI.OnMessageHide?.Invoke();
                return;
            }

            switch (appMode)
            {
                case AppMode.NULL:
                    break;
                case AppMode.INTRO:
                    break;
                case AppMode.LOGIN:
                    break;
                case AppMode.AREA_SELECTION:
                    //GlobalActionsUI.OnHideMenuAreas?.Invoke();
                    if (isSideMenuOpen)
                    {
                        menuPanel.HideSideMenu();
                        return;
                    }
                    MenuPanel.OnQuitApp?.Invoke();
                    break;
                case AppMode.TOPIC_SELECTION:
                    if (isSideMenuOpen)
                    {
                        menuPanel.HideSideMenu();
                        return;
                    }
                    GlobalActionsUI.OnHideAreaTopics?.Invoke();
                    GlobalActionsUI.OnToggleHideAll?.Invoke();
                    break;
                case AppMode.TOUR_SELECTION:
                    if (isSideMenuOpen)
                    {
                        menuPanel.HideSideMenu();
                        return;
                    }
                    GlobalActionsUI.OnHideTopicTours?.Invoke();
                    GlobalActionsUI.OnToggleHideAll?.Invoke();
                    AudioManager.Instance.StopAudio();
                    break;
                case AppMode.MAP:
                    if (isSideMenuOpen)
                    {
                        menuPanel.HideSideMenu();
                        return;
                    }
                    // GlobalActionsUI.OnShowAreaTopics?.Invoke();
                    GlobalActionsUI.OnShowTopicTours?.Invoke();
                    break;
                case AppMode.MAP_INFO_AREA:
                    break;
                case AppMode.MAP_INFO_POI:
                    break;
                case AppMode.MESSAGE:
                    break;
                case AppMode.EXIT:
                    break;
                case AppMode.CREDITS:
                    break;
                default:
                    break;
            }
        }
    }
}