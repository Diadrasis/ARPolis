using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StaGeUnityTools
{
    
    public class PanelSize : MonoBehaviour
    {

        public delegate void PoiInfoAction(bool isOn);
        public static PoiInfoAction OnMoveToFullStart, OnMoveComplete;
        public delegate void PoiInfoSizeAction(float width);
        public static PoiInfoSizeAction OnResize;
        public delegate void PoiPanelAction();
        public static PoiPanelAction OnStartMoving;

        public RectTransform fullScreenPanel, poiPanel, scrollContainer, rectArrow;
        RectTransform thisRectTransform;
        public Vector2 parentSize;
        public float finalPanelWidth;

        public Image poiPanelImage;

        public float moveSpeed = 10f;

        bool isHiding = true;


        private void Awake()
        {
            thisRectTransform = GetComponent<RectTransform>();
            parentSize = fullScreenPanel.rect.size;
            finalPanelWidth = parentSize.x * 0.7f;
            thisRectTransform.sizeDelta = new Vector2(finalPanelWidth, parentSize.y);
            //Draggable.OnDragEnd += LerpMovePanel;
            //PoiInfoPanel.OnInfoLoaded += ShowPanel;
            //PoiInfoPanel.OnReturnFromSelectedCategory += HideInstant;
            OnlineMaps.instance.OnChangePosition += HideInstant;
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha1)) { ShowPanel(); }
        //    if (Input.GetKeyDown(KeyCode.Alpha0)) { HidePanel(false); }
        //}

        void HideInstant() { if (!isHiding) { HidePanel(false); } }

        void HidePanel(bool immediately)
        {
            //if (B.isEditor) Debug.LogWarning("HidePanel");
            isHiding = true;
            poiPanelImage.raycastTarget = false;

            rectArrow.transform.parent.gameObject.SetActive(false);
            //PoiInfoPanel.sideInfoIsOpen = false;

            if (immediately)
            {
                Vector2 pos = poiPanel.anchoredPosition;
                float widthPanel = poiPanel.rect.width;
                pos.x = widthPanel;
                poiPanel.anchoredPosition = pos;
            }
            else
            {
                StopCoroutine("HideLerpPanel");
                StartCoroutine(HideLerpPanel());
            }

        }

        IEnumerator HideLerpPanel()
        {
            //if (B.isEditor) Debug.LogWarning("HideLerpPanel");
            //Vector2 pos = poiPanel.anchoredPosition;
            float widthPanel = poiPanel.rect.width;
            while (Vector2.Distance(poiPanel.anchoredPosition, new Vector2(widthPanel, 0f)) > 10f)
            {
                poiPanel.anchoredPosition = Vector2.Lerp(poiPanel.anchoredPosition, new Vector2(widthPanel, 0f), Time.deltaTime * moveSpeed);
                yield return null;
            }
            poiPanel.anchoredPosition = new Vector2(widthPanel, 0f);

            yield break;
        }

        void ShowPanel()
        {
            //if (B.isEditor) Debug.LogWarning("ShowPanel");
            poiPanelImage.raycastTarget = false;
            //Vector2 pos = poiPanel.anchoredPosition;
            //pos.x = 0f;
            //poiPanel.anchoredPosition = pos;
            rectArrow.transform.parent.gameObject.SetActive(true);
            //PoiInfoPanel.sideInfoIsOpen = true;
            if (poiPanel.anchoredPosition == Vector2.zero) return;
            StopCoroutine("ShowLerpPanel");
            StartCoroutine(ShowLerpPanel());
        }

        bool isMoving;
        IEnumerator ShowLerpPanel()
        {
            if (isMoving) yield break;
            isMoving = true;
            //if (B.isEditor) Debug.LogWarning("ShowLerpPanel");
            while (Vector2.Distance(poiPanel.anchoredPosition, Vector2.zero) > 10f)
            {
                poiPanel.anchoredPosition = Vector2.Lerp(poiPanel.anchoredPosition, Vector2.zero, Time.deltaTime * moveSpeed);
                yield return null;
            }
            poiPanel.anchoredPosition = Vector2.zero;

            isHiding = false;
            isMoving = false;
            yield break;
        }

        IEnumerator ShowFullLerpPanel()
        {
            if (isMoving) yield break;
            isMoving = true;
            OnMoveToFullStart?.Invoke(true);
            if (B.isEditor) Debug.LogWarning("ShowFullLerpPanel");
            while (Vector2.Distance(poiPanel.anchoredPosition, new Vector2(-finalPanelWidth, 0f)) > 10f)
            {
                poiPanel.anchoredPosition = Vector2.Lerp(poiPanel.anchoredPosition, new Vector2(-finalPanelWidth, 0f), Time.deltaTime * moveSpeed);
                yield return null;
            }
            poiPanel.anchoredPosition = new Vector2(-finalPanelWidth, 0f);

            poiPanelImage.raycastTarget = true;
            poiPanel.anchoredPosition = new Vector2(-finalPanelWidth, 0f);
            OnMoveComplete?.Invoke(true);
            Vector3 arrowRotation = rectArrow.localEulerAngles;
            arrowRotation.z = -180f;
            rectArrow.localEulerAngles = arrowRotation;

            isHiding = false;
            isMoving = false;
            yield break;
        }

        IEnumerator HideFullLerpPanel()
        {
            if (isMoving) yield break;
            isMoving = true;

            if (B.isRealEditor) Debug.LogWarning("HideFullLerpPanel");

            while (Vector2.Distance(poiPanel.anchoredPosition, Vector2.zero) > 10f)
            {
                poiPanel.anchoredPosition = Vector2.Lerp(poiPanel.anchoredPosition, Vector2.zero, Time.deltaTime * moveSpeed);
                yield return null;
            }
            poiPanel.anchoredPosition = Vector2.zero;

            poiPanelImage.raycastTarget = false;
            poiPanel.anchoredPosition = Vector2.zero;
            OnMoveComplete?.Invoke(false);
            OnMoveToFullStart?.Invoke(false);
            ResetScrollPosition();
            Vector3 arrowRotation = rectArrow.localEulerAngles;
            arrowRotation.z = 0f;
            rectArrow.localEulerAngles = arrowRotation;

            isHiding = false;
            isMoving = false;
            yield break;
        }

        public void MovePanel()
        {
            OnStartMoving?.Invoke();

            //if (B.isEditor) Debug.LogWarning("Move Panel");
            // LerpMovePanel(); return;
            Vector2 pos = poiPanel.anchoredPosition;
            Vector3 arrowRotation = rectArrow.localEulerAngles;
            if (pos.x == 0)
            {
                StartCoroutine(ShowFullLerpPanel());

                //stop dragging of map
                //poiPanelImage.raycastTarget = true;
                //poiPanel.anchoredPosition = new Vector2(-finalPanelWidth, 0f);
                //OnMove?.Invoke(true);
                //arrowRotation.z = -180f;
            }
            else
            {
                StartCoroutine(HideFullLerpPanel());
                //poiPanelImage.raycastTarget = false;
                //poiPanel.anchoredPosition = Vector2.zero;
                //OnMove?.Invoke(false);
                //ResetScrollPosition();
                //arrowRotation.z = 0f;
            }
            rectArrow.localEulerAngles = arrowRotation;
        }

        void OnEnable()
        {
            ResetScrollPosition();

            HidePanel(true);

            parentSize = fullScreenPanel.rect.size;
            if (finalPanelWidth != parentSize.x)
            {
                parentSize = fullScreenPanel.rect.size;
                finalPanelWidth = parentSize.x * 0.7f;
                thisRectTransform.sizeDelta = new Vector2(finalPanelWidth, parentSize.y);
            }

            OnResize?.Invoke(finalPanelWidth);
        }

        private void OnDisable()
        {
            isHiding = true;
            StopAllCoroutines();
        }

        void ResetScrollPosition()
        {
            scrollContainer.localPosition = Vector3.zero;
        }

        void LerpMovePanel()
        {
            StopCoroutine("FinalMove");
            StartCoroutine(FinalMove());
        }

        IEnumerator FinalMove()
        {
            Vector2 pos = poiPanel.anchoredPosition;
            Vector3 arrowRotation = rectArrow.localEulerAngles;

            float distFromStart = Vector2.Distance(poiPanel.anchoredPosition, Vector2.zero);
            float distFromEnd = Vector2.Distance(poiPanel.anchoredPosition, new Vector2(-finalPanelWidth, 0f));

            if (distFromEnd > distFromStart)
            {
                OnMoveComplete?.Invoke(false);
                OnMoveToFullStart?.Invoke(false);
                arrowRotation.z = 0f;
                rectArrow.localEulerAngles = arrowRotation;

                while (Vector2.Distance(poiPanel.anchoredPosition, Vector2.zero) > 10f)
                {
                    poiPanel.anchoredPosition = Vector2.Lerp(poiPanel.anchoredPosition, Vector2.zero, Time.deltaTime * moveSpeed);
                    yield return null;
                }
                poiPanel.anchoredPosition = Vector2.zero;
                ResetScrollPosition();
                poiPanelImage.raycastTarget = false;
            }
            else
            {
                OnMoveComplete?.Invoke(true);
                OnMoveToFullStart?.Invoke(true);
                arrowRotation.z = -180f;
                rectArrow.localEulerAngles = arrowRotation;

                while (Vector2.Distance(poiPanel.anchoredPosition, new Vector2(-finalPanelWidth, 0f)) > 10f)
                {
                    poiPanel.anchoredPosition = Vector2.Lerp(poiPanel.anchoredPosition, new Vector2(-finalPanelWidth, 0f), Time.deltaTime * moveSpeed);
                    yield return null;
                }
                poiPanel.anchoredPosition = new Vector2(-finalPanelWidth, 0f);
                //stop dragging of map
                poiPanelImage.raycastTarget = true;
            }


            yield break;
        }
    }

}
