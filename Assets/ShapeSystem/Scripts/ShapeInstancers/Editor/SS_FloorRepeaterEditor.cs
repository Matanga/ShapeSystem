using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VFX.ShapeSystem
{
    [CustomEditor(typeof(SS_FloorRepeater))]
    public class SS_FloorRepeaterEditor : Editor
    {
        public SS_FloorRepeater Target
        {
            get
            {
                return (SS_FloorRepeater)target; ;
            }
        }


        SerializedProperty  theFloor;

         SerializedProperty floorHeight;

         SerializedProperty floorCount;




        void OnEnable()
        {
            theFloor = serializedObject.FindProperty("theFloor");
            floorHeight = serializedObject.FindProperty("floorHeight");
            floorCount = serializedObject.FindProperty("floorCount");

        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            EditorGUILayout.PropertyField(theFloor);

            EditorGUILayout.PropertyField(floorHeight);
            EditorGUILayout.PropertyField(floorCount);



            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Target, "Moved Point");
                Target.GeneratePrefabs();
            }
        }

    }


}