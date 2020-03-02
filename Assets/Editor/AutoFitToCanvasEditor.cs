using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StaGeUnityTools
{

    [CustomEditor(typeof(AutoFitToCanvas))]
    public class AutoFitToCanvasEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.textField);
            DrawDefaultInspector();

            AutoFitToCanvas myTarget = (AutoFitToCanvas)target;

            //Label Color
            //EditorStyles.label.normal.textColor = Color.red;
            //Value Color
            //TextFieldStyles.normal.textColor = Color.yellow;
            //GUI.color = Color.green;
            myTarget.widthPercent = EditorGUILayout.FloatField("Width Percent", myTarget.widthPercent);
            EditorGUILayout.LabelField("Final Width", myTarget.WidthFinal.ToString());

            myTarget.heightPercent = EditorGUILayout.FloatField("Height Percent", myTarget.heightPercent, TextFieldStyles);
            EditorGUILayout.LabelField("Final Height", myTarget.HeightFinal.ToString());

            
            GUI.color = Color.cyan;
            if (GUILayout.Button("Set Panel Size")) { myTarget.Init(); }

            Debug.Log("OnInspectorGUI");

        }
    }

}
