using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
// for messages
//using ARPolis.Android;
using ARPolis.Data;
using ARPolis.Info;
//using ARPolis.Map;
//using ARPolis.UI;


namespace ARPolis
{
    public class ARManager : Singleton<ARManager>
    {

        public Button btnStartAR, btnCloseAR, btnResetAR;
        public GameObject iconARbtn;

        public Camera camUI;
        public UniversalAdditionalCameraData camData;
        public LayerMask maskDefault, maskAR;

        [Space]
        public GameObject CustomMarkersPanel, btnMenu, btnsMbtnsTopRightMenu, btnsAR;
        public StaGeUnityTools.PanelTransitionClass bottomPanel;

        public delegate void ARaction(string msg = null, bool showInstallButton = false);
        public ARaction OnCheckMessage;

        [Space]
        [SerializeField]
        Button btnInstall;

        public ARSession arSession;

        public bool IsAR_Enabled;

        void Start()
        {
            IsAR_Enabled = false;
            PauseAR();
            camUI.cullingMask = maskDefault;
            btnStartAR.onClick.AddListener(StartARSession);
            btnCloseAR.onClick.AddListener(StopARSession);
        }

        void PauseAR() { arSession.enabled = false; }

        void ResumeAR() { arSession.enabled = true; }

        void ResetAR() { arSession.Reset(); }

        public void EnableButtonAR(bool val) {
            iconARbtn.SetActive(val);
            btnStartAR.interactable = val;
        }

        public void StartARSession()
        {
            ResumeAR();
            camUI.cullingMask = maskAR;
            CustomMarkersPanel.SetActive(false);
            btnMenu.SetActive(false);
            btnsMbtnsTopRightMenu.SetActive(false);
            btnsAR.SetActive(true);
            bottomPanel.HidePanel();
        }

        public void StopARSession()
        {
            ResetAR();
            PauseAR();
            camUI.cullingMask = maskDefault;
            CustomMarkersPanel.SetActive(true);
            btnMenu.SetActive(true);
            btnsMbtnsTopRightMenu.SetActive(true);
            btnsAR.SetActive(false);
            bottomPanel.ShowPanel();
        }

        #region Check Support

        int checkTimes;

        public void CheckARsupport(float wait_time)
        {
            if(checkTimes==0) StartCoroutine(CheckSupport(wait_time));
            checkTimes++;
        }

        IEnumerator CheckSupport(float wTime)
        {
            yield return new WaitForSeconds(wTime);

            IsAR_Enabled = false;
            SetInstallButtonActive(false);

            Log("Checking for AR support...");
            OnCheckMessage?.Invoke(AppData.Instance.FindTermValue(StaticData.msgARCheckSupport));

            yield return ARSession.CheckAvailability();

            if (ARSession.state == ARSessionState.NeedsInstall)
            {
                OnCheckMessage?.Invoke(AppData.Instance.FindTermValue(StaticData.msgARNeedsUpdate));
                Log("Your device supports AR, but requires a software update.");
                Log("Attempting install...");
                yield return ARSession.Install();
            }

            if (ARSession.state == ARSessionState.Ready)
            {
                OnCheckMessage?.Invoke("Your device supports AR!");
                Log("Your device supports AR!");
                Log("Starting AR session...");

                IsAR_Enabled = true;
                // To start the ARSession, we just need to enable it.
                //arSession.enabled = true;
            }
            else
            {
                switch (ARSession.state)
                {
                    case ARSessionState.Unsupported:
                        OnCheckMessage?.Invoke(AppData.Instance.FindTermValue(StaticData.msgARNotSupported));
                        Log("Your device does not support AR.");
                        break;
                    case ARSessionState.NeedsInstall:
                        OnCheckMessage?.Invoke(AppData.Instance.FindTermValue(StaticData.msgARUpdateFailed), true);
                        Log("The software update failed, or you declined the update.");

                        // In this case, we enable a button which allows the user
                        // to try again in the event they decline the update the first time.
                        SetInstallButtonActive(true);
                        break;
                }

                PauseAR();

                //OnCheckMessage?.Invoke("Your device does not support AR.");
                OnCheckMessage?.Invoke(AppData.Instance.FindTermValue(StaticData.msgARNotSupported));                
                Log("[Start non-AR experience instead]");
                //
                // Start a non-AR fallback experience here...
                //
            }
        }

        void SetInstallButtonActive(bool active)
        {
            if (btnInstall != null)
                btnInstall.gameObject.SetActive(active);
        }

        void Log(string message)
        {
            if(Application.isEditor) Debug.Log($"{message}");
        }

        #endregion

    }

}
