using ARPolis.Data;
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

        public void Init()
        {
            if (icon) col = icon.color;
            iconSelected.enabled  =false;
            textSelected.enabled = false;
            if (btn) btn.onClick.AddListener(SelectPoi);
        }

        void SelectPoi()
        {
            if(AppManager.Instance.IsGpsNotInUse()) GlobalActionsUI.OnPoiSelected?.Invoke(poiID);
        }

        private void OnEnable()
        {
            GlobalActionsUI.OnPoiSelected += OnPoiSelected;
            GlobalActionsUI.OnResetMarkersLabel += ShowLabel;
        }

        private void OnDisable()
        {
            GlobalActionsUI.OnPoiSelected -= OnPoiSelected;
            GlobalActionsUI.OnResetMarkersLabel -= ShowLabel;
            if (btn) btn.onClick.RemoveAllListeners();
        }

        void ShowLabel()
        {
            label.SetActive(true);
        }

        void OnPoiSelected(string id)
        {
            bool isThisPoi = poiID == id;
            if (iconSelected) iconSelected.enabled = isThisPoi;
            if (textSelected) textSelected.enabled = isThisPoi;
            if (icon) icon.color = isThisPoi ? Color.white : col;

            if (isThisPoi)
            {
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
