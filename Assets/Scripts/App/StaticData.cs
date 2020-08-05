using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace ARPolis.Data
{

    public class StaticData
    {
        public static int isPoiInfoVisible = 0;

        public static string lang = "gr";

        private static readonly string imageNull = "images/image_null";
        private static readonly string imageEmpty = "images/image_empty";
        public static Texture2D GetImageNull() { return Resources.Load<Texture2D>(imageNull); }
        public static Texture2D GetImageEmpty() { return Resources.Load<Texture2D>(imageEmpty); }

        private static readonly string poiFolder = "poiIcons/";
        public static Texture2D GetPoiIcon(string topicID) { return Resources.Load<Texture2D>(poiFolder + topicID); }

        #region terms

        public static readonly string termGpsOffTitle = "msgMenuGpsOffTitle";
        public static readonly string termGpsOffDesc = "msgMenuGpsOffDesc";
        public static readonly string termBtnEnableGps = "msgMenuGpsOffOpenSettings";
        public static readonly string termBtnOK = "term_OK";
        public static readonly string termBtnCancel = "term_Cancel";
        public static readonly string termGpsFarTitle = "msgMenuGpsFarTitle";
        public static readonly string termGpsFarDesc = "msgMenuGpsFarDesc";
        public static readonly string termGpsNearTitle = "msgMenuGpsNearTitle";
        public static readonly string termGpsNearDesc = "msgMenuGpsNearDesc";
        public static readonly string termGpsInsideTitle = "msgMenuGpsInsideTitle";
        public static readonly string termGpsInsideDesc = "msgMenuGpsIndideDesc";

        public static readonly string termQuitAppTitle = "msgQuitAppTitle";
        public static readonly string termQuitAppDesc = "msgQuitAppDesc";

        public static readonly string termLogoutTitle = "msgLogoutTitle";
        public static readonly string termLogoutDesc = "msgLogoutDesc";

        public static readonly string termPasswordsMismatch = "passwordsMismatch";
        public static readonly string termUsernameEmptyField = "usernameEmptyField";
        public static readonly string termPasswordEmptyField = "passwordEmptyField";

        #endregion

        #region JSONS

        //areas
        public static readonly string folderAthens = "athens/";
        //topics
        public static string FolderTopic(string id) { return "topic_" + id + "/"; }

        //files folders
        public static readonly string folderNarrations = "narrations/";
        public static readonly string folderModels = "3D/";
        public static readonly string folderImages = "images/";
        public static readonly string folderAudio = "audio/";
        public static readonly string folderVideos = "videos/";
        public static readonly string folderJsons = "data/";

        //json files - common names for all area & topics
        public static readonly string jsonTopicsFileURL = "exportTopics";
        public static readonly string jsonDigitalExhibitsFileURL = "exportDigitalExhibits";
        public static readonly string jsonPersonsFileURL = "exportPersons";
        public static readonly string jsonPOIsFileURL = "exportPOI";
        public static readonly string jsonTestimoniesFileURL = "exportTestimony";
        public static readonly string jsonTourFileURL = "exportTour";
        public static readonly string jsonPoiTestimonyFileURL = "PoiTestimony";
        public static readonly string jsonTourPoiFileURL = "TourPoi";
        public static readonly string jsonEventsFileURL = "exportEvents";

        /// <summary>
        /// Loads list of objects reading a json file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonFileName"></param>
        /// <returns></returns>
        public static T[] LoadDataFromJson<T>(string jsonFileName)
        {
            string jsonDigDatas = FixJsonItems(LoadResourceTextfile(jsonFileName));

            if (string.IsNullOrEmpty(jsonDigDatas))
            {
                Debug.Log("error reading " + jsonFileName);
                return null;
            }
            else
            {
                if(jsonDigDatas.Length < 10)
                {
                    Debug.Log("error on file " + jsonFileName);
                    return null;
                }
                else
                {
                    return JsonHelper.FromJson<T>(jsonDigDatas);
                }
            }
        }

        public static IEnumerator LoadJsonData<T>(string filename)//, List<T> t)
        {
            UnityWebRequest www = UnityWebRequest.Get(filename);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                yield return null;
            }
            else
            {
                string jsonData = www.downloadHandler.text;
                if (string.IsNullOrEmpty(jsonData) || jsonData.Length < 10)
                {
                    yield return null;
                }
                else
                {
                    jsonData = FixJsonItems(www.downloadHandler.text);
                    yield return JsonHelper.FromJson<T>(jsonData).ToList();
                }
            }
        }

        public static IEnumerator LoadTextureData<T>(string filename)//, List<T> t)
        {
            byte[] imgData;
            Texture2D tex = new Texture2D(2, 2);

            UnityWebRequest www = UnityWebRequest.Get(filename);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                yield return null;
            }
            else
            {
                imgData = www.downloadHandler.data;
                //Load raw Data into Texture2D 
                tex.LoadImage(imgData);
                yield return tex;
            }
        }

        private static string LoadResourceTextfile(string filename)
        {
            string filePath = folderJsons + filename;//.Replace(".json", "");

            TextAsset targetFile = Resources.Load<TextAsset>(filePath);

            if (targetFile == null)
            {
                Debug.LogWarning(filename + ".json is missing!!!");
                return string.Empty;
            }
            return targetFile.text;
        }

        private static string FixJsonItems(string value)
        {
            value = "{\"Items\":" + value + "}";
            return value;
        }

        #endregion

    }

}
