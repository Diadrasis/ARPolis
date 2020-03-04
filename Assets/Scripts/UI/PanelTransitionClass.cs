using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaGeUnityTools
{
    //[ExecuteAlways]
   // [ExecuteInEditMode]
    public class PanelTransitionClass : MonoBehaviour
    {

        public StaGeUnityTools.Tools_UI.Mode sideMode = StaGeUnityTools.Tools_UI.Mode.none;
        [Header("Target RectTransform To Move - If empty then nothing happens.")]
        public RectTransform targetRect;
        
        [Header("True: Calculate Speed - False: Use moveSpeedCustom.")]//, order =1)]
        //[Header("False: Use moveSpeedCustom.", order = 2)]
        public bool isAutoSpeed = true;

        [Header("Custom Move Speed")]
        public float moveSpeedCustom = 250f;

        Vector2 panelInitPosition, panelHiddenPosition;

        public void Init(RectTransform rect, Tools_UI.Mode mode, bool isVisibleAtStart)
        {
            panelInitPosition = targetRect.anchoredPosition;
            targetRect = rect;
            sideMode = mode;
            panelHiddenPosition = HidePosition();
            CalculateSpeed();


            if (!isVisibleAtStart)
            {
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
            if(targetRect.anchoredPosition == panelHiddenPosition) { ShowPanel(); } else { HidePanel(); }
        }

        public void HidePanel()
        {
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

            yield break;
        }

        public void ShowPanel()
        {
            StartCoroutine(ShowLerpPanel());
        }

        IEnumerator ShowLerpPanel()
        {
            yield return new WaitForEndOfFrame();
            while (Vector2.Distance(targetRect.anchoredPosition, panelInitPosition) > 10f)
            {
                targetRect.anchoredPosition = Vector2.MoveTowards(targetRect.anchoredPosition, panelInitPosition, Time.smoothDeltaTime * moveSpeedCustom);
                yield return null;
            }
            targetRect.anchoredPosition = panelInitPosition;

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


    }

}
