using ARPolis.Data;
using ARPolis.Info;
using ARPolis.Map;
using ARPolis.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis
{

    public class AppSettings : Singleton<AppSettings>
    {
        protected AppSettings() { }

        public GameObject[] gpsItems;
        public TMPro.TMP_Dropdown dropDownNavigation;

        public Slider sliderOnSiteDistance, sliderTriggerPoiDistance;
        public TMPro.TextMeshProUGUI txtOnSiteDistance, txtTriggerPoiDistance;
        private readonly string _space = " ";

        private void Awake()
        {
            sliderOnSiteDistance.minValue = 0.5f;
            sliderOnSiteDistance.maxValue = 10f;
            sliderOnSiteDistance.wholeNumbers = false;
            sliderOnSiteDistance.onValueChanged.AddListener((b) => OnSiteDistanceChanged());
            if (PlayerPrefs.HasKey("OnSiteDistance"))
            {
                OnSiteManager.Instance.maxKmDistanceForOnSiteMode = PlayerPrefs.GetFloat("OnSiteDistance");
            }
            sliderOnSiteDistance.value = OnSiteManager.Instance.maxKmDistanceForOnSiteMode;

            sliderTriggerPoiDistance.minValue = 10f;
            sliderTriggerPoiDistance.maxValue = 200f;
            sliderTriggerPoiDistance.wholeNumbers = false;
            sliderTriggerPoiDistance.onValueChanged.AddListener((b) => OnTriggerPoiDistanceChanged());

            if (PlayerPrefs.HasKey("TriggerPoiDistance"))
            {
                OnSiteManager.Instance.triggerPoiDist = PlayerPrefs.GetFloat("TriggerPoiDistance");
            }
            sliderTriggerPoiDistance.value = OnSiteManager.Instance.triggerPoiDist;

            AppData.OnDataReaded += ChangeLanguange;

            dropDownNavigation.onValueChanged.AddListener((b)=>SelectNavigationMode());
        }

        void SelectNavigationMode()
        {
            switch (dropDownNavigation.value)
            {
                case 0:
                    //off-site
                    AppManager.Instance.SetNavigationMode(AppManager.NavigationMode.OFF_SITE);
                    gpsItems.ToList().ForEach(b => b.SetActive(false));
                    MenuPanel.Instance.ShowArrowSelectAreasButtons(true);
                    break;
                case 1:
                    //on-site
                    AppManager.Instance.SetNavigationMode(AppManager.NavigationMode.ON_SITE);
                    gpsItems.ToList().ForEach(b => b.SetActive(true));
                    MenuPanel.Instance.ShowArrowSelectAreasButtons(false);
                    break;
                case 2:
                    //on-site + AR
                    AppManager.Instance.SetNavigationMode(AppManager.NavigationMode.ON_SITE_AR);
                    gpsItems.ToList().ForEach(b => b.SetActive(true));
                    MenuPanel.Instance.ShowArrowSelectAreasButtons(false);
                    break;
                default:
                    //off-site
                    AppManager.Instance.SetNavigationMode(AppManager.NavigationMode.OFF_SITE);
                    gpsItems.ToList().ForEach(b => b.SetActive(false));
                    MenuPanel.Instance.ShowArrowSelectAreasButtons(true);
                    break;
            }
        }

        public void CheckDropDownNavigation(int val)
        {
            dropDownNavigation.value = val;            
            SelectNavigationMode();
        }

        public void CheckOptions()
        {
            Debug.Log("CheckOptions");
            var toggles = dropDownNavigation.GetComponentsInChildren<Toggle>(true);
            Toggle arToggle = toggles.ToList().Find(b => b.name.StartsWith("Item 2"));
            if (arToggle)
            {
                arToggle.interactable = ARManager.Instance.arMode == ARManager.ARMode.SUPPORT;
            }
        }

        private void OnSiteDistanceChanged()
        {
            txtOnSiteDistance.text = sliderOnSiteDistance.value.ToString("F1") + _space + AppData.Instance.FindTermValue("km");
            PlayerPrefs.SetFloat("OnSiteDistance", sliderOnSiteDistance.value);
            OnSiteManager.Instance.maxKmDistanceForOnSiteMode = sliderOnSiteDistance.value;
        }

        private void OnTriggerPoiDistanceChanged()
        {
            txtTriggerPoiDistance.text = sliderTriggerPoiDistance.value.ToString("F1") +_space + AppData.Instance.FindTermValue("meters");
            PlayerPrefs.SetFloat("TriggerPoiDistance", sliderTriggerPoiDistance.value);
            OnSiteManager.Instance.triggerPoiDist = sliderTriggerPoiDistance.value;
        }

        void ChangeLanguange()
        {
            txtOnSiteDistance.text = sliderOnSiteDistance.value.ToString("F1") + _space + AppData.Instance.FindTermValue("km");
            txtTriggerPoiDistance.text = sliderTriggerPoiDistance.value.ToString("F1") + _space + AppData.Instance.FindTermValue("meters");

            
            dropDownNavigation.options[0].text = AppData.Instance.FindTermValue(StaticData.navModeOffsite);
            dropDownNavigation.options[1].text = AppData.Instance.FindTermValue(StaticData.navModeOnsite);
            dropDownNavigation.options[2].text = AppData.Instance.FindTermValue(StaticData.navModeOnsiteAR);
            dropDownNavigation.captionText.text = dropDownNavigation.options[dropDownNavigation.value].text;

        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }

    }

}