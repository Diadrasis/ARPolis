using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.Data
{

    [Serializable]
    public class TourEntity
    {
        public string id, topicID, thesis;
        public List<string> images, videos, audios, narrations;

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
        public List<DigitalExhibitObject> digitalExhibitImages;
        public List<DigitalExhibitObject> digitalExhibitAudios;
        public List<DigitalExhibitObject> digitalExhibitVideos;
        public List<DigitalExhibitObject> digitalExhibitNarrations;

        DigitalExhibitObject GetMultimediaWithID(string id) { return string.IsNullOrWhiteSpace(id) ? null : topicEntity.allMultimedia.Find(b => b.id == id); }

        public void Init(TopicEntity topic)
        {
            topicEntity = topic;
            if (topicEntity == null) { Debug.Log("NULL TOPIC, aborting initialization of tours..."); return; }

            digitalExhibitImages = new List<DigitalExhibitObject>();
            if (images.Count > 0)
            {
                foreach (string s in images)
                {
                    DigitalExhibitObject myImage = GetMultimediaWithID(s);
                    if (myImage != null) digitalExhibitImages.Add(myImage);
                }
            }

            digitalExhibitNarrations = new List<DigitalExhibitObject>();
            if (narrations.Count > 0)
            {
                foreach (string s in narrations)
                {
                    DigitalExhibitObject myNarration = GetMultimediaWithID(s);
                    if (myNarration != null) digitalExhibitNarrations.Add(myNarration);
                }
            }

            digitalExhibitVideos = new List<DigitalExhibitObject>();
            if (videos.Count > 0)
            {
                foreach (string s in videos)
                {
                    DigitalExhibitObject myVideo = GetMultimediaWithID(s);
                    if (myVideo != null) digitalExhibitVideos.Add(myVideo);
                }
            }

            digitalExhibitAudios = new List<DigitalExhibitObject>();
            if (audios.Count > 0)
            {
                foreach (string s in audios)
                {
                    DigitalExhibitObject myAudio = GetMultimediaWithID(s);
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
                    poi.digitalExhibitImages = new List<DigitalExhibitObject>();
                    foreach (string s in poi.images)
                    {
                        DigitalExhibitObject myImage = GetMultimediaWithID(s);
                        if (myImage != null) poi.digitalExhibitImages.Add(myImage);
                    }
                }
                if (poi.videos != null)
                {
                    poi.digitalExhibitVideos = new List<DigitalExhibitObject>();
                    foreach (string s in poi.videos)
                    {
                        DigitalExhibitObject myVideo = GetMultimediaWithID(s);
                        if (myVideo != null) poi.digitalExhibitVideos.Add(myVideo);
                    }
                }
                if (poi.testimonies != null)
                {
                    poi.digitalExhibitTestimonies = new List<DigitalExhibitObject>();
                    foreach (string s in poi.testimonies)
                    {
                        DigitalExhibitObject myTestimony = GetMultimediaWithID(s);
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

    [Serializable]
    public struct TourLanguange
    {
        public string title, shortdesc, desc;
    }

}
