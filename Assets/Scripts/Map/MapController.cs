using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.Map
{

    public class MapController : MonoBehaviour
    {

        //private readonly string mapboxAccessToken = "pk.eyJ1Ijoic3RhZ2UiLCJhIjoiY2lrNmtyN2lvMDB1ZHBpa3N3YnZjOHdrNCJ9.MeQ4hTL5aNkJ_02D8_Ppww";
        //private readonly string mapboxMapId = "ck626iopt0s1m1ip3nn2o6a26";
        //private readonly string mapbocUserId = "stage";

        //Athens
        Vector2 mapInitPos = new Vector2(23.6729876618352f, 37.8665345360163f);
        int zoomGreece = 7;

        public delegate void MapAction();
        public static MapAction OnMapDrag;

        //we need this to allow map drag in case user is touching poi icons (UI)
        public static readonly string mapMarkerTag = "mapMarker";

        public List<Sprite> iconPois = new List<Sprite>();

        Sprite SprPoi(string type)
        {
            Sprite spr = iconPois.Find(b => b.name.Contains(type));
            if (spr != null) return spr;
            return iconPois[0];
        }


        public void InitMap()
        {
            // Lock map zoom range
            OnlineMaps.instance.zoomRange = new OnlineMapsRange(6, 20);

            // Lock map coordinates range
            OnlineMaps.instance.positionRange = new OnlineMapsPositionRange(35, 20, 41, 28);

            ResetMap();

            OnlineMapsControlBase.instance.OnMapDrag += DragMapStarted;

            // TryLoadMarkers();

        }


        void DragMapStarted()
        {
            OnMapDrag?.Invoke();
        }

        public void ResetMap()
        {
            // Initializes the position and zoom
            OnlineMaps.instance.zoom = 15;
            OnlineMaps.instance.position = mapInitPos;
            CustomMarkerEngineGUI.RemoveAllMarkers();
        }

        public void ResetMapZoomPos()
        {
            // Initializes the position and zoom
            OnlineMaps.instance.zoom = zoomGreece;
            OnlineMaps.instance.position = mapInitPos;
        }

        #region Example to show custom markers
        /*
        public bool ShowTripMarkers(string tripID)
        {
            if (B.isEditor) Debug.Log("ShowTripMarkers for trip " + tripID);

            List<TmiPoiMarker> tripPoiMarkers = new List<TmiPoiMarker>();

            ServerManager.GetTripPoiMarkers(tripID, out tripPoiMarkers);

            if (tripPoiMarkers.Count > 0)
            {
                List<Vector2> line = new List<Vector2>();

                for (int i = 0; i < tripPoiMarkers.Count; i++)// (TmiMarker marker in tripmarkers)
                {
                    int stopID = i + 1;
                    // Create marker
                    //OnlineMapsMarkerManager.CreateItem(marker.lng, marker.lat, marker.id.ToString());
                    line.Add(new Vector2((float)tripPoiMarkers[i].lng, (float)tripPoiMarkers[i].lat));
                    CustomMarkerGUI marker = CustomMarkerEngineGUI.AddMarker((double)tripPoiMarkers[i].lng, (double)tripPoiMarkers[i].lat, "stop " + stopID.ToString());

                    marker.Init();
                    marker.btnMarker.onClick.AddListener(() => OnCustomMarkerClick(marker));
                    marker.icon.sprite = SprPoi(tripPoiMarkers[i].type);
                    marker.poiMarker = tripPoiMarkers[i];
                }

                // Draw line
                OnlineMapsDrawingElementManager.AddItem(new OnlineMapsDrawingLine(line, Color.blue, 2));

                ZoomOnMarkers(true);

                return true;
            }

            return false;
        }
        */

        #endregion

        public delegate void ActionMarkerCustomClick(string title, string desc);
        public static ActionMarkerCustomClick OnMarkerCustomClick;

        void OnCustomMarkerClick(CustomMarkerGUI marker)
        {
            //StartCoroutine(SmoothZoomToPositionCustom(marker.pos, 14));
            //SmoothZoomToPositionOnUpdate(marker.pos, 12);
            Vector2 center;
            int zoom;

            List<Vector2> markers = new List<Vector2>();
            markers.Add(marker.pos);

            // Get the center point and zoom the best for all markers.
            OnlineMapsUtils.GetCenterPointAndZoom(markers.ToArray(), out center, out zoom);

            center.y -= 0.00023f;

            // Change the position and zoom of the map.
            OnlineMaps.instance.position = center;
            OnlineMaps.instance.zoom = zoom;

            zoom -= 1;

            //if (marker.poiMarker != null)
            //{
            //    OnMarkerCustomClick?.Invoke(marker.poiMarker.label, marker.poiMarker.desc);
            //}
            //else if (marker.activityMarker != null)
            //{
            //    OnMarkerCustomClick?.Invoke(marker.activityMarker.label, marker.activityMarker.desc);
            //}
        }

        public void ClearMap()
        {
            OnlineMapsDrawingElementManager.RemoveAllItems();
            CustomMarkerEngineGUI.RemoveAllMarkers();
        }

        public void ZoomOnMarkers(bool isAuto)
        {
            if (B.isRealEditor) Debug.LogWarning("### ZoomOnMarkers ###");

            if (CustomMarkerEngineGUI.allMarkersPositions().Length <= 0)
            {
                if (B.isRealEditor) Debug.LogWarning("### NO Markers TO ZOOM ###");
                return;
            }

            if (isAuto)
            {
                ResetMapZoomPos();
            }
            else
            {
                Vector2 center;
                int zoom;

                // Get the center point and zoom the best for all markers.
                OnlineMapsUtils.GetCenterPointAndZoom(CustomMarkerEngineGUI.allMarkersPositions(), out center, out zoom);

                zoom -= 1;

                //if (OnlineMaps.instance.position == center)
                //{
                //    ResetMapZoomPos();
                //    return;
                //}

                OnlineMaps.instance.position = center;
                OnlineMaps.instance.zoom = zoom;
            }


            // Change the position and zoom of the map.
            //if (!isSmoothMoveZoom)
            //{
            //    OnlineMaps.instance.position = center;
            //    OnlineMaps.instance.zoom = zoom;
            //}
            //else
            //{
            //    //SmoothZoomToPositionOnUpdate(center, zoom);
            //    StartCoroutine(SmoothZoomToPositionCustom(center, zoom, isAuto));
            //}
        }


    }

}
