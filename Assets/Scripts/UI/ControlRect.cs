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

    void OnEnable()
    {
        myRect = GetComponent<RectTransform>();

        //SetRectSize();
        Invoke("SetRectSize", 0.15f);
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
