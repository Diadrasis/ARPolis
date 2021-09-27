using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
                AppManager.Instance.SetMode(AppManager.AppState.LOGIN);
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
