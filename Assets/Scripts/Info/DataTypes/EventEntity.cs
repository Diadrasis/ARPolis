using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.Data
{
    public class EventEntity
    {
        public string id, topicID;
        public List<string> images;
        public EventLanguange infoGR, infoEN;

        public string GetTitle()
        {
            if (StaticData.lang == "gr") { return infoGR.title; }
            else
            if (StaticData.lang == "en") { return infoEN.title; }
            return string.Empty;
        }
        public string GetShortDesc()
        {
            if (StaticData.lang == "gr") { return infoGR.shortdesc; }
            else
            if (StaticData.lang == "en") { return infoEN.shortdesc; }
            return string.Empty;
        }
    }

    public struct EventLanguange
    {
        public string title, shortdesc;
    }

}
