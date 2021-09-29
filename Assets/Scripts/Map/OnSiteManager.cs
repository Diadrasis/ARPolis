using ARPolis.Data;
using ARPolis.UI;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace ARPolis.Map
{

    public class OnSiteManager : Singleton<OnSiteManager>
    {

        public enum SiteMode { OFF, NEAR, FAR }
        public SiteMode siteMode = SiteMode.OFF;

        //manage on site logic and ui messages
        public delegate void ActionMessages();
        public static ActionMessages OnGpsFar, OnGpsClose, OnCloseToPointOnPath, OnGpsOn, OnGpsOff,
                                     OnGpsNearNafpaktos, OnGpsNearAthens, OnGpsNearHerakleion;


        readonly Vector2 mountainCheckPoint = new Vector2(21.903f, 38.5635f);
        readonly Vector2 westCheckPointA = new Vector2(21.70192f, 38.4515f);
        readonly Vector2 westCheckPointB = new Vector2(21.76776f, 38.46531f);
        readonly Vector2 athensCheckPoint = new Vector2(23.72685f, 37.97925f);
        readonly Vector2 herakleionCheckPoint = new Vector2(25.133935f, 35.339641f);
        readonly Vector2 nafpaktosCheckPoint = new Vector2(21.829400f, 38.393076f);

        public Vector2 userPosition = Vector2.zero;
        //PathInfoManager pathInfoManager;
        //CustomMarkerGUI markerOnNearestPath;

        [Space]
        public float maxKmDistanceForOnSiteMode = 5f;
        public float triggerPoiDist = 50f;

        [Space]
        public GameObject panelUserMarker;
        [Space]
        public float LabelsZoomVisibility = 17f;
        public bool IsMarkerLabelAbleToShow() {
            return LabelsZoomVisibility <= OnlineMaps.instance.floatZoom /*&& !InfoPoiPanel.Instance.transitionClass.isVisible*/; 
        }
        public bool OnPoiSelectAllowMapToMoveToPoiPosition = true;

        //Diadrasis
        //37.979889, 23.724089

        public bool UseDiadrasisOffice;

        private bool UserWasNearPoiOnLastCheck;

        public CustomMarkerGUI markerUser;

        /// <summary>
        /// The container where markers will be created.
        /// </summary>
        public RectTransform markerContainer, markerUserContainer;
        public RectTransform panelHideOtherMarkers;

        public void ShowMarkerPanels(bool val) {
            if (val)
            {
                //show user marker if user wants on-site navigation
                markerUserContainer.gameObject.SetActive(!AppManager.Instance.IsGpsNotInUse());
            }
            else
            {
                markerUserContainer.gameObject.SetActive(false);
                
            }
            markerContainer.gameObject.SetActive(val);
            panelHideOtherMarkers.gameObject.SetActive(false);
        }
        public void ShowUserPanel(bool val) { markerUserContainer.gameObject.SetActive(val); }

        private void Awake()
        {
            //enable only for mobile or editor
            if (B.isDesctop)
            {
                this.enabled = false;
                return;
            }

            GlobalActionsUI.OnInfoPoiShow += OnInfoPoiShow;
        }

        void OnInfoPoiShow()
        {
            if(AppManager.Instance.appState == AppManager.AppState.MAP /*&& !AppManager.Instance.IsGpsNotInUse()*/)
            {
                panelHideOtherMarkers.gameObject.SetActive(true);
            }
        }

        IEnumerator Start()
        {

            Application.runInBackground = true;

            if (B.isDesctop)
            {
                this.enabled = false;
                yield break;
            }
            OnlineMapsLocationService.instance.OnLocationInited += OnLocationInited;
            OnlineMapsLocationService.instance.OnLocationChanged += OnGpsLocationChanged;
            OnlineMapsLocationService.instance.OnFindLocationByIPComplete += OnFindLocationByIPComplete;

            yield return new WaitForSeconds(2f);

            yield break;

        }

        void OnLocationInited()
        {
            userPosition = OnlineMapsLocationService.instance.position;
            OnGpsOn?.Invoke();
        }

        void OnFindLocationByIPComplete()
        {
            userPosition = OnlineMapsLocationService.instance.position;
        }

        public void CheckPosition()
        {
            if (siteMode != SiteMode.OFF)
            {
                if (AppManager.Instance.appState != AppManager.AppState.MAP) return;
                userPosition = OnlineMapsLocationService.instance.position;
                OnGpsLocationChanged(userPosition);
            
                if(markerUser==null) markerUser = CustomMarkerEngineGUI.AddUserMarker(userPosition, "", Instance.markerUserContainer);
            }
        }

        void OnGpsLocationChanged(Vector2 pos)
        {
            if (Application.isEditor) Debug.Log("OnGpsLocationChanged");
            userPosition = pos;
            ARManager.Instance.EnableButtonAR(false);

            if (siteMode != SiteMode.NEAR) { CheckUserDistance(); }
            else
            {
                if (AppManager.Instance.appState != AppManager.AppState.MAP) return;//check only if user is viewing map

                if (CustomMarkerEngineGUI.markers.Count <= 0) return;//if no pois return

                if (AppManager.Instance.IsGpsNotInUse()) return;

                if (markerUser != null)
                {
                    markerUser.SetPosition(userPosition);
                    CustomMarkerEngineGUI.instance.UpdateMarker(markerUser);
                }

                float minDist = Mathf.Infinity;
                PoiEntity poiEntity = null;
                foreach (CustomMarkerGUI marker in CustomMarkerEngineGUI.markers)
                {
                    float dist = GetDistanceBetweenPoints(userPosition, marker.pos);// new Vector2((float)marker.lng, (float)marker.lat));
                    PoiItem poiItem = marker.GetComponent<PoiItem>();

                    if (dist < minDist)
                    {
                        minDist = dist;
                        poiEntity = poiItem.poiEntity;
                    }
                }

                //convert km to meters
                float finalDistanceCheck = triggerPoiDist / 1000f;

                //if user is near to any poi
                if (minDist < finalDistanceCheck)
                {
                    if (poiEntity != null)
                    {
                        if (Application.isEditor)
                        {
                            Debug.Log("Near to " + poiEntity.id);
                            Debug.Log("Near to " + poiEntity.infoGR.name);// GetTitle());
                        }

                        Handheld.Vibrate();

                        if (AppManager.Instance.navigationMode == AppManager.NavigationMode.ON_SITE_AR)
                        {
                            if (ARManager.Instance.arMode == ARManager.ARMode.SUPPORTED)
                            {
                                if (!ARManager.Instance.iconARbtn.activeSelf)
                                {
                                    UserWasNearPoiOnLastCheck = true;
                                    ARManager.Instance.EnableButtonAR(true);
                                }
                            }
                        }
                        else if (AppManager.Instance.navigationMode == AppManager.NavigationMode.ON_SITE)
                        {
                            //show poi info panel
                            GlobalActionsUI.OnPoiSelected?.Invoke(poiEntity.id);
                        }
                    }
                }
                else//if user is far from pois
                {
                    if(UserWasNearPoiOnLastCheck) Handheld.Vibrate();//only once
                    UserWasNearPoiOnLastCheck = false;

                    if (AppManager.Instance.navigationMode == AppManager.NavigationMode.ON_SITE_AR)
                    {
                        if (ARManager.Instance.arMode == ARManager.ARMode.SUPPORTED)
                        {
                            ARManager.Instance.StopARSession();
                        }
                    }
                    else
                    {
                        //hide poi info panel
                        GlobalActionsUI.OnInfoPoiJustHide?.Invoke();
                    }
                }
            }
            //CheckSiteMode();
        }

        private IEnumerator Vibrate()
        {
            float interval = 0.05f;
            WaitForSeconds wait = new WaitForSeconds(interval);
            float t;

            for (t = 0; t < 1; t += interval) // Change the end condition (t < 1) if you want
            {
                Handheld.Vibrate();
                yield return wait;
            }

            yield return new WaitForSeconds(0.4f);

            for (t = 0; t < 1; t += interval) // Change the end condition (t < 1) if you want
            {
                Handheld.Vibrate();
                yield return wait;
            }
        }


        float GetDistanceBetweenPoints(Vector2 pA, Vector2 pB)
        {
            // Calculate the distance in km between locations.
            return OnlineMapsUtils.DistanceBetweenPoints(pA, pB).magnitude;
        }

        //void RemovePointOnNearestPath()
        //{
        //    CustomMarkerEngineGUI.RemoveMarker(markerOnNearestPath);
        //}

        private bool IsSiteModeFar()
        {
            //set max distance
            float dist = Mathf.Infinity;

            if (dist > GetDistanceBetweenPoints(userPosition, athensCheckPoint))
            {
                dist = GetDistanceBetweenPoints(userPosition, athensCheckPoint);
            }

            //if distance is more than max km
            if (dist > maxKmDistanceForOnSiteMode)
            {
                OnGpsFar?.Invoke();
                siteMode = SiteMode.FAR;
                //message far away
                if (B.isRealEditor) Debug.Log("GPS FAR");
                return true;
            }

            OnGpsClose?.Invoke();
            siteMode = SiteMode.NEAR;
            if (B.isRealEditor) Debug.Log("GPS CLOSE");
            return false;
        }

        #region Search OnSite Town
        

        public void CheckUserDistance()
        {

            if(Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (!OnlineMapsLocationService.instance.useGPSEmulator)
                {
                    if (Application.isEditor) Debug.Log("GPS - OFF!!!");

                    OnGpsOff?.Invoke();
                    siteMode = SiteMode.OFF;
                    return;
                }
            }
            else if (Application.isMobilePlatform)
            {
                if (!OnlineMapsLocationService.instance.IsLocationServiceRunning())
                {
                    if (Application.isEditor) Debug.Log("GPS - OFF!!!");

                    OnGpsOff?.Invoke();
                    siteMode = SiteMode.OFF;
                    return;
                }
            }

            if (Application.isEditor) Debug.Log("Editor - checking gps distance for "+ userPosition);

            //0 = far
            //1 = Nafpaktos mountain
            //2 = Nafpaktos west
            //3 = Nafpaktos center
            //4 = Athens
            //5 = Herakleion
            int isNear = 0;
            //set max distance
            float dist = Mathf.Infinity;

            if (dist > GetDistanceBetweenPoints(userPosition, mountainCheckPoint))
            {
                dist = GetDistanceBetweenPoints(userPosition, mountainCheckPoint);
                isNear = 1;
            }

            if (dist > GetDistanceBetweenPoints(userPosition, westCheckPointA))
            {
                dist = GetDistanceBetweenPoints(userPosition, westCheckPointA);
                isNear = 2;
            }

            if (dist > GetDistanceBetweenPoints(userPosition, westCheckPointB))
            {
                dist = GetDistanceBetweenPoints(userPosition, westCheckPointB);
                isNear = 2;
            }

            if (dist > GetDistanceBetweenPoints(userPosition, nafpaktosCheckPoint))
            {
                dist = GetDistanceBetweenPoints(userPosition, nafpaktosCheckPoint);
                isNear = 3;
            }

            if (dist > GetDistanceBetweenPoints(userPosition, athensCheckPoint))
            {
                dist = GetDistanceBetweenPoints(userPosition, athensCheckPoint);
                isNear = 4;
            }

            if (dist > GetDistanceBetweenPoints(userPosition, herakleionCheckPoint))
            {
                dist = GetDistanceBetweenPoints(userPosition, herakleionCheckPoint);
                isNear = 5;
            }

            if (Application.isEditor) Debug.Log("herakleionCheckPoint = " + herakleionCheckPoint);
            if (Application.isEditor) Debug.Log("dist = " + dist);
            if (Application.isEditor) Debug.Log("Near = " + isNear);

            //if distance is more than max km
            if (dist > maxKmDistanceForOnSiteMode)
            {
                //isNear = 0;
                //message far away
                if (Application.isEditor) Debug.Log("FAR AWAY!!");

                OnGpsFar?.Invoke();
                siteMode = SiteMode.FAR;
                return;
            }

            if (isNear == 4)
            {
                if (Application.isEditor) Debug.Log("Near Athens!!");
                OnGpsNearAthens?.Invoke();
                siteMode = SiteMode.NEAR;
            }

            #region not in use
            /*
             * 
            //if (isNear == 0)
            //{
            //    //message far away
            //    if(B.isRealEditor) Debug.Log("FAR AWAY!!");
            //}

            if (isNear == 1)
            {
                if (Application.isEditor) Debug.Log("Near Nafpaktos Mountain!!");
                //near mountain - find path and nearest point on path

                OnGpsNearNafpaktos?.Invoke();
                OnGpsClose?.Invoke();
                siteMode = SiteMode.NEAR;
            }
            else if (Application.isEditor)
            {
                if (B.isRealEditor) Debug.Log("Near Nafpaktos West!!");
                OnGpsNearNafpaktos?.Invoke();
                OnGpsClose?.Invoke();
                siteMode = SiteMode.NEAR;
            }
            else if (isNear == 3)
            {
                if (Application.isEditor) Debug.Log("Near Nafpaktos Center!!");
                OnGpsNearNafpaktos?.Invoke();
                OnGpsClose?.Invoke();
                siteMode = SiteMode.NEAR;
                OnGpsClose?.Invoke();
                siteMode = SiteMode.NEAR;
            }
            else if (isNear == 4)
            {
                if (Application.isEditor) Debug.Log("Near Athens!!");
                OnGpsNearAthens?.Invoke();
                OnGpsClose?.Invoke();
                siteMode = SiteMode.NEAR;
            }
            else if (isNear == 5)
            {
                if (Application.isEditor) Debug.Log("Near Herakleion!!");
                OnGpsNearHerakleion?.Invoke();
                OnGpsClose?.Invoke();
                siteMode = SiteMode.NEAR;
            }
            */
            #endregion
        }


        #endregion
    }

}
