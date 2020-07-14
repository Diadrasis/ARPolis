using System;

namespace ARPolis.Data
{
    [Serializable]
    public class JsonClassPoi
    {
        public float X, Y;
        public string id, poi_ID, topic_ID, attribute_ID, attribute_Lang, attribute_Label, attribute_Value;
    }
}