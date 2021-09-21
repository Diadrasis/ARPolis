using ARPolis.Data;
using ARPolis.Info;
using StaGeUnityTools;
using System.Collections;
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
        string imageName;
        public string sourceURL, topicID;

        public AutoFitToCanvas fitImage, fitLabel, fitSource; 

        public void InitNull()
        {
            btnSourceUrl.gameObject.SetActive(false);
            fitLabel.gameObject.SetActive(false);
            fitImage.heightPercent = 100f;
            img.texture = StaticData.GetImageEmpty();
            ratioFitter.aspectRatio = (float)img.texture.width / (float)img.texture.height;
            fitImage.ManualDelayInit();
        }

        public void Init(string name, string topic_ID, string label, string source)
        {
            txtLabel.text = label;
            sourceURL = source;
            imageName = name;
            topicID = topic_ID;
            if (string.IsNullOrEmpty(sourceURL))
            {
                btnSourceUrl.gameObject.SetActive(false);
                if (!string.IsNullOrEmpty(label))
                {
                    fitImage.heightPercent = 70f;
                    fitLabel.heightPercent = 30f;
                    fitLabel.ManualDelayInit();
                }
                else
                {
                    fitLabel.gameObject.SetActive(false);
                    fitImage.heightPercent = 100f;
                }

                fitImage.ManualDelayInit();


                btnSourceUrl.interactable = false;
                txtSource.GetComponent<AutoSetLanguange>().enabled = false;
                txtSource.text = sourceURL;

            }
            else
            {
                btnSourceUrl.onClick.AddListener(OpenSourceURL);
            }
            Invoke(nameof(DelaySetImage), 0.1f);
        }

        private void OpenSourceURL()
        {
            Application.OpenURL(sourceURL);
        }

        void DelaySetImage() { StartCoroutine(LoadImage(imageName, topicID)); }

        private IEnumerator LoadImage(string filename, string topicID)
        {
            string imageFile = StaticData.folderImages + StaticData.folderAthens + StaticData.FolderTopic(topicID) + filename;
            
            if (InfoManager.Instance.jsonFolder == InfoManager.LoadJsonFolder.RESOURCES)
            {
                //null for now
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
                    Debug.LogWarning("Error reading "+ filename+ " from " + imageFile);
                }
                else
                {
                    img.texture = (Texture2D)cd.result;
                }

                ratioFitter.aspectRatio = (float)img.texture.width / (float)img.texture.height;
            }
        }

        public void DestroyItem() { Destroy(gameObject); }

    }

}
