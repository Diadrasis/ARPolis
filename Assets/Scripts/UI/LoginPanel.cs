using ARPolis.Data;
using ARPolis.Info;
using ARPolis.Save;
using ARPolis.Server;
using StaGeUnityTools;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ARPolis.UI
{

    public class LoginPanel : MonoBehaviour
    {

        public Button btnShowSignIn, btnShowSignUp, btnLoginSubmit, btnSignUpSubmit, btnLoginAnonymous, btnCancelSignIn, btnCancelSignUp;
        public InputField inpSignInUser, inpSignInPass, inpSignUpUser, inpSignUpPass, inpSignUpPassCheck;
        public Toggle toggleRememberUser;
        public Text txtMessageLogin, txtMessageSignUp;
        public GameObject panelSelectSignInUp, panelSignIn, panelSignUp, loginPanel;
        Animator animLogin, animSignUp, animSignIn;
        VerticalLayoutGroup verticalLayoutGroupLoginPage;

        #region Andrew Variables
        [Space]
        [Header("Andrew UI References")]
        public Button btnSaveSignUp;
        public Button btnLanguage;
        public Button btnSaveSurvey;
        public Image iconBtnLanguage;
        public Sprite sprEng, sprGR;
        public Text textSurveyIntro, textSurveyOutro, textInput;

        string defaultUserUsername = "giannisL";
        string defaultUserPassword = "2791";
        User newUser;
        #endregion

        private void Awake()
        {
            animLogin = loginPanel.GetComponent<Animator>();
            animSignUp = panelSignUp.GetComponent<Animator>();
            animSignIn = panelSignIn.GetComponent<Animator>();
            verticalLayoutGroupLoginPage = loginPanel.GetComponent<VerticalLayoutGroup>();
            verticalLayoutGroupLoginPage.enabled = false;
            ServerController.Instance.Init();

            inpSignInUser.inputType = InputField.InputType.Standard;
            inpSignInUser.characterLimit = 10;
            inpSignInPass.inputType = InputField.InputType.Password;
            inpSignInPass.characterLimit = 10;

            inpSignUpUser.inputType = InputField.InputType.Standard;
            inpSignUpUser.characterLimit = 10;
            inpSignUpPass.inputType = InputField.InputType.Password;
            inpSignUpPass.characterLimit = 10;
            inpSignUpPassCheck.inputType = InputField.InputType.Password;
            inpSignUpPassCheck.characterLimit = 10;

            HideMessage();
            HideSignUpMessage();

            //btnLoginSubmit.onClick.AddListener(ServerLoginUser);
            btnSignUpSubmit.onClick.AddListener(ServerSubmitSignUp);
            btnLoginAnonymous.onClick.AddListener(LoginAnonymous);

            //btnHideMessage.onClick.AddListener(HideMessage);

            int appEntrances = SaveLoad.GetSetAppEntrances();

            if (B.isEditor) Debug.LogWarning("appEntrances = " + appEntrances);

            toggleRememberUser.isOn = appEntrances > 0 ? SaveLoad.GetRememberCredentialsState() : true;

            if (toggleRememberUser.isOn)
            {
                if (SaveLoad.LoadUserCredentials(out string user, out string pass))
                {
                    inpSignInUser.text = user; inpSignInPass.text = pass;
                }
            }

            btnShowSignIn.onClick.AddListener(ShowSignInPanel);
            btnShowSignUp.onClick.AddListener(ShowSignUpPanel);
            btnCancelSignIn.onClick.AddListener(CancelSignInUser);
            btnCancelSignUp.onClick.AddListener(CancelSignUpUser);

            GlobalActionsUI.OnPanelSignUpCancel += PanelSignTypeSelectShow;
            GlobalActionsUI.OnSignUpSubmit += PanelSignTypeSelectShow;
            GlobalActionsUI.OnLoginShow += ShowLogin;
            GlobalActionsUI.OnShowMenuAreas += HideLogin;

            panelSignUp.SetActive(true);
            panelSignIn.SetActive(true);

            // ----------- Andrew ----------- //
            // Create default user
            if (User.LoadUserByUsername(defaultUserUsername) == null)
            {
                User defaultUser = new User(defaultUserUsername, defaultUserPassword);
                User.Save(defaultUser);
            }

            // Init new user
            newUser = null;

            // Subscribe
            SubscribeButtons();
            SubscribeToEvents();

            // Set language
            SetTextsLanguage();
            SetLanguageButtonIcon();
        }

        private IEnumerator Start()
        {
            PanelSignTypeSelectShow();            
            yield return new WaitForSeconds(0.15f);
            panelSignUp.SetActive(false);
            //panelSignIn.SetActive(false);
            verticalLayoutGroupLoginPage.enabled = true;
            yield break;
        }

        void ShowLogin() {
            loginPanel.SetActive(true);
            animLogin.SetBool("show", true);

            ShowSignInPanel();

            AppManager.Instance.SetMode(AppManager.AppMode.LOGIN);
        }
        void HideLogin() { StartCoroutine(DelayCloseLoginPanel()); }
        IEnumerator DelayCloseLoginPanel()
        {
            animLogin.SetBool("show", false);
            yield return new WaitForSeconds(0.7f);
            loginPanel.SetActive(false);
            panelSignUp.SetActive(false);
            panelSignIn.SetActive(false);
        }

        void PanelSignTypeSelectShow() {
            //panelSelectSignInUp.SetActive(true);
            panelSignIn.SetActive(true);
            animSignIn.SetBool("show", true);
        }

        void ShowSignInPanel() {
            if (B.isEditor) Debug.LogWarning("open sign in panel");
            panelSignIn.SetActive(true);
            animSignIn.SetBool("show", true);
            animSignUp.SetBool("show", false);
            Invoke("HideSignUpPanel", 0.7f);
            //panelSelectSignInUp.SetActive(false);
        }

        void HideSignUpPanel() { panelSignUp.SetActive(false); }
        void HideSignInPanel() { panelSignIn.SetActive(false); }

        void CancelSignInUser()
        {
           // animSignIn.SetBool("show", false);
            //Invoke("HideSignInPanel", 0.7f);
           // animSignUp.SetBool("show", false);
           // Invoke("HideSignUpPanel", 0.7f);
            //panelSelectSignInUp.SetActive(true);
        }

        void ShowSignUpPanel() {
            if (B.isEditor) Debug.LogWarning("open sign up panel");
            panelSignUp.SetActive(true);
            animSignUp.SetBool("show", true);
            // panelSelectSignInUp.SetActive(false);
            animSignIn.SetBool("show", false);
            Invoke("HideSignInPanel", 0.7f);

            // Initialize or Reset survey
            AppManager.Instance.surveyManager.InitializeSurvey();

            // Reset input fields
            ResetInputFields();
        }

        void CancelSignUpUser()
        {
            //animSignIn.SetBool("show", false);
            //Invoke("HideSignInPanel", 0.7f);

            panelSignIn.SetActive(true);
            animSignIn.SetBool("show", true);
            animSignUp.SetBool("show", false);
            Invoke("HideSignUpPanel", 0.7f);
            //panelSelectSignInUp.SetActive(true);
        }

        void ServerLoginUser()
        {
            if (!CheckInputLoginValidation()) return;

            SaveLoad.SaveRememberCredentialsState(toggleRememberUser.isOn);
            if (toggleRememberUser.isOn) { SaveLoad.SaveUserCredentials(inpSignInUser.text, inpSignInPass.text); }

            //server request
            ServerController.Instance.LoginUser(inpSignInUser.text, inpSignInPass.text);
        }

        void ServerSubmitSignUp()
        {
            Debug.Log("ServerSubmitSignUp");
            if (!CheckInputSignUpValidation()) return;

            if (B.isEditor)
            {
                string val = "User= " + inpSignUpUser.text + " Pass= " + inpSignUpPass.text;
                Debug.Log(val);
            }

            ServerController.Instance.SignUpUser(inpSignUpUser.text, inpSignUpPass.text);
            //GlobalActionsUI.OnSignUpSubmit?.Invoke();
        }

        private bool CheckInputLoginValidation()
        {
            if (string.IsNullOrEmpty(inpSignInUser.text))
            {
                ShowMessage(StaticData.termUsernameEmptyField);
                return false;
            }

            if (string.IsNullOrEmpty(inpSignInPass.text))
            {
                ShowMessage(StaticData.termPasswordEmptyField);
                return false;
            }

            return true;
        }

        private bool CheckInputSignUpValidation()
        {
            if (string.IsNullOrEmpty(inpSignUpUser.text))
            {
                ShowSignUpMessage(StaticData.termUsernameEmptyField);
                return false;
            }

            if (string.IsNullOrEmpty(inpSignUpPass.text) || string.IsNullOrEmpty(inpSignUpPassCheck.text))
            {
                ShowSignUpMessage(StaticData.termPasswordEmptyField);
                return false;
            }

            if (inpSignUpPass.text != inpSignUpPassCheck.text)
            {
                ShowSignUpMessage(StaticData.termPasswordsMismatch);
                return false;
            }

            return true;
        }

        void LoginAnonymous()
        {
            Debug.Log("LoginAnonymous");
            GlobalActionsUI.OnLoginAnonymous?.Invoke();
            GlobalActionsUI.OnShowMenuAreas?.Invoke();
            ServerController.Instance.LoginAnonymous();
        }

        #region Andrew Methods
        void SubscribeButtons()
        {
            btnSaveSignUp.onClick.AddListener(SaveSignUp);
            btnSaveSurvey.onClick.AddListener(SaveSurvey);
            btnLoginSubmit.onClick.AddListener(SignIn);
            btnLanguage.onClick.AddListener(() => ChangeLanguage());
        }

        void SubscribeToEvents()
        {
            GlobalActionsUI.OnLangChanged += SetTextsLanguage;
            GlobalActionsUI.OnLangChanged += SetLanguageButtonIcon;
        }

        void SaveSignUp()
        {
            // Check if input is valid
            if (!CheckInputSignUpValidation())
                return;

            // Check if user already exists
            User loadedUser = User.LoadUserByUsername(inpSignUpUser.text);
            if (loadedUser != null)
            {
                /*Debug.Log("User already exists!");
                Debug.Log("loadedUser.server_user_id : " + loadedUser.server_user_id);
                Debug.Log("loadedUser.local_user_id : " + loadedUser.local_user_id);
                Debug.Log("loadedUser.username : " + loadedUser.username);
                Debug.Log("loadedUser.password : " + loadedUser.password);*/

                // Show message: User already exists
                ShowSignUpMessage(StaticData.userAlreadyExists);

                /*// Load and show survey
                Survey loadedSurvey = Survey.LoadSurveyByLocalUserId(loadedUser.local_user_id);
                if (loadedSurvey != null)
                {
                    foreach (int question in loadedSurvey.answersDictionary.Keys)
                    {
                        Debug.Log("Question : " + question);
                        foreach (int answer in loadedSurvey.answersDictionary[question])
                        {
                            Debug.Log("Answer : " + answer);
                        }
                    }
                }*/
                return;
            }

            // Save new user locally
            newUser = new User(inpSignUpUser.text, inpSignUpPass.text);
            User.Save(newUser);

            // Set currentUser
            AppManager.Instance.currentUser = newUser;

            // Show message: Sign up complete
            ShowSignUpMessage(StaticData.signUpComplete, Color.green);
        }

        void SaveSurvey()
        {
            if (newUser != null)
            {
                // Save new survey locally
                AppManager.Instance.surveyManager.SaveSurvey(newUser);
            }
            else
            {
                User emptyUser = new User(string.Empty, string.Empty);

                // Save new survey locally
                AppManager.Instance.surveyManager.SaveSurvey(emptyUser);
            }

            // Change UI
            GlobalActionsUI.OnShowMenuAreas?.Invoke();
            GlobalActionsUI.OnUserLoggedIn?.Invoke();
        }

        void SignIn()
        {
            // Check if input is valid
            if (!CheckInputLoginValidation())
                return;

            // Check if user exists
            User loadedUser = User.LoadUserByUsername(inpSignInUser.text);
            if (loadedUser != null)
            {
                // Check password
                if (loadedUser.password.Equals(inpSignInPass.text))
                {
                    // Set currentUser
                    AppManager.Instance.currentUser = loadedUser;

                    // Change UI
                    GlobalActionsUI.OnShowMenuAreas?.Invoke();
                    GlobalActionsUI.OnUserLoggedIn?.Invoke();
                }
                else
                {
                    //Reset current user
                    AppManager.Instance.currentUser = null;

                    // Show message: User not found
                    ShowMessage(StaticData.wrongPassword);
                }
            }
            else
            {
                // Show message: User not found
                ShowMessage(StaticData.userNotFound);
            }
        }

        void ResetInputFields()
        {
            inpSignUpUser.text = string.Empty;
            inpSignUpPass.text = string.Empty;
            inpSignUpPassCheck.text = string.Empty;
        }

        void SetTextsLanguage()
        {
            textSurveyIntro.text = AppData.Instance.FindTermValue(StaticData.surveyIntro);
            textSurveyOutro.text = AppData.Instance.FindTermValue(StaticData.surveyOutro);

            inpSignInUser.placeholder.GetComponent<Text>().text = AppData.Instance.FindTermValue(StaticData.signInUserPlaceholder);
            inpSignInPass.placeholder.GetComponent<Text>().text = AppData.Instance.FindTermValue(StaticData.signInPasswordPlaceholder);
            btnLoginSubmit.gameObject.GetComponentInChildren<Text>().text = AppData.Instance.FindTermValue(StaticData.btnLoginSubmit);
            toggleRememberUser.gameObject.GetComponentInChildren<Text>().text = AppData.Instance.FindTermValue(StaticData.toggleRememberUser);
            btnShowSignUp.gameObject.GetComponentInChildren<Text>().text = AppData.Instance.FindTermValue(StaticData.btnShowSignUp);
            btnLoginAnonymous.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = AppData.Instance.FindTermValue(StaticData.btnLoginAnonymous);

            inpSignUpUser.placeholder.GetComponent<Text>().text = AppData.Instance.FindTermValue(StaticData.signInUserPlaceholder);
            inpSignUpPass.placeholder.GetComponent<Text>().text = AppData.Instance.FindTermValue(StaticData.signInPasswordPlaceholder);
            inpSignUpPassCheck.placeholder.GetComponent<Text>().text = AppData.Instance.FindTermValue(StaticData.signUpPasswordCheck);
            btnSignUpSubmit.gameObject.GetComponentInChildren<Text>().text = AppData.Instance.FindTermValue(StaticData.btnSignUpSubmit);
            btnSaveSurvey.gameObject.GetComponentInChildren<Text>().text = AppData.Instance.FindTermValue(StaticData.btnSaveSurvey);
            textInput.text = AppData.Instance.FindTermValue(StaticData.signUpInput);
        }

        void ChangeLanguage()
        {
            //// Get current language
            //bool isEng = StaticData.lang == "en";

            //// Change lang
            //StaticData.lang = isEng ? "gr" : "en";
            //PlayerPrefs.SetString("Lang", StaticData.lang);
            //PlayerPrefs.Save();

            //// Get terms
            //AppData.Instance.Init();
            //GlobalActionsUI.OnLangChanged?.Invoke();

            MenuPanel.Instance.ChangeLanguage();
        }

        void SetLanguageButtonIcon()
        {
            // Get current language
            bool isEng = StaticData.lang == "en";

            // Change icon
            iconBtnLanguage.sprite = isEng ? sprEng : sprGR;
        }
        #endregion

        #region Messages

        void ShowMessage(string val)
        {
            txtMessageLogin.text = AppData.Instance.FindTermValue(val);
            //txtMessageLogin.gameObject.SetActive(true);
            StartCoroutine(HideMessageDelayded());
        }

        void HideMessage() { StopAllCoroutines(); txtMessageLogin.text = string.Empty; /*txtMessageLogin.gameObject.SetActive(false);*/ }

        IEnumerator HideMessageDelayded()
        {
            yield return new WaitForSeconds(2f);
            HideMessage();
            yield break;
        }

        void ShowSignUpMessage(string val)
        {
            txtMessageSignUp.text = AppData.Instance.FindTermValue(val);

            // Set color
            txtMessageSignUp.color = Color.red;

            //txtMessageSignUp.gameObject.SetActive(true);
            StartCoroutine(HideSignUpMessageDelayded());
        }

        void ShowSignUpMessage(string val, Color _color)
        {
            txtMessageSignUp.text = AppData.Instance.FindTermValue(val);

            // Set color
            txtMessageSignUp.color = _color;

            txtMessageSignUp.gameObject.SetActive(true);
            StartCoroutine(HideSignUpMessageDelayded());
        }

        void HideSignUpMessage() { StopAllCoroutines(); txtMessageSignUp.text = string.Empty; /*txtMessageSignUp.gameObject.SetActive(false);*/ }

        IEnumerator HideSignUpMessageDelayded()
        {
            yield return new WaitForSeconds(2f);
            HideSignUpMessage();
            yield break;
        }

        #endregion

    }

}
