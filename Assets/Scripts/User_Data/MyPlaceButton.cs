using ARPolis.Data;
using ARPolis.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis
{
    public class MyPlaceButton : MonoBehaviour
    {
        public Button btn;
        public TMPro.TextMeshProUGUI txt;
        public Image icon;
        private UserPlaceItem userPlaceItem;
        private PoiEntity poi;

        public void Init(UserPlaceItem item)
        {
            if (!UserPlacesManager.Instance.IsPlaceItemExists(item))
            {
                DestroyItem();
                return;
            }
            userPlaceItem = item;
            poi = InfoManager.Instance.GetPoiEntityFromMenu(userPlaceItem.poiID, userPlaceItem.tourID, userPlaceItem.topicID, userPlaceItem.areaID);
            if (!poi.Exists())
            {
                DestroyItem();
                return;
            }
            icon.color = InfoPoiPanel.Instance.TopicColor(poi.topicID);
            //btn action
            btn.onClick.AddListener(ShowPoiInfo);
            ChangeLanguage();
        }

        void ShowPoiInfo()
        {
            if (AppManager.Instance.IsGpsInUse())
            {
                if (!AppManager.Instance.AllowPlaceClickWhileOnSite)
                {
                    Debug.Log("[AppManager] AllowPlaceClickWhileOnSite = false");
                    return;
                }
            }
            GlobalActionsUI.OnMyPlaceSelected?.Invoke(poi);
        }

        void ChangeLanguage()
        {
            txt.text = poi.GetTitle();
        }

        public void DestroyItem() { Destroy(gameObject); }

        private void OnEnable()
        {
            GlobalActionsUI.OnLangChanged += ChangeLanguage;
        }

        private void OnDestroy()
        {
            GlobalActionsUI.OnLangChanged -= ChangeLanguage;
            btn.onClick.RemoveAllListeners();
        }

    }

}
