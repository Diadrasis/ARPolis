using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.Data
{

    [Serializable]
    public class PoiEntity
    {
        public string id, tourID, topicID, areaID, narrationID, soundID;
        public List<string> images, videos, testimonies, events;
        public Vector2 pos;
        public PoiLanguange infoGR, infoEN;

        public List<DigitalExhibitObject> digitalExhibitImages;
        public List<DigitalExhibitObject> digitalExhibitAudios;
        public List<DigitalExhibitObject> digitalExhibitVideos;
        public List<DigitalExhibitObject> digitalExhibitNarrations;

        public string GetTitle()
        {
            if (StaticData.lang == "gr") { return infoGR.name; }
            else
            if (StaticData.lang == "en") { return infoEN.name; }
            return string.Empty;
        }
        public string GetShortDesc()
        {
            if (StaticData.lang == "gr") { return infoGR.shortdesc; }
            else
            if (StaticData.lang == "en") { return infoEN.shortdesc; }
            return string.Empty;
        }
        public string GetHistoricalReview()
        {
            if (StaticData.lang == "gr") { return infoGR.historicalReview; }
            else
            if (StaticData.lang == "en") { return infoEN.historicalReview; }
            return string.Empty;
        }

        public bool Exists() { return !string.IsNullOrWhiteSpace(id); }

    }

    [Serializable]
    public struct PoiLanguange
    {
        public string name, shortdesc, historicalReview;
    }

}
