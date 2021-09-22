using ARPolis.Info;
using ARPolis.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis
{

    public class AppSettings : MonoBehaviour
    {

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
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }

    }

}