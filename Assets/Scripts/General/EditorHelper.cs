using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaGeUnityTools
{

    public class EditorHelper : MonoBehaviour
    {
        public enum PlatformMode { Editor, Desctop, Mobile }
        public PlatformMode platformMode = PlatformMode.Editor;

        void Awake()
        {
            Camera.main.backgroundColor = Color.white;
#if !UNITY_EDITOR
            this.enabled = false;
            return;
#endif

            B.isEditor = false;
            B.isMobile = false;
            B.isAndroid = false;
            B.isWindows = false;
            B.isMac = false;
            B.isDesctop = false;

            switch (platformMode)
            {
                case PlatformMode.Editor:
                    B.isEditor = true;
                    break;
                case PlatformMode.Desctop:
                    B.isDesctop = true;
                    break;
                case PlatformMode.Mobile:
                    B.isMobile = true;
                    B.isAndroid = true;
                    break;
                default:
                    B.isEditor = true;
                    break;
            }
        }

    }


}