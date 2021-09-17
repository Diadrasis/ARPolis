using System.Collections.Generic;
using UnityEngine;


public class Survey
{
    #region Variables
    public int server_user_id;
    public int local_user_id;
    //public List<string> answers;
    public Dictionary<int, List<int>> answersDictionary = new Dictionary<int, List<int>>(); // Key Value Pair = question_id, list of answer ids or indexes

    public static readonly string PREFS_KEY = "surveys";
    public static readonly string SURVEYS = "surveys";

    public static readonly string SURVEY = "survey";
    public static readonly string SERVER_USER_ID = "server_user_id";
    public static readonly string LOCAL_USER_ID = "local_user_id";
    public static readonly string QUESTIONS = "questions";

    public static readonly string QUESTION = "question";
    //public static readonly string QUESTION_ID = "question_id";
    public static readonly string ANSWERS = "answers";

    public static readonly string ANSWER = "answer";
    #endregion

    #region Methods
    // Constructor for Loading from Player Prefs and creating a new survey
    public Survey(int _server_user_id, int _local_user_id, Dictionary<int, List<int>> _answersDictionary) // , List<string> _answers
    {
        server_user_id = _server_user_id; // Probably always equals -1
        local_user_id = _local_user_id;
        //answers = _answers;
        answersDictionary = _answersDictionary;
    }

    public static OnlineMapsXML GetXML()
    {
        // Load xml string from PlayerPrefs
        string xmlData = PlayerPrefs.GetString(PREFS_KEY);

        // Load xml document, if null create new
        OnlineMapsXML xml = OnlineMapsXML.Load(xmlData);
        if (xml.isNull)
        {
            xml = new OnlineMapsXML(SURVEYS);
        }

        return xml;
    }

    public static void Save(Survey _surveyToSave)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Check if area is already saved
        OnlineMapsXML surveySaved = xml.Find("/" + SURVEYS + "/" + SURVEY + "[" + LOCAL_USER_ID + "=" + _surveyToSave.local_user_id + "]");
        if (!surveySaved.isNull)
        {
            Debug.Log("Survey is already saved!");
            return;
        }

        // Create a new survey node
        OnlineMapsXML surveyNode = xml.Create(SURVEY);
        surveyNode.Create(SERVER_USER_ID, _surveyToSave.server_user_id);
        surveyNode.Create(LOCAL_USER_ID, _surveyToSave.local_user_id);
        OnlineMapsXML questionsNode = surveyNode.Create(QUESTIONS);
        foreach (int question in _surveyToSave.answersDictionary.Keys)
        {
            OnlineMapsXML questionNode = questionsNode.Create(QUESTION, question);
            OnlineMapsXML answersNode = questionNode.Create(ANSWERS);
            //Debug.Log("_surveyToSave.answersDictionary[question].Count : " + _surveyToSave.answersDictionary[question].Count);
            foreach (int answer in _surveyToSave.answersDictionary[question])
            {
                answersNode.Create(ANSWER, answer);
            }
        }

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();

        // Debug
        //Debug.Log(xml.outerXml);
    }

    /*public static void SetServerUserId(int _server_user_id, int _local_user_id)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Find survey
        OnlineMapsXML surveyNode = xml.Find("/" + SURVEYS + "/" + SURVEY + "[" + LOCAL_USER_ID + "=" + _local_user_id + "]");

        if (!surveyNode.isNull)
        {
            // Load survey
            Survey loadedSurvey = Load(surveyNode);

            if (loadedSurvey != null)
            {
                loadedSurvey.server_user_id = _server_user_id;

                // Edit user
                EditServerUserId(loadedSurvey);
            }
        }
    }

    private static void EditServerUserId(Survey _surveyToEdit)
    {
        // Load xml document, if null create new
        OnlineMapsXML xml = GetXML();

        // Create a new user
        OnlineMapsXML surveyNode = xml.Find("/" + SURVEYS + "/" + SURVEY + "[" + LOCAL_USER_ID + "=" + _surveyToEdit.local_user_id + "]");
        surveyNode.Remove(SERVER_USER_ID);
        surveyNode.Create(SERVER_USER_ID, _surveyToEdit.server_user_id);

        // Save xml string to PlayerPrefs
        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }*/

    public static Survey LoadSurveyByLocalUserId(int _local_user_id)
    {
        // Load xml document
        OnlineMapsXML xml = GetXML();

        // Get survey node
        OnlineMapsXML surveyNode = xml.Find("/" + SURVEYS + "/" + SURVEY + "[" + LOCAL_USER_ID + "=" + _local_user_id + "]");
        if (surveyNode.isNull)
        {
            Debug.Log("Survey with local_user_id: " + _local_user_id + " has been deleted!");
            return null;
        }

        return Load(surveyNode);
    }

    private static Survey Load(OnlineMapsXML _surveyNode)
    {
        int server_user_id = _surveyNode.Get<int>(SERVER_USER_ID);
        int local_user_id = _surveyNode.Get<int>(LOCAL_USER_ID);

        // Initialize dictionary
        Dictionary<int, List<int>> answersDictionary = new Dictionary<int, List<int>>();

        // Get question nodes
        OnlineMapsXMLList questionNodes = _surveyNode.FindAll("/" + QUESTIONS + "/" + QUESTION);
        foreach (OnlineMapsXML questionNode in questionNodes)
        {
            // Get question id
            int question = questionNode.Get<int>(questionNode.element);

            // Initialize list
            List<int> answers = new List<int>();

            // Get answer nodes
            OnlineMapsXMLList answerNodes = questionNode.FindAll("/" + ANSWERS + "/" + ANSWER);
            foreach (OnlineMapsXML answerNode in answerNodes)
            {
                int answer = answerNode.Get<int>(answerNode.element);
                answers.Add(answer);
            }

            answersDictionary.Add(question, answers);
        }

        /*OnlineMapsXMLList answerNodes = _surveyNode.FindAll("/" + QUESTIONS + "/" + QUESTION + "/" + ANSWERS + "/" + ANSWER);
        Dictionary<int, List<int>> answersDictionary = new Dictionary<int, List<int>>();
        foreach (OnlineMapsXML answerNode in answerNodes)
        {
            string answer = answerNode.Get<string>(answerNode.element);
            answers.Add(answer);
        }*/

        Survey loadedSurvey = new Survey(server_user_id, local_user_id, answersDictionary);
        return loadedSurvey;
    }

    /*private static Survey Load(OnlineMapsXML _surveyNode)
    {
        int server_user_id = _surveyNode.Get<int>(SERVER_USER_ID);
        int local_user_id = _surveyNode.Get<int>(LOCAL_USER_ID);

        OnlineMapsXMLList answerNodes = _surveyNode.FindAll("/" + SURVEYS + "/" + SURVEY + "[" + LOCAL_USER_ID + "=" + local_user_id + "]/" + ANSWERS + "/" + ANSWER);
        List<string> answers = new List<string>();
        foreach (OnlineMapsXML answerNode in answerNodes)
        {
            string answer = answerNode.Get<string>(answerNode.element);
            answers.Add(answer);
        }

        Survey loadedSurvey = new Survey(server_user_id, local_user_id, answers);
        return loadedSurvey;
    }*/

    /*public static void Delete(int _local_user_id)
    {
        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Get survey node
        OnlineMapsXML surveyToDelete = xml.Find("/" + SURVEYS + "/" + SURVEY + "[" + LOCAL_USER_ID + "=" + _local_user_id + "]");
        if (!surveyToDelete.isNull)
            surveyToDelete.Remove();

        PlayerPrefs.SetString(PREFS_KEY, xml.outerXml);
        PlayerPrefs.Save();
    }*/

    /*public static List<Survey> GetSurveysToUpload()
    {
        // List of surveys
        List<Survey> surveysToUpload = new List<Survey>();

        // Load xml document, if null creates new
        OnlineMapsXML xml = GetXML();

        // Get surveys with server_user_id != -1
        OnlineMapsXMLList surveyNodes = xml.FindAll("/" + SURVEYS + "/" + SURVEY + "[" + SERVER_USER_ID + "!=" + (-1) + "]");

        foreach (OnlineMapsXML surveyNode in surveyNodes)
        {
            if (surveyNode.isNull)
            {
                Debug.Log("Survey has been deleted!");
                continue;
            }

            Survey loadedSurvey = Load(surveyNode);

            if (loadedSurvey != null)
                surveysToUpload.Add(loadedSurvey);
        }

        return surveysToUpload;
    }*/
    #endregion
}
