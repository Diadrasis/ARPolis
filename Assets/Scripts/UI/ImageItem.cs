using ARPolis.Data;
using ARPolis.Info;
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
        public Button btnSourceUrl;
        public RawImage img;
        public AspectRatioFitter ratioFitter;
        public Text txtLabel, txtSource;
        string imageName, topicID;
        public string sourceURL;

        public void Init(string name, string topic_ID, string label, string source)
        {
            txtLabel.text = label;
            sourceURL = source;
            imageName = name;
            topicID = topic_ID;
            btnSourceUrl.onClick.AddListener(OpenSourceURL);
            //StartCoroutine(LoadImage(name, topic_ID));
            Invoke("DelaySetImage", 0.1f);
        }

        void OpenSourceURL()
        {
            if (string.IsNullOrEmpty(sourceURL)) {
                btnSourceUrl.interactable = false;
                txtSource.GetComponent<AutoSetLanguange>().enabled = false;
                txtSource.text = sourceURL;
                return;
            }
            //if (!sourceURL.StartsWith("htpp://") || !sourceURL.StartsWith("htpps://")) { btnSourceUrl.interactable = false; return; }

            Application.OpenURL(sourceURL);
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
