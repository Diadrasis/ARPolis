using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.UI
{

    public class GlobalActionsUI : MonoBehaviour
    {

        public delegate void ActionUI();
        public static ActionUI OnPanelSignUpCancel, OnSignUpSubmit;

        public delegate void ActionObjectlUI(GameObject gb);
        public static ActionObjectlUI OnToggleTarget;

        public delegate void ActionPoi(string poiID, string topicID);
        public static ActionPoi OnPoiSelected;

    }

}