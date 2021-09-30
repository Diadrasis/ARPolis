using ARPolis.Data;
using ARPolis.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ARPolis
{
    public class HelpController : Singleton<HelpController>
    {
        protected HelpController() { }

        public Transform myPanel;
        [Space]
        public Button btnCloseHelp;
        [Space]
        public float pageViewTime = 1f;

        [Space]
        public Texture2D[] greekPages, engPages;

        [HideInInspector]
        public List<Animator> animPages = new List<Animator>();
        [HideInInspector]
        public List<RawImage> rawImages = new List<RawImage>();

        private Coroutine coroutine;
        [SerializeField]
        int currentPage = 0;

        private void Awake()
        {
            #region Find UI elements
            if (myPanel == null)
            {
                GameObject canv = GameObject.Find("Canvas");
                if (canv)
                {
                    myPanel = canv.transform.Find("HelpPanel");
                    if (myPanel)
                    {
                        if(btnCloseHelp == null)
                        {
                            Transform btn = myPanel.transform.Find("btnClose");
                            if (btn)
                            {
                                btnCloseHelp = btn.GetComponent<Button>();
                            }
                            else
                            {
                                Debug.LogError("[HelpController] Error: missing UI elements!");
                                return;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("[HelpController] Error: missing UI elements!");
                        return;
                    }
                }
                else
                {
                    Debug.LogError("[HelpController] Error: missing UI elements!");
                    return;
                }
            }
            #endregion

            if (myPanel) animPages = myPanel.GetComponentsInChildren<Animator>(true).ToList();
            if (animPages.Count > 0)
            {
                for(int i = 1; i<animPages.Count; i++)
                {
                    RawImage rawimg = animPages[i].GetComponentInChildren<RawImage>();
                    if (rawimg) rawImages.Add(rawimg);
                }
            }

            myPanel.gameObject.SetActive(false);
            btnCloseHelp.onClick.AddListener(CloseHelp);

            GlobalActionsUI.OnLangChanged += OnLangChanged;

            OnLangChanged();

            if (Application.isEditor)
            {
                CheckForErrors();
            }
        }

        void CheckForErrors()
        {
            if (greekPages.Length != engPages.Length)
            {
                Debug.LogError("Greek textures are not the same with English textures");
                UnityEditor.EditorApplication.isPlaying = false;
            }
            if (greekPages.Length != animPages.Count-1 || engPages.Length != animPages.Count-1)
            {
                Debug.LogError("Texures are not as many as pages!");
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        void OnLangChanged()
        {
            for (int i = 0; i < rawImages.Count; i++)
            {
                rawImages[i].texture = StaticData.IsLangGR() ? greekPages[i] : engPages[i];
            }
        }

        void CloseHelp()
        {
            if (coroutine != null) StopCoroutine(coroutine);
            animPages.ForEach(b => b.SetBool("show", false));
            StopAllCoroutines();
            myPanel.gameObject.SetActive(false);
        }

        [ContextMenu("Show Help")]
        public void ShowHelp()
        {
            if (animPages.Count <= 0) return;
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(PlayHelp());
        }

        IEnumerator PlayHelp()
        {
            myPanel.gameObject.SetActive(true);
            animPages[0].SetBool("show", true);
            for (int i = 1; i < animPages.Count; i++)
            {
                currentPage = i;
                animPages[i].SetBool("show", true);
                yield return new WaitForSeconds(pageViewTime);
                animPages[i].SetBool("show", false);
            }
            currentPage = 0;
            myPanel.gameObject.SetActive(false);
            yield break;
        }

    }

}
