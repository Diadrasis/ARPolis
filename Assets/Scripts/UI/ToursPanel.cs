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

        public Image logoTour;

        TopicEntity topicEntity;

        public ScrollSnapCustom snapCustom;

        public Color colTopic1, colTopic2, colTopic3, colTopic4;
        public Sprite sprTopic1, sprTopic2, sprTopic3, sprTopic4;

        [SerializeField]
        private string toursLang;

        [SerializeField]
        private Button btnNextTour, btnPrevTour;

        private void Awake()
        {
            snapCustom = containerParent.transform.parent.GetComponent<ScrollSnapCustom>();
            snapCustom.OnSelectionPageChangedEvent.AddListener(OnTourItemPageStartChange);
            snapCustom.OnSelectionChangeEndEvent.AddListener(OnTourItemPageChanged);
            snapCustom.OnSelectionChangeStartEvent.AddListener(OnTourItemPageStartChanging);

            GlobalActionsUI.OnShowTopicTours += ShowTopicTours;
            GlobalActionsUI.OnHideTopicTours += HideTopicTours;
            GlobalActionsUI.OnLangChanged += ChangeLanguange;

            GlobalActionsUI.OnShowPoisOnMap += HideTourPanel;
            GlobalActionsUI.OnShowPoisOnMap += StopRestartSnap;

            GlobalActionsUI.OnLogoutUser += HideTourPanel;

            GlobalActionsUI.OnToggleTarget += RefreshContainer;

            GlobalActionsUI.OnLangChanged += OnLangChanged;

            animToursPanel.gameObject.SetActive(false);
            animToursPanel.speed = 0.5f;

            btnNextTour.onClick.AddListener(AudioManager.Instance.StopAudio);
            btnPrevTour.onClick.AddListener(AudioManager.Instance.StopAudio);
        }

        private void OnLangChanged()
        {
            //toursLang = StaticData.lang;
            DestroyPreviousTours();
        }

        private void OnTourItemPageChanged()
        {
            GlobalActionsUI.OnTourItemPageChanged?.Invoke();
        }

        //GLOBALLY SET CURRENT TOUR ID
        private void OnTourItemPageStartChange(int pageNo)
        {
            InfoManager.Instance.tourNowID = topicEntity.tours[pageNo].id;
        }

        //RESET OTHER TOUR PAGES ELEMENTS POSITIONS
        private void OnTourItemPageChanged(int pageNo)
        {
            GlobalActionsUI.OnTourPageChanged?.Invoke(pageNo);
        }

        private void OnTourItemPageStartChanging()
        {
            AudioManager.Instance.StopAudio();
        }

        private void ShowTopicTours()
        {
            toursLang = StaticData.lang;
            animToursPanel.gameObject.SetActive(true);
            animToursPanel.SetBool("show", true);
            AppManager.Instance.SetMode(AppManager.AppState.TOUR_SELECTION);
            if (DestroyPreviousTours())
            {
                CreateTours();
                snapCustom.enabled = false;
                snapCustom.RemoveAllChildren();
                snapCustom.RestartOnEnable = true;
                Invoke(nameof(ResetScrollSnap), 0.1f);
            }
        }

        public List<TourItem> tourPagesItems = new List<TourItem>();

        private void CreateTours()
        {
            topicEntity = InfoManager.Instance.GetActiveTopic();

            if (topicEntity == null) { if (Application.isEditor) Debug.Log("Null Topic"); return; }
            if (topicEntity.tours.Count <= 0) { if (Application.isEditor) Debug.Log("no tour items"); return; }

            SetTextInfos();
            logoTour.sprite = LogoTopicSprite(topicEntity.id);
            InfoManager.Instance.tourNowID = topicEntity.tours[0].id;

            for (int i = 0; i < topicEntity.tours.Count; i++)
            {
                Transform trTour = Instantiate(prefabTour, containerParent);
                TourItem tourItem = trTour.GetComponent<TourItem>();
                tourItem.Init(topicEntity.tours[i]);
                tourItem.SetTourColor(TitleTopicColor(topicEntity.id));
                tourItem.pageID = i;
                tourPagesItems.Add(tourItem);
            }

            RefreshContainer(null);
        }

        private bool DestroyPreviousTours()
        {
            if (tourPagesItems.Count <= 0) return true;

            if (toursLang != StaticData.lang)
            {
                DestroyAllItems();
                return true;
            }

            //if it is the same topic, no need to destroy
            if (topicEntity == InfoManager.Instance.GetActiveTopic())
            {
                if (Application.isEditor) Debug.Log("same topic selected - keeping items");
                return false;
            }

            //TourItem[] items = containerParent.GetComponentsInChildren<TourItem>(true);
            ////no tours - we need to create
            //if (items.Length <= 0) return true;
            //if it is the same topic, no need to destroy
            if (tourPagesItems[0].topicID == InfoManager.Instance.topicNowID)
            {
                //if (B.isEditor) Debug.Log("same topic - avoid destroying items");
                return false;
            }
            //new topic selected - destroy tours
            //foreach (TourItem item in tourPagesItems) item.DestroyItem();
            DestroyAllItems();

            //reset scroll
            snapCustom.enabled = false;
            snapCustom.RemoveAllChildren();
            Invoke(nameof(ResetScrollSnap), 0.1f);

            return true;
        }

        void DestroyAllItems()
        {
            tourPagesItems.ForEach(b => b.DestroyItem());
            tourPagesItems.Clear();            
            //TourItem[] items = containerParent.GetComponentsInChildren<TourItem>(true);
            //foreach (TourItem item in items) item.DestroyItem();
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
            HideTourPanel();
        }

        void HideTourPanel()
        {
            animToursPanel.SetBool("show", false);
            StartCoroutine(DelayHideTopics());
        }

        //keep tour page when user returns from map
        void StopRestartSnap() { snapCustom.RestartOnEnable = false; }

        private IEnumerator DelayHideTopics()
        {
            yield return new WaitForSeconds(0.75f);
            animToursPanel.gameObject.SetActive(false);
        }

        private void RefreshContainer(GameObject gb)
        {
            //Debug.Log("tours panel refresh");
            foreach (RectTransform rt in rectsForRefresh) LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }

        Sprite LogoTopicSprite(string id)
        {
            switch (id)
            {
                case "1":
                    return sprTopic1;
                case "2":
                    return sprTopic2;
                case "3":
                    return sprTopic3;
                case "4":
                    return sprTopic4;
                default:
                    return sprTopic1;
            }
        }

        Color TitleTopicColor(string id)
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
