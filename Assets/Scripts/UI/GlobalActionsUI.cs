﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.UI
{

    public class GlobalActionsUI : MonoBehaviour
    {

        public delegate void ActionUI();
        public static ActionUI OnLangChanged, OnPanelSignUpCancel, OnSignUpSubmit, OnToggleHideAll, OnToggleRefreshUI;

        public delegate void ActionObjectlUI(GameObject gb);
        public static ActionObjectlUI OnToggleTarget;

        public delegate void ActionPoi(string poiID, string topicID);
        public static ActionPoi OnPoiSelected;

        public delegate void PageSnapAction(int pageId);
        public static PageSnapAction OnTourPageChanged;

        public delegate void UIAction();
        public static UIAction OnIntroStart, OnIntroFinished, 
                               OnShowMenuAreas, OnHideMenuAreas,
                               OnShowAreaTopics, OnHideAreaTopics, 
                               OnShowTopicTours, OnHideTopicTours,
                               OnTourItemPageChanged,
                               OnShowPoisOnMap, OnHideMap,
                               OnLoginShow, OnLogoutUser,
                               OnInfoAreaShow, OnInfoPoiShow, OnMessageHide;

    }

}