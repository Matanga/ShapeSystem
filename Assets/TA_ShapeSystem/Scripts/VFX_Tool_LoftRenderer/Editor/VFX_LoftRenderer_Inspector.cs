using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VFX.ShapeSystem
{

    [CustomEditor(typeof(VFX_LoftRenderer))]
    [CanEditMultipleObjects()]
    public class VFX_LoftRenderer_Inspector : Editor
    {

        VFX_LoftRenderer myScript;
        #region LineRenderer Properties
        SerializedProperty sizeMultiplier;

        #endregion
        SerializedProperty theLoftCurve;

        SerializedProperty radius;
        SerializedProperty segments;
        SerializedProperty flipNormals;

        SerializedProperty skipEveryN;

        SerializedProperty usePath;
        SerializedProperty myPath;






        void OnEnable()
        {
            myScript = (VFX_LoftRenderer)target;

            theLoftCurve = serializedObject.FindProperty("theLoftCurve");
            sizeMultiplier = serializedObject.FindProperty("sizeMultiplier");
            radius = serializedObject.FindProperty("radius");
            segments = serializedObject.FindProperty("segments");
            flipNormals = serializedObject.FindProperty("flipNormals");
            skipEveryN = serializedObject.FindProperty("skipEveryN");


            usePath = serializedObject.FindProperty("usePath");
            myPath = serializedObject.FindProperty("myPath");



            myScript.ClampValues();
            myScript.DrawCircleMesh();

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Label("Loft Shape", "button");

            EditorGUILayout.PropertyField(theLoftCurve);

            GUILayout.Label("Circle Loft Options", "button");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(radius);
            EditorGUILayout.PropertyField(segments);
            EditorGUILayout.PropertyField(flipNormals);
            EditorGUILayout.PropertyField(sizeMultiplier);
            EditorGUILayout.PropertyField(skipEveryN);


            EditorGUILayout.PropertyField(usePath);
            EditorGUILayout.PropertyField(myPath);


            var rebuild = EditorGUI.EndChangeCheck();
            serializedObject.ApplyModifiedProperties();

            if (rebuild)
            {
                myScript.ClampValues();
                myScript.DrawCircleMesh();
            }
            GUILayout.Label((myScript.TheMF.sharedMesh.triangles.Length / 3).ToString());

        }
    }
}