using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.Data
{

    [Serializable]
    public class TopicEntity
    {
        public string id, fotofilename;
        public TopicLanguange infoGR, infoEN;
        public string GetTitle()
        {
            if (StaticData.lang == "gr") { return infoGR.title; }
            else
            if (StaticData.lang == "en") { return infoEN.title; }
            return string.Empty;
        }
        public string GetDesc()
        {
            if (StaticData.lang == "gr") { return infoGR.desc; }
            else
            if (StaticData.lang == "en") { return infoEN.desc; }
            return string.Empty;
        }

        public List<TourEntity> tours;

        #region json files - all tours
        //[HideInInspector]
        public List<JsonClassTour> jsonClassTours;
        //[HideInInspector]
        public List<JsonClassPoi> jsonClassPois;
        //[HideInInspector]
        public List<JsonClassTourPoi> jsonClassTourPois;
        //[HideInInspector]
        public List<JsonClassTestimony> jsonClassTestimonies;
        //[HideInInspector]
        public List<JsonClassPerson> jsonClassPersons;
        //[HideInInspector]
        public List<JsonClassDigitalExhibit> jsonClassDigitalExhibits;
        //[HideInInspector]
        public List<JsonClassEvent> jsonClassEvents;
        #endregion


        public void InitTours()
        {
            if (jsonClassTours.Count <= 0) { Debug.Log("No tours found!"); return; }

            tours = new List<TourEntity>();

            for (int i = 0; i < jsonClassTours.Count; i++)
            {
                //skip if topic id is different
                if (jsonClassTours[i].topic_ID != id) continue;//skip

                //skip if id is the same as we extract data from all items 
                //with the same id later
                if (i > 0)
                {
                    if (tours.Count > 0)
                    {
                        if (tours.Exists(b => b.id == jsonClassTours[i].tour_ID))
                        {
                            continue;//skip
                        }
                    }
                }

                TourEntity tour = new TourEntity();
                tour.infoGR = new TourLanguange();
                tour.infoEN = new TourLanguange();

                tour.id = jsonClassTours[i].tour_ID;
                tour.topicID = id;

                //extract info data from all items with the same id
                foreach (JsonClassTour val in jsonClassTours)
                {
                    if (val.tour_ID == tour.id)
                    {
                        if (val.attribute_ID == "1")//Ονομασία Ξενάγησης
                        {
                            tour.infoGR.title = val.attribute_Value;
                        }
                        else if (val.attribute_ID == "3")//Συνοπτική Περιγραφή
                        {
                            tour.infoGR.shortdesc = val.attribute_Value;
                        }
                        else if (val.attribute_ID == "5")//Περιγραφή
                        {
                            tour.infoGR.desc = val.attribute_Value;
                        }
                        else if (val.attribute_ID == "10")//Θέση
                        {
                            tour.thesis = val.attribute_Value;
                        }
                        else if (val.attribute_ID == "2")//Tour Name
                        {
                            tour.infoEN.title = val.attribute_Value;
                        }
                        else if (val.attribute_ID == "4")//Tour Short Description
                        {
                            tour.infoEN.shortdesc = val.attribute_Value;
                        }
                        else if (val.attribute_ID == "6")//Tour Description
                        {
                            tour.infoEN.desc = val.attribute_Value;
                        }
                    }
                }

                tours.Add(tour);

            }

            for (int t = 0; t < tours.Count; t++)
            {

                tours[t].events = new List<EventEntity>();

                //init events
                for (int ev = 0; ev < jsonClassEvents.Count; ev++)
                {

                    //skip if topic id is different
                    if (jsonClassEvents[ev].topic_ID != id && jsonClassEvents[ev].attribute_ID != "20") continue;//skip if not an image

                    //skip if id is the same as we extract data from all items 
                    //with the same id later
                    if (ev > 0)
                    {
                        if (tours[t].events.Count > 0)
                        {
                            if (tours[t].events.Exists(b => b.id == jsonClassEvents[ev].event_ID))
                            {
                                continue;//skip
                            }
                        }
                    }

                    EventEntity eventEntity = new EventEntity();
                    eventEntity.infoGR = new EventLanguange();
                    eventEntity.infoEN = new EventLanguange();

                    eventEntity.id = jsonClassEvents[ev].event_ID;
                    eventEntity.topicID = jsonClassEvents[ev].topic_ID;

                    eventEntity.images = new List<string>();

                    //extract info data from all items with the same id
                    foreach (JsonClassEvent val in jsonClassEvents)
                    {
                        if (val.event_ID == eventEntity.id)
                        {
                            if (val.attribute_ID == "1")//Τίτλος
                            {
                                eventEntity.infoGR.title = val.attribute_Value;
                            }
                            else if (val.attribute_ID == "3")//Συνοπτική Περιγραφή
                            {
                                eventEntity.infoGR.shortdesc = val.attribute_Value;
                            }
                            else if (val.attribute_ID == "2")//Title
                            {
                                eventEntity.infoEN.title = val.attribute_Value;
                            }
                            else if (val.attribute_ID == "4")//Tour Short Description
                            {
                                eventEntity.infoEN.shortdesc = val.attribute_Value;
                            }
                            else if (val.attribute_ID == "20")//Tour Description
                            {
                                eventEntity.images.Add(val.attribute_Value);
                            }
                        }
                    }

                    tours[t].events.Add(eventEntity);
                }
            }

        }

    }

    [Serializable]
    public struct TopicLanguange
    {
        public string title, desc;
    }

}
