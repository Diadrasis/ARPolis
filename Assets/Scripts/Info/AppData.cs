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

    public class AppData : Singleton<AppData>
    {
        protected AppData() { }

        private XmlDocument termXml, creditsXml;
        private List<Term> textTerms = new List<Term>();
        private List<PersonCredit> personCredits = new List<PersonCredit>();
        //static string[] savedTermsXml;// = PlayerPrefsX.GetStringArray("fromServer"+xml);

        public delegate void DataReadAction();
        public static DataReadAction OnDataReaded;
        private static bool hasInit;

        public void Init()
        {
            if(B.isEditor) Debug.Log("AppData.Init()");

            if (hasInit)
            {
                ReadCredits();
                ReadTerms();
                return;
            }

            //savedTermsXml = PlayerPrefsX.GetStringArray("fromServer_terms");

            //if (savedTermsXml.Length <= 0)
            //{
                //load terms and help
                termXml = new XmlDocument();
                TextAsset terms = (TextAsset)Resources.Load("XML/terms");
                termXml.LoadXml(Regex.Replace(terms.text, "(<!--(.*?)-->)", string.Empty));
            //}
            //else
            //{
            //check local saved xml version
            //termXml = new XmlDocument();
            //string textAsset = string.Empty;

            //for (int x = 0; x < savedTermsXml.Length; x++)
            //{
            //    textAsset += savedTermsXml[x];
            //}

            //string localExcludedComments = Regex.Replace(textAsset, "(<!--(.*?)-->)", string.Empty);
            //termXml.LoadXml(localExcludedComments);
            //}

            //Debug.Log(termXml.InnerText);

            creditsXml = new XmlDocument();
            TextAsset creditsPersons = (TextAsset)Resources.Load("XML/credits");
            creditsXml.LoadXml(Regex.Replace(creditsPersons.text, "(<!--(.*?)-->)", string.Empty));

            ReadCredits();
            ReadTerms();

            hasInit = true;
        }

        private void ReadCredits()
        {
            // Clear previous loaded Terms
            personCredits = new List<PersonCredit>();
            personCredits.Clear();
            XmlNodeList personList = creditsXml.GetElementsByTagName("person");

            foreach (XmlNode person in personList)
            {
                PersonCredit myPerson = new PersonCredit();
                myPerson.Id = person["id"].InnerText;
                myPerson.Name = person["name"][StaticData.lang].InnerText.Replace("\\n", "\n").Replace("&amp;", "&");
                myPerson.Text = person["idiotita"][StaticData.lang].InnerText.Replace("\\n", "\n").Replace("&amp;", "&");
                personCredits.Add(myPerson);
            }

            Debug.LogWarning("credits are " + personCredits.Count.ToString());

        }


        //read all terms
        private void ReadTerms()
        {
            // Clear previous loaded Terms
            textTerms = new List<Term>();
            textTerms.Clear();
            XmlNodeList termList = termXml.GetElementsByTagName("term");

            foreach (XmlNode term in termList)
            {
                Term myTerm = new Term();
                myTerm.Name = term["name"].InnerText;
                myTerm.Text = term["value"][StaticData.lang].InnerText.Replace("\\n", "\n").Replace("&amp;", "&");
                textTerms.Add(myTerm);
            }

            Debug.LogWarning("terms are " + textTerms.Count.ToString());

            OnDataReaded?.Invoke();

        }

        public string FindTermValue(string name)
        {
            for (int i = 0; i < textTerms.Count; i++)
            {
                if (textTerms[i].Name == name) return textTerms[i].Text;
            }
            return string.Empty;
        }


        public string FindPersonCreditName(string id)
        {
            for (int i = 0; i < personCredits.Count; i++)
            {
                if (personCredits[i].Id == id) return personCredits[i].Name;
            }
            return string.Empty;
        }

        public string FindPersonCreditProperty(string id)
        {
            for (int i = 0; i < personCredits.Count; i++)
            {
                if (personCredits[i].Id == id) return personCredits[i].Text;
            }
            return string.Empty;
        }

    }

}
