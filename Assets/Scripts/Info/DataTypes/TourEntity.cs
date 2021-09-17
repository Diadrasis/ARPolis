using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.Data
{

    [Serializable]
    public class TourEntity
    {
        public string id, topicID, thesis, narrationID, soundID;

        public TourLanguange infoGR, infoEN;

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
        public string GetDesc()
        {
            if (StaticData.lang == "gr") { return infoGR.desc; }
            else
            if (StaticData.lang == "en") { return infoEN.desc; }
            return string.Empty;
        }

        public List<TourPoiEntity> tourPoiEntities;//do we need this data???

        public List<PoiEntity> pois;
        public List<DigitalExhibitObject> digitalExhibitImages;
        public List<DigitalExhibitObject> digitalExhibitAudios;
        public List<DigitalExhibitObject> digitalExhibitVideos;
        public List<DigitalExhibitObject> digitalExhibitNarrations;


        public void InitPOIs()
        {
            foreach(PoiEntity poi in pois)
            {
                if (poi.images != null)
                {
                    poi.digitalExhibitImages = new List<DigitalExhibitObject>();
                    foreach (string s in poi.images)
                    {
                        poi.digitalExhibitImages.Add(digitalExhibitImages.Find(b => b.id == s));
                    }
                }
            }
        }

        public List<EventEntity> events;

        public void InitEvents()
        {
            
        }
    }

    [Serializable]
    public struct TourLanguange
    {
        public string title, shortdesc, desc;
    }

}
