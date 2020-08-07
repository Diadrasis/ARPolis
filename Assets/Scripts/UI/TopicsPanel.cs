using ARPolis.Data;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis.UI
{

    public class TopicsPanel : MonoBehaviour
    {
        public Transform prefabTopic, containerParent;

        public RectTransform[] rectsForRefresh;

        public Animator animTopicsPanel;

        public Text txtAreaName, txtShortDesc;

        public Color colTopic1, colTopic2, colTopic3, colTopic4;
        public Sprite sprTopic1, sprTopic2, sprTopic3, sprTopic4;

        AreaEntity areaEntity;
        string topicsLang;

        private void Awake()
        {
            GlobalActionsUI.OnShowAreaTopics += ShowAreaTopics;
            GlobalActionsUI.OnHideAreaTopics += HideAreaTopics;

            GlobalActionsUI.OnToggleTarget += RefreshContainer;
            GlobalActionsUI.OnLangChanged += ChangeLanguange;

            GlobalActionsUI.OnShowPoisOnMap += HideTopicsPanel;
            GlobalActionsUI.OnLogoutUser += HideTopicsPanel;

            animTopicsPanel.gameObject.SetActive(false);
        }

        

        void ShowAreaTopics()
        {
            if (DestroyPreviousTopics()) CreateTopics();
            topicsLang = StaticData.lang;
            animTopicsPanel.gameObject.SetActive(true);
            animTopicsPanel.SetBool("show", true);
            AppManager.Instance.SetMode(AppManager.AppMode.TOPIC_SELECTION);

        }
        private void CreateTopics()
        {

            areaEntity = InfoManager.Instance.GetActiveArea();

            if (areaEntity == null || areaEntity.topics == null) { if (B.isEditor) Debug.Log("Null Area"); return; }
            if (areaEntity.topics.Count <= 0) { if (B.isEditor) Debug.Log("no topic items"); return; }

            SetTextInfos();

            for (int i=0; i<areaEntity.topics.Count; i++)
            {
                Transform trTopic = Instantiate(prefabTopic, containerParent);
                TopicItem topicItem = trTopic.GetComponent<TopicItem>();
                topicItem.infoGR = areaEntity.topics[i].infoGR;
                topicItem.infoEN = areaEntity.topics[i].infoEN;
                topicItem.topicID = areaEntity.topics[i].id;
                topicItem.SetTopicInfo();
                topicItem.SetTitlteColor(TitleTopicColor(i));
                topicItem.SetLogo(LogoTopicSprite(i));
                topicItem.SetNumberOfTours(InfoManager.Instance.GetTopicToursLength(i).ToString());
            }
        }

        Sprite LogoTopicSprite(int indx)
        {
            switch (indx)
            {
                case 0:
                    return sprTopic1;
                case 1:
                    return sprTopic2;
                case 2:
                    return sprTopic3;
                case 3:
                    return sprTopic4;
                default:
                    return sprTopic1;
            }
        }

        Color TitleTopicColor(int indx)
        {
            switch (indx)
            {
                case 0:
                    return colTopic1;
                case 1:
                    return colTopic2;
                case 2:
                    return colTopic3;
                case 3:
                    return colTopic4;
                default:
                    return colTopic1;
            }
        }

        private bool DestroyPreviousTopics()
        {
            if(topicsLang != StaticData.lang)
            {
                DestroyAllItems();
                return true;
            }

            if (areaEntity == InfoManager.Instance.GetActiveArea())
            {
                //if (B.isEditor) Debug.Log("same area selected - keeping items");
                return false;
            }

            TopicItem[] items = containerParent.GetComponentsInChildren<TopicItem>(true);
            if (items.Length <= 0)
            {
                //if (B.isEditor) Debug.Log("no topic items to destroy");
                return true;
            }
            if (items[0].areaID == InfoManager.Instance.areaNowID)
            {
                //if (B.isEditor) Debug.Log("same area - avoid destroying items");
                return false;
            }
            foreach (TopicItem item in items) item.DestroyItem();
            return true;
        }

        void DestroyAllItems()
        {
            TopicItem[] items = containerParent.GetComponentsInChildren<TopicItem>(true);
            foreach (TopicItem item in items) item.DestroyItem();
        }

        private void SetTextInfos()
        {
            if (areaEntity == null) return;
            txtAreaName.text = areaEntity.GetName();
            txtShortDesc.text = areaEntity.GetDesc();
        }

        private void ChangeLanguange()
        {
            SetTextInfos();
        }

        void HideAreaTopics()
        {
            if (AppManager.Instance.isSideMenuOpen)  return;

            GlobalActionsUI.OnShowMenuAreas?.Invoke();
            HideTopicsPanel();
        }

        void HideTopicsPanel()
        {
            animTopicsPanel.SetBool("show", false);
            StartCoroutine(DelayHideTopics());
        }

        IEnumerator DelayHideTopics()
        {
            yield return new WaitForSeconds(0.75f);
            animTopicsPanel.gameObject.SetActive(false);
        }

        void RefreshContainer(GameObject gb)
        {
            foreach (RectTransform rt in rectsForRefresh) LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }

    }

}
