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

        public enum AppMode { NULL, INTRO, LOGIN, TOPIC_SELECTION, AREA_SELECTION, TOUR_SELECTION, MAP, MAP_INFO_AREA, MAP_INFO_POI, MESSAGE, EXIT }
        public AppMode appMode = AppMode.NULL;
        public AppMode modeMessage = AppMode.NULL;

        public bool isSideMenuOpen, isUserPrefersOffSiteMode;

        public delegate void AppAction();
        public static AppAction OnInit, OnExit, OnMessage;

        private void Awake()
        {
            Debug.Log("APP MANAGER INIT");
            B.Init();
            GlobalActionsUI.OnShowMenuAreas += SetModeMenu;
        }

        void Start()
        {
            SetMode(AppMode.INTRO);
        }

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
                    AppData.Init();
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
                    GlobalActionsUI.OnHideMenuAreas?.Invoke();
                    break;
                case AppMode.TOPIC_SELECTION:
                    GlobalActionsUI.OnHideAreaTopics?.Invoke();
                    GlobalActionsUI.OnToggleHideAll?.Invoke();
                    break;
                case AppMode.TOUR_SELECTION:
                    GlobalActionsUI.OnHideTopicTours?.Invoke();
                    GlobalActionsUI.OnToggleHideAll?.Invoke();
                    break;
                case AppMode.MAP:
                    break;
                case AppMode.MAP_INFO_AREA:
                    break;
                case AppMode.MAP_INFO_POI:
                    break;
                case AppMode.MESSAGE:
                    break;
                case AppMode.EXIT:
                    break;
                default:
                    break;
            }
        }

    }


}