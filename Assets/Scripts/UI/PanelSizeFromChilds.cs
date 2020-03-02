using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StaGeUnityTools
{

    public class PanelSizeFromChilds : MonoBehaviour
    {
        public RectTransform rectPhotosContainer, rectScrollingParent, rectTextContainer, rectAudio, rectParentAll,
                             rectPhotoTitle, rectPhotoLabel, rectPhotoSource;

        public int photosInRow = 3;
        float currentWidth;

        private void Awake()
        {
            PanelSize.OnResize += Resize;
        }

        void Resize(float width)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectParentAll);

            currentWidth = width;
            Vector2 size = rectPhotosContainer.sizeDelta;
            if (size.x != width) size.x = width;
            rectPhotosContainer.sizeDelta = size; //new Vector2(width, 500f);

            size = rectPhotoTitle.sizeDelta;
            if (size.x != width) size.x = width;
            rectPhotoTitle.sizeDelta = size;

            size = rectPhotoLabel.sizeDelta;
            if (size.x != width) size.x = width;
            rectPhotoLabel.sizeDelta = size;

            size = rectPhotoSource.sizeDelta;
            if (size.x != width) size.x = width;
            rectPhotoSource.sizeDelta = size;

            Vector2 parentSize = rectScrollingParent.sizeDelta;
            parentSize.x = currentWidth;
            rectScrollingParent.sizeDelta = parentSize;

            Init();
        }

        bool hasInit;

        void Init()
        {
            if (!hasInit)
            {
                //hasInit = true;
                //float w = currentWidth - 140f;
                //w = w / (float)photosInRow;
                //gridPhotos.cellSize = new Vector2(w, 300f);

                Vector2 textSize = rectTextContainer.sizeDelta;
                textSize.x = currentWidth;
                rectTextContainer.sizeDelta = textSize;

                Vector2 audioSize = rectAudio.sizeDelta;
                audioSize.x = currentWidth - 60f;
                rectAudio.sizeDelta = audioSize;

                LayoutRebuilder.ForceRebuildLayoutImmediate(rectAudio);
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTextContainer);
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectPhotosContainer);
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectScrollingParent);
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectParentAll);

                //Canvas.ForceUpdateCanvases();
            }
            else
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectPhotosContainer);
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTextContainer);
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectParentAll);
            }
        }
    }

}
