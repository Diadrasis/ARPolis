using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StaGeUnityTools {

    public class MenuItems : MonoBehaviour
    {
        [MenuItem("StaGe/Panels/Reset Size of Panels")]
        private static void ResetSizeOfPanels()
        {
            AutoFitToCanvas[] autoFits = FindObjectsOfType<AutoFitToCanvas>();
            foreach (AutoFitToCanvas fit in autoFits) fit.Init();
        }
    }

}

