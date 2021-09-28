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
using ARPolis.Map;
//using ARPolis.Map;
//using ARPolis.UI;


namespace ARPolis
{
    public class ARManager : Singleton<ARManager>
    {

        public enum ARMode { NOT_SUPPORTED, SUPPORTED }
        public ARMode arMode = ARMode.NOT_SUPPORTED;

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

        public delegate void ARCheckAction();
        public ARCheckAction OnCheckStarted, OnCheckFinished;

        [Space]
        [SerializeField]
        Button btnInstall;

        public ARSession arSession;

        [Header("Editor Only")]
        public bool EditorSupportsAR;



        private void Awake()
        {
            if (!Application.isEditor) EditorSupportsAR = false;
            OnSiteManager.OnGpsFar += OnGpsFar;
        }

        void OnGpsFar()
        {
            PauseAR();
            camUI.cullingMask = maskDefault;
        }

        void Start()
        {
            PauseAR();
            camUI.cullingMask = maskDefault;
            btnStartAR.onClick.AddListener(StartARSession);
            btnCloseAR.onClick.AddListener(StopARSession);
        }

        void PauseAR() { if(arSession) arSession.enabled = false; }

        void ResumeAR() { if (arSession) arSession.enabled = true; }

        void ResetAR() { if (arSession) arSession.Reset(); }

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
            OnCheckStarted?.Invoke();
            if (Application.isEditor && EditorSupportsAR)
            {
                arMode = ARMode.SUPPORTED;
                AppManager.Instance.navigationAbilities = AppManager.NavigationAbilities.AR;
                OnCheckFinished?.Invoke();
            }
            else
            {
                StartCoroutine(CheckSupport(wait_time));
            }
            checkTimes++;
        }

        IEnumerator CheckSupport(float wTime)
        {
            yield return new WaitForSeconds(wTime);

            SetInstallButtonActive(false);

            Log("Checking for AR support...");
            //OnCheckMessage?.Invoke(AppData.Instance.FindTermValue(StaticData.msgARCheckSupport));

            yield return ARSession.CheckAvailability();

            if (ARSession.state == ARSessionState.NeedsInstall)
            {
                //OnCheckMessage?.Invoke(AppData.Instance.FindTermValue(StaticData.msgARNeedsUpdate));
                Log("Your device supports AR, but requires a software update.");
                Log("Attempting install...");
                yield return ARSession.Install();
            }

            if (ARSession.state == ARSessionState.Ready)
            {
                //OnCheckMessage?.Invoke(AppData.Instance.FindTermValue(StaticData.msgARSupported));
                Log("Your device supports AR!");
                Log("Starting AR session...");

                arMode = ARMode.SUPPORTED;
                AppManager.Instance.navigationAbilities = AppManager.NavigationAbilities.AR;
                OnCheckFinished?.Invoke();
            }
            else
            {
                switch (ARSession.state)
                {
                    case ARSessionState.Unsupported:
                        //OnCheckMessage?.Invoke(AppData.Instance.FindTermValue(StaticData.msgARNotSupported));
                        Log("Your device does not support AR.");
                        arMode = ARMode.NOT_SUPPORTED;
                        AppManager.Instance.navigationAbilities = AppManager.NavigationAbilities.NULL;
                        OnCheckFinished?.Invoke();
                        break;
                    case ARSessionState.NeedsInstall:
                        //OnCheckMessage?.Invoke(AppData.Instance.FindTermValue(StaticData.msgARUpdateFailed), true);
                        Log("The software update failed, or you declined the update.");

                        // In this case, we enable a button which allows the user
                        // to try again in the event they decline the update the first time.
                        SetInstallButtonActive(true);
                        break;
                }

                PauseAR();
                arMode = ARMode.NOT_SUPPORTED;
                AppManager.Instance.navigationAbilities = AppManager.NavigationAbilities.NULL;
                OnCheckFinished?.Invoke();

                //OnCheckMessage?.Invoke(AppData.Instance.FindTermValue(StaticData.msgARNotSupported));                
                Log("[Start non-AR experience instead]");
                //
                // Start a non-AR fallback experience here...
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
