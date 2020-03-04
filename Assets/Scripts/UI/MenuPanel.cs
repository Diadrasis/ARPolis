﻿using System.Collections;
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
        public GameObject menuPanel, btnPrevCity, btnNextCity, creditsExtraButtonsPanel;
        public ScrollSnapCustom snapCustom;
        public ScrollRect scrollRect;

        public Button btnToggleSite, btnToggleSideMenu, btnCloseSideMenuBehind,
                      btnQuitApp, btnCredits;

        public Sprite sprOnsite, sprOffSite, sprMenuOn, sprMenuOff;

        public delegate void ButtonAction();
        public static ButtonAction OnUserClickOnSiteModeNotAble;

        public PanelTransitionClass panelSideMenuTransition;

        public GameObject[] extraCreditsButtons;
        public Transform arrowCredits;


        private void Awake()
        {
            UIController.OnMenuShow += ShowMenu;

            OnSiteManager.OnGpsOff += SetStatusOffSite;
            OnSiteManager.OnGpsFar += SetStatusOffSite;
            OnSiteManager.OnGpsClose += SetStatusOnSite;

            btnToggleSite.onClick.AddListener(() => ToggleSiteMode());
            btnToggleSideMenu.onClick.AddListener(() => ToggleSideMenu());
            btnCloseSideMenuBehind.onClick.AddListener(() => ToggleSideMenu());
            btnCredits.onClick.AddListener(() => ToggleExtraButtonsCredits());
            btnQuitApp.onClick.AddListener(() => Application.Quit());

            menuPanel.SetActive(false);
        }

        void ToggleExtraButtonsCredits()
        {
            foreach (GameObject gb in extraCreditsButtons) gb.SetActive(false);

            if (!creditsExtraButtonsPanel.activeSelf)
            {
                creditsExtraButtonsPanel.SetActive(true);
                StartCoroutine(DelayShowButtons(extraCreditsButtons, true));
                arrowCredits.localEulerAngles = new Vector3(0f, 0f, -90f);
            }
            else
            {
                StartCoroutine(DelayShowButtons(extraCreditsButtons, false));
                arrowCredits.localEulerAngles = new Vector3(0f, 0f, 0f);
            }
        }

        IEnumerator DelayShowButtons(GameObject[] buttons, bool isOn)
        {
            foreach (GameObject gb in extraCreditsButtons)
            {
                gb.SetActive(isOn);
                yield return new WaitForEndOfFrame();
            }
            creditsExtraButtonsPanel.SetActive(isOn);
            yield break;
        }

        void ToggleSideMenu()
        {
            if (B.isRealEditor) Debug.Log("ToggleSideMenu");

            if (btnToggleSideMenu.image.sprite == sprMenuOff)
            {
                //hide panel
                btnToggleSideMenu.image.sprite = sprMenuOn;
                panelSideMenuTransition.HidePanel();
                btnCloseSideMenuBehind.gameObject.SetActive(false);
            }
            else
            {
                //show panel
                btnToggleSideMenu.image.sprite = sprMenuOff;
                panelSideMenuTransition.ShowPanel();
                btnCloseSideMenuBehind.gameObject.SetActive(true);
            }
        }

        void ToggleSiteMode()
        {
            if (B.isRealEditor) Debug.Log("ToggleSiteMode");

            if(btnToggleSite.image.sprite == sprOffSite)
            {
                if (isAbleToChangeToOnSiteMode)
                {
                    //go on-site
                    btnToggleSite.image.sprite = sprOnsite;
                    ShowButtons(false);
                }
                else
                {
                    OnUserClickOnSiteModeNotAble?.Invoke();
                }
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

        bool isAbleToChangeToOnSiteMode;

        void SetStatusOffSite()
        {
            btnToggleSite.image.sprite = sprOffSite;
            isAbleToChangeToOnSiteMode = false;
            ShowButtons(true);
        }

        void SetStatusOnSite()
        {
            btnToggleSite.image.sprite = sprOnsite;
            isAbleToChangeToOnSiteMode = true;
            ShowButtons(false);
        }

        void ShowMenu()
        {
            menuPanel.SetActive(true);
            animMenuPanel.SetBool("show", true);
        }
    }

}
