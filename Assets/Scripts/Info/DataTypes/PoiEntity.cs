using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.Data
{
    public class PoiEntity
    {
        public string id, tourID, topicID, areaID, narrationID, soundID;
        public List<string> images, videos, testimonies, events;
        public Vector2 pos;
        public PoiLanguange infoGR, infoEN;

        public List<MultimediaObject> digitalExhibitImages;
        public List<MultimediaObject> digitalExhibitAudios;
        public List<MultimediaObject> digitalExhibitVideos;
        public List<MultimediaObject> digitalExhibitTestimonies;

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

    public struct PoiLanguange
    {
        public string name, shortdesc, historicalReview;
    }

}
