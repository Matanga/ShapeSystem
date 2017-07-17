using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VFX.ShapeSystem
{
    [CustomEditor(typeof(SS_GridScatter))]
    public class SS_GridScatterEditor : Editor
    {
        public SS_GridScatter Target
        {
            get
            {
                return (SS_GridScatter)target;
            }
        }

        bool showPrefabs = true;
        SerializedProperty thePrefabs;

        SerializedProperty scatterCount;

        SerializedProperty myName;


        bool showTransformOptions = true;
        bool showDistributionOptions = true;
        bool showPrefabOptions;


        SerializedProperty moduleSize;
        SerializedProperty offsetTranslate;
        SerializedProperty offsetRotate;

        SerializedProperty myDistrType;
        SerializedProperty randSeed;

        SerializedProperty wallOffset;
        SerializedProperty randomRotation;

        SerializedProperty avoidSelfCollision;




        int guiSpacing = 7;


        void OnEnable()
        {
            thePrefabs = serializedObject.FindProperty("thePrefabs");

            scatterCount = serializedObject.FindProperty("scatterCount");

            myName = serializedObject.FindProperty("myName");

            moduleSize = serializedObject.FindProperty("moduleSize");
            offsetTranslate = serializedObject.FindProperty("offsetTranslate");
            offsetRotate = serializedObject.FindProperty("offsetRotate");

            myDistrType = serializedObject.FindProperty("myDistrType");
            randSeed = serializedObject.FindProperty("randSeed");

            wallOffset = serializedObject.FindProperty("wallOffset");

            randomRotation = serializedObject.FindProperty("randomRotation");

            avoidSelfCollision = serializedObject.FindProperty("avoidSelfCollision");

        }




        void Space()
        {
            GUILayout.Space(guiSpacing);
        }

        void GuessModuleSize()
        {
            Vector3 newVec = SS_Common.GetPrefabSize(Target.thePrefabs[0].thePrefab.name, '_');
            if (newVec != Vector3.zero)
            {
                moduleSize.vector3Value = newVec;
            }
        }

        void DrawAreaName()
        {
            EditorGUILayout.BeginVertical("Button");
            GUILayout.Space(3);
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(myName, new GUIContent("Wall Name"));

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Target, "Change Area Name");
                SS_Common.RenameArea("SS_Wall", myName.stringValue, Target.transform);
            }

            GUILayout.Space(3);
            EditorGUILayout.EndVertical();
        }

        void DrawTransformOffsetsFoldout()
        {
            EditorGUILayout.BeginVertical("Button");
            EditorGUI.indentLevel++;
            showTransformOptions = EditorGUILayout.Foldout(showTransformOptions, "Transform Offsets");
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
                DrawPrefabSizeController();
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
            showPrefabs = EditorGUILayout.Foldout(showPrefabs, "Prefabs ( " + GetPrefabArraySize().intValue.ToString() + " )");
            EditorGUI.indentLevel--;

            if (showPrefabs)
            {
                for (int i = 0; i < GetPrefabArraySize().intValue; i++)
                {
                    EditorGUILayout.BeginVertical("Button");
                    Space();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Prefab", GUILayout.Width(40));
                    SerializedProperty theProp = GetPrefabArrayAtIndex(i);
                    EditorGUILayout.PropertyField(GetPrefabGO(theProp), new GUIContent(""));

                    EditorGUILayout.EndHorizontal();

                    Space();
                    EditorGUILayout.EndVertical();
                }
                //PREFAB ADD / REMOVE
                DrawPrefabAddRemoveButtons();
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Target, "Changed Prefab");
                if (GetPrefabArraySize().intValue == 1)
                {
                    GuessModuleSize();
                }
            }
        }

        void DrawPrefabSizeController()
        {
            EditorGUILayout.BeginVertical("Button");

            GUILayout.Space(2);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Size", GUILayout.Width(35));

            EditorGUILayout.PropertyField(moduleSize, new GUIContent(""));

            if (GUILayout.Button("Guess"))
            {
                GuessModuleSize();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);

            EditorGUILayout.EndVertical();

        }


        #region Prefabs Array Serialized Properties Fetchers

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
        SerializedProperty GetPrefabSize(SerializedProperty theProp)
        {
            SerializedProperty subProp0 = theProp.Copy();
            subProp0.Next(true);    // property 0
            subProp0.Next(false);    // property 0
            return subProp0;
        }

        #endregion

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            Space();

            EditorGUILayout.LabelField("Main Options", SS_Common.StyleTitle);

            // AREA NAME
            DrawAreaName();

            // AREA PARENT
            SS_Common.DrawInstancerParentUI(Target.gameObject);


            // SEGMENTS OPTIONS
            DrawTransformOffsetsFoldout();


            Space();
            Space();
            EditorGUILayout.LabelField("Scatter Options", SS_Common.StyleTitle);
            serializedObject.Update();

            EditorGUILayout.PropertyField(scatterCount);

            EditorGUILayout.PropertyField(randSeed);

            EditorGUILayout.PropertyField(wallOffset);

            EditorGUILayout.PropertyField(randomRotation);

            EditorGUILayout.PropertyField(avoidSelfCollision);


            serializedObject.ApplyModifiedProperties();

            Space();
            Space();

            EditorGUILayout.LabelField("Prefabs", SS_Common.StyleTitle);


            // PREFAB OPTIONS
            DrawPrefabOptions();

            //DISTRIBUTION OPTIONS
            if (GetPrefabArraySize().intValue > 1) DrawDistributionOptions();

            // PREFAB OPTIONS FOLDOUT
            DrawPrefabArrayFoldout();


            /*

            EditorGUILayout.PropertyField(serializedObject.FindProperty("thePrefabs"));
            EditorGUILayout.PropertyField(GetPrefabArraySize());

            for (int i = 0; i < GetPrefabArraySize().intValue; i++)
            {
                SerializedProperty theProp = GetPrefabArrayAtIndex(i);

                EditorGUILayout.PropertyField(theProp);
                EditorGUILayout.PropertyField(GetPrefabGO(theProp));
                EditorGUILayout.PropertyField(GetPrefabSize(theProp));

            }

            EditorGUILayout.PropertyField(scatterCount);
            */

            serializedObject.ApplyModifiedProperties();
            
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

    }
}