using ARPolis.UI;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace ARPolis.Data
{
    public class InfoManager : Singleton<InfoManager>
    {
        protected InfoManager(){}

        #region json storage to retrieve data
        public enum LoadJsonFolder { STREAMING_ASSETS, RESOURCES, CUSTOM }
        public LoadJsonFolder jsonFolder = LoadJsonFolder.STREAMING_ASSETS;

        public enum LoadImagesFolder { STREAMING_ASSETS, RESOURCES, CUSTOM }
        public LoadImagesFolder imagesFolder = LoadImagesFolder.STREAMING_ASSETS;

        public enum LoadAudioFolder { STREAMING_ASSETS, RESOURCES, CUSTOM }
        public LoadAudioFolder audioFolder = LoadAudioFolder.STREAMING_ASSETS;
        #endregion

        public AreaEntity areaAthens;

        public string areaNowID, topicNowID, tourNowID, poiNowID;

        public void Init()
        {
            if (B.isEditor) Debug.LogWarning("Initializing Info Data");

            jsonFolder = LoadJsonFolder.STREAMING_ASSETS;
            StartCoroutine(ReadAllData());
        }

        private IEnumerator ReadAllData()
        {
            yield return new WaitForSeconds(0.5f);

            //Athens - area
            areaAthens = new AreaEntity();
            string folderDataAthens = StaticData.folderJsons + StaticData.folderAthens;

            #region get topics
            string topicsAthens = folderDataAthens + StaticData.jsonTopicsFileURL;
            areaAthens.jsonClassTopics = new List<JsonClassTopic>();
            if (jsonFolder == LoadJsonFolder.RESOURCES)
            {
                areaAthens.jsonClassTopics = StaticData.LoadDataFromJson<JsonClassTopic>(topicsAthens).ToList();
            }
            else
            if (jsonFolder == LoadJsonFolder.STREAMING_ASSETS)
            {
                topicsAthens = Path.Combine(Application.streamingAssetsPath, topicsAthens);
                topicsAthens += ".json";
                CoroutineWithData cd = new CoroutineWithData(this, StaticData.LoadJsonData<JsonClassTopic>(topicsAthens));
                yield return cd.Coroutine;
                if (cd.result == null) { Debug.LogWarning("Error reading " + topicsAthens); }
                else {
                    areaAthens.jsonClassTopics = cd.result as List<JsonClassTopic>;

                    areaAthens.InitTopics();
                }
            }
            yield return new WaitForEndOfFrame();
            #endregion

            #region get jsons for all topics

            if (areaAthens.topics.Count > 0)
            {
                for (int i = 0; i < areaAthens.topics.Count; i++)
                {
                    //topic folder url
                    string topicFolder = folderDataAthens + StaticData.FolderTopic(areaAthens.topics[i].id);


                    #region get tours 
                    //tours file url
                    string topicToursFolder = topicFolder + StaticData.jsonTourFileURL;
                    //create new list
                    areaAthens.topics[i].jsonClassTours = new List<JsonClassTour>();
                    //fill list
                    if (jsonFolder == LoadJsonFolder.RESOURCES)
                    {
                        areaAthens.topics[i].jsonClassTours = StaticData.LoadDataFromJson<JsonClassTour>(topicToursFolder).ToList();
                    }
                    else
                    if (jsonFolder == LoadJsonFolder.STREAMING_ASSETS)
                    {
                        topicToursFolder = Path.Combine(Application.streamingAssetsPath, topicToursFolder);
                        topicToursFolder += ".json";

                        //Debug.Log(areaAthens.topics[i].id + " - "+topicToursFolder);

                        CoroutineWithData cd = new CoroutineWithData(this, StaticData.LoadJsonData<JsonClassTour>(topicToursFolder));
                        yield return cd.Coroutine;
                        if (cd.result == null) { Debug.LogWarning("Error reading " + topicToursFolder); }
                        else
                        {
                            areaAthens.topics[i].jsonClassTours = cd.result as List<JsonClassTour>;

                            //areaAthens.topics[i].InitTours();
                        }
                    }

                    #endregion

                    #region get pois 

                    //pois file url
                    string topicPoisFile = topicFolder + StaticData.jsonPOIsFileURL;
                    //create new list
                    areaAthens.topics[i].jsonClassPois = new List<JsonClassPoi>();
                    //fill list
                    if (jsonFolder == LoadJsonFolder.RESOURCES)
                    {
                        areaAthens.topics[i].jsonClassPois = StaticData.LoadDataFromJson<JsonClassPoi>(topicPoisFile).ToList();
                    }
                    else
                    if (jsonFolder == LoadJsonFolder.STREAMING_ASSETS)
                    {
                        topicPoisFile = Path.Combine(Application.streamingAssetsPath, topicPoisFile);
                        topicPoisFile += ".json";

                        CoroutineWithData cd = new CoroutineWithData(this, StaticData.LoadJsonData<JsonClassPoi>(topicPoisFile));
                        yield return cd.Coroutine;
                        if (cd.result == null) { Debug.LogWarning("Error reading " + topicPoisFile); }
                        else
                        {
                            areaAthens.topics[i].jsonClassPois = cd.result as List<JsonClassPoi>;
                        }
                    }

                    #endregion

                    #region get tour-pois connection

                    //pois file url
                    string topicTourPoiFile = topicFolder + StaticData.jsonTourPoiFileURL;
                    //create new list
                    areaAthens.topics[i].jsonClassTourPois = new List<JsonClassTourPoi>();
                    //fill list
                    if (jsonFolder == LoadJsonFolder.RESOURCES)
                    {
                        areaAthens.topics[i].jsonClassTourPois = StaticData.LoadDataFromJson<JsonClassTourPoi>(topicTourPoiFile).ToList();
                    }
                    else
                    if (jsonFolder == LoadJsonFolder.STREAMING_ASSETS)
                    {
                        topicTourPoiFile = Path.Combine(Application.streamingAssetsPath, topicTourPoiFile);
                        topicTourPoiFile += ".json";

                        CoroutineWithData cd = new CoroutineWithData(this, StaticData.LoadJsonData<JsonClassTourPoi>(topicTourPoiFile));
                        yield return cd.Coroutine;
                        if (cd.result == null) { Debug.LogWarning("Error reading " + topicTourPoiFile); }
                        else
                        {
                            areaAthens.topics[i].jsonClassTourPois = cd.result as List<JsonClassTourPoi>;
                        }
                    }

                    #endregion

                    #region get testimonies

                    //pois file url
                    string topicTestimoniesFile = topicFolder + StaticData.jsonTestimoniesFileURL;
                    //create new list
                    areaAthens.topics[i].jsonClassTestimonies = new List<JsonClassTestimony>();
                    //fill list
                    if (jsonFolder == LoadJsonFolder.RESOURCES)
                    {
                        areaAthens.topics[i].jsonClassTestimonies = StaticData.LoadDataFromJson<JsonClassTestimony>(topicTestimoniesFile).ToList();
                    }
                    else
                    if (jsonFolder == LoadJsonFolder.STREAMING_ASSETS)
                    {
                        topicTestimoniesFile = Path.Combine(Application.streamingAssetsPath, topicTestimoniesFile);
                        topicTestimoniesFile += ".json";

                        CoroutineWithData cd = new CoroutineWithData(this, StaticData.LoadJsonData<JsonClassTestimony>(topicTestimoniesFile));
                        yield return cd.Coroutine;
                        if (cd.result == null) { Debug.LogWarning("Error reading " + topicTestimoniesFile); }
                        else
                        {
                            areaAthens.topics[i].jsonClassTestimonies = cd.result as List<JsonClassTestimony>;
                        }
                    }

                    #endregion

                    #region get persons

                    //pois file url
                    string topicPersonsFile = topicFolder + StaticData.jsonPersonsFileURL;
                    //create new list
                    areaAthens.topics[i].jsonClassPersons = new List<JsonClassPerson>();
                    //fill list
                    if (jsonFolder == LoadJsonFolder.RESOURCES)
                    {
                        areaAthens.topics[i].jsonClassPersons = StaticData.LoadDataFromJson<JsonClassPerson>(topicPersonsFile).ToList();
                    }
                    else
                    if (jsonFolder == LoadJsonFolder.STREAMING_ASSETS)
                    {
                        topicPersonsFile = Path.Combine(Application.streamingAssetsPath, topicPersonsFile);
                        topicPersonsFile += ".json";

                        CoroutineWithData cd = new CoroutineWithData(this, StaticData.LoadJsonData<JsonClassPerson>(topicPersonsFile));
                        yield return cd.Coroutine;
                        if (cd.result == null) { Debug.LogWarning("Error reading " + topicPersonsFile); }
                        else
                        {
                            areaAthens.topics[i].jsonClassPersons = cd.result as List<JsonClassPerson>;
                        }
                    }

                    #endregion

                    #region get digital exhibits

                    //pois file url
                    string topicDigitalExhibitsFile = topicFolder + StaticData.jsonDigitalExhibitsFileURL;
                    //create new list
                    areaAthens.topics[i].jsonClassDigitalExhibits = new List<JsonClassDigitalExhibit>();
                    //fill list
                    if (jsonFolder == LoadJsonFolder.RESOURCES)
                    {
                        areaAthens.topics[i].jsonClassDigitalExhibits = StaticData.LoadDataFromJson<JsonClassDigitalExhibit>(topicDigitalExhibitsFile).ToList();
                    }
                    else
                    if (jsonFolder == LoadJsonFolder.STREAMING_ASSETS)
                    {
                        topicDigitalExhibitsFile = Path.Combine(Application.streamingAssetsPath, topicDigitalExhibitsFile);
                        topicDigitalExhibitsFile += ".json";

                        CoroutineWithData cd = new CoroutineWithData(this, StaticData.LoadJsonData<JsonClassDigitalExhibit>(topicDigitalExhibitsFile));
                        yield return cd.Coroutine;
                        if (cd.result == null) { Debug.LogWarning("Error reading " + topicDigitalExhibitsFile); }
                        else
                        {
                            areaAthens.topics[i].jsonClassDigitalExhibits = cd.result as List<JsonClassDigitalExhibit>;
                        }
                    }

                    #endregion

                    #region get events

                    //pois file url
                    string topicEventsFile = topicFolder + StaticData.jsonEventsFileURL;
                    //create new list
                    areaAthens.topics[i].jsonClassEvents = new List<JsonClassEvent>();
                    //fill list
                    if (jsonFolder == LoadJsonFolder.RESOURCES)
                    {
                        areaAthens.topics[i].jsonClassEvents = StaticData.LoadDataFromJson<JsonClassEvent>(topicEventsFile).ToList();
                    }
                    else
                    if (jsonFolder == LoadJsonFolder.STREAMING_ASSETS)
                    {
                        topicEventsFile = Path.Combine(Application.streamingAssetsPath, topicEventsFile);
                        topicEventsFile += ".json";

                        CoroutineWithData cd = new CoroutineWithData(this, StaticData.LoadJsonData<JsonClassEvent>(topicEventsFile));
                        yield return cd.Coroutine;
                        if (cd.result == null) { Debug.LogWarning("Error reading " + topicEventsFile); }
                        else
                        {
                            areaAthens.topics[i].jsonClassEvents = cd.result as List<JsonClassEvent>;
                        }
                    }

                    #endregion

                    areaAthens.topics[i].InitTours();
                }

                
                yield return new WaitForEndOfFrame();
            }
            #endregion

            
            //jsonClassTourPois = StaticData.LoadDataFromJson<JsonClassTourPoi>(StaticData.jsonTourPoiFileURL).ToList();
            //yield return new WaitForEndOfFrame();
            //jsonClassPoiTestimonies = StaticData.LoadDataFromJson<JsonClassPoiTestimony>(StaticData.jsonPoiTestimonyFileURL).ToList();
            //yield return new WaitForEndOfFrame();
            
            //jsonClassTestimonies = StaticData.LoadDataFromJson<JsonClassTestimony>(StaticData.jsonTestimoniesFileURL).ToList();
            //yield return new WaitForEndOfFrame();
            //jsonClassPois = StaticData.LoadDataFromJson<JsonClassPoi>(StaticData.jsonPOIsFileURL).ToList();
            //yield return new WaitForEndOfFrame();
            //jsonClassPersons = StaticData.LoadDataFromJson<JsonClassPerson>(StaticData.jsonPersonsFileURL).ToList();
            //yield return new WaitForEndOfFrame();
            //jsonClassDigitalExhibits = StaticData.LoadDataFromJson<JsonClassDigitalExhibit>(StaticData.jsonDigitalExhibitsFileURL).ToList();
            //yield return new WaitForEndOfFrame();

            InitApp.OnDataLoaded?.Invoke();

            yield break;
        }

        string FixJsonItems(string value)
        {
            value = "{\"Items\":" + value + "}";
            return value;
        }


        #region Copy Data
        /*

        IEnumerator Start()
        {

            if (infoManager.jsonFolder == PathInfoManager.LoadJsonFolder.STREAMING_ASSETS)
            {
                //digital exhibits
                jsonDigitalExhibitFileUrl = Path.Combine(Application.streamingAssetsPath, StaticDataUI.jsonDigitalExhibitsFileName);
                jsonDigitalExhibitFileUrl += ".json";
            }
            else
            if (infoManager.jsonFolder == PathInfoManager.LoadJsonFolder.RESOURCES)
            {
                jsonDigitalExhibitFileUrl = StaticDataUI.jsonDigitalExhibitsFileName;
                jsonDigitalExhibitFileUrl += ".json";
            }

            CoroutineWithData cd = new CoroutineWithData(this, GetDigitalExhibitInfo());
            yield return cd.Coroutine;
            if (cd.result.ToString() == "fail")
            {
                Debug.LogWarning("Error reading " + jsonDigitalExhibitFileUrl);
            }
            else
            {
                yield return new WaitForEndOfFrame();
                if (digitExhibitInfos.Length > 0)
                {
                    //get all ids 
                    uniqueExhibitIds = new List<int>();
                    foreach (DigitalExhibitInfoAttribute digitalExhibit in digitExhibitInfos)
                    {
                        if (!uniqueExhibitIds.Contains(digitalExhibit.exhibit_ID))
                        {
                            uniqueExhibitIds.Add(digitalExhibit.exhibit_ID);
                        }
                    }
                    yield return new WaitForEndOfFrame();
                    GroupIDs();
                }
            }
            yield break;
        }

        IEnumerator GetDigitalExhibitInfo()
        {
            if (infoManager.jsonFolder == PathInfoManager.LoadJsonFolder.RESOURCES)
            {
                string jsonDigDatas = FixJsonItems(StaticDataUI.LoadResourceTextfile(jsonDigitalExhibitFileUrl));
                if (string.IsNullOrEmpty(jsonDigDatas))
                {
                    Debug.Log("error reading " + jsonDigitalExhibitFileUrl);
                    yield return "fail";
                }
                else
                {
                    digitExhibitInfos = JsonHelper.FromJson<DigitalExhibitInfoAttribute>(jsonDigDatas);

                    yield return "success";
                }
            }
            else if (infoManager.jsonFolder == PathInfoManager.LoadJsonFolder.STREAMING_ASSETS)
            {
                UnityWebRequest wwwDigitalExhibit = UnityWebRequest.Get(jsonDigitalExhibitFileUrl);
                yield return wwwDigitalExhibit.SendWebRequest();

                if (wwwDigitalExhibit.isNetworkError || wwwDigitalExhibit.isHttpError)
                {
                    Debug.Log(wwwDigitalExhibit.error);
                    yield return "fail";
                }
                else
                {
                    string jsonDigitalDatas = FixJsonItems(wwwDigitalExhibit.downloadHandler.text);

                    digitExhibitInfos = JsonHelper.FromJson<DigitalExhibitInfoAttribute>(jsonDigitalDatas);

                    yield return "success";
                }
            }
            yield break;
        }

        */
        #endregion

    }

}
