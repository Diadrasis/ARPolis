﻿using ARPolis.Data;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis.UI
{

    public class PoiItem : MonoBehaviour
    {

        public string poiID;
        RawImage img;

        public void Init()
        {
            Button btn = GetComponent<Button>();
            img = GetComponent<RawImage>();

            if (btn)
            {
                //SaveAsVisited("west", poiInfo.poiID, btn, false);
                btn.onClick.AddListener(() => GlobalActionsUI.OnPoiSelected?.Invoke(poiID));
                //if (saveVisitedPlaces) btn.onClick.AddListener(() => SaveAsVisited("west", poiInfo.poiID, btn, true));
            }
        }

        public void SetImage(Texture2D spr) { if (img) img.texture = spr; }

    }

}
