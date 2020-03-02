using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.UI
{

    public class UIController : MonoBehaviour
    {

        public delegate void UIAction();
        public static UIAction OnIntroStart, OnIntroFinished, OnMenuShow;

        public bool skipIntro;

        private void Awake()
        {
            OnIntroFinished += ShowMenu;
            AppManager.OnInit += StartIntro;
        }

        void StartIntro()
        {
            if (!B.isEditor) skipIntro = false;

            if (skipIntro)
            {
                ShowMenu();
            }
            else
            {
                OnIntroStart?.Invoke();
            }
        }

        void ShowMenu()
        {
            OnMenuShow?.Invoke();
        }
    }

}
