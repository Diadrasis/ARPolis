using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.Map
{

    public class OnSiteManager : MonoBehaviour
    {

        public enum SiteMode { OFF, NEAR, FAR }
        public SiteMode siteMode = SiteMode.OFF;

        //manage on site logic and ui messages
        public delegate void ActionMessages();
        public static ActionMessages OnGpsFar, OnGpsClose, OnCloseToPointOnPath, OnGpsOn, OnGpsOff;

        public float maxKmDistanceForOnSiteMode = 5f;

        readonly Vector2 mountainCheckPoint = new Vector2(21.903f, 38.5635f);
        readonly Vector2 westCheckPointA = new Vector2(21.70192f, 38.4515f);
        readonly Vector2 westCheckPointB = new Vector2(21.76776f, 38.46531f);
        readonly Vector2 athensCheckPoint = new Vector2(23.72685f, 37.97925f);

        public Vector2 userPosition;
        //PathInfoManager pathInfoManager;
        //CustomMarkerGUI markerOnNearestPath;
        MapController mapController;

        private void Awake()
        {
            //enable only for mobile or editor
            if (B.isDesctop)
            {
                this.enabled = false;
                return;
            }

            mapController = FindObjectOfType<MapController>();
            //pathInfoManager = FindObjectOfType<PathInfoManager>();
        }

        void Start()
        {

            if (B.isDesctop)
            {
                this.enabled = false;
                return;
            }

            //if (B.isMobileHaveGyro) { Input.gyro.enabled = true; }

            if (!OnlineMapsLocationService.instance.IsLocationServiceRunning())
            {
                if (B.isEditor) Debug.Log("GPS OFF");
                OnGpsOff?.Invoke();
                siteMode = SiteMode.OFF;
            }
            OnlineMapsLocationService.instance.OnLocationInited += OnLocationInited;
            OnlineMapsLocationService.instance.OnLocationChanged += OnGpsLocationChanged;
            OnlineMapsLocationService.instance.OnFindLocationByIPComplete += OnFindLocationByIPComplete;

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

        void OnGpsLocationChanged(Vector2 pos)
        {
            userPosition = pos;
            //SearchNearestPath();
            CheckSiteMode();
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

        void CheckSiteMode()
        {
            //set max distance
            float dist = Mathf.Infinity;

            if (dist > GetDistanceBetweenPoints(userPosition, athensCheckPoint))
            {
                dist = GetDistanceBetweenPoints(userPosition, athensCheckPoint);
            }

            //if distance is more than 10km
            if (dist > maxKmDistanceForOnSiteMode)
            {
                OnGpsFar?.Invoke();
                siteMode = SiteMode.FAR;
                //message far away
                if (B.isEditor) Debug.Log("GPS FAR");
                return;
            }

            OnGpsClose?.Invoke();
            siteMode = SiteMode.NEAR;
            if (B.isEditor) Debug.Log("GPS CLOSE");
        }

        #region Just Test
        /*

        void SearchNearestPath()
        {
            float calcStartTime = Time.timeSinceLevelLoad;

            //0 = far
            //1 = mountain
            //2 = west
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

            if (B.isEditor) Debug.Log("dist = " + dist);

            //if distance is more than 10km
            if (dist > maxKmDistanceForOnSiteMode)
            {
                //isNear = 0;
                //message far away
                if (B.isEditor) Debug.Log("FAR AWAY!!");
            }

            if (isNear == 0)
            {
                //message far away
                //if(B.isEditor) Debug.Log("FAR AWAY!!");
            }

            if (isNear == 1)
            {
                if (B.isEditor) Debug.Log("Near Mountain!!");
                //near mountain - find path and nearest point on path
            }
            else if (isNear == 2)
            {
                if (B.isEditor) Debug.Log("Near West!!");
                //near west - find path and nearest point on path

                float westDist = Mathf.Infinity;
                Vector2 nearestPos = new Vector2(Mathf.Infinity, Mathf.Infinity);




            }

        }

        */
        #endregion
    }

}
