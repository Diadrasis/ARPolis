using System;
using UnityEngine;

namespace ARPolis.Data
{

    [Serializable]
    public class DigitalExhibitObject
    {
        public enum Type { NULL, IMAGE, NARRATION, VIDEO, SOUND, TESTIMONY }
        public Type type = Type.NULL;
        public DigitalExhibitInfo infoGR, infoEng;
        public string id, fileName, sourceLabel;

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
    }

    [Serializable]
    public class DigitalExhibitInfo
    {
        public string title, label;
    }
}