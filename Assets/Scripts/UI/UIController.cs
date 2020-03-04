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

        private void Awake()
        {
            OnIntroFinished += ShowMenu;
            AppManager.OnInit += StartIntro;
        }

        void StartIntro()
        {
            if (B.isEditor)
            {
                OnIntroFinished?.Invoke();
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
