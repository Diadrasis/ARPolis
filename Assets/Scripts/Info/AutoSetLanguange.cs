using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ARPolis.Data;

namespace ARPolis.Info
{

    public class AutoSetLanguange : MonoBehaviour
    {

        public bool setFontSizeManually = false;
        public bool autoReplaceText = true;
        public bool useCaps = false;

        private Text myText;
        private TMPro.TextMeshProUGUI txt;
        public string myLang;
        public string termToFind;

        RawImage myRawImage;
        public Texture grSprite;
        public Texture enSprite;

        public bool autoResizeTextFont, changeLineForStringToAdd;
        //	int tabletFontSize = 17;
        //	int mobileFontSize = 21;
        //	int smallMobile = 24;

        public string stringToAdd = string.Empty;

        LayoutElement myLayOut;

        ContentSizeFitter sizeFitter;

        [Space]
        public bool isCreditsPerson;
        public int creditPersonValue;//0 = property, 1 = name

        void OnEnable()
        {
            AppData.OnDataReaded += ChangeLanguange;

            ChangeLanguange();

            if (!sizeFitter) { sizeFitter = gameObject.GetComponent<ContentSizeFitter>(); }
            if (!myLayOut) myLayOut = GetComponent<LayoutElement>();
            if (myLayOut)
            {
                //ContentSizeFitter cs = GetComponent<ContentSizeFitter>();
                //if(cs){
                //	myLayOut.preferredWidth = RuntimeManager.Instance.canvasMainRT.sizeDelta.x / 1.5f;
                //}
            }
            if (myText == null) { myText = GetComponent<Text>(); }
            if (txt == null) { txt = GetComponent<TMPro.TextMeshProUGUI>(); }
            if (string.IsNullOrEmpty(termToFind) && myText) termToFind = myText.transform.name;
            if (string.IsNullOrEmpty(termToFind) && txt) termToFind = txt.transform.name;

            //if (setFontSizeManually) {
            //	if(autoResizeTextFont)
            //	{
            //		myText.fontSize = appSettings.fontSize_keimeno;
            //	}
            //}

            if (myRawImage == null && grSprite && enSprite) myRawImage = GetComponent<RawImage>();

            myLang = StaticData.lang;

            ReplaceText();
            ReplaceImage();

            StopAllCoroutines();
            StartCoroutine("SetSizeFitterOnDelayed");
        }

        private void OnDisable()
        {
            AppData.OnDataReaded -= ChangeLanguange;
        }

        IEnumerator SetSizeFitterOnDelayed()
        {
            yield return new WaitForEndOfFrame();
            if (sizeFitter) { sizeFitter.enabled = false; }
            yield return new WaitForEndOfFrame();
            if (sizeFitter) { sizeFitter.enabled = true; }
            yield break;
        }

        void ChangeLanguange()
        {
            if (myLang == StaticData.lang) return;
            myLang = StaticData.lang;
            //Invoke("Init", 0.25f);
            Init();
        }

        public void Init()
        {
            if (myText) myText.text = string.Empty;
            if (txt) txt.text = string.Empty;
            ReplaceText();

            ReplaceImage();
        }

        void ReplaceText()
        {
            if (!autoReplaceText) return;

            if (isCreditsPerson)
            {
                string term = AppData.Instance.FindPersonCreditName(termToFind);
                if(creditPersonValue==1) term = AppData.Instance.FindPersonCreditProperty(termToFind);
                if (useCaps) term = term.ToUpper();
                if (myText) myText.text = term;
                if (txt) txt.text = term;
            }
            else
            {

                //find text from terms xml with the name of transform if exists
                string term = AppData.Instance.FindTermValue(termToFind);
                if (useCaps) term = term.ToUpper();
                if (myText) myText.text = term;
                if (txt) txt.text = term;
            }

            //if (autoResizeTextFont)
            //{
            //	myText.fontSize = appSettings.fontSize_keimeno;
            //}

            if (!string.IsNullOrEmpty(stringToAdd))
            {
                if (changeLineForStringToAdd)
                {
                    if (myText) myText.text += "\n" + stringToAdd;
                    if (txt) txt.text += "\n" + stringToAdd;
                }
                else
                {
                    if (myText) myText.text += stringToAdd;
                    if (txt) txt.text += stringToAdd;
                }

            }
        }

        void ReplaceImage()
        {
            if (myRawImage && grSprite && enSprite)
            {
                if (myLang == "gr")
                {
                    myRawImage.texture = grSprite;
                }
                else
                if (myLang == "en")
                {
                    myRawImage.texture = enSprite;
                }
            }
        }
    }

}
