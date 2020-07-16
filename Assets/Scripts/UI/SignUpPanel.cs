using StaGeUnityTools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace ARPolis.UI
{

    public class SignUpPanel : MonoBehaviour
    {
        public Button btnCancel, btnSubmit;

        public Text txtMessage;

        public GameObject panelParent;

        public InputField inpUsername, inpPass, inpPassCheck;

        private void Awake()
        {
            btnCancel.onClick.AddListener(() => HidePanel());
            btnSubmit.onClick.AddListener(() => CheckUserFields());

            inpUsername.contentType = InputField.ContentType.EmailAddress;// = InputField.InputType.Standard;
            inpPass.inputType = InputField.InputType.Password;
            inpPassCheck.inputType = InputField.InputType.Password;

        }

        bool IsPassValid()
        {
            return inpPass.text == inpPassCheck.text && !string.IsNullOrEmpty(inpPass.text);
        }

        void ShowPanel()
        {
            OnSnapPrevButtonHide();
            inpPass.text = inpPassCheck.text = inpUsername.text = string.Empty;
        }

        void HidePanel()
        {
            GlobalActionsUI.OnPanelSignUpCancel?.Invoke();
        }

        void CheckUserFields()
        {
            if (!IsPassValid() || string.IsNullOrEmpty(inpUsername.text)) {
                txtMessage.gameObject.SetActive(true);
                Invoke("DelayHideMessage", 2f);
                return;
            }
            ServerSubmitSignUp();
        }

        void DelayHideMessage()
        {
            txtMessage.gameObject.SetActive(false);
            CancelInvoke();
        }

        void ServerSubmitSignUp()
        {
            string val = "User= "+inpUsername.text+ " Pass= "+inpPass.text;

            if (B.isEditor) Debug.Log(val);

            GlobalActionsUI.OnSignUpSubmit?.Invoke();
        }

        void OnSnapButtonsShow() {
            btnCancel.gameObject.SetActive(false);
            btnSubmit.gameObject.SetActive(false);
        }
        void OnSnapNextButtonHide() {
            btnSubmit.gameObject.SetActive(true);
            btnSubmit.interactable = false;
            Invoke("DelayShowSubmit", 0.5f);
        }
        void DelayShowSubmit() { btnSubmit.interactable = true; CancelInvoke(); }
        void OnSnapPrevButtonHide() {
            btnCancel.gameObject.SetActive(true);
            btnCancel.interactable = false;
            Invoke("DelayShowCancel", 0.5f);
        }
        void DelayShowCancel() { btnCancel.interactable = true; CancelInvoke(); }
    }
}
