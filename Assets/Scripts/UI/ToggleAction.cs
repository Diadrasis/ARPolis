using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.UI
{
    public class ToggleAction : MonoBehaviour
    {
        public GameObject toggleTarget;
        public bool isOn;

        private void OnEnable()
        {
            GlobalActionsUI.OnToggleTarget += OnHideTarget;
        }

        void Start()
        {
            toggleTarget.SetActive(isOn);
        }

        public void ToggleStatus()
        {
            toggleTarget.SetActive(!toggleTarget.activeSelf);

            if (toggleTarget.activeSelf) GlobalActionsUI.OnToggleTarget?.Invoke(toggleTarget);
        }

        void OnHideTarget(GameObject gb)
        {
            if (gb != toggleTarget) toggleTarget.SetActive(false);
        }
    }
}
