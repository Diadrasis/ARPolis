using ARPolis.Data;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis.UI
{

    public class ImageItem : MonoBehaviour
    {

        public RawImage img;
        public AspectRatioFitter ratioFitter;
        public Text txtLabel, txtSource;
        string imageName, topicID;

        public void Init(string name, string topic_ID, string label, string source)
        {
            txtLabel.text = label;
            txtSource.text = source;
            imageName = name;
            topicID = topic_ID;
            //StartCoroutine(LoadImage(name, topic_ID));
            Invoke("DelaySetImage", 0.1f);
        }

        void DelaySetImage() { StartCoroutine(LoadImage(imageName, topicID)); }

        private IEnumerator LoadImage(string filename, string topicID)
        {
            string imageFile = StaticData.folderImages + StaticData.folderAthens + StaticData.FolderTopic(topicID) + filename;
            
            if (InfoManager.Instance.jsonFolder == InfoManager.LoadJsonFolder.RESOURCES)
            {
                //null
            }
            else
            if (InfoManager.Instance.jsonFolder == InfoManager.LoadJsonFolder.STREAMING_ASSETS)
            {
                imageFile = Path.Combine(Application.streamingAssetsPath, imageFile);
                //Debug.Log(imageFile);

                CoroutineWithData cd = new CoroutineWithData(this, StaticData.LoadTextureData<Texture2D>(imageFile));
                yield return cd.Coroutine;
                if (cd.result == null) {
                    img.texture = StaticData.GetImageNull();
                    Debug.LogWarning("Error reading " + imageFile);
                }
                else
                {
                    img.texture = (Texture2D)cd.result;
                }

                ratioFitter.aspectRatio = (float)img.texture.width / (float)img.texture.height;
            }
        }

    }

}
