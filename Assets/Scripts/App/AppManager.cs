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

        public enum AppMode { NULL, INTRO, LOGIN, TOPIC_SELECTION, AREA_SELECTION, MAP, MAP_INFO_AREA, MAP_INFO_POI, MESSAGE, EXIT }
        public AppMode appMode = AppMode.NULL;
        public AppMode modeMessage = AppMode.NULL;

        public delegate void AppAction();
        public static AppAction OnInit, OnExit, OnMessage;

        private void Awake()
        {
            Debug.Log("APP MANAGER INIT");
            B.Init();
            UIController.OnShowMenuAreas += SetModeMenu;
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
                default:
                    break;
            }
        }

        [ContextMenu("Return")]
        public void ReturnMode()
        {
            if(modeMessage == AppMode.MESSAGE)
            {
                UIController.OnMessageHide?.Invoke();
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
                    UIController.OnHideMenuAreas?.Invoke();
                    break;
                case AppMode.TOPIC_SELECTION:
                    UIController.OnHideMenuTopics?.Invoke();
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