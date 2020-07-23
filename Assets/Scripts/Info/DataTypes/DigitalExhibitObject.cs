using System;
using UnityEngine;

namespace ARPolis.Data
{

    [Serializable]
    public class DigitalExhibitObject
    {
        public enum Type { NULL, IMAGE, NARRATION, VIDEO }
        public Type type = Type.NULL;
        public int id;
        public Texture2D photo;
        public DiditalExhibitInfoGR infoGR;
        public DiditalExhibitInfoENG infoEng;
        public DiditalExhibitInfoCommon infoCommon;

        public string GetTitle()
        {
            if (StaticData.lang == "gr") { return infoGR.title; }
            else
            if (StaticData.lang == "en") { return infoEng.title; }
            return string.Empty;
        }

        public string GetLabel()
        {
            if (StaticData.lang == "gr") { return infoGR.label; }
            else
            if (StaticData.lang == "en") { return infoEng.label; }
            return string.Empty;
        }

        public string GetSourceLabel()
        {
            if (StaticData.lang == "gr") { return infoGR.sourceLabel; }
            else
            if (StaticData.lang == "en") { return infoEng.sourceLabel; }
            return string.Empty;
        }
    }

    [Serializable]
    public class DiditalExhibitInfoGR
    {
        public string title, label, sourceLabel;
    }

    [Serializable]
    public class DiditalExhibitInfoENG
    {
        public string title, label, sourceLabel;
    }

    [Serializable]
    public class DiditalExhibitInfoCommon
    {
        public string digitalFileName;
    }

    [Serializable]
    public class DigitalExhibitInfoAttribute
    {
        public int id, exhibit_ID, attribute_ID;
        public string attribute_Label, attribute_Value, attribute_Lang;
    }


}