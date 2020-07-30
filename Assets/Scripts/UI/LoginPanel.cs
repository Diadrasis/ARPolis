using ARPolis.Data;
using ARPolis.Info;
using ARPolis.Save;
using ARPolis.Server;
using StaGeUnityTools;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

            btnLoginSubmit.onClick.AddListener(ServerLoginUser);
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
            GlobalActionsUI.OnShowMenuAreas?.Invoke();
            ServerController.Instance.LoginAnonymous();
        }

        #region Messages

        void ShowMessage(string val)
        {
            txtMessageLogin.text = AppData.Instance.FindTermValue(val);
            txtMessageLogin.gameObject.SetActive(true);
            StartCoroutine(HideMessageDelayded());
        }

        void HideMessage() { StopAllCoroutines(); txtMessageLogin.gameObject.SetActive(false); }

        IEnumerator HideMessageDelayded()
        {
            yield return new WaitForSeconds(2f);
            HideMessage();
            yield break;
        }

        void ShowSignUpMessage(string val)
        {
            txtMessageSignUp.text = AppData.Instance.FindTermValue(val);
            txtMessageSignUp.gameObject.SetActive(true);
            StartCoroutine(HideSignUpMessageDelayded());
        }

        void HideSignUpMessage() { StopAllCoroutines(); txtMessageSignUp.gameObject.SetActive(false); }

        IEnumerator HideSignUpMessageDelayded()
        {
            yield return new WaitForSeconds(2f);
            HideSignUpMessage();
            yield break;
        }

        #endregion

    }

}
