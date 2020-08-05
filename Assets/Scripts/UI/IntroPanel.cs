using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPolis.UI
{

    public class IntroPanel : MonoBehaviour
    {
        public Animator animInfoPanel, animAppLogo, animEspaLogo, animCompanyLogo;
        public GameObject infoPanel, panelBack;

        private void Awake()
        {
            GlobalActionsUI.OnIntroStart += StartIntro;
            panelBack.SetActive(true);
        }

        void StartIntro()
        {
            StartCoroutine(ShowIntro());
        }

        IEnumerator ShowIntro()
        {
            #region Fade
            infoPanel.SetActive(true);
            yield return new WaitForEndOfFrame();
            animInfoPanel.SetBool("show", true);
            yield return new WaitForSeconds(0.5f);
            animAppLogo.SetBool("show", true);
            yield return new WaitForSeconds(2f);
            animEspaLogo.SetBool("show", true);
            yield return new WaitForSeconds(2.5f);
            animEspaLogo.SetBool("show", false);
            yield return new WaitForSeconds(0.5f);
            animCompanyLogo.SetBool("show", true);
            yield return new WaitForSeconds(2.5f);
            panelBack.SetActive(false);
            animCompanyLogo.SetBool("show", false);
            yield return new WaitForSeconds(1.5f);
            animAppLogo.SetBool("show", false);
            animInfoPanel.SetBool("show", false);
            yield return new WaitForEndOfFrame();
            infoPanel.SetActive(false);
            #endregion

            //invoke end of intro
            GlobalActionsUI.OnIntroFinished?.Invoke();
            AppManager.Instance.SetMode(AppManager.AppMode.LOGIN);

            yield break;
        }

    }
}
