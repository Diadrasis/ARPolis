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

        [ContextMenu("HidePanel")]
        void HidePanel()
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

            if (B.isEditor) Debug.Log("Move from "+targetRect.anchoredPosition+" to "+ hidePos);
            StartCoroutine(HideLerpPanel(hidePos));
        }


        IEnumerator HideLerpPanel(Vector2 hidePos)
        {
            //auto set speed from width
            if (hidePos.x > 0)
            {
                moveSpeedCustom = hidePos.x + (hidePos.x / 2f);
            }
            else if (hidePos.y > 0)
            {
                moveSpeedCustom = hidePos.y + (hidePos.y / 2f);
            }
            yield return new WaitForEndOfFrame();
            while (Vector2.Distance(targetRect.anchoredPosition, hidePos) > 10f)
            {
                targetRect.anchoredPosition = Vector2.MoveTowards(targetRect.anchoredPosition, hidePos, Time.smoothDeltaTime * moveSpeedCustom);
                yield return null;
            }
            targetRect.anchoredPosition = hidePos;

            yield break;
        }
    }

}
