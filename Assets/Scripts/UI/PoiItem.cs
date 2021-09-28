using ARPolis.Data;
using ARPolis.Map;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis.UI
{

    public class PoiItem : MonoBehaviour
    {

        public string poiID;
        public PoiEntity poiEntity;
        public RawImage icon, iconSelected;
        public Image textSelected;
        public Button btn;
        Color col;
        [HideInInspector]
        public RectTransform panelHideOtherMarkers;
        public GameObject label;
        [HideInInspector]
        public Vector2 pos;

        public void Init()
        {
            if (icon) col = icon.color;
            iconSelected.enabled  =false;
            textSelected.enabled = false;
            //if (btn) btn.onClick.AddListener(SelectPoi);
            ShowLabel();
        }

        void SelectPoi()
        {
            if (AppManager.Instance.IsGpsNotInUse())
            {
                if (OnSiteManager.Instance.OnPoiSelectAllowMapToMoveToPoiPosition)
                {
                    OnlineMaps.instance.position = pos;
                    StartCoroutine(DelaySelect());
                }
                else
                {
                    GlobalActionsUI.OnPoiSelected?.Invoke(poiID);
                }
            }
        }
        //delay selection in case map movement in poi position
        //because map movement hides info panel
        IEnumerator DelaySelect()
        {
            while (InfoPoiPanel.Instance.transitionClass.isVisible) yield return null;
            GlobalActionsUI.OnPoiSelected?.Invoke(poiID);
        }

        private void OnEnable()
        {
            GlobalActionsUI.OnPoiSelected += OnPoiSelected;
            GlobalActionsUI.OnResetMarkersLabel += ShowLabel;
            OnlineMaps.instance.OnChangeZoom += OnMapChangeZoom;
            if (btn) btn.onClick.AddListener(SelectPoi);
            ShowLabel();
        }

        private void OnDisable()
        {
            GlobalActionsUI.OnPoiSelected -= OnPoiSelected;
            GlobalActionsUI.OnResetMarkersLabel -= ShowLabel;
            if(OnlineMaps.instance != null) OnlineMaps.instance.OnChangeZoom -= OnMapChangeZoom;
            if (btn) btn.onClick.RemoveAllListeners();
            label.SetActive(false);
        }

        void OnMapChangeZoom()
        {
            label.SetActive(OnSiteManager.Instance.IsMarkerLabelAbleToShow());
        }

        void ShowLabel()
        {
            if (OnSiteManager.Instance.IsMarkerLabelAbleToShow()) label.SetActive(true);
        }

        void OnPoiSelected(string id)
        {
            bool isThisPoi = poiID == id;
            if (iconSelected) iconSelected.enabled = isThisPoi;
            if (textSelected) textSelected.enabled = isThisPoi;
            if (icon) icon.color = isThisPoi ? Color.white : col;

            if (isThisPoi)
            {
                panelHideOtherMarkers.gameObject.SetActive(true);
                panelHideOtherMarkers.SetAsLastSibling();
                GetComponent<RectTransform>().SetAsLastSibling();//set to front in view
                label.SetActive(true);
            }
            else {
                GetComponent<RectTransform>().SetAsFirstSibling();//set to last in view
                label.SetActive(false);
            }
        }

        public void SetImage(Texture2D spr) { if (icon) icon.texture = spr; }

    }

}
