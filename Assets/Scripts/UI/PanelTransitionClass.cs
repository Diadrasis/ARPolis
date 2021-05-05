using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaGeUnityTools
{
    //[ExecuteAlways]
   // [ExecuteInEditMode]
    public class PanelTransitionClass : MonoBehaviour
    {

        public Tools_UI.Mode sideMode = Tools_UI.Mode.none;
        [Header("Target RectTransform To Move - If empty then nothing happens.")]
        public RectTransform targetRect;
        
        [Header("True: Calculate Speed - False: Use moveSpeedCustom.")]//, order =1)]
        //[Header("False: Use moveSpeedCustom.", order = 2)]
        public bool isAutoSpeed = true;

        [Header("Custom Move Speed")]
        public float moveSpeedCustom = 250f;

        [Header("Custom Move Position")]
        public float movePercentage = 100f;
        public bool isMoveByPercentage;

        [Header("Custom Space StartPos")]
        public float spaceStartPos = 0f;
        [Header("Custom Space Corner Up or Down Pos")]
        public float spaceUpDown = 0f;

        public enum MoveMode { Hidden, Percentage, Full}
        public MoveMode moveMode = MoveMode.Hidden;

        public bool isVisible;

        private float GetViewPercentage() { return movePercentage / 100f; }
        Vector2 panelInitPosition, panelHiddenPosition, panelPercentagePosition;

        public UnityEngine.Events.UnityEvent OnHide, OnShowFull, OnShowPercentage;


        public void Init(RectTransform rect, Tools_UI.Mode mode, bool isVisibleAtStart)
        {
            if (targetRect == null) targetRect = GetComponent<RectTransform>();
            panelInitPosition = targetRect.anchoredPosition;

            SetPercentagePosition();

            targetRect = rect;
            sideMode = mode;
            panelHiddenPosition = HidePosition();
            CalculateSpeed();

            if (!isVisibleAtStart)
            {
                moveMode = MoveMode.Hidden;
                if (B.isEditor)
                {
                    HidePanel();
                }
                else
                {
                    targetRect.anchoredPosition = panelHiddenPosition;
                }
            }
        }

        public void TogglePanelAppearance()
        {
            if (moveMode == MoveMode.Hidden) { ShowPanel(); } else { HidePanel(); }
        }

        public void HidePanel()
        {
            //if (moveMode == MoveMode.Hidden) return;
            moveMode = MoveMode.Hidden;
            StartCoroutine(HideLerpPanel());
        }
        
        IEnumerator HideLerpPanel()
        {
            yield return new WaitForEndOfFrame();
            while (Vector2.Distance(targetRect.anchoredPosition, panelHiddenPosition) > 10f)
            {
                targetRect.anchoredPosition = Vector2.MoveTowards(targetRect.anchoredPosition, panelHiddenPosition, Time.smoothDeltaTime * moveSpeedCustom);
                yield return null;
            }
            targetRect.anchoredPosition = panelHiddenPosition;
            OnHide?.Invoke();
            isVisible = false;
            yield break;
        }

        public void ShowPanel()
        {
            if (moveMode == MoveMode.Full) return;
            moveMode = MoveMode.Full;
            StartCoroutine(ShowLerpPanel(panelInitPosition, true));
        }

        public void ShowPercentagePanel()
        {
            if (moveMode == MoveMode.Percentage) return;
            moveMode = MoveMode.Percentage;
            StartCoroutine(ShowLerpPanel(panelPercentagePosition, false));
        }

        IEnumerator ShowLerpPanel(Vector2 vectorTarget, bool isFull)
        {
            yield return new WaitForEndOfFrame();
            while (Vector2.Distance(targetRect.anchoredPosition, vectorTarget) > 10f)
            {
                targetRect.anchoredPosition = Vector2.MoveTowards(targetRect.anchoredPosition, vectorTarget, Time.smoothDeltaTime * moveSpeedCustom);
                yield return null;
            }
            targetRect.anchoredPosition = vectorTarget;
            if (isFull) { OnShowFull?.Invoke(); } else { OnShowPercentage?.Invoke(); }
            isVisible = true;
            yield break;
        }

        void CalculateSpeed()
        {
            //auto set speed from width
            if (isAutoSpeed)
            {
                Vector2 pos = panelHiddenPosition;// HidePosition();
                if (pos.x != 0)
                {
                    moveSpeedCustom = Mathf.Abs(3f * pos.x);// + (pos.x / 2f));
                }
                else if (pos.y != 0)
                {
                    moveSpeedCustom = Mathf.Abs(3f * pos.y);// + (pos.y / 2f));
                }
            }
            else
            {
                if (moveSpeedCustom < 0f) moveSpeedCustom = Mathf.Abs(moveSpeedCustom);
            }
        }

        void SetPercentagePosition()
        {
            if (isMoveByPercentage)
            {
                float Y = targetRect.rect.height - (targetRect.rect.height * GetViewPercentage());
                float X = targetRect.rect.width - (targetRect.rect.width * GetViewPercentage());

                switch (sideMode)
                {
                    case Tools_UI.Mode.none:
                        break;
                    case Tools_UI.Mode.center:
                        break;
                    case Tools_UI.Mode.downCenter:
                        panelPercentagePosition = new Vector2(0f, -Y);
                        break;
                    case Tools_UI.Mode.downLeft:
                        panelPercentagePosition = new Vector2(-X, 0f);
                        break;
                    case Tools_UI.Mode.downRight:
                        panelPercentagePosition = new Vector2(X, 0f);
                        break;
                    case Tools_UI.Mode.upCenter:
                        panelPercentagePosition = new Vector2(0f, Y);
                        break;
                    case Tools_UI.Mode.upLeft:
                        panelPercentagePosition = new Vector2(-X, 0f);
                        break;
                    case Tools_UI.Mode.upRight:
                        panelPercentagePosition = new Vector2(X, 0f);
                        break;
                    case Tools_UI.Mode.leftCenter:
                        panelPercentagePosition = new Vector2(-X, 0f);
                        break;
                    case Tools_UI.Mode.rightCenter:
                        panelPercentagePosition = new Vector2(X, 0f);
                        break;
                    default:
                        break;
                }

                //panelInitPosition = panelPercentagePosition;

            }
            else
            {
                //panelInitPosition = targetRect.anchoredPosition;
            }
        }

        #region simple version
        /*
        Vector2 HidePosition()
        {
            Vector2 hidePos = targetRect.anchoredPosition;

            switch (sideMode)
            {
                case Tools_UI.Mode.none:
                    break;
                case Tools_UI.Mode.center:
                    break;
                case Tools_UI.Mode.downCenter:
                    hidePos = new Vector2(0f, -targetRect.rect.height);
                    break;
                case Tools_UI.Mode.downLeft:
                    hidePos = new Vector2(-targetRect.rect.width, 0f);
                    break;
                case Tools_UI.Mode.downRight:
                    hidePos = new Vector2(targetRect.rect.width, 0f);
                    break;
                case Tools_UI.Mode.upCenter:
                    hidePos = new Vector2(0f, targetRect.rect.height);
                    break;
                case Tools_UI.Mode.upLeft:
                    hidePos = new Vector2(-targetRect.rect.width, 0f);
                    break;
                case Tools_UI.Mode.upRight:
                    hidePos = new Vector2(targetRect.rect.width, 0f);
                    break;
                case Tools_UI.Mode.leftCenter:
                    hidePos = new Vector2(-targetRect.rect.width, 0f);
                    break;
                case Tools_UI.Mode.rightCenter:
                    hidePos = new Vector2(targetRect.rect.width, 0f);
                    break;
                default:
                    break;
            }

            return hidePos;
        }
        */
        #endregion

        Vector2 HidePosition()
        {
            Vector2 hidePos = targetRect.anchoredPosition;

            switch (sideMode)
            {
                case Tools_UI.Mode.none:
                    break;
                case Tools_UI.Mode.center:
                    break;
                case Tools_UI.Mode.downCenter:
                    hidePos = new Vector2(0f, -targetRect.rect.height);
                    panelInitPosition.y = panelInitPosition.x + spaceStartPos;
                    break;
                case Tools_UI.Mode.downLeft:
                    hidePos = new Vector2(-targetRect.rect.width, spaceUpDown);
                    panelInitPosition.x = panelInitPosition.x + spaceStartPos;
                    panelInitPosition.y = panelInitPosition.y + spaceUpDown;
                    break;
                case Tools_UI.Mode.downRight:
                    hidePos = new Vector2(targetRect.rect.width, spaceUpDown);
                    panelInitPosition.x = panelInitPosition.x - spaceStartPos;
                    panelInitPosition.y = panelInitPosition.y + spaceUpDown;
                    break;
                case Tools_UI.Mode.upCenter:
                    hidePos = new Vector2(0f, targetRect.rect.height);
                    panelInitPosition.y = panelInitPosition.x - spaceStartPos;
                    break;
                case Tools_UI.Mode.upLeft:
                    hidePos = new Vector2(-targetRect.rect.width, -spaceUpDown);
                    panelInitPosition.x = panelInitPosition.x + spaceStartPos;
                    panelInitPosition.y = panelInitPosition.y - spaceUpDown;
                    break;
                case Tools_UI.Mode.upRight:
                    hidePos = new Vector2(targetRect.rect.width, -spaceUpDown);
                    panelInitPosition.x = panelInitPosition.x - spaceStartPos;
                    panelInitPosition.y = panelInitPosition.y - spaceUpDown;
                    break;
                case Tools_UI.Mode.leftCenter:
                    hidePos = new Vector2(-targetRect.rect.width, 0f);
                    panelInitPosition.x = panelInitPosition.x + spaceStartPos;
                    break;
                case Tools_UI.Mode.rightCenter:
                    hidePos = new Vector2(targetRect.rect.width, 0f);
                    panelInitPosition.x = panelInitPosition.x - spaceStartPos;
                    break;
                default:
                    break;
            }

            return hidePos;
        }

    }

}
