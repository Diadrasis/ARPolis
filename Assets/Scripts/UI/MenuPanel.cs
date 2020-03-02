using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ARPolis.Map;
using StaGeUnityTools;

namespace ARPolis.UI
{

    public class MenuPanel : MonoBehaviour
    {
        public Animator animMenuPanel;
        public GameObject menuPanel, btnPrevCity, btnNextCity;
        public ScrollSnapCustom snapCustom;
        public ScrollRect scrollRect;

        public Button btnToggleSite;

        public Sprite sprOnsite, sprOffSite;

        private void Awake()
        {
            UIController.OnMenuShow += ShowMenu;

            OnSiteManager.OnGpsOff += SetStatusOffSite;
            OnSiteManager.OnGpsFar += SetStatusOffSite;
            OnSiteManager.OnGpsClose += SetStatusOnSite;

            btnToggleSite.onClick.AddListener(() => ToggleSiteMode());

            menuPanel.SetActive(false);
        }

        void ToggleSiteMode()
        {
            if (B.isEditor) Debug.Log("ToggleSiteMode");

            if(btnToggleSite.image.sprite == sprOffSite)
            {
                //go on-site
                btnToggleSite.image.sprite = sprOnsite;
                ShowButtons(false);
            }
            else
            {
                //go off-site
                btnToggleSite.image.sprite = sprOffSite;
                ShowButtons(true);
            }
        }

        void ShowButtons(bool val)
        {
            scrollRect.enabled = val;
            snapCustom.Init();
            snapCustom.SetFirstPage();
            snapCustom.enabled = val;
            if (val == false)
            {
                btnNextCity.SetActive(val);
                btnPrevCity.SetActive(val);
            }
        }

        void SetStatusOffSite()
        {
            btnToggleSite.image.sprite = sprOffSite;
            btnToggleSite.interactable = false;
            ShowButtons(true);
        }

        void SetStatusOnSite()
        {
            btnToggleSite.image.sprite = sprOnsite;
            btnToggleSite.interactable = true;
            ShowButtons(false);
        }

        void ShowMenu()
        {
            menuPanel.SetActive(true);
            animMenuPanel.SetBool("show", true);
        }
    }

}
