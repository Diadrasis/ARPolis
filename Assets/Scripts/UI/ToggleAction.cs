using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis.UI
{
    public class ToggleAction : MonoBehaviour
    {
        public RectTransform rectParent;
        public GameObject toggleTarget;
        public bool isOn;

        private void OnEnable()
        {
            GlobalActionsUI.OnToggleTarget += OnHideTarget;
            GlobalActionsUI.OnToggleHideAll += HideFast;
        }

        void Start()
        {
            toggleTarget.SetActive(isOn);
        }

        public void ToggleStatus()
        {
            toggleTarget.SetActive(!toggleTarget.activeSelf);
            RefreshContainer();

            if (toggleTarget.activeSelf)
            {
                GlobalActionsUI.OnToggleTarget?.Invoke(toggleTarget);
            }
        }

        void OnHideTarget(GameObject gb)
        {
            if (gb != toggleTarget)
            {
                toggleTarget.SetActive(false);
                RefreshContainer();
            }
        }
        void HideFast()
        {
            if (!toggleTarget.activeSelf) return;
            toggleTarget.SetActive(false);
            RefreshContainer();
        }
        void RefreshContainer()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectParent);
        }

        private void OnDisable()
        {
            GlobalActionsUI.OnToggleTarget -= OnHideTarget;
            GlobalActionsUI.OnToggleHideAll -= HideFast;
        }

        private void OnDestroy()
        {
            GlobalActionsUI.OnToggleTarget -= OnHideTarget;
            GlobalActionsUI.OnToggleHideAll -= HideFast;
        }

    }
}
