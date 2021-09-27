using ARPolis.Data;
using ARPolis.Map;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ARPolis.UI
{

    public class InfoPoiPanel : Singleton<InfoPoiPanel>
    {
        protected InfoPoiPanel() { }

        public TMPro.TextMeshProUGUI txtTitle, txtDesc, txtShortDesc;
        public Transform rectImagesContainer;
        public Transform prefabImage;
        public PanelTransitionClass transitionClass;
        public GameObject btnArrow, dragButton;
        public Transform trArrow;
        public RectTransform rectScrollContainer, rectScrollImages;
        public RectTransform[] allRects;

        public GameObject trNext, trPrev, trNextBack, trPrevBack;

        public PoiEntity poiEntityNow;

        public ScrollSnapCustom snapCustom;

        public Image[] panelColors;

        public Color colTopic1, colTopic2, colTopic3, colTopic4;

        private string poiLang;

        public Transform prefabAudioPanel;

        private void Awake()
        {
            GlobalActionsUI.OnPoiSelected += OnSelectPoi;
            GlobalActionsUI.OnMyPlaceSelected += OnMyPlaceSelected;
            GlobalActionsUI.OnShowTopicTours += HideInfo;
            GlobalActionsUI.OnInfoPoiJustHide += HideInfo;

            GlobalActionsUI.OnLogoutUser += HideInfo;

            //dragButton.GetComponent<EventTrigger>().OnEndDrag?.Invoke(OnDragEnd);
        }

        private void Start()
        {
            ShowButtonArrow(false);

            OnlineMaps.instance.OnChangeZoom += OnMapUserInteraction;
            OnlineMaps.instance.OnChangePosition += OnMapUserInteraction;
        }

        private void Update()
        {
            trNextBack.SetActive(trNext.activeSelf);
            trPrevBack.SetActive(trPrev.activeSelf);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                //areaAthens.topics.Find(b => b.id == topicNowID).tours.Find(t => t.id == tourNowID).pois.Find(p => p.id == id);
                //OnSelectPoi(InfoManager.Instance.areaAthens.topics[0].tours[0].tourPoiEntities[0].poiID);
            }
        }

        private void OnMyPlaceSelected(PoiEntity poi)
        {
            if (poi == null || !poi.Exists()) return;
            //poiEntityNow = poi;

            ChangePanelColor(poi.topicID);

            txtTitle.text = poi.GetTitle();
            txtShortDesc.text = poi.GetShortDesc();
            txtDesc.text = poi.GetHistoricalReview();

            //get poi data

            //get images
            if (DestroyPreviousImages()) CreateImages();

            poiLang = StaticData.lang;

            //get testimonies
            if(poi.digitalExhibitTestimonies.Count > 0)
            {
                for(int i= 0; i < poi.digitalExhibitTestimonies.Count; i++){
                    //rectScrollContainer
                    //Instantiate(prefabAudioPanel, rectScrollContainer);
                }
            }

            ScrollResetPosition();
            RefreshElements();

            //show panel
            ShowPreviewInfo();

            InfoManager.Instance.poiNowID = poi.id;

            GlobalActionsUI.OnInfoPoiShow?.Invoke();//check saved status
        }

        private void OnSelectPoi(string poiID)
        {
            poiEntityNow = InfoManager.Instance.GetPoiEntity(poiID);

            if (poiEntityNow == null || !poiEntityNow.Exists()) return;

            ChangePanelColor(InfoManager.Instance.topicNowID);

            txtTitle.text = poiEntityNow.GetTitle();
            txtShortDesc.text = poiEntityNow.GetShortDesc();
            txtDesc.text = poiEntityNow.GetHistoricalReview();

            //get poi data

            //get images
            if (DestroyPreviousImages()) CreateImages();

            poiLang = StaticData.lang;

            //get testimonies
            if(Application.isEditor) Debug.Log("Testimonies = " + poiEntityNow.digitalExhibitTestimonies.Count);
            if (poiEntityNow.digitalExhibitTestimonies.Count > 0)
            {
                for (int i = 0; i < poiEntityNow.digitalExhibitTestimonies.Count; i++)
                {
                    //rectScrollContainer
                    //Instantiate(prefabAudioPanel, rectScrollContainer);
                }
            }

            ScrollResetPosition();
            RefreshElements();

            //show panel
            ShowPreviewInfo();

            InfoManager.Instance.poiNowID = poiID;

            GlobalActionsUI.OnInfoPoiShow?.Invoke();//check saved status


        }

        void CreateImages()
        {
            if (poiEntityNow == null) return;
            if (poiEntityNow.digitalExhibitImages.Count <= 0)
            {
                ShowNoImagesPanel();
                return;
            }

            int total = 0;
            for (int i = 0; i < poiEntityNow.digitalExhibitImages.Count; i++)
            {
                MultimediaObject dgImage = poiEntityNow.digitalExhibitImages[i];
                if (dgImage == null) { Debug.Log("NULL Multimedia content [IMAGE]"); }
                else
                {
                    total++;
                    Transform pImg = Instantiate(prefabImage, rectScrollImages);
                    ImageItem item = pImg.GetComponent<ImageItem>();
                    item.Init(dgImage.fileName, poiEntityNow.topicID, dgImage.GetLabel(), dgImage.sourceLabel);
                }
            }
            if (total <= 0) ShowNoImagesPanel();
        }

        void ShowNoImagesPanel()
        {
            Transform pImg = Instantiate(prefabImage, rectScrollImages);
            ImageItem item = pImg.GetComponent<ImageItem>();
            item.InitNull();
            Invoke(nameof(RefreshElements), 0.15f);
        }

        private bool DestroyPreviousImages()
        {
            if (poiLang != StaticData.lang)
            {
                DestroyAllItems();
                return true;
            }

            //if it is the same topic, no need to destroy
            if (poiEntityNow.id == InfoManager.Instance.poiNowID)
            {
                if (B.isEditor) Debug.Log("same poi selected - keeping items");
                return false;
            }
            ImageItem[] items = rectImagesContainer.GetComponentsInChildren<ImageItem>(true);
            //no tours - we need to create
            if (items.Length <= 0) return true;

            //new topic selected - destroy tours
            foreach (ImageItem item in items) item.DestroyItem();

            //reset scroll
            snapCustom.enabled = false;
            snapCustom.RemoveAllChildren();
            Invoke("ResetScrollSnap", 0.1f);

            return true;
        }

        void DestroyAllItems()
        {
            ImageItem[] items = rectImagesContainer.GetComponentsInChildren<ImageItem>(true);
            foreach (ImageItem item in items) item.DestroyItem();
        }

        void ResetScrollSnap() { snapCustom.enabled = true; }

        private void ScrollResetPosition()
        {
            Vector3 pos = rectScrollContainer.localPosition;
            pos.y = 0f;
            rectScrollContainer.localPosition = pos;
            Vector3 pos2 = rectScrollImages.localPosition;
            pos2.x = 0f;
            rectScrollImages.localPosition = pos2;
            rectScrollContainer.parent.GetComponent<ScrollRect>().enabled = true;
            rectScrollImages.parent.GetComponent<ScrollRect>().enabled = true;
            //RefreshElements();
        }

        private void RefreshElements() { foreach (RectTransform rt in allRects) LayoutRebuilder.ForceRebuildLayoutImmediate(rt); }

        private void ShowPreviewInfo() { ShowButtonArrow(true); SetArrowUp(true); transitionClass.ShowPercentagePanel(); StaticData.isPoiInfoVisible = 1; }

        public void ToggleFullInfo() {

            if (transitionClass.moveMode == PanelTransitionClass.MoveMode.Full)
            {
                HideInfo();
            }
            else if (transitionClass.moveMode == PanelTransitionClass.MoveMode.Percentage)
            {
                ShowButtonArrow(true);
                SetArrowUp(false);
                transitionClass.ShowPanel();
                StaticData.isPoiInfoVisible = 2;
            }
        }

        void OnMapUserInteraction()
        {
            HideInfo();
        }

        public void HideInfo() {
            if (transitionClass.moveMode == PanelTransitionClass.MoveMode.Hidden) return;
            ShowButtonArrow(false); 
            SetArrowUp(true); 
            transitionClass.HidePanel(); 
            StaticData.isPoiInfoVisible = 0;
            InfoManager.Instance.poiNowID = string.Empty;
            MenuPanel.Instance.OnInfoHideSetBottomsButtons();
            GlobalActionsUI.OnPoiSelected?.Invoke("");
            OnSiteManager.Instance.panelHideOtherMarkers.gameObject.SetActive(false);
            GlobalActionsUI.OnResetMarkersLabel?.Invoke();
        }

        public bool IsInfoPanelHidden() { return !transitionClass.isVisible; }

        private void ShowButtonArrow(bool val) { btnArrow.SetActive(val); dragButton.SetActive(val); }

        private void SetArrowUp(bool val)
        {
            Vector3 rot = trArrow.localEulerAngles;
            rot.z = val ? 90f : -90f;
            trArrow.localEulerAngles = rot;
        }

        void ChangePanelColor(string topicID)
        {
            Color col = TopicColor(topicID);
            foreach (Image img in panelColors) img.color = col;
        }

        public Color TopicColor(string id)
        {
            switch (id)
            {
                case "1":
                    return colTopic1;
                case "2":
                    return colTopic2;
                case "3":
                    return colTopic3;
                case "4":
                    return colTopic4;
                default:
                    return colTopic1;
            }
        }

    }

}
