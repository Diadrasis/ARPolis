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

        public void Init(string name, string topicID, string label, string source)
        {
            txtLabel.text = label;
            txtSource.text = source;
            StartCoroutine(LoadImage(name, topicID));
        }

        private IEnumerator LoadImage(string filename, string topicID)
        {
            string imageFile = StaticData.folderImages + StaticData.folderAthens + StaticData.FolderTopic(topicID) + filename;
            
            if (InfoManager.Instance.jsonFolder == InfoManager.LoadJsonFolder.RESOURCES)
            {
                
            }
            else
            if (InfoManager.Instance.jsonFolder == InfoManager.LoadJsonFolder.STREAMING_ASSETS)
            {
                imageFile = Path.Combine(Application.streamingAssetsPath, imageFile);

//                if(!imageFile.EndsWith)

                CoroutineWithData cd = new CoroutineWithData(this, StaticData.LoadJsonData<JsonClassTopic>(imageFile));
                yield return cd.Coroutine;
                if (cd.result == null) { Debug.LogWarning("Error reading " + imageFile); }
                else
                {
                    img.texture = cd.result as Texture2D;

                    ratioFitter.aspectRatio = (float)img.texture.width / (float)img.texture.height;

                }
            }
        }

    }

}
