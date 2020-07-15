using ARPolis.UI;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace ARPolis.Data
{

    public class InfoManager : Singleton<InfoManager>
    {
        protected InfoManager(){}

        public List<JsonClassTourPoi> jsonClassTourPois;
        public List<JsonClassPoiTestimony> jsonClassPoiTestimonies;
        public List<JsonClassTour> jsonClassTours;
        public List<JsonClassTestimony> jsonClassTestimonies;
        public List<JsonClassPoi> jsonClassPois;
        public List<JsonClassPerson> jsonClassPersons;
        public List<JsonClassDigitalExhibit> jsonClassDigitalExhibits;

        public string thematicNowID, tourNowID, poiNowID;

        public void Init()
        {
            if (B.isEditor) Debug.LogWarning("Initializing Info Data");
            StartCoroutine(ReadAllData());
        }

        private IEnumerator ReadAllData()
        {
            yield return new WaitForSeconds(2f); 
            jsonClassTourPois = StaticData.LoadDataFromJson<JsonClassTourPoi>(StaticData.jsonTourPoiFileURL).ToList();
            yield return new WaitForEndOfFrame();
            jsonClassPoiTestimonies = StaticData.LoadDataFromJson<JsonClassPoiTestimony>(StaticData.jsonPoiTestimonyFileURL).ToList();
            yield return new WaitForEndOfFrame();
            jsonClassTours = StaticData.LoadDataFromJson<JsonClassTour>(StaticData.jsonTourFileURL).ToList();
            yield return new WaitForEndOfFrame();
            jsonClassTestimonies = StaticData.LoadDataFromJson<JsonClassTestimony>(StaticData.jsonTestimoniesFileURL).ToList();
            yield return new WaitForEndOfFrame();
            jsonClassPois = StaticData.LoadDataFromJson<JsonClassPoi>(StaticData.jsonPOIsFileURL).ToList();
            yield return new WaitForEndOfFrame();
            jsonClassPersons = StaticData.LoadDataFromJson<JsonClassPerson>(StaticData.jsonPersonsFileURL).ToList();
            yield return new WaitForEndOfFrame();
            jsonClassDigitalExhibits = StaticData.LoadDataFromJson<JsonClassDigitalExhibit>(StaticData.jsonDigitalExhibitsFileURL).ToList();
            yield return new WaitForEndOfFrame();

            //testing
            if (B.isEditor)
            {
                Debug.Log("Search for topic 1 - tour 2 - total pois");
                List<JsonClassTourPoi> tourPois = jsonClassTourPois.FindAll(b => b.topic_ID == "1" && b.tour_ID == "2");
                Debug.Log("total tourPois = " + tourPois.Count);
                List<JsonClassPoi> pois = new List<JsonClassPoi>();
                foreach (JsonClassTourPoi t in tourPois)// foreach(JsonClassTourPoi t in tourPois) jsonClassPois.FindAll(b=> b.poi_ID)
                {
                    //JsonClassPois p = jsonClassPois.Find(b => b.poi_ID== t.poi_ID);
                    pois.Add(jsonClassPois.Find(b => b.poi_ID == t.poi_ID));// && b.attribute_ID=="1"));
                }
                Debug.Log("total Pois = " + pois.Count);
                Debug.Log("pois[0] greek name = " + pois.Find(p => p.poi_ID == pois[0].poi_ID && p.attribute_Lang == "gr").attribute_Value);
                //Debug.Log("pois[0] english name = " + pois.Find(p => p.poi_ID == pois[0].poi_ID && p.attribute_Lang == "en").attribute_Value);
            }

            yield break;
        }


        
    }

}
