using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.Data
{
    [Serializable]
    public class AreaEntity
    {
        public string id, fotofilename;
        public AreaLanguange infoGR, infoEN;
        public string GetName()
        {
            if (StaticData.lang == "gr") { return infoGR.name; }
            else
            if (StaticData.lang == "en") { return infoEN.name; }
            return string.Empty;
        }
        public string GetDesc()
        {
            if (StaticData.lang == "gr") { return infoGR.desc; }
            else
            if (StaticData.lang == "en") { return infoEN.desc; }
            return string.Empty;
        }

        public List<TopicEntity> topics;

        [HideInInspector]
        public List<JsonClassTopic> jsonClassTopics;

        public void InitTopics()
        {
            if (jsonClassTopics.Count <= 0) { Debug.Log("No topics found!"); return; }

            topics = new List<TopicEntity>();

            for (int i=0; i< jsonClassTopics.Count; i++)
            {
                if (i > 0)
                {
                    if (topics.Count > 0)
                    {
                        if(topics.Exists(b => b.id == jsonClassTopics[i].topic_ID))
                        {
                            continue;//skip
                        }
                    }
                }

                TopicEntity topic = new TopicEntity();
                topic.infoGR = new TopicLanguange();
                topic.infoEN = new TopicLanguange();

                topic.id = jsonClassTopics[i].topic_ID;

                foreach(JsonClassTopic val in jsonClassTopics)
                {
                    if(val.topic_ID == topic.id)
                    {
                        if(val.attribute_ID == "1")//Ονομασία Ενότητας
                        {
                            topic.infoGR.title = val.attribute_Value;
                        }
                        else if (val.attribute_ID == "3")//Περιγραφή Ενόητας
                        {
                            topic.infoGR.desc = val.attribute_Value;
                        }
                        else if (val.attribute_ID == "2")//Topic Name
                        {
                            topic.infoEN.title = val.attribute_Value;
                        }
                        else if (val.attribute_ID == "4")//Topic Description
                        {
                            topic.infoEN.desc = val.attribute_Value;
                        }
                    }
                }

                topics.Add(topic);

            }
        }

    }

    [Serializable]
    public struct AreaLanguange
    {
        public string name, desc;
    }

}
