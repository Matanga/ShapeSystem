using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
namespace VFX.ShapeSystem
{


    [CustomEditor(typeof(SS_GridInstancer))]
    public class SS_GridInstancerEditor : Editor
    {
        public SS_GridInstancer Target
        {
            get
            {
                return (SS_GridInstancer)target;
            }
        }

        bool showPrefabs = true;
        SerializedProperty thePrefabs;

        SerializedProperty gridRows;
        SerializedProperty gridColumns;
        SerializedProperty useBorders;
        SerializedProperty useInterior;

        SerializedProperty myName;

        bool showPrefabOptions = true;

        bool showTransformOptions = true;
        SerializedProperty offsetTranslate;
        SerializedProperty offsetRotate;

        bool showDistributionOptions = true;
        SerializedProperty myDistrType;


        SerializedProperty useRandomRotation;
        SerializedProperty useRandomProbability;
        SerializedProperty rndProbValue;
        SerializedProperty randSeed;


        int guiSpacing = 7;
        
        void Space()
        {
            GUILayout.Space(guiSpacing);
        }

        void OnEnable()
        {
            thePrefabs = serializedObject.FindProperty("thePrefabs");
            gridRows = serializedObject.FindProperty("gridRows");
            gridColumns = serializedObject.FindProperty("gridColumns");
            useBorders = serializedObject.FindProperty("useBorders");
            useInterior = serializedObject.FindProperty("useInterior");

            myName = serializedObject.FindProperty("myName");


            offsetTranslate = serializedObject.FindProperty("offsetTranslate");
            offsetRotate = serializedObject.FindProperty("offsetRotate");

            myDistrType = serializedObject.FindProperty("myDistrType");



            useRandomRotation = serializedObject.FindProperty("useRandomRotation");
            useRandomProbability = serializedObject.FindProperty("useRandomProbability");
            rndProbValue = serializedObject.FindProperty("rndProbValue");
            randSeed = serializedObject.FindProperty("randSeed");
        }

        #region Grid Objects Properties Getters
        
        SerializedProperty GetPrefabArraySize()
        {
            SerializedProperty subProp0 = thePrefabs.Copy();
            subProp0.Next(true);    // property 0
            subProp0.Next(true);    // property 0
            return subProp0;
        }
        SerializedProperty GetPrefabArrayAtIndex(int Index)
        {
            SerializedProperty subProp0 = thePrefabs.Copy();
            subProp0.Next(true);    // property 0
            subProp0.Next(true);    // property 0
            subProp0.Next(true);    // property 0
            for (int i = 0; i < Index; i++)
            {
                subProp0.Next(false);
            }
            return subProp0;
        }
        SerializedProperty GetPrefabGO(SerializedProperty theProp)
        {
            SerializedProperty subProp0 = theProp.Copy();
            subProp0.Next(true);    // property 0
            //subProp0.Next(true);    // property 0
            return subProp0;
        }

        #endregion

        #region GUI Drawers

