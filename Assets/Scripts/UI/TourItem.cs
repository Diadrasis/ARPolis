using ARPolis.Data;
using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis.UI
{

    public class TourItem : MonoBehaviour
    {
        TourEntity tourEntity;

        public string tourID, topicID;
        public TourLanguange infoGR, infoEN;

        public Text txtTitle, txtDesc, txtShortText;

        public Button btnShowPoisOnMap;

        public RectTransform[] allRects;

        public RectTransform rectScrollContainer, rectScrollImages;
        public int pageID;

        public Transform imageItemPrefab;

        public GameObject panelPhotos;

        private void OnEnable()
        {
            GlobalActionsUI.OnToggleTarget += RefreshContainer;
            GlobalActionsUI.OnLangChanged += SetTextInfo;
            GlobalActionsUI.OnTourItemPageChanged += OnTourItemPageChanged;
            GlobalActionsUI.OnTourPageChanged += OnTourItemPageChanged;
            btnShowPoisOnMap.onClick.AddListener(SelectTourPois);
            btnShowPoisOnMap.onClick.AddListener(ScrollResetPosition);
        }

        public void Init(TourEntity tour)
        {
            tourEntity = tour;

            infoGR = tour.infoGR;
            infoEN = tour.infoEN;
            topicID =tour.id;
            tourID = tour.id;
            SetTextInfo();
            CreateImages();
        }

        void CreateImages()
        {
            if (tourEntity.digitalExhibitImages.Count <= 0)
            {
                panelPhotos.SetActive(false);
                Invoke("RefreshElements", 0.15f);
                return;
            }
            for(int i=0; i<3; i++)
            {
                int rand = Random.Range(0, tourEntity.digitalExhibitImages.Count);
                DigitalExhibitObject dgImage = tourEntity.digitalExhibitImages[rand];
                Transform pImg = Instantiate(imageItemPrefab, rectScrollImages);
                ImageItem item = pImg.GetComponent<ImageItem>();
                item.Init(dgImage.fileName, tourEntity.topicID, dgImage.GetLabel(), dgImage.sourceLabel);
            }
        }

        void OnTourItemPageChanged()
        {
            rectScrollContainer.parent.GetComponent<ScrollRect>().enabled = false;
            rectScrollImages.parent.GetComponent<ScrollRect>().enabled = false;
            ScrollResetPosition();
        }

        void OnTourItemPageChanged(int id)
        {
            if (pageID == id) return;
            rectScrollContainer.parent.GetComponent<ScrollRect>().enabled = false;
            rectScrollImages.parent.GetComponent<ScrollRect>().enabled = false;
            ScrollResetPosition();
        }

        private void ScrollResetPosition()
        {
            Vector3 pos = rectScrollContainer.localPosition;
            pos.y = 0f;
            rectScrollContainer.localPosition = pos;
            Vector3 pos2 = rectScrollImages.localPosition;
            pos2.x = 0f;
            rectScrollImages.localPosition = pos2;
            rectScrollContainer.parent.GetComponent<ScrollRect>().enabled = true;
            rectScrollImages.parent.GetComponent<ScrollRect>().enabled = true;
            //RefreshElements();
        }

        private void SelectTourPois()
        {
            InfoManager.Instance.tourNowID = tourID;
            GlobalActionsUI.OnShowPoisOnMap?.Invoke();
        }

        private void SetTextInfo()
        {
            txtTitle.text = GetTitle();
            txtDesc.text = GetDesc();
            txtShortText.text = GetShortDesc();
            //RefreshContainer(null);
            Invoke("RefreshElements", 0.15f);
        }

        private string GetTitle()
        {
            if (StaticData.lang == "gr") { return infoGR.title; }
            else
            if (StaticData.lang == "en") { return infoEN.title; }
            return string.Empty;
        }
        private string GetDesc()
        {
            if (StaticData.lang == "gr") { return infoGR.desc; }
            else
            if (StaticData.lang == "en") { return infoEN.desc; }
            return string.Empty;
        }
        private string GetShortDesc()
        {
            if (StaticData.lang == "gr") { return infoGR.shortdesc; }
            else
            if (StaticData.lang == "en") { return infoEN.shortdesc; }
            return string.Empty;
        }

        private void RefreshContainer(GameObject gb)
        {
            //Debug.Log("tours item refresh");
            RefreshElements();
        }

        private void RefreshElements() { foreach (RectTransform rt in allRects) LayoutRebuilder.ForceRebuildLayoutImmediate(rt); }

        public void DestroyItem() { Destroy(gameObject); }

        private void OnDisable()
        {
            GlobalActionsUI.OnToggleTarget -= RefreshContainer;
            GlobalActionsUI.OnLangChanged -= SetTextInfo;
            GlobalActionsUI.OnTourItemPageChanged -= OnTourItemPageChanged;
            GlobalActionsUI.OnTourPageChanged -= OnTourItemPageChanged;
            btnShowPoisOnMap.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            GlobalActionsUI.OnToggleTarget -= RefreshContainer;
            GlobalActionsUI.OnLangChanged -= SetTextInfo;
            GlobalActionsUI.OnTourItemPageChanged -= OnTourItemPageChanged;
            GlobalActionsUI.OnTourPageChanged -= OnTourItemPageChanged;
            btnShowPoisOnMap.onClick.RemoveAllListeners();
        }


        IEnumerator LoadPreviewImages()
        {
            //OnLoadPreviewImagesStart?.Invoke();

            //previewImages.Clear();

            //if (gPath.pathInfos.Count <= 0) yield break;

            string imageURL = string.Empty;
            //foreach (PathInfo info in gPath.pathInfos)
            //{
            //    imageURL = string.Empty;

            //    if (info.attribute_ID == 854)
            //    {
            //        imageURL = info.attribute_Value;

            //        if (string.IsNullOrEmpty(imageURL)) continue;
            //    }
            //    else { continue; }

            //    DigitalExhibitComplete dg = digiExhibitManager.GetImageExhibitById(imageURL);
            //    string fileNameOrId = dg.infoCommon.digitalFileName;

            //    if (string.IsNullOrEmpty(fileNameOrId)) continue;

            //    CoroutineWithData cdImage = new CoroutineWithData(this, LoadImage(fileNameOrId));
            //    yield return cdImage.Coroutine;

            //    Texture2D tex = (Texture2D)cdImage.result;
            //    if (tex != null)
            //    {
            //        int indx = fileNameOrId.LastIndexOf(".");
            //        string nameWithoutExtension = fileNameOrId.Remove(indx);
            //        if (string.IsNullOrEmpty(nameWithoutExtension)) { tex.name = nameWithoutExtension; }
            //        previewImages.Add(tex);
            //        AddPhoto(tex, nameWithoutExtension);
            //    }
            //    yield return null;// new WaitForEndOfFrame();
            //}

            #region old code
            /*

            foreach (PathInfo info in gPath.pathInfos)
            {
                if (info.attribute_ID == 857)
                {
                    url = Path.Combine(Application.streamingAssetsPath, StaticDataUI.folderPathImages + info.attribute_Value);

                    //if(B.isEditor) Debug.LogWarning(url);

                    if (string.IsNullOrEmpty(url)) continue;
                }
                else
                {
                    continue;
                }

                //Debug.Log("get data for id " + info.attribute_ID);

                byte[] imgData;
                Texture2D tex = new Texture2D(2, 2);

                UnityWebRequest wwwImage = UnityWebRequest.Get(url);
                yield return wwwImage.SendWebRequest();

                if (wwwImage.isNetworkError || wwwImage.isHttpError)
                {
                    Debug.Log(wwwImage.error);
                    yield return "fail";
                }

                imgData = wwwImage.downloadHandler.data;

                //if (B.isEditor)
                //{
                //    float binarySize = imgData.Length / 1024;
                //    if (binarySize < 1000f)
                //    {
                //        Debug.Log(info.attribute_Value + " size = " + binarySize + " KB");
                //    }
                //    else
                //    {
                //        binarySize = binarySize / 1000f;
                //        Debug.Log(info.attribute_Value + " size = " + binarySize + " MB");
                //    }
                //}

                //Load raw Data into Texture2D 
                tex.LoadImage(imgData);

                if (tex != null)
                {
                    previewImages.Add(tex);
                }


                ////Convert Texture2D to Sprite
                //Vector2 pivot = new Vector2(0.5f, 0.5f);
                //Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot, 100.0f);

                ////Apply Sprite to SpriteRenderer
                //SpriteRenderer renderer = GetComponent<SpriteRenderer>();
                //renderer.sprite = sprite;

            }                

            */
            #endregion

            if (B.isEditor) Debug.Log("OnLoadPreviewImages");

            //OnLoadPreviewImagesComplete?.Invoke();

            yield break;
        }


        //public IEnumerator LoadImage(string filename)
        //{
        //    Texture2D tex = GetPhoto(filename);
        //    if (tex)
        //    {
        //        if (B.isEditor) Debug.LogWarning("loading image from list");
        //        yield return tex;
        //    }
        //    else
        //    {
        //        if (imagesFolder == LoadImagesFolder.RESOURCES)
        //        {
        //            int indx = filename.LastIndexOf(".");
        //            string nameWithoutExtension = filename.Remove(indx);
        //            if (string.IsNullOrEmpty(nameWithoutExtension)) yield return null;
        //            tex = Resources.Load<Texture2D>(StaticDataUI.folderImages + nameWithoutExtension);
        //            yield return tex;
        //        }
        //        else
        //        if (imagesFolder == LoadImagesFolder.STREAMING_ASSETS)
        //        {
        //            string url = "";// Path.Combine(Application.streamingAssetsPath, StaticDataUI.folderImages+filename);
        //            if (string.IsNullOrEmpty(url)) { url = Application.streamingAssetsPath + "/" + StaticDataUI.folderImages + filename; }
        //            if (string.IsNullOrEmpty(url)) { yield return null; }

        //            byte[] imgData;
        //            tex = new Texture2D(2, 2);

        //            UnityWebRequest www = UnityWebRequest.Get(url);
        //            yield return www.SendWebRequest();
        //            imgData = www.downloadHandler.data;

        //            //Load raw Data into Texture2D 
        //            tex.LoadImage(imgData);

        //            yield return tex;
        //        }
        //    }
        //}

    }

}
