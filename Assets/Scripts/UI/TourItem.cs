﻿using ARPolis.Data;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis.UI
{

    public class TourItem : MonoBehaviour
    {
        TourEntity tourEntity;

        public string tourID, topicID;
        public TourLanguange infoGR, infoEN;

        public TMPro.TextMeshProUGUI txtTitle, txtDesc, txtShortText;
        

        public Button btnShowPoisOnMap;

        public RectTransform[] allRects;

        public RectTransform rectScrollContainer, rectScrollImages;
        public int pageID;

        public Transform imageItemPrefab;

        public GameObject panelPhotos;

        public Image colorTourImage, colorBtnNext, colorBtnPrev;

        private void OnEnable()
        {
            GlobalActionsUI.OnToggleTarget += RefreshContainer;
            GlobalActionsUI.OnLangChanged += SetTextInfo;
            GlobalActionsUI.OnTourItemPageChanged += OnTourItemPageChanged;
            GlobalActionsUI.OnTourPageChanged += OnTourItemPageChanged;
            btnShowPoisOnMap.onClick.AddListener(SelectTourPois);
            btnShowPoisOnMap.onClick.AddListener(ScrollResetPosition);
        }

        public void Init(TourEntity tour)
        {
            tourEntity = tour;

            infoGR = tour.infoGR;
            infoEN = tour.infoEN;
            topicID =tour.id;
            tourID = tour.id;
            SetTextInfo();
            CreateImages();
        }

        public void SetTourColor(Color col) { colorTourImage.color = col; colorBtnNext.color = col; colorBtnPrev.color = col; }

        void CreateImages()
        {
            if (tourEntity.digitalExhibitImages.Count > 0)
            {
                foreach(DigitalExhibitObject dgImage in tourEntity.digitalExhibitImages)
                {
                    Transform pImg = Instantiate(imageItemPrefab, rectScrollImages);
                    ImageItem item = pImg.GetComponent<ImageItem>();
                    item.Init(dgImage.fileName, tourEntity.topicID, dgImage.GetLabel(), dgImage.sourceLabel);
                }
            }
            else
            {
                panelPhotos.SetActive(false);
                Invoke(nameof(RefreshElements), 0.15f);
            }
        }

        void OnTourItemPageChanged()
        {
            rectScrollContainer.parent.GetComponent<ScrollRect>().enabled = false;
            rectScrollImages.parent.GetComponent<ScrollRect>().enabled = false;
            ScrollResetPosition();
        }

        void OnTourItemPageChanged(int id)
        {
            if (pageID == id) return;
            rectScrollContainer.parent.GetComponent<ScrollRect>().enabled = false;
            rectScrollImages.parent.GetComponent<ScrollRect>().enabled = false;
            ScrollResetPosition();
        }

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

        private void SelectTourPois()
        {
            InfoManager.Instance.tourNowID = tourID;
            GlobalActionsUI.OnShowPoisOnMap?.Invoke();
        }

        private void SetTextInfo()
        {
            txtTitle.text = GetTitle();
            txtDesc.text = GetDesc();
            txtShortText.text = GetShortDesc();
            //RefreshContainer(null);
            Invoke("RefreshElements", 0.15f);
        }

        private string GetTitle()
        {
            if (StaticData.lang == "gr") { return infoGR.title; }
            else
            if (StaticData.lang == "en") { return infoEN.title; }
            return string.Empty;
        }
        private string GetDesc()
        {
            if (StaticData.lang == "gr") { return infoGR.desc; }
            else
            if (StaticData.lang == "en") { return infoEN.desc; }
            return string.Empty;
        }
        private string GetShortDesc()
        {
            if (StaticData.lang == "gr") { return infoGR.shortdesc; }
            else
            if (StaticData.lang == "en") { return infoEN.shortdesc; }
            return string.Empty;
        }

        private void RefreshContainer(GameObject gb)
        {
            //Debug.Log("tours item refresh");
            RefreshElements();
        }

        private void RefreshElements() { foreach (RectTransform rt in allRects) LayoutRebuilder.ForceRebuildLayoutImmediate(rt); }

        public void DestroyItem() { Destroy(gameObject); }

        private void OnDisable()
        {
            GlobalActionsUI.OnToggleTarget -= RefreshContainer;
            GlobalActionsUI.OnLangChanged -= SetTextInfo;
            GlobalActionsUI.OnTourItemPageChanged -= OnTourItemPageChanged;
            GlobalActionsUI.OnTourPageChanged -= OnTourItemPageChanged;
            btnShowPoisOnMap.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            GlobalActionsUI.OnToggleTarget -= RefreshContainer;
            GlobalActionsUI.OnLangChanged -= SetTextInfo;
            GlobalActionsUI.OnTourItemPageChanged -= OnTourItemPageChanged;
            GlobalActionsUI.OnTourPageChanged -= OnTourItemPageChanged;
            btnShowPoisOnMap.onClick.RemoveAllListeners();
        }

    }

}
