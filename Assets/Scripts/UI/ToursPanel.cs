using ARPolis.Data;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis.UI
{

    public class ToursPanel : MonoBehaviour
    {

        public Transform prefabTour, containerParent;

        public RectTransform[] rectsForRefresh;

        public Animator animToursPanel;

        public Text txtTopicTitle, txtShortDesc;

        TopicEntity topicEntity;

        public ScrollSnapCustom snapCustom;

        private void Awake()
        {
            snapCustom = containerParent.transform.parent.GetComponent<ScrollSnapCustom>();
            snapCustom.OnSelectionChangeStartEvent.AddListener(OnTourItemPageChanged);

            GlobalActionsUI.OnShowTopicTours += ShowTopicTours;
            GlobalActionsUI.OnHideTopicTours += HideTopicTours;
            GlobalActionsUI.OnLangChanged += ChangeLanguange;

            GlobalActionsUI.OnToggleTarget += RefreshContainer;

            animToursPanel.gameObject.SetActive(false);
        }

        private void OnTourItemPageChanged()
        {
            GlobalActionsUI.OnTourItemPageChanged?.Invoke();
        }

        private void ShowTopicTours()
        {
            if (DestroyPreviousTours()) CreateTours();
            animToursPanel.gameObject.SetActive(true);
            animToursPanel.SetBool("show", true);
            AppManager.Instance.SetMode(AppManager.AppMode.TOUR_SELECTION);

        }

        private void CreateTours()
        {

            topicEntity = InfoManager.Instance.GetActiveTopic();

            if (topicEntity == null) { if (B.isEditor) Debug.Log("Null Topic"); return; }
            if (topicEntity.tours.Count <= 0) { if (B.isEditor) Debug.Log("no tour items"); return; }

            SetTextInfos();
            
            for (int i = 0; i < topicEntity.tours.Count; i++)
            {
                Transform trTour = Instantiate(prefabTour, containerParent);
                TourItem tourItem = trTour.GetComponent<TourItem>();
                tourItem.Init(topicEntity.tours[i]);
                tourItem.pageID = i;
            }

            RefreshContainer(null);
        }

        private bool DestroyPreviousTours()
        {
            //if it is the same topic, no need to destroy
            if (topicEntity == InfoManager.Instance.GetActiveTopic())
            {
                //if (B.isEditor) Debug.Log("same topic selected - keeping items");
                return false;
            }

            TourItem[] items = containerParent.GetComponentsInChildren<TourItem>(true);
            //no tours - we need to create
            if (items.Length <= 0) return true;
            //if it is the same topic, no need to destroy
            if (items[0].topicID == InfoManager.Instance.topicNowID)
            {
                //if (B.isEditor) Debug.Log("same topic - avoid destroying items");
                return false;
            }
            //new topic selected - destroy tours
            foreach (TourItem item in items) item.DestroyItem();

            //reset scroll
            snapCustom.enabled = false;
            snapCustom.RemoveAllChildren();
            Invoke("ResetScrollSnap", 0.1f);

            return true;
        }
            
        void ResetScrollSnap() { snapCustom.enabled = true; }

        private void SetTextInfos()
        {
            if (topicEntity == null) return;
            txtTopicTitle.text = topicEntity.GetTitle();
            txtShortDesc.text = topicEntity.GetDesc();
        }

        private void ChangeLanguange()
        {
            SetTextInfos();
        }

        private void HideTopicTours()
        {
            if (AppManager.Instance.isSideMenuOpen) return;

            GlobalActionsUI.OnShowAreaTopics?.Invoke();
            animToursPanel.SetBool("show", false);
            StartCoroutine(DelayHideTopics());
        }

        private IEnumerator DelayHideTopics()
        {
            yield return new WaitForSeconds(0.75f);
            animToursPanel.gameObject.SetActive(false);
        }

        private void RefreshContainer(GameObject gb)
        {
            Debug.Log("tours panel refresh");
            foreach (RectTransform rt in rectsForRefresh) LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }

    }

}
