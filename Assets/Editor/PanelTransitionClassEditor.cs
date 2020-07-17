using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace StaGeUnityTools
{

    [CustomEditor(typeof(PanelTransitionClass))]
    public class PanelTransitionClassEditor : Editor
    {
        bool showVariables;
        string btnLabel = "Show Script Variables";

        public override void OnInspectorGUI()
        {
            PanelTransitionClass myTarget = (PanelTransitionClass)target;

            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Methods.HexColor("#C2C2C2", Color.gray);

            GUIStyle TextFieldStyles = new GUIStyle(EditorStyles.textField);
            TextFieldStyles.richText = true;

            EditorGUILayout.LabelField("©<color=cyan>Sta</color><color=red>Ge</color> 2020 - <color=black>eust.georgiou@gmail.com</color>", TextFieldStyles);
            //if (Event.current.type == EventType.MouseUp && yourLabelRect.Contains(Event.current.mousePosition))

            TextFieldStyles.fontSize = 14;
            TextFieldStyles.fontStyle = FontStyle.Bold;
            TextFieldStyles.normal.textColor = Color.yellow;
            TextFieldStyles.alignment = TextAnchor.MiddleCenter;

            GUI.backgroundColor = Color.black;

            if (GUILayout.Button(btnLabel, TextFieldStyles)) {
                showVariables = !showVariables;
                btnLabel = showVariables ? "Hide Script Variables" : "Show Script Variables";
            }
            GUI.backgroundColor = oldColor;
            GUI.color = Color.white;
            if (showVariables) DrawDefaultInspector();

            // Starts a horizontal group
            GUILayout.BeginHorizontal("box");
            GUI.color = Color.cyan;
            TextFieldStyles.normal.textColor = Color.white;
            if (GUILayout.Button("Show Target", TextFieldStyles)) { myTarget.ShowPanel(); }
            if (myTarget.isMoveByPercentage) { if (GUILayout.Button("Show Percentage", TextFieldStyles)) { myTarget.ShowPercentagePanel(); } }
            if (GUILayout.Button("Hide Target", TextFieldStyles)) { myTarget.HidePanel(); }
            GUILayout.EndHorizontal();

#if UNITY_EDITOR
            // Ensure continuous Update calls.
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
            }
#endif
            

            //GUILayout.Button("I'm the first button");
            //GUILayout.Button("I'm to the right");

        }
    }

}
