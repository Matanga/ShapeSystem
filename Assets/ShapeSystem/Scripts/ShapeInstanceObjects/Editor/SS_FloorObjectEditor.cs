
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace VFX.ShapeSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SS_FloorObject))]
    public class SS_FloorObjectEditor : Editor
    {
        private SS_FloorObject theTarget;
        public SS_FloorObject TheTarget
        {
            get
            {
                if (theTarget == null)
                    theTarget = (SS_FloorObject)target;
                return theTarget;
            }
            set { theTarget = value; }
        }

                
        
        
        SerializedProperty myPrefab;
        

        void OnEnable()
        {
            myPrefab = serializedObject.FindProperty("myPrefab");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            EditorGUILayout.PropertyField(myPrefab);

            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(TheTarget, "Level Canvas");

                foreach (Object obj in targets)
                {
                    ((SS_FloorObject)obj).RebuildPrefab();
                }

            }
        }



    }
}
