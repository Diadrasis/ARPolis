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
        RawImage img;
        public GameObject iconSelected, textSelected;
        Color col;
        public void Init()
        {
            Button btn = GetComponent<Button>();
            img = GetComponent<RawImage>();
            if (img) col = img.color;
            iconSelected = transform.Find("iconSelected").gameObject;
            iconSelected.SetActive(false);
            textSelected = transform.Find("Text/textSelected").gameObject;
            textSelected.SetActive(false);
            if (btn)
            {
                //SaveAsVisited("west", poiInfo.poiID, btn, false);
                btn.onClick.AddListener(() => GlobalActionsUI.OnPoiSelected?.Invoke(poiID));
                //if (saveVisitedPlaces) btn.onClick.AddListener(() => SaveAsVisited("west", poiInfo.poiID, btn, true));
            }
        }

        private void OnEnable()
        {
            GlobalActionsUI.OnPoiSelected += OnPoiSelected;
        }

        private void OnDisable()
        {
            GlobalActionsUI.OnPoiSelected -= OnPoiSelected;
        }

        void OnPoiSelected(string id)
        {
            if (iconSelected) iconSelected.SetActive(poiID == id);
                
            if (poiID == id)
            {
                GetComponent<RectTransform>().SetAsLastSibling();//set to front in view
                if (img) img.color = Color.white;
                if (textSelected) textSelected.SetActive(true);
            }
            else {
                GetComponent<RectTransform>().SetAsFirstSibling();//set to last in view
                if (img) img.color = col; 
                if (textSelected) textSelected.SetActive(false);
            }
        }

        public void SetImage(Texture2D spr) { if (img) img.texture = spr; }

    }

}
