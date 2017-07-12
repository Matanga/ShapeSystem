using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace VFX.ShapeSystem
{


    class MyWindow : EditorWindow
    {
        [MenuItem("Window/My Window")]

        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(MyWindow));
        }

        void OnGUI()
        {
            // The actual window code goes here
            GUILayout.Label((Resources.Load("GUI_FLoor", typeof(Texture2D))) as Texture2D);
        }
    }


    [CustomEditor(typeof(SS_Floor))]
    public class SS_FloorEditor : Editor
    {
        public SS_Floor Target
        {
            get
            {
                return (SS_Floor)target; ;
            }
        }

        bool showPrefabs = true;
        SerializedProperty thePrefabs;

        SerializedProperty myName;

        bool showPrefabOptions = true;


        bool showTransformOptions = true;
        SerializedProperty moduleSize;
        SerializedProperty offsetTranslate;
        SerializedProperty offsetRotate;

        bool showDistributionOptions = true;
        SerializedProperty myDistrType;
        SerializedProperty randSeed;

        SerializedProperty generateCollider;
        SerializedProperty colliderHeight;
        SerializedProperty navigationStatic;

        
        void OnEnable()
        {
            thePrefabs = serializedObject.FindProperty("thePrefabs");

            moduleSize = serializedObject.FindProperty("moduleSize");
            offsetTranslate = serializedObject.FindProperty("offsetTranslate");
            offsetRotate = serializedObject.FindProperty("offsetRotate");

            myName = serializedObject.FindProperty("myName");

            myDistrType = serializedObject.FindProperty("myDistrType");
            randSeed = serializedObject.FindProperty("randSeed");


            generateCollider = serializedObject.FindProperty("generateCollider");
            colliderHeight = serializedObject.FindProperty("colliderHeight");
            navigationStatic = serializedObject.FindProperty("navigationStatic");


        }
        
        ///// MAIN PROPERTY FETCHER

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
            SerializedProperty GetPrefabUseRandomRotation(SerializedProperty theProp)
            {
                SerializedProperty subProp0 = theProp.Copy();
                subProp0.Next(true);    // property 0
                subProp0.Next(false);    // property 0
                return subProp0;
            }

        

        #endregion


        int guiSpacing = 7;
        void Space()
        {
            GUILayout.Space(guiSpacing);
        }

        void GuessModuleSize()
        {
            Vector3 newVec=SS_Common.GetPrefabSize(Target.thePrefabs[0].thePrefab.name, '_');
            if(newVec!=Vector3.zero)
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

            EditorGUILayout.PropertyField(myName, new GUIContent("Floor Name"));

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Target, "Change Area Name");
                SS_Common.RenameArea("SS_Floor", myName.stringValue, Target.transform);
            }

            GUILayout.Space(3);
            EditorGUILayout.EndVertical();
        }

        void DrawColliderOptions()
        {
            EditorGUILayout.BeginVertical("Button");
            EditorGUILayout.PropertyField(generateCollider);
            if (generateCollider.boolValue)
            {
                EditorGUILayout.PropertyField(colliderHeight);
                EditorGUILayout.PropertyField(navigationStatic);
            }
            EditorGUILayout.EndVertical();     
        
        }
        
        void DrawTransformOffsetsFoldout()
        {
            serializedObject.Update();

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

            serializedObject.ApplyModifiedProperties();

        }

        void DrawPrefabOptions()
        {

            serializedObject.Update();


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

            serializedObject.ApplyModifiedProperties();

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
                    EditorGUILayout.PropertyField(GetPrefabUseRandomRotation(theProp));                        
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
        







        
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();


            Space();

            EditorGUILayout.LabelField("Main Options", SS_Common.StyleTitle);

            DrawAreaName();

            SS_Common.DrawInstancerParentUI(Target.gameObject);


            // TRANSFORM OPTIONS FOLDOUT
            DrawTransformOffsetsFoldout();


            Space();
            Space();
            EditorGUILayout.LabelField("Floor Options", SS_Common.StyleTitle);

            // COLLIDER OPTIONS            
            DrawColliderOptions();




            Space();
            Space();

            EditorGUILayout.LabelField("Prefabs", SS_Common.StyleTitle);

            // PREFABS OPTIONS
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