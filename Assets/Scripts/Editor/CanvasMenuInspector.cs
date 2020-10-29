using UnityEngine;
using UnityEditor;

namespace ProjectMenuZ
{
    [CustomEditor(typeof(CanvasMenu))]
    public class CanvasMenuInspector : Editor
    {


        public override void OnInspectorGUI()
        {
            CanvasMenu targetMenu = (CanvasMenu)target;

            // Open status.
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Open State:");
                Color startContentColor = GUI.contentColor;
                GUI.contentColor = targetMenu.IsOpen ? Color.green : new Color(1, 0.8f, 0);
                GUILayout.Label(targetMenu.IsOpen ? "Open" : "Closed");
                GUI.contentColor = startContentColor;
            }

            GUILayout.Space(8);

            // Transitions.
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_openTransitionType"));
            if (targetMenu.CloseTransitionType == CanvasMenu.TransitionType.Animation)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_openTriggerName"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_closeTransitionType"));
            if (targetMenu.CloseTransitionType == CanvasMenu.TransitionType.Animation)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_closeTriggerName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_closeStateName"));
                EditorGUI.indentLevel--;
            }


            GUILayout.Space(8);

            // Events.
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_onOpened"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_onClosed"));

            // Open close toggles.
            if (MenuManager.CanvasMenus.Count == 0)
                MenuManager.ForcePopulate();

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Toggle " + (targetMenu.IsOpen ? "Closed" : "Open")))
                {
                    if (targetMenu.IsOpen)
                        targetMenu.CloseImmediate();
                    else
                        targetMenu.OpenImmediate();
                }

                if (GUILayout.Button("Solo Open"))
                {
                    for (int i = MenuManager.CanvasMenus.Count - 1; i >= 0; i--)
                    {
                        if (MenuManager.CanvasMenus[i] == null)
                        {
                            MenuManager.CanvasMenus.RemoveAt(i);
                            continue;
                        }

                        MenuManager.CanvasMenus[i].CloseImmediate();
                    }
                    targetMenu.OpenImmediate();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    } 
}
