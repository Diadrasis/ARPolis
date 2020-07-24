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

        private void OnEnable()
        {
            GlobalActionsUI.OnToggleTarget += RefreshContainer;
            GlobalActionsUI.OnLangChanged += SetTextInfo;
            btnShowPoisOnMap.onClick.AddListener(SelectTourPois);
        }

        void SelectTourPois()
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

        void RefreshContainer(GameObject gb)
        {
            Debug.Log("tours item refresh");
            RefreshElements();
        }

        void RefreshElements() { foreach (RectTransform rt in allRects) LayoutRebuilder.ForceRebuildLayoutImmediate(rt); }

        public void DestroyItem() { Destroy(gameObject); }

        private void OnDisable()
        {
            GlobalActionsUI.OnToggleTarget -= RefreshContainer;
            GlobalActionsUI.OnLangChanged -= SetTextInfo;
            btnShowPoisOnMap.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            GlobalActionsUI.OnToggleTarget -= RefreshContainer;
            GlobalActionsUI.OnLangChanged -= SetTextInfo;
            btnShowPoisOnMap.onClick.RemoveAllListeners();
        }
    }

}
