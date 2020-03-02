using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace StaGeUnityTools
{

    public class AutoFitToCanvas : MonoBehaviour
    {

        public RectTransform target, kanvas;
        public bool isWidthRelative, isHeightRelative;

        [HideInInspector]
        public float widthPercent, heightPercent;

        public float WidthFinal
        {
            get { return (kanvas.sizeDelta.x * widthPercent) / 100f; }
        }

        public float HeightFinal
        {
            get { return (kanvas.sizeDelta.y * heightPercent) / 100f; }
        }

        public bool isMovable;

        //the size of canvas during development
        Vector2 initKanvasSize;// = new Vector2(1080f, 1920f);
        bool hasLayoutElement;
        LayoutElement layOutElem;

        public StaGeUnityTools.Tools_UI.Mode sideMode = StaGeUnityTools.Tools_UI.Mode.none;

        //offset

        private void OnEnable()
        {
            
            Init();
        }

        [ContextMenu("Test Layout")]
        public void Init()
        {
            if (target == null) target = GetComponent<RectTransform>();
            if (kanvas == null) kanvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();

            initKanvasSize = kanvas.sizeDelta;

            layOutElem = target.gameObject.GetComponent<LayoutElement>();
            if (layOutElem != null && !hasLayoutElement)
            {
                hasLayoutElement = true;
            }

            Vector2 size = initKanvasSize;

            if (widthPercent > 0) size.x = (size.x * widthPercent) / 100f;
            if (heightPercent > 0) size.y = (size.y * heightPercent) / 100f;

            if (isWidthRelative)
            {

                float val = (size.x / initKanvasSize.x) * target.sizeDelta.x;

                if (hasLayoutElement)
                {
                    layOutElem.minWidth = val;
                    layOutElem.preferredWidth = val;
                }
                else
                {
                    size.x = val;
                }
            }

            if (isHeightRelative)
            {

                float val = (size.y / initKanvasSize.y) * target.sizeDelta.y;

                if (hasLayoutElement)
                {
                    layOutElem.minHeight = val;
                    layOutElem.preferredHeight = val;
                }
                else
                {
                    size.y = val;
                }
            }
            
            target.sizeDelta = size;

            StaGeUnityTools.Tools_UI.Move(target, sideMode);

            target.anchoredPosition = Vector3.zero;

            ForceRebuildLayout();

            if (isMovable)
            {
                PanelTransitionClass transitionClass = target.gameObject.GetComponent<PanelTransitionClass>();
                if(transitionClass==null) transitionClass = target.gameObject.AddComponent<PanelTransitionClass>();
                transitionClass.targetRect = target;
                transitionClass.sideMode = sideMode;
            }
        }


        void ForceRebuildLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        }

    }

}