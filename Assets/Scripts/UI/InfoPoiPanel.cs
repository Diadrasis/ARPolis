﻿using ARPolis.Data;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ARPolis.UI
{

    public class InfoPoiPanel : MonoBehaviour
    {

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

        private void Awake()
        {
            GlobalActionsUI.OnPoiSelected += OnSelectPoi;
            GlobalActionsUI.OnShowTopicTours += HideInfo;
            //dragButton.GetComponent<EventTrigger>().OnEndDrag?.Invoke(OnDragEnd);
        }

        private void Start()
        {
            ShowButtonArrow(false);
        }

        private void Update()
        {
            trNextBack.SetActive(trNext.activeSelf);
            trPrevBack.SetActive(trPrev.activeSelf);
        }

        private void OnSelectPoi(string poiID)
        {
            poiEntityNow = InfoManager.Instance.GetPoiEntity(poiID);

            txtTitle.text = poiEntityNow.GetTitle();
            txtShortDesc.text = poiEntityNow.GetShortDesc();
            txtDesc.text = poiEntityNow.GetHistoricalReview();

            //get poi data

            //get images
            if (DestroyPreviousImages()) CreateImages();
            
            //get testimony

            ScrollResetPosition();
            RefreshElements();

            //show panel
            ShowPreviewInfo();

            InfoManager.Instance.poiNowID = poiID;

            GlobalActionsUI.OnInfoPoiShow?.Invoke();//hide tours bootom bar
        }

        void CreateImages()
        {
            if (poiEntityNow.digitalExhibitImages.Count <= 0)
            {
                // panelPhotos.SetActive(false);
                Transform pImg = Instantiate(prefabImage, rectScrollImages);
                ImageItem item = pImg.GetComponent<ImageItem>();
                item.InitNull();
                Invoke("RefreshElements", 0.15f);
                return;
            }
            
            for (int i = 0; i < poiEntityNow.digitalExhibitImages.Count; i++)
            {
                DigitalExhibitObject dgImage = poiEntityNow.digitalExhibitImages[i];
                Transform pImg = Instantiate(prefabImage, rectScrollImages);
                ImageItem item = pImg.GetComponent<ImageItem>();
                item.Init(dgImage.fileName, poiEntityNow.topicID, dgImage.GetLabel(), dgImage.sourceLabel);
            }
        }

        private bool DestroyPreviousImages()
        {
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

        private void ShowPreviewInfo() { ShowButtonArrow(true); SetArrowUp(true); transitionClass.ShowPercentagePanel(); }

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
            }
        }

        private void HideInfo() { ShowButtonArrow(false); SetArrowUp(true); transitionClass.HidePanel();}

        private void ShowButtonArrow(bool val) { btnArrow.SetActive(val); dragButton.SetActive(val); }

        private void SetArrowUp(bool val)
        {
            Vector3 rot = trArrow.localEulerAngles;
            rot.z = val ? 90f : -90f;
            trArrow.localEulerAngles = rot;
        }
    }

}
