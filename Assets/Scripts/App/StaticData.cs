using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.Data
{

    public class StaticData
    {

        public static string lang = "gr";

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


        #region JSONS

        public static string folderImages = "images/";
        public static string folderJsons = "jsons/";

        public static string jsonDigitalExhibitsFileURL = "exportDigitalExhibits";
        public static string jsonPersonsFileURL = "exportPersons";
        public static string jsonPOIsFileURL = "exportPOI";
        public static string jsonTestimoniesFileURL = "exportTestimony";
        public static string jsonTourFileURL = "exportTour";
        public static string jsonPoiTestimonyFileURL = "PoiTestimony";
        public static string jsonTourPoiFileURL = "TourPoi";

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
                return default(T[]);
            }
            else
            {
                return JsonHelper.FromJson<T>(jsonDigDatas);

            }
        }

        private static string LoadResourceTextfile(string path)
        {

            string filePath = folderJsons + path;//.Replace(".json", "");

            TextAsset targetFile = Resources.Load<TextAsset>(filePath);

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
