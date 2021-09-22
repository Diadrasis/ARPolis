using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.Data
{
    public class TourEntity
    {
        public string id, topicID, thesis;
        public List<string> images, videos, audios;

        public List<TourNarration> narrations = new List<TourNarration>();

        public TourLanguange infoGR, infoEN;

        private TopicEntity topicEntity;

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
        public List<MultimediaObject> digitalExhibitImages;
        public List<MultimediaObject> digitalExhibitAudios;
        public List<MultimediaObject> digitalExhibitVideos;
        public List<MultimediaObject> digitalExhibitNarrations;

        public MultimediaObject GetNarrationLocalized()
        {
                TourNarration nar = narrations.Find(b => b.lang == StaticData.lang);
                return topicEntity.allMultimedia.Find(b => b.id == nar.attributeValue);
        }

        private MultimediaObject GetMultimediaWithID(string id) { return string.IsNullOrWhiteSpace(id) ? null : topicEntity.allMultimedia.Find(b => b.id == id); }

        public void Init(TopicEntity topic)
        {
            topicEntity = topic;
            if (topicEntity == null) { Debug.Log("NULL TOPIC, aborting initialization of tours..."); return; }

            digitalExhibitImages = new List<MultimediaObject>();
            if (images.Count > 0)
            {
                foreach (string s in images)
                {
                    MultimediaObject myImage = GetMultimediaWithID(s);
                    if (myImage != null) digitalExhibitImages.Add(myImage);
                }
            }

            digitalExhibitNarrations = new List<MultimediaObject>();
            if (narrations.Count > 0)
            {
                foreach (TourNarration nar in narrations)
                {
                    MultimediaObject myNarration = GetMultimediaWithID(nar.attributeValue);
                    if (myNarration != null)
                    {
                        digitalExhibitNarrations.Add(myNarration);
                    }
                }
            }

            digitalExhibitVideos = new List<MultimediaObject>();
            if (videos.Count > 0)
            {
                foreach (string s in videos)
                {
                    MultimediaObject myVideo = GetMultimediaWithID(s);
                    if (myVideo != null) digitalExhibitVideos.Add(myVideo);
                }
            }

            digitalExhibitAudios = new List<MultimediaObject>();
            if (audios.Count > 0)
            {
                foreach (string s in audios)
                {
                    MultimediaObject myAudio = GetMultimediaWithID(s);
                    if (myAudio != null) digitalExhibitAudios.Add(myAudio);
                }
            }

            InitPOIs();
        }

        public void InitPOIs()
        {
            foreach(PoiEntity poi in pois)
            {
                if (poi.images != null)
                {
                    poi.digitalExhibitImages = new List<MultimediaObject>();
                    foreach (string s in poi.images)
                    {
                        MultimediaObject myImage = GetMultimediaWithID(s);
                        if (myImage != null) poi.digitalExhibitImages.Add(myImage);
                    }
                }
                if (poi.videos != null)
                {
                    poi.digitalExhibitVideos = new List<MultimediaObject>();
                    foreach (string s in poi.videos)
                    {
                        MultimediaObject myVideo = GetMultimediaWithID(s);
                        if (myVideo != null) poi.digitalExhibitVideos.Add(myVideo);
                    }
                }
                if (poi.testimonies != null)
                {
                    poi.digitalExhibitTestimonies = new List<MultimediaObject>();
                    foreach (string s in poi.testimonies)
                    {
                        MultimediaObject myTestimony = GetMultimediaWithID(s);
                        if (myTestimony != null) poi.digitalExhibitTestimonies.Add(myTestimony);
                    }
                }
            }
        }

        public List<EventEntity> events;

        public void InitEvents()
        {
            
        }
    }

    public struct TourLanguange
    {
        public string title, shortdesc, desc;
    }

    public struct TourNarration
    {
        public string attributeValue, attributeID, lang; 
    }

}
