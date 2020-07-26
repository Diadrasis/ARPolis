using ARPolis.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis.UI
{

    public class TourItem : MonoBehaviour
    {

        public string tourID, topicID;
        public TourLanguange infoGR, infoEN;

        public Text txtTitle, txtDesc, txtShortText;

        public Button btnShowPoisOnMap;

        public RectTransform[] allRects;

        public RectTransform rectScrollContainer, rectScrollImages;
        public int pageID;

        private void OnEnable()
        {
            GlobalActionsUI.OnToggleTarget += RefreshContainer;
            GlobalActionsUI.OnLangChanged += SetTextInfo;
            GlobalActionsUI.OnTourItemPageChanged += OnTourItemPageChanged;
            btnShowPoisOnMap.onClick.AddListener(SelectTourPois);
            btnShowPoisOnMap.onClick.AddListener(ScrollResetPosition);
        }

        void OnTourItemPageChanged()
        {
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
        }

        private void SelectTourPois()
        {
            InfoManager.Instance.tourNowID = tourID;
            GlobalActionsUI.OnShowPoisOnMap?.Invoke();
        }

        public void SetTextInfo()
        {
            txtTitle.text = GetTitle();
            txtDesc.text = GetDesc();
            txtShortText.text = GetShortDesc();
            //RefreshContainer(null);
            Invoke("RefreshElements", 0.1f);
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
            Debug.Log("tours item refresh");
            RefreshElements();
        }

        private void RefreshElements() { foreach (RectTransform rt in allRects) LayoutRebuilder.ForceRebuildLayoutImmediate(rt); }

        public void DestroyItem() { Destroy(gameObject); }

        private void OnDisable()
        {
            GlobalActionsUI.OnToggleTarget -= RefreshContainer;
            GlobalActionsUI.OnLangChanged -= SetTextInfo;
            GlobalActionsUI.OnTourItemPageChanged -= OnTourItemPageChanged;
            btnShowPoisOnMap.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            GlobalActionsUI.OnToggleTarget -= RefreshContainer;
            GlobalActionsUI.OnLangChanged -= SetTextInfo;
            GlobalActionsUI.OnTourItemPageChanged -= OnTourItemPageChanged;
            btnShowPoisOnMap.onClick.RemoveAllListeners();
        }
    }

}
