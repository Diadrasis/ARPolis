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
            if (!B.isEditor) skipIntro = false;
        }

        void Start()
        {
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
