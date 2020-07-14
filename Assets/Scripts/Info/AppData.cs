using UnityEngine;
using System.Collections;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using ARPolis.Info.Objects;
using StaGeUnityTools;
using ARPolis.Data;

namespace ARPolis.Info
{

    public class AppData
    {
        public static XmlDocument termXml;
        public static List<Term> textTerms = new List<Term>();
        static string[] savedTermsXml;// = PlayerPrefsX.GetStringArray("fromServer"+xml);

        public delegate void DataReadAction();
        public static DataReadAction OnDataRead;

        public static void Init()
        {

            savedTermsXml = PlayerPrefsX.GetStringArray("fromServer_terms");

            if (savedTermsXml.Length <= 0)
            {
                //load terms and help
                termXml = new XmlDocument();
                TextAsset terms = (TextAsset)Resources.Load("XML/terms");
                termXml.LoadXml(Regex.Replace(terms.text, "(<!--(.*?)-->)", string.Empty));
            }
            else
            {
                //check local saved xml version
                termXml = new XmlDocument();
                string textAsset = string.Empty;

                for (int x = 0; x < savedTermsXml.Length; x++)
                {
                    textAsset += savedTermsXml[x];
                }

                string localExcludedComments = Regex.Replace(textAsset, "(<!--(.*?)-->)", string.Empty);
                termXml.LoadXml(localExcludedComments);
            }

            ReadTerms();
        }

        //read all terms
        public static void ReadTerms()
        {
            // Clear previous loaded Terms
            textTerms.Clear();
            Term myTerm;
            XmlNodeList termList = termXml.GetElementsByTagName("term");

            foreach (XmlNode term in termList)
            {
                myTerm = new Term();
                myTerm.Name = term["name"].InnerText;
                myTerm.Text = term["value"][StaticData.lang].InnerText.Replace("\\n", "\n").Replace("&amp;", "&");
                textTerms.Add(myTerm);
            }

            if (B.isRealEditor) Debug.LogWarning("terms are " + textTerms.Count.ToString());

            OnDataRead?.Invoke();

        }

        public static string FindTermValue(string name)
        {
            for (int i = 0; i < textTerms.Count; i++)
            {
                if (textTerms[i].Name == name) return textTerms[i].Text;
            }
            return string.Empty;
        }


    }

}
