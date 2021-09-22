using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace ARPolis.Data
{

    public class StaticData
    {
        public static int isPoiInfoVisible = 0;

        public static string lang = "gr";

        public static bool IsLangGR() { return lang == "gr"; }

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

        public static readonly string surveyIntro = "surveyIntro";
        public static readonly string surveyOutro = "surveyOutro";
        public static readonly string signInUserPlaceholder = "signInUserPlaceholder";
        public static readonly string signInPasswordPlaceholder = "signInPasswordPlaceholder";
        public static readonly string btnLoginSubmit = "btnLoginSubmit";
        public static readonly string toggleRememberUser = "toggleRememberUser";
        public static readonly string btnShowSignUp = "btnShowSignUp";
        public static readonly string btnLoginAnonymous = "btnLoginAnonymous";
        public static readonly string signUpPasswordCheck = "signUpPasswordCheck";
        public static readonly string btnSignUpSubmit = "btnSignUpSubmit";

        public static readonly string userNotFound = "userNotFound"; 
        public static readonly string wrongPassword = "wrongPassword";
        public static readonly string userAlreadyExists = "userAlreadyExists";
        public static readonly string signUpInput = "signUpInput";
        public static readonly string btnSaveSurvey = "btnSaveSurvey";

        public static readonly string signUpComplete = "signUpComplete";

        //AR related messages        
        public static readonly string msgARCheckSupport = "msgARCheckSupport";
        public static readonly string msgARSupported = "msgARSupported";
        public static readonly string msgARNeedsUpdate = "msgARNeedsUpdate";
        public static readonly string msgARUpdateFailed = "msgARUpdateFailed";
        public static readonly string msgARNotSupported = "msgARNotSupported";
        
        #endregion

        #region JSONS

        //areas
        public static readonly string folderAthens = "athens/";
        //topics
        public static string FolderTopic(string id) { return "topic_" + id + "/"; }

        public static string GetNarrationsTopicPath(string topicID)
        {
            string pathFolderNarrations = Path.Combine(Application.streamingAssetsPath, StaticData.folderNarrations);
            pathFolderNarrations = Path.Combine(pathFolderNarrations, StaticData.folderAthens);
            pathFolderNarrations = Path.Combine(pathFolderNarrations, StaticData.FolderTopic(topicID));
            return pathFolderNarrations;
        }

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
            string jsonDigDatas = LoadResourceTextfile(jsonFileName);

            if (string.IsNullOrWhiteSpace(jsonDigDatas))
            {
                Debug.Log("error reading " + jsonFileName);
                return null;
            }
            else
            {
                jsonDigDatas = FixJsonItems(jsonDigDatas);
                if (jsonDigDatas.Length < 10)
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

        private static bool ISiOS()
        {
            return Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer;
        }

        public static IEnumerator LoadJsonData<T>(string filename)//, List<T> t)
        {
            if (ISiOS())
            {
                FileInfo fInfo = new FileInfo(filename);
                if (fInfo == null || !fInfo.Exists)
                {
                    Debug.Log("NOT FOUND: " + filename);
                    yield return null;
                }
                else
                {
                    string myjsondata = File.ReadAllText(fInfo.FullName);
                    if (myjsondata.Length > 10)
                    {
                        myjsondata = FixJsonItems(myjsondata);
                        yield return JsonHelper.FromJson<T>(myjsondata).ToList();
                    }
                    else
                    {
                        Debug.Log("empty - NOT FOUND: " + filename);
                        yield return null;
                    }
                }
            }
            else {

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
        }

        public static IEnumerator LoadTextureData<T>(string filename)//, List<T> t)
        {
            byte[] imgData;
            Texture2D tex = new Texture2D(2, 2);

            if (ISiOS())
            {
                yield return LoadTexture(filename);
            }
            else
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
                    imgData = www.downloadHandler.data;
                    //Load raw Data into Texture2D 
                    tex.LoadImage(imgData);
                    yield return tex;
                }
            }
        }

        private static string LoadResourceTextfile(string filename)
        {
            string filePath = Path.Combine(folderJsons, filename);//.Replace(".json", "");

            TextAsset targetFile = Resources.Load<TextAsset>(filePath);

            if (targetFile == null)
            {
                Debug.LogWarning(filename + " is missing!!!");
                return string.Empty;
            }
            return targetFile.text;
        }

        private static string FixJsonItems(string value)
        {
            value = "{\"Items\":" + value + "}";
            return value;
        }

        /// <summary>
        /// Load a PNG or JPG file from disk to a Texture2D
        /// Returns null if load fails
        /// </summary>
        static Texture2D LoadTexture(string FilePath)
        {

            Texture2D Tex2D;
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
                if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                    return Tex2D;                 // If data = readable -> return texture
            }
#if UNITY_EDITOR
            Debug.LogWarning("image from " + FilePath + " is null");
#endif

            return null;                     // Return null if load failed
        }

        #endregion

    }

}
