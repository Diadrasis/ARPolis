using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.UI
{

    public class UIController : MonoBehaviour
    { 

        private void Awake()
        {
            GlobalActionsUI.OnIntroFinished += LoginShow;
            AppManager.OnInit += StartIntro;
        }

        void StartIntro()
        {
            if (B.isEditor)
            {
                GlobalActionsUI.OnIntroFinished?.Invoke();
                AppManager.Instance.SetMode(AppManager.AppMode.LOGIN);
            }
            else
            {
                GlobalActionsUI.OnIntroStart?.Invoke();
            }
        }
        
        void LoginShow()
        {
            GlobalActionsUI.OnLoginShow?.Invoke();
        }
    }

}
