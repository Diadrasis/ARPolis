using ARPolis.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis.UI
{

    public class TopicItem : MonoBehaviour
    {
        public RectTransform[] allRects;

        public Image logo;
        public Text txtTitle, txtDesc;
        public Button btnShowTours;

        public TopicLanguange infoGR, infoEN;
        public string areaID, topicID;

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

        private void OnEnable()
        {
            GlobalActionsUI.OnToggleTarget += RefreshContainer;
            GlobalActionsUI.OnLangChanged += SetTextInfo;
            btnShowTours.onClick.AddListener(SelectTopicID);
        }

        void SelectTopicID()
        {
            InfoManager.Instance.topicNowID = topicID;
            GlobalActionsUI.OnShowTopicTours?.Invoke();
        }

        public void SetTextInfo()
        {
            txtTitle.text = GetTitle();
            txtDesc.text = GetDesc();
        }

        void RefreshContainer(GameObject gb)
        {
            foreach (RectTransform rt in allRects) LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }

        public void DestroyItem() { Destroy(gameObject); }

        private void OnDisable()
        {
            GlobalActionsUI.OnToggleTarget -= RefreshContainer;
            GlobalActionsUI.OnLangChanged -= SetTextInfo;
            btnShowTours.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            GlobalActionsUI.OnToggleTarget -= RefreshContainer;
            GlobalActionsUI.OnLangChanged -= SetTextInfo;
            btnShowTours.onClick.RemoveAllListeners();
        }

    }

}
