using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlRect : MonoBehaviour
{
    public RectTransform rtParent;
    RectTransform myRect;

    public bool setWidthSame, setHeightSame;
    public float percentSize = 100;

    Vector2 totalSize;

    void Start()
    {
        myRect = GetComponent<RectTransform>();

        SetRectSize();
    }

    [ContextMenu("Set Size")]
    void SetRectSize()
    {
        if(!myRect) myRect = GetComponent<RectTransform>();

        totalSize = rtParent.rect.size * (percentSize / 100f);

        if (setWidthSame)
        {
            myRect.sizeDelta = new Vector2(totalSize.x, myRect.sizeDelta.y);
        }
        if (setHeightSame)
        {
            myRect.sizeDelta = new Vector2(myRect.sizeDelta.x, totalSize.y);
        }
    }

}