        void DrawAreaName()
        {

            EditorGUILayout.BeginVertical("Button");
            GUILayout.Space(7);
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(myName, new GUIContent("Grid Name"));

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Target, "Change Area Name");
                SS_Common.RenameArea("SS_Grid", myName.stringValue, Target.transform);
            }
            GUILayout.Space(7);
            EditorGUILayout.EndVertical();

        }

        void DrawTransformOffsetsFoldout()
        {
            EditorGUILayout.BeginVertical("Button");
            EditorGUI.indentLevel++;
            showTransformOptions = EditorGUILayout.Foldout(showTransformOptions, "Transform Options");
            EditorGUI.indentLevel--;

            ////////////////////////
            // TRANSFORM OPTIONS
            // TRANSFORM OPTIONS
            ///////////////////////
            if (showTransformOptions)
            {

                ////////////////////////
                // TRANSLATE OFFSET
                ///////////////////////
                EditorGUILayout.BeginHorizontal("Label");
                EditorGUILayout.PrefixLabel("Translate");
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUILayout.PropertyField(offsetTranslate, new GUIContent(""));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                EditorGUILayout.EndHorizontal();

                ////////////////////////
                // ROTATE OFFSET
                ///////////////////////
                EditorGUILayout.BeginHorizontal("Label");
                EditorGUILayout.PrefixLabel("Rotate");
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUILayout.PropertyField(offsetRotate, new GUIContent(""));
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                EditorGUILayout.EndHorizontal();

            }
            EditorGUILayout.EndVertical();
        }

        void DrawPrefabOptions()
        {
            // PREFABS OPTIONS
            EditorGUILayout.BeginVertical("Button");

            EditorGUI.indentLevel++;
            showPrefabOptions = EditorGUILayout.Foldout(showPrefabOptions, "Size Options");
            EditorGUI.indentLevel--;

            ////////////////////////
            // TRANSFORM OPTIONS
            ///////////////////////
            if (showPrefabOptions)
            {
                // PREFABS SIZE

                //Space();  
            }
            // TRANSFORM OPTIONS FOLDOUT
            EditorGUILayout.EndVertical();
        }

        void DrawDistributionOptions()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical("Button");

            EditorGUI.indentLevel++;
            showDistributionOptions = EditorGUILayout.Foldout(showDistributionOptions, "Distribution Options");
            EditorGUI.indentLevel--;


            if (showDistributionOptions)
            {
                EditorGUILayout.PropertyField(myDistrType);

                switch (myDistrType.enumDisplayNames[myDistrType.enumValueIndex])
                {
                    case ("Random"):
                        EditorGUILayout.PropertyField(randSeed);
                        break;
                }
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

        }

        void DrawPrefabAddRemoveButtons()
        {
            //if (GetPrefabArraySize().intValue == 0) return;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("    ");

            if (GetPrefabArraySize().intValue > 1)
            {
                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    Target.RemovePrefab();
                    Debug.Log("Removing Prefab");
                }
            }
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                Target.AddPrefab();
                Debug.Log("adding Prefab");
            }
            EditorGUILayout.EndHorizontal();

        }

        void DrawPrefabArrayFoldout()
        {
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            EditorGUILayout.BeginVertical("Button");

            EditorGUI.indentLevel++;
            showPrefabs = EditorGUILayout.Foldout(showPrefabs, "Prefabs");
            EditorGUI.indentLevel--;

            if (showPrefabs)
            {
                for (int i = 0; i < GetPrefabArraySize().intValue; i++)
                {
                    EditorGUILayout.BeginVertical("Button");
                    Space();
                    SerializedProperty theProp = GetPrefabArrayAtIndex(i);
                    EditorGUILayout.PropertyField(GetPrefabGO(theProp));
                    Space();
                    EditorGUILayout.EndVertical();
                }
            }

            //PREFAB ADD / REMOVE
            DrawPrefabAddRemoveButtons();

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {

            }
        }

        void DrawGridSizeOptions()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical("Button");

            EditorGUILayout.PropertyField(gridRows);
            EditorGUILayout.PropertyField(gridColumns);


            EditorGUILayout.PropertyField(useRandomRotation);

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        void DrawRandomProbabilityOptions()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical("Button");

            EditorGUILayout.PropertyField(useRandomProbability);
            if (useRandomProbability.boolValue)
            {
                EditorGUILayout.PropertyField(rndProbValue);
                EditorGUILayout.PropertyField(randSeed);
            }

            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        void DrawGridBorderOptions()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical("Button");

            EditorGUILayout.PropertyField(useBorders);


            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
       
        #endregion

        #region Editor Methods

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            /////////////////////////////////////////////////
            /////////////////////////////////////////////////
            /////////////////////////////////////////////////
            Space();

            EditorGUILayout.LabelField("Main Options", SS_Common.StyleTitle);

            //Area NAme UI
            DrawAreaName();

            //Draw Parent Name
            SS_Common.DrawInstancerParentUI(Target.gameObject);

            // TRANSFORM OPTIONS FOLDOUT
            DrawTransformOffsetsFoldout();

            Space();
            Space();
            EditorGUILayout.LabelField("Grid Options", SS_Common.StyleTitle);


            DrawGridSizeOptions();

            DrawRandomProbabilityOptions();

            DrawGridBorderOptions();


            Space();
            Space();
            EditorGUILayout.LabelField("Prefabs", SS_Common.StyleTitle);


            EditorGUILayout.BeginVertical("Button");
            Space();
            EditorGUILayout.PrefixLabel("Prefabs Options");

            DrawPrefabOptions();


            //DISTRIBUTION OPTIONS
            if (GetPrefabArraySize().intValue > 1) DrawDistributionOptions();

            // PREFAB OPTIONS FOLDOUT
            DrawPrefabArrayFoldout();
            Space();

            EditorGUILayout.EndVertical();


            /////////////////////////////////////////////////
            /////////////////////////////////////////////////


            /*
            EditorGUILayout.BeginVertical("Button");

            GUILayout.Space(7);
            //GUILayout.Label("");
            EditorGUILayout.PropertyField(GetPrefabArraySize());
            GUILayout.Space(7);

            EditorGUILayout.EndVertical();

            for (int i = 0; i < GetPrefabArraySize().intValue; i++)
            {
                EditorGUILayout.BeginVertical("Button");

                GUILayout.Space(7);
                SerializedProperty theProp = GetPrefabArrayAtIndex(i);
                EditorGUILayout.PropertyField(GetPrefabGO(theProp));
                GUILayout.Space(7);

                EditorGUILayout.EndVertical();
            }
            */



            serializedObject.ApplyModifiedProperties();

            /////////////////////////////////////////////////
            /////////////////////////////////////////////////

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Target, "Moved Point");
                Target.GeneratePrefabs();
            }
        }

        private void OnSceneGUI()
        {
            SS_Common.CheckForSwitchDisplayInput(Target.gameObject);
            SS_Common.CheckForLevelCanvasUpdateInput(Target.gameObject);
        }

        #endregion

    }    

}