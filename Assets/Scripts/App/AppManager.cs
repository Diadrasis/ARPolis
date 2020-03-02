using ARPolis.Info;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis
{

    public class AppManager : MonoBehaviour
    {
        public enum AppMode { NULL, INIT, SITE_SELECTION, MENU, INFO, MESSAGE, EXIT }
        public AppMode appMode = AppMode.NULL;


        public delegate void AppAction();
        public static AppAction OnInit, OnExit, OnMessage;

        private void Awake()
        {
            B.Init();
        }

        void Start()
        {
            SetMode(AppMode.INIT);
        }

        void SetMode(AppMode appMode)
        {
            switch (appMode)
            {
                case AppMode.NULL:
                    break;
                case AppMode.INIT:
                    AppData.Init();
                    OnInit?.Invoke();
                    break;
                case AppMode.SITE_SELECTION:
                    break;
                case AppMode.MENU:
                    break;
                case AppMode.INFO:
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