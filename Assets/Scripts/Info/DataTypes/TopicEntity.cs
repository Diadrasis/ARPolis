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

        public List<DigitalExhibitObject> allMultimedia;

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


        public void InitTours(string areaID)
        {
            if (jsonClassTours.Count <= 0) { Debug.Log("No tours found!"); return; }

            #region get topic tours

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
                tour.images = new List<string>();
                tour.narrations = new List<string>();
                tour.videos = new List<string>();
                tour.audios = new List<string>();

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
                        else if (val.attribute_ID == "20")//Images
                        {
                            tour.images.Add(val.attribute_Value);
                        }
                        else if (val.attribute_ID == "21")//Narrations
                        {
                            tour.narrations.Add(val.attribute_Value);
                        }
                    }
                }

                tours.Add(tour);

            }

            #endregion

            #region get infos

            for (int t = 0; t < tours.Count; t++)
            {
                //Debug.Log("tour id = " + tours[t].id);

                #region events

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

                #endregion

                #region tour - pois

                tours[t].tourPoiEntities = new List<TourPoiEntity>();

                for(int tp=0; tp< jsonClassTourPois.Count; tp++)
                {
                    //skip if topic id is different
                    if (jsonClassTourPois[tp].topic_ID != tours[t].topicID && jsonClassTourPois[tp].tour_ID != tours[t].id) continue;

                    TourPoiEntity tourPoiEntity = new TourPoiEntity();
                    tourPoiEntity.topicID = jsonClassTourPois[tp].topic_ID;
                    tourPoiEntity.tourID = jsonClassTourPois[tp].tour_ID;
                    tourPoiEntity.poiID = jsonClassTourPois[tp].poi_ID;

                    tours[t].tourPoiEntities.Add(tourPoiEntity);
                }

                #endregion

                #region pois

                tours[t].pois = new List<PoiEntity>();

                //init pois
                for (int x = 0; x < tours[t].tourPoiEntities.Count; x++)
                {
                    //skip if topic id is different
                    if (tours[t].tourPoiEntities[x].topicID != tours[t].topicID) continue;
                    //skip if tour id is different
                    if (tours[t].tourPoiEntities[x].tourID != tours[t].id) continue;

                    JsonClassPoi jsonClassPoi = jsonClassPois.Find(b => b.poi_ID == tours[t].tourPoiEntities[x].poiID);

                    PoiEntity poiEntity = new PoiEntity();
                    poiEntity.infoGR = new PoiLanguange();
                    poiEntity.infoEN = new PoiLanguange();

                    poiEntity.id = jsonClassPoi.poi_ID;
                    poiEntity.tourID = tours[t].tourPoiEntities[x].tourID;// tours[t].id;
                    poiEntity.topicID = jsonClassPoi.topic_ID;
                    poiEntity.areaID = areaID;

                    poiEntity.images = new List<string>();
                    poiEntity.videos = new List<string>();
                    poiEntity.testimonies = new List<string>();
                    poiEntity.events = new List<string>();

                    foreach (JsonClassPoi val in jsonClassPois)
                    {
                        if (val.poi_ID == poiEntity.id)
                        {
                            if (val.attribute_ID == "1")//Τίτλος
                            {
                                poiEntity.infoGR.name = val.attribute_Value;
                            }
                            else if (val.attribute_ID == "3")//Συνοπτική Περιγραφή
                            {
                                poiEntity.infoGR.shortdesc = val.attribute_Value;
                            }
                            else if (val.attribute_ID == "5")//Ιστορική Ανασκόπηση
                            {
                                poiEntity.infoGR.historicalReview = val.attribute_Value;
                            }
                            else if (val.attribute_ID == "2")//Title
                            {
                                poiEntity.infoEN.name = val.attribute_Value;
                            }
                            else if (val.attribute_ID == "4")//Tour Short Description
                            {
                                poiEntity.infoEN.shortdesc = val.attribute_Value;
                            }
                            else if (val.attribute_ID == "6")//Historical Review
                            {
                                poiEntity.infoEN.historicalReview = val.attribute_Value;
                            }
                            else if ((val.X != 0 && val.Y != 0) || val.attribute_ID == "10")//Position
                            {
                                poiEntity.pos = new Vector2(val.X, val.Y);
                            }
                            else if (val.attribute_ID == "25")//testimonies
                            {
                                poiEntity.testimonies.Add(val.attribute_Value);
                            }
                            else if (val.attribute_ID == "20")// && val.attribute_Label == "Εικόνα")//photos
                            {
                                poiEntity.images.Add(val.attribute_Value);
                            }
                        }
                    }

                    if (poiEntity.pos != Vector2.zero) tours[t].pois.Add(poiEntity);

                }

                #endregion

                #region digital exhibits

                allMultimedia = new List<DigitalExhibitObject>();

                //get files for tour and pois

                List<string> uniqueExhibitIds = new List<string>();

                foreach(JsonClassDigitalExhibit exhibit in jsonClassDigitalExhibits)
                {
                    //skip if topic id is different
                    if (exhibit.topic_ID != id) continue;

                    if (!uniqueExhibitIds.Contains(exhibit.exhibit_ID))
                    {
                        uniqueExhibitIds.Add(exhibit.exhibit_ID);
                    }
                }

                tours[t].digitalExhibitImages = new List<DigitalExhibitObject>();
                tours[t].digitalExhibitNarrations = new List<DigitalExhibitObject>();
                tours[t].digitalExhibitVideos = new List<DigitalExhibitObject>();
                tours[t].digitalExhibitAudios = new List<DigitalExhibitObject>();

                foreach (string uniqueID in uniqueExhibitIds)
                {
                    DigitalExhibitObject digitalExhibit = new DigitalExhibitObject();
                    digitalExhibit.infoGR = new DigitalExhibitInfo();
                    digitalExhibit.infoEng = new DigitalExhibitInfo();
                    digitalExhibit.id = uniqueID;


                    List<JsonClassDigitalExhibit> dglist = jsonClassDigitalExhibits.FindAll(b => b.exhibit_ID == uniqueID);

                    foreach(JsonClassDigitalExhibit dg in dglist)
                    {
                        if(dg.attribute_ID == "1") { digitalExhibit.infoGR.title = dg.attribute_Value; }//Τίτλος
                        else if (dg.attribute_ID == "2") { digitalExhibit.infoEng.title = dg.attribute_Value; }//Title
                        else if (dg.attribute_ID == "3") { digitalExhibit.infoGR.label = dg.attribute_Value; }//Λεζάντα
                        else if (dg.attribute_ID == "4") { digitalExhibit.infoEng.label = dg.attribute_Value; }//Label
                        else if (dg.attribute_ID == "11") { digitalExhibit.sourceLabel = dg.attribute_Value; }//Πηγή
                        else if (dg.attribute_ID == "30") //Αρχείο
                        {
                            digitalExhibit.fileName = dg.attribute_Value;
                        }
                        else if (dg.attribute_ID == "31") //Τύπος Αρχείου
                        {
                            //Εικόνα - Αφήγηση
                            if (dg.attribute_Value == "Εικόνα") { 
                                digitalExhibit.type = DigitalExhibitObject.Type.IMAGE;
                                tours[t].digitalExhibitImages.Add(digitalExhibit);
                            }
                            else if (dg.attribute_Value == "Αφήγηση") { 
                                digitalExhibit.type = DigitalExhibitObject.Type.NARRATION;
                                tours[t].digitalExhibitNarrations.Add(digitalExhibit);
                            }

                        }
                    }

                    allMultimedia.Add(digitalExhibit);
                }

                //set exhibits for pois
                tours[t].InitPOIs(this);


                #endregion
            }

            #endregion

        }


    }

    [Serializable]
    public struct TopicLanguange
    {
        public string title, desc;
    }

}
