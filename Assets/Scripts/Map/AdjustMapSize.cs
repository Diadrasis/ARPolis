using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustMapSize : MonoBehaviour {

    public float zoomSize = 2.25f;

    private int screenWidth;
    private int screenHeight;

    private void Start()
    {
        // Initial resizing
        ResizeMap();
    }

    private void Update()
    {
        // If the screen size changes, resize the map
        if (screenWidth != Screen.width || screenHeight != Screen.height) ResizeMap();
    }

    private void ResizeMap()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        int mapWidth = screenWidth / 256 * 256;
        int mapHeight = screenHeight / 256 * 256;
        if (screenWidth % 256 != 0) mapWidth += 256;
        if (screenHeight % 256 != 0) mapHeight += 256;

        OnlineMapsTileSetControl.instance.Resize(mapWidth, mapHeight);
        OnlineMapsCameraOrbit.instance.distance = screenHeight * 0.9f;

        OnlineMapsTileSetControl.instance.sizeInScene = OnlineMapsTileSetControl.instance.sizeInScene * zoomSize;
    }

    #region Not In Use
    /*
     
    void SetCamHeight()
    {
        OnlineMapsCameraOrbit.instance.distance = OnlineMapsTileSetControl.instance.sizeInScene.x / 2f;
    }

    */
    #endregion
}
