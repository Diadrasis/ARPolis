using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis.Map
{

    public class CustomMarkerGUI : MonoBehaviour
    {
        /// <summary>
        /// Longitude
        /// </summary>
        public double lng;

        /// <summary>
        /// Latitude
        /// </summary>
        public double lat;

        /// <summary>
        /// Reference to the TextField
        /// </summary>
        public Text textField;

        private string _text;

        /// <summary>
        /// Gets / sets the marker text
        /// </summary>
        public string text
        {
            get { return _text; }
            set
            {
                if (textField != null) textField.text = value;
                txt = GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (txt != null) txt.text = value;
                _text = value;
            }
        }

        public void SetPosition(Vector2 pos) { lng = pos.x; lat = pos.y; }

        public Vector2 pos;

        public Button btnMarker;
        public Image icon;
        public TMPro.TextMeshProUGUI txt;

        public void Init()
        {
            icon = GetComponent<Image>();
            btnMarker = GetComponent<Button>();
            pos = new Vector2((float)lng, (float)lat);
            txt = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }

        /// <summary>
        /// Disposes the marker
        /// </summary>
        public void Dispose()
        {
            textField = null;
        }
    }

}
