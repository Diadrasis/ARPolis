using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.UI
{

    public class UIController : MonoBehaviour
    {

        public delegate void UIAction();
        public static UIAction OnIntroStart, OnIntroFinished, OnShowMenuAreas, OnHideMenuAreas,
            OnShowMenuTopics, OnHideMenuTopics, OnLoginShow,
            OnInfoAreaShow, OnInfoPoiShow, OnMessageHide;

        private void Awake()
        {
            OnIntroFinished += LoginShow;
            AppManager.OnInit += StartIntro;
        }

        void StartIntro()
        {
            if (B.isEditor)
            {
                OnIntroFinished?.Invoke();
                AppManager.Instance.SetMode(AppManager.AppMode.LOGIN);
            }
            else
            {
                OnIntroStart?.Invoke();
            }
        }
        
        void LoginShow()
        {
            OnLoginShow?.Invoke();
        }
    }

}
