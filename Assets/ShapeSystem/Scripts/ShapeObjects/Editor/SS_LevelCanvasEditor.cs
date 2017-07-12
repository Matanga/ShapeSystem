using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VFX.ShapeSystem
{
    [CustomEditor(typeof(SS_LevelCanvas))]
    public class SS_LevelCanvasEditor : Editor
    {
        public SS_LevelCanvas TheTarget
        {
            get
            {
                return (SS_LevelCanvas)target;
            }
        }

        #region Serialized Properties

        SerializedProperty debugMode;
        SerializedProperty showCreateNew;
       
        #endregion

        #region Temp Properties

        /// <summary>
        /// The temp string to have the name for the new area
        /// </summary>
        string newAreaName  = "";
        /// <summary>
        /// The temp color to have the name for the new area
        /// </summary>  
        Color newColor = Color.grey;

        #endregion

        #region Editor Methods

        void OnEnable()
        {
            debugMode = serializedObject.FindProperty("debugMode");
            showCreateNew = serializedObject.FindProperty("showCreateNew");

        }

        public override void OnInspectorGUI()
        {

            DrawDebugSwitch();


            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            DrawNewAreaGUI();


            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(TheTarget, "Level Canvas");
            }
        }

        private void OnSceneGUI()
        {
            SS_Common.CheckForSwitchDisplayInput(TheTarget.gameObject);

            SS_Common.CheckForLevelCanvasUpdateInput(TheTarget.gameObject);
        }
    
        #endregion

        #region GUI Draw Methods
        
        void DrawNewAreaGUI()
        {
            EditorGUILayout.BeginVertical("Button");

            EditorGUI.indentLevel++;
            showCreateNew.boolValue = EditorGUILayout.Foldout(showCreateNew.boolValue, "Create New Area");
            EditorGUI.indentLevel--;


            if (showCreateNew.boolValue)
            {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Name", GUILayout.MaxWidth(50));

            newAreaName = EditorGUILayout.TextField("", newAreaName, GUILayout.MaxWidth(120));

            newColor = EditorGUILayout.ColorField((""), newColor, GUILayout.MaxWidth(60));

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Create Area"))
            {
                SS_Common.SS_CreateLevelArea(TheTarget.gameObject, newAreaName, newColor);
                newAreaName = "";
            }



            }
            EditorGUILayout.EndVertical();


        }

        void DrawDebugSwitch()
        { 
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            EditorGUILayout.BeginVertical("Button");

            GUILayout.Space(3);
            EditorGUILayout.PropertyField(debugMode);
            GUILayout.Space(3);

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                TheTarget.SetLevelCanvasDebugDisplay(debugMode.boolValue);         
            }        
        }

        #endregion

    }
}