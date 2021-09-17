using ARPolis.Data;
using ARPolis.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ARPolis
{

    public class UserPlacesManager : Singleton<UserPlacesManager>
    {
        protected UserPlacesManager() { }

        public List<UserPlaceItem> userPlaces = new List<UserPlaceItem>();

        private readonly string prefsFavKey = "my_places";

        private void OnEnable()
        {
            GlobalActionsUI.OnLoginAnonymous += GetUserPlaces;
            GlobalActionsUI.OnUserLoggedIn += GetUserPlaces;
            GlobalActionsUI.OnLogoutUser += OnLogoutUser;
        }

        public void Init()
        {
            //just create an instance

            if (Application.isEditor) Debug.LogWarning("[UserPlacesManager] Init");
        }

        void GetUserPlaces()
        {
            ClearMyPlaces();

            string mySaveKey = prefsFavKey + AppManager.Instance.GetUserSaveKey();
            Debug.Log(mySaveKey);

            List<string> allPlaces = PlayerPrefsX.GetStringArray(mySaveKey).ToList();
            foreach (string s in allPlaces) userPlaces.Add(JsonUtility.FromJson<UserPlaceItem>(s));

            MenuPanel.Instance.CreateListOnUI(userPlaces);
        }

        void OnLogoutUser()
        {
            ClearMyPlaces();
        }

        private void ClearMyPlaces()
        {
            MenuPanel.Instance.DestroyListFromUI(userPlaces);
            userPlaces.Clear();
        }


        public bool SaveCurrentPoi()
        {
            if (InfoManager.Instance.IsUserPlaceItemNowExists(out UserPlaceItem userPlace)) {
                if (!IsThisPlaceSaved(userPlace))
                {
                    string mySaveKey = prefsFavKey + AppManager.Instance.GetUserSaveKey();

                    if(Application.isEditor) Debug.Log(mySaveKey);

                    string jsonData = JsonUtility.ToJson(userPlace);
                    if (PlayerPrefs.HasKey(mySaveKey))
                    {
                        List<string> allPlaces = PlayerPrefsX.GetStringArray(mySaveKey).ToList(); 
                        if (allPlaces.Contains(jsonData)) return false;
                        allPlaces.Add(jsonData);
                        PlayerPrefsX.SetStringArray(mySaveKey, allPlaces.ToArray());
                        PlayerPrefs.Save();
                        RecreateList();
                        return true;
                    }
                    else
                    {
                        PlayerPrefsX.SetStringArray(mySaveKey, new string[] { jsonData });
                        PlayerPrefs.Save();
                        RecreateList();
                        return true;
                    }
                }
            }

            return false;
        }

        public bool DeleteCurrentPoi()
        {
            if (InfoManager.Instance.IsUserPlaceItemNowExists(out UserPlaceItem userPlace))
            {
                if (!IsThisPlaceSaved(userPlace)) return false;
                string mySaveKey = prefsFavKey + AppManager.Instance.GetUserSaveKey();

                if (Application.isEditor) Debug.Log(mySaveKey);

                string jsonData = JsonUtility.ToJson(userPlace);
                if (PlayerPrefs.HasKey(mySaveKey))
                {
                    List<string> allPlaces = PlayerPrefsX.GetStringArray(mySaveKey).ToList();
                    if (!allPlaces.Contains(jsonData)) return false;
                    allPlaces.Remove(jsonData);
                    PlayerPrefsX.SetStringArray(mySaveKey, allPlaces.ToArray());
                    PlayerPrefs.Save();
                    RecreateList();
                    return true;
                }
            }
            return false;
        }

        void RecreateList()
        {
            ClearMyPlaces();
            GetUserPlaces();
        }

        public bool IsThisPoiSaved(string poiID)
        {
            return userPlaces.FindAll(b => b.poiID == poiID).Count > 0;
        }


        public bool IsThisPlaceSaved(UserPlaceItem placeItem)
        {
            return userPlaces.Find(b => b.areaID == placeItem.areaID && b.topicID == placeItem.topicID
                                   && b.tourID == placeItem.tourID && b.poiID == placeItem.poiID) != null; 
        }

        public bool IsPlaceItemExists(UserPlaceItem item)
        {
            return item != null && !string.IsNullOrWhiteSpace(item.areaID)
                   && !string.IsNullOrWhiteSpace(item.topicID)
                   && !string.IsNullOrWhiteSpace(item.tourID)
                   && !string.IsNullOrWhiteSpace(item.poiID);
        }


        private void OnDisable()
        {
            GlobalActionsUI.OnUserLoggedIn -= GetUserPlaces;
            GlobalActionsUI.OnLoginAnonymous -= GetUserPlaces;
            GlobalActionsUI.OnLogoutUser -= OnLogoutUser;
        }

    }

}
