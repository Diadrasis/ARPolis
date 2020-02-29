using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ARPolis.UI
{

    public class AutoFitToCanvas : MonoBehaviour
    {

        public RectTransform target, kanvas;
        public bool isWidthRelative, isHeightRelative;
        //the size of canvas during development
        public Vector2 initKanvasSize = new Vector2(1080f, 1920f);
        bool hasLayoutElement;
        LayoutElement layOutElem;

        private void OnEnable()
        {
            Init();
        }

        [ContextMenu("Test Layout")]
        void Init()
        {

            initKanvasSize = kanvas.sizeDelta;


            layOutElem = target.gameObject.GetComponent<LayoutElement>();
            if (layOutElem != null && !hasLayoutElement)
            {
                hasLayoutElement = true;
            }

            Vector2 size = target.sizeDelta;

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

            if (!isWidthRelative && !isHeightRelative)
            {
                target.sizeDelta = initKanvasSize;
            }
            else
            {
                target.sizeDelta = size;
            }


            ForceRebuildLayout();
        }


        void ForceRebuildLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        }

    }

}