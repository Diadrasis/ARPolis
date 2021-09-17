using ARPolis.Data;
using ARPolis.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurveyManager : MonoBehaviour
{
    #region Variables
    [Header("UI References")]
    public RectTransform rtParent;
    public GameObject questionsPanel;

    [Space]
    [Header("Prefabs")]
    public GameObject questionPrefab;
    public GameObject answerPrefab;

    [Space]
    [Header("Questions Data")]
    public QuestionData[] questionsData;

    // Language
    private bool isGreek;

    // Question Texts for Language change : question index, question text, answers texts
    Dictionary<int, KeyValuePair<Text, Text[]>> questionTexts = new Dictionary<int, KeyValuePair<Text, Text[]>>();

    // Answers : question index, answer toggles
    Dictionary<int, Toggle[]> answerTogglesDictionary = new Dictionary<int, Toggle[]>();

    // Answers indexes
    List<int>[] answers;

    Dictionary<int, List<int>> answersDictionary = new Dictionary<int, List<int>>(); // question_id, list of answer ids or indexes

    // Reload layout
    private float interval = 0.001f;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        // Initialize the Survey
        InitializeSurvey();
    }

    private void OnEnable()
    {
        GlobalActionsUI.OnLangChanged += SetLanguage;
        SetLanguage();
    }

    private void OnDestroy()
    {
        //PlayerPrefs.DeleteAll(); // TODO: REMOVE!!!
    }
    #endregion

    #region Methods
    public void InitializeSurvey()
    {
        // Reset answers if questions have been instantiated already otherwise instantiate the questions
        if (answerTogglesDictionary !=null && answerTogglesDictionary.Count > 0)
            ResetAnwers();
        else
        {
            // Initialize answers
            InitializeAnswers();

            // Instantiate questions
            for (int i = 0; i < questionsData.Length; i++)
            {
                InstantiateQuestion(i);
            }

            // Reload layout (Current doesn't change anything)
            StartCoroutine(ReloadLayout(questionsPanel));
        }
    }

    void ResetAnwers()
    {
        foreach (Toggle[] toggles in answerTogglesDictionary.Values)
        {
            foreach (Toggle toggle in toggles)
            {
                toggle.isOn = false;
            }
        }
    }

    void InitializeAnswers()
    {
        answers = new List<int>[questionsData.Length];
        for (int i = 0; i < questionsData.Length; i++)
        {
            answers[i] = new List<int>();
        }
    }

    GameObject InstantiateQuestion(int _index)
    {
        // Get question data
        QuestionData questionData = questionsData[_index];

        // Instantiate question, set position and add it to the questions array
        GameObject newQuestion = Instantiate(questionPrefab, questionsPanel.transform.position, Quaternion.identity, questionsPanel.transform);
        newQuestion.transform.SetSiblingIndex(_index + 1);

        // Set rtParent
        newQuestion.GetComponent<ControlRect>().rtParent = rtParent;

        // GetSet question text
        Text txtQuestion = newQuestion.transform.Find("Text").gameObject.GetComponent<Text>();
        if (isGreek)
            txtQuestion.text = questionData.questionGr;
        else
            txtQuestion.text = questionData.questionEn;

        // Initialize answerTexts and add a new question text to questionTexts dictionary
        Text[] answerTexts = new Text[questionData.questionGr.Length];
        questionTexts.Add(_index, new KeyValuePair<Text, Text[]>(txtQuestion, answerTexts));

        // Get answers panel
        GameObject answersPanel = newQuestion.transform.Find("AnswersPanel").gameObject;

        // Get toggle group
        ToggleGroup toggleGroup = newQuestion.GetComponentInChildren<ToggleGroup>();

        // Intitialize answer toggles
        Toggle[] answerToggles = new Toggle[questionData.answersGr.Length];

        // Instantiate Answers
        for (int i = 0; i < questionData.answersGr.Length; i++)
        {
            // Instantiate answer and set text
            GameObject newAnswer = Instantiate(answerPrefab, answersPanel.transform.position, Quaternion.identity, answersPanel.transform);
            Text txt = newAnswer.GetComponentInChildren<Text>();
            if (isGreek || questionData.dontTranslate)
                txt.text = questionData.answersGr[i];
            else
                txt.text = questionData.answersEn[i];

            // Set answerTexts
            answerTexts[i] = txt;

            // Get toggle
            Toggle toggle = newAnswer.gameObject.GetComponent<Toggle>();
            answerToggles[i] = toggle;

            // Set toggle group
            if (!questionData.multipleChoice)
            {
                toggle.group = toggleGroup;
            }
        }

        // Add answer toggles to questionsDictionary
        answerTogglesDictionary.Add(_index, answerToggles);

        return newQuestion;
    }

    void GetAnswers()
    {
        // Reset answers
        InitializeAnswers();

        // For every question in the questions dictionary
        foreach (KeyValuePair<int,Toggle[]> question in answerTogglesDictionary)
        {
            // For every answer toggle
            for (int i = 0; i < question.Value.Length; i++)
            {
                // Check if answer toggle is on and add answer index to answers list
                if (question.Value[i].isOn)
                    answers[question.Key].Add(i);
            }
        }
    }

    List<string> GetAnswersString()
    {
        // Initialize string list
        List<string> answersStringList = new List<string>();

        // Get answers
        GetAnswers();

        // Set answers string
        for (int i = 0; i < answers.Length; i++)
        {
            // Get question data
            QuestionData questionData = questionsData[i];

            foreach (int index in answers[i])
            {
                // Answer string
                string answerString = "";

                if (isGreek || questionData.dontTranslate)
                    answerString = questionData.answersGr[index];
                else
                    answerString = questionData.answersEn[index];

                answersStringList.Add(answerString);
            }
        }

        return answersStringList;
    }

    Dictionary<int, List<int>> GetAnswersDictionary()
    {
        // Initialize dictionary
        Dictionary<int, List<int>> answersDictionary = new Dictionary<int, List<int>>();

        // Get Answers
        GetAnswers();

        // Get question ids
        for (int i = 0; i < questionsData.Length; i++)
        {
            // Get question data
            QuestionData questionData = questionsData[i];

            // Get question id
            int question_id = questionData.question_id;

            /*Debug.Log("question id : " + question_id);
            Debug.Log("answers count : " + answers[i].Count);*/

            answersDictionary.Add(question_id, answers[i]);
        }

        // Debug
        /*foreach (int question in answersDictionary.Keys)
        {
            Debug.Log("Question : " + question);
            Debug.Log("Answers count : " + answersDictionary[question].Count);
        }*/

        return answersDictionary;
    }

    public void SaveSurvey()
    {
        // Reload current user to get server path id if it was uploaded
        User currentUser = ARPolis.AppManager.Instance.currentUser;
        currentUser = User.Reload(currentUser);

        // Initialize and get answers string list
        //List<string> answers = GetAnswersString();
        Dictionary<int, List<int>> answersDictionary = GetAnswersDictionary();

        // Save Survey
        Survey surveyToSave = new Survey(currentUser.server_user_id, currentUser.local_user_id, answersDictionary); // answers
        Survey.Save(surveyToSave);

        // Upload to server
        //AppManager.Instance.serverManager.postUserData = true;
        //AppManager.Instance.serverManager.timeRemaining = 0f;
    }

    public void SaveSurvey(User _user)
    {
        // Initialize and get answers string list
        //List<string> answers = GetAnswersString();
        Dictionary<int, List<int>> answersDictionary = GetAnswersDictionary();

        // Save Survey
        Survey surveyToSave = new Survey(_user.server_user_id, _user.local_user_id, answersDictionary); // answers
        Survey.Save(surveyToSave);
    }

    public void UploadSurvey()
    {
        // Upload to server
        //AppManager.Instance.serverManager.postUserData = true;
        //AppManager.Instance.serverManager.timeRemaining = 0f;
    }

    public void SetLanguage()
    {
        // Get Set language
        isGreek = StaticData.lang == "gr";

        // For each question and answers change text object text
        foreach (KeyValuePair<int, KeyValuePair<Text, Text[]>> question in questionTexts)
        {
            // Get question data from index
            int questionIndex = question.Key;
            QuestionData questionData = questionsData[questionIndex];

            // Set question text
            if (isGreek)
                question.Value.Key.text = questionData.questionGr;
            else
                question.Value.Key.text = questionData.questionEn;

            // Set answers texts
            for (int i = 0; i < questionData.answersGr.Length; i++) // TODO: Length can be set from either english of greek answers length to avoid errors
            {
                Text answerText = question.Value.Value[i];

                if (isGreek || questionData.dontTranslate)
                    answerText.text = questionData.answersGr[i];
                else
                    answerText.text = questionData.answersEn[i];
            }
        }
    }

    IEnumerator ReloadLayout(GameObject _layoutGameObject)
    {
        yield return new WaitForSeconds(interval);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGameObject.GetComponent<RectTransform>());
    }

    #region Debug
    public void PrintAnswers()
    {
        foreach (string answerString in GetAnswersString())
        {
            Debug.Log(answerString);
        }
    }
    #endregion

    #endregion
}
