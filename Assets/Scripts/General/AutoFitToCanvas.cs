using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace StaGeUnityTools
{

    public class AutoFitToCanvas : MonoBehaviour
    {

        public RectTransform target, kanvas;
        public bool isWidthRelative, isHeightRelative, delayRefresh;

        [HideInInspector]
        public float widthPercent, heightPercent, fixedWidth, fixedHeight;

        public float WidthFinal
        {
            get { return (kanvas.sizeDelta.x * widthPercent) / 100f; }
        }

        public float HeightFinal
        {
            get { return (kanvas.sizeDelta.y * heightPercent) / 100f; }
        }

        public bool isMovable, isVisibleOnStart;

        //the size of canvas during development
        Vector2 initKanvasSize;// = new Vector2(1080f, 1920f);
        bool hasLayoutElement;
        LayoutElement layOutElem;

        public StaGeUnityTools.Tools_UI.Mode sideMode = StaGeUnityTools.Tools_UI.Mode.none;

        //offset

        private void Start()
        {
            if (delayRefresh)
            {
                Invoke("DelayInit", 0.1f);
            }
            if (transform.parent.GetComponent<AutoFitToCanvas>() != null)
            {
                Invoke("DelayInit", 1f);
                return;
            }
            Init();
        }

        public void ManualDelayInit() { Invoke("DelayInit", 0.1f); }

        void DelayInit() {
            //Debug.Log("Delay Inited!!");
            Init();
        }

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
            if (fixedWidth > 0) size.x = fixedWidth;
            if (fixedHeight > 0) size.y = fixedHeight;

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

                transitionClass.Init(target, sideMode, isVisibleOnStart);
            }
        }


        void ForceRebuildLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        }

    }

}