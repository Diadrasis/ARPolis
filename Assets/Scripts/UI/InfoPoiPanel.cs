using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis.UI
{

    public class InfoPoiPanel : MonoBehaviour
    {

        public Text txtTitle, txtDesc, txtShortDesc;
        public Transform rectImagesContainer;
        public Transform prefabImage;
        public PanelTransitionClass transitionClass;
        public Button btnArrow;
        public Transform trArrow;

        private void Awake()
        {
            GlobalActionsUI.OnPoiSelected += OnSelectPoi;
        }

        private void Start()
        {

        }

        private void OnSelectPoi(string poiID, string topicID)
        {
            //get poi data

            //get images

            //get testimony

            //show panel
            ShowPreviewInfo();
        }

        private void ShowPreviewInfo() { ShowButtonArrow(true); transitionClass.ShowPercentagePanel(); }

        private void ShowFullInfo() { ShowButtonArrow(true); transitionClass.ShowPanel(); }

        private void HideInfo() { ShowButtonArrow(false); transitionClass.HidePanel(); }

        private void ShowButtonArrow(bool val) { btnArrow.gameObject.SetActive(val); }
    }

}
