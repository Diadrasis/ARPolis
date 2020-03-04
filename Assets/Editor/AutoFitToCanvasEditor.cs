using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StaGeUnityTools
{

    [CustomEditor(typeof(AutoFitToCanvas))]
    public class AutoFitToCanvasEditor : Editor
    {
        bool showVariables;
        string btnLabel = "Show Script Variables";

        public override void OnInspectorGUI()
        {
            AutoFitToCanvas myTarget = (AutoFitToCanvas)target;

            if (myTarget.target == null) myTarget.target = myTarget.GetComponent<RectTransform>();
            if (myTarget.kanvas == null) myTarget.kanvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();

            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Methods.HexColor("#C2C2C2", Color.gray);

            GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.textField);
            TextFieldStyles.richText = true;

            EditorGUILayout.LabelField("©<color=cyan>Sta</color><color=red>Ge</color> 2020 - <color=black>eust.georgiou@gmail.com</color>", TextFieldStyles);
            //EditorGUILayout.LabelField("@StaGe 2020 - eust.georgiou@gmail.com");

            TextFieldStyles.fontSize = 14;
            TextFieldStyles.fontStyle = FontStyle.Bold;
            TextFieldStyles.normal.textColor = Color.yellow;
            TextFieldStyles.alignment = TextAnchor.MiddleCenter;

            GUI.backgroundColor = Color.black;

            if (GUILayout.Button(btnLabel, TextFieldStyles))
            {
                showVariables = !showVariables;
                btnLabel = showVariables ? "Hide Script Variables" : "Show Script Variables";
            }
            GUI.backgroundColor = oldColor;
            GUI.color = Color.white;
            if (showVariables) DrawDefaultInspector();


            myTarget.widthPercent = EditorGUILayout.FloatField("Width Percent", myTarget.widthPercent);
            EditorGUILayout.LabelField("Final Width", myTarget.WidthFinal.ToString());

            myTarget.heightPercent = EditorGUILayout.FloatField("Height Percent", myTarget.heightPercent);
            EditorGUILayout.LabelField("Final Height", myTarget.HeightFinal.ToString());

            if(!showVariables) myTarget.isMovable = EditorGUILayout.Toggle("Is Panel Movable?", myTarget.isMovable);
            if (!showVariables) myTarget.isVisibleOnStart = EditorGUILayout.Toggle("Should Panel be Visible on Start?", myTarget.isVisibleOnStart);

            GUI.color = Color.cyan;
            if (GUILayout.Button("Set Panel Size")) { myTarget.Init(); }

        }
    }

}
