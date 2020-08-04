using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace ARPolis.Data
{

    public class StaticData
    {

        public static string lang = "gr";

        public static string imageNull = "images/image_null";
        public static Texture2D GetImageNull() { return Resources.Load<Texture2D>(imageNull); }

        #region terms

        public static string termGpsOffTitle = "msgMenuGpsOffTitle";
        public static string termGpsOffDesc = "msgMenuGpsOffDesc";
        public static string termBtnEnableGps = "msgMenuGpsOffOpenSettings";
        public static string termBtnOK = "term_OK";
        public static string termBtnCancel = "term_Cancel";
        public static string termGpsFarTitle = "msgMenuGpsFarTitle";
        public static string termGpsFarDesc = "msgMenuGpsFarDesc";
        public static string termGpsNearTitle = "msgMenuGpsNearTitle";
        public static string termGpsNearDesc = "msgMenuGpsNearDesc";
        public static string termGpsInsideTitle = "msgMenuGpsInsideTitle";
        public static string termGpsInsideDesc = "msgMenuGpsIndideDesc";

        public static string termQuitAppTitle = "msgQuitAppTitle";
        public static string termQuitAppDesc = "msgQuitAppDesc";

        public static string termLogoutTitle = "msgLogoutTitle";
        public static string termLogoutDesc = "msgLogoutDesc";

        public static string termPasswordsMismatch = "passwordsMismatch";
        public static string termUsernameEmptyField = "usernameEmptyField";
        public static string termPasswordEmptyField = "passwordEmptyField";

        #endregion

        #region JSONS

        //areas
        public static string folderAthens = "athens/";
        //topics
        public static string FolderTopic(string id) { return "topic_" + id + "/"; }

        //files folders
        public static string folderNarrations = "narrations/";
        public static string folderModels = "3D/";
        public static string folderImages = "images/";
        public static string folderAudio = "audio/";
        public static string folderVideos = "videos/";
        public static string folderJsons = "data/";

        //json files - common names for all area & topics
        public static string jsonTopicsFileURL = "exportTopics";
        public static string jsonDigitalExhibitsFileURL = "exportDigitalExhibits";
        public static string jsonPersonsFileURL = "exportPersons";
        public static string jsonPOIsFileURL = "exportPOI";
        public static string jsonTestimoniesFileURL = "exportTestimony";
        public static string jsonTourFileURL = "exportTour";
        public static string jsonPoiTestimonyFileURL = "PoiTestimony";
        public static string jsonTourPoiFileURL = "TourPoi";
        public static string jsonEventsFileURL = "exportEvents";

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
