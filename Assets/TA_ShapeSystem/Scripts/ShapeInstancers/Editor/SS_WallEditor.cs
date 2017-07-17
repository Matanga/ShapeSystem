using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace VFX.ShapeSystem
{
    [CustomEditor(typeof(SS_Wall))]
    public class SS_WallEditor : Editor
    {
        public SS_Wall Target
        {
            get
            {
                return (SS_Wall)target;
            }
        }

        bool showHoles = true;
        SerializedProperty theHoles;

        bool showPrefabs = true;
        SerializedProperty theWallPrefabs;

        bool showCorners = true;
        SerializedProperty theCornerPrefabs;


        SerializedProperty segmentFlag;
        SerializedProperty useCustomSegments;

        bool showTransformOptions = true;

        SerializedProperty myName;

        bool showPrefabOptions;


        SerializedProperty moduleSize;
        SerializedProperty offsetTranslate;
        SerializedProperty offsetRotate;

        SerializedProperty myDistrType;
        SerializedProperty randSeed;
        
        SerializedProperty generateCollider;
        SerializedProperty colliderHeight;
        SerializedProperty navigationStatic;



        SerializedProperty theShapeTransform;






        bool showDistributionOptions = true;
        int guiSpacing = 7;
        
        void OnEnable()
        {
            theWallPrefabs = serializedObject.FindProperty("theWallPrefabs");
            theCornerPrefabs = serializedObject.FindProperty("theCornerPrefabs");


            theHoles = serializedObject.FindProperty("theHoles");

            segmentFlag = serializedObject.FindProperty("segmentFlag");
            useCustomSegments = serializedObject.FindProperty("useCustomSegments");

            myName = serializedObject.FindProperty("myName");


            moduleSize = serializedObject.FindProperty("moduleSize");
            offsetTranslate = serializedObject.FindProperty("offsetTranslate");
            offsetRotate = serializedObject.FindProperty("offsetRotate");


            myDistrType = serializedObject.FindProperty("myDistrType");
            randSeed = serializedObject.FindProperty("randSeed");

            generateCollider = serializedObject.FindProperty("generateCollider");
            colliderHeight = serializedObject.FindProperty("colliderHeight");
            navigationStatic = serializedObject.FindProperty("navigationStatic");

            theShapeTransform = serializedObject.FindProperty("theShapeTransform");



        }

        ///// MAIN PROPERTY FETCHER

        #region Properties Getters for referenced  "Walls" components


        SerializedProperty GetCornerPrefabArraySize()
        {
            SerializedProperty subProp0 = theCornerPrefabs.Copy();
            subProp0.Next(true);    // property 0
            subProp0.Next(true);    // property 0
            return subProp0;
        }

        SerializedProperty GetCornerPrefabArrayAtIndex(int Index)
        {
            SerializedProperty subProp0 = theCornerPrefabs.Copy();
            subProp0.Next(true);    // property 0
            subProp0.Next(true);    // property 0
            subProp0.Next(true);    // property 0
            for (int i = 0; i < Index; i++)
            {
                subProp0.Next(false);
            }
            return subProp0;
        }


        SerializedProperty GetWallPrefabArraySize()
        {
            SerializedProperty subProp0 = theWallPrefabs.Copy();
            subProp0.Next(true);    // property 0
            subProp0.Next(true);    // property 0
            return subProp0;
        }
        
        SerializedProperty GetWallPrefabArrayAtIndex(int Index)
        {
            SerializedProperty subProp0 = theWallPrefabs.Copy();
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

        SerializedProperty GetPrefabFaceInwards(SerializedProperty theProp)
        {
            SerializedProperty subProp0 = theProp.Copy();
            subProp0.Next(true);    // property 0
            subProp0.Next(false);    // property 0
            return subProp0;
        }
        
        #endregion


        #region Properties Getters for referenced  "Holes" components
        
        SerializedProperty GetHolesArraySize()
        {
            SerializedProperty subProp0 = theHoles.Copy();
            subProp0.Next(true);    // property 0
            subProp0.Next(true);    // property 0
            return subProp0;
        }
        
        SerializedProperty GetHolesArrayAtIndex(int Index)
        {
            SerializedProperty subProp0 = theHoles.Copy();
            subProp0.Next(true);    // property 0
            subProp0.Next(true);    // property 0
            subProp0.Next(true);    // property 0
            for (int i = 0; i < Index; i++)
            {
                subProp0.Next(false);
            }
            return subProp0;
        }

        SerializedProperty GetHoleSegmentIndex(SerializedProperty theProp)
        {
            SerializedProperty subProp0 = theProp.Copy();
            subProp0.Next(true);    // property 0
            //subProp0.Next(true);    // property 0
            return subProp0;
        }
        
        SerializedProperty GetHoleTheShape(SerializedProperty theProp)
        {
            SerializedProperty subProp0 = theProp.Copy();
            subProp0.Next(true);    // property 0
            subProp0.Next(false);    // property 0
            return subProp0;
        }
        
        SerializedProperty GetHoleShapeIndex(SerializedProperty theProp)       
        {
            SerializedProperty subProp0 = theProp.Copy();
            subProp0.Next(true);    // property 0
            subProp0.Next(false);    // property 0
            subProp0.Next(false);    // property 0
            return subProp0;
        }

        #endregion


        void Space()
        {
            GUILayout.Space(guiSpacing);        
        }

        void GuessModuleSize()
        {
            Vector3 newVec = SS_Common.GetPrefabSize(Target.theWallPrefabs[0].thePrefab.name, '_');
            if (newVec != Vector3.zero)
            {
                moduleSize.vector3Value = newVec;
            }
        }




        #region GUI Drawer Methods 
        
        void DrawAreaName()
        {
            EditorGUILayout.BeginVertical("Button");
            GUILayout.Space(3);
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(myName,new GUIContent("Wall Name"));

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Target, "Change Area Name");
                SS_Common.RenameArea("SS_Wall", myName.stringValue, Target.transform);
            }

            GUILayout.Space(3);
            EditorGUILayout.EndVertical();
        }
      
        void DrawSegmentOptions()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical("Button");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Use Segments", GUILayout.MaxWidth(90));

            SerializedProperty prop = segmentFlag.Copy();
            prop.Next(true);
            prop.Next(true);//SIZE
            prop.Next(false);//Value 1

            EditorGUILayout.LabelField(new GUIContent("0"), GUILayout.MaxWidth(10));

            EditorGUILayout.PropertyField(prop, new GUIContent(""), GUILayout.MaxWidth(20));

            SerializedProperty prop2 = prop.Copy();
            prop2.Next(false);//Value 2
            EditorGUILayout.LabelField(new GUIContent("1"), GUILayout.MaxWidth(10));
            EditorGUILayout.PropertyField(prop2, new GUIContent(""), GUILayout.MaxWidth(20));

            SerializedProperty prop3 = prop2.Copy();
            prop3.Next(false);//Value 3
            EditorGUILayout.LabelField(new GUIContent("2"), GUILayout.MaxWidth(10));
            EditorGUILayout.PropertyField(prop3, new GUIContent(""), GUILayout.MaxWidth(20));

            SerializedProperty prop4 = prop3.Copy();
            prop4.Next(false);//Value 4
            EditorGUILayout.LabelField(new GUIContent("3"), GUILayout.MaxWidth(10));
            EditorGUILayout.PropertyField(prop4, new GUIContent(""), GUILayout.MaxWidth(20));



            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();

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
            serializedObject.ApplyModifiedProperties();

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

            if (GetWallPrefabArraySize().intValue >1)
            {
                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    Target.RemovePrefab();
                    Debug.Log("Removing Prefab");
                }                
            }
            if(GUILayout.Button("+",GUILayout.Width(25)))
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
            showPrefabs = EditorGUILayout.Foldout(showPrefabs, "Prefabs ( " + GetWallPrefabArraySize().intValue.ToString() + " )");
            EditorGUI.indentLevel--;

            if (showPrefabs)
            {
                for (int i = 0; i < GetWallPrefabArraySize().intValue; i++)
                {
                    EditorGUILayout.BeginVertical("Button");
                    Space();

                    EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Prefab", GUILayout.Width(40));
                        SerializedProperty theProp = GetWallPrefabArrayAtIndex(i);
                        EditorGUILayout.PropertyField(GetPrefabGO(theProp),new GUIContent(""));

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.PropertyField(GetPrefabFaceInwards(theProp));
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
                if (GetWallPrefabArraySize().intValue == 1)
                {
                    GuessModuleSize();
                }
            }
        }

        void DrawHolesOptions()
        {
            EditorGUILayout.BeginVertical("Button");

            Space();
            EditorGUILayout.PrefixLabel("Holes Options");
            EditorGUILayout.PropertyField(GetHolesArraySize(), new GUIContent("Holes Count"));
            Space();

            EditorGUILayout.BeginVertical("Button");

            EditorGUI.indentLevel++;
            showHoles = EditorGUILayout.Foldout(showHoles, "Holes");
            EditorGUI.indentLevel--;

            if (showHoles)
            {
                for (int i = 0; i < GetHolesArraySize().intValue; i++)
                {
                    EditorGUILayout.BeginVertical("Button");
                    SerializedProperty theProp2 = GetHolesArrayAtIndex(i);
                    EditorGUILayout.PropertyField(GetHoleSegmentIndex(theProp2));
                    EditorGUILayout.PropertyField(GetHoleTheShape(theProp2));
                    EditorGUILayout.PropertyField(GetHoleShapeIndex(theProp2));

                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();

            Space();

            EditorGUILayout.EndVertical();
        }

        void DrawPrefabSizeController()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical("Button");

                GUILayout.Space(2);

                EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Size",GUILayout.Width(35));

                    EditorGUILayout.PropertyField(moduleSize, new GUIContent(""));

                    if (GUILayout.Button("Guess"))
                    {
                        GuessModuleSize();
                    }

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(2);

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        void DrawCornerArrayFoldout()
        {
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();

            EditorGUILayout.BeginVertical("Button");

            EditorGUI.indentLevel++;
            showPrefabs = EditorGUILayout.Foldout(showPrefabs, "Prefabs ( " + GetCornerPrefabArraySize().intValue.ToString() + " )");
            EditorGUI.indentLevel--;

            if (showPrefabs)
            {
                for (int i = 0; i < GetCornerPrefabArraySize().intValue; i++)
                {
                    EditorGUILayout.BeginVertical("Button");
                    Space();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Corner", GUILayout.Width(40));
                    SerializedProperty theProp = GetCornerPrefabArrayAtIndex(i);
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
                if (GetWallPrefabArraySize().intValue == 1)
                {
                    GuessModuleSize();
                }
            }
        }



        #endregion



        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            Space();

            serializedObject.Update();

            EditorGUILayout.PropertyField(theShapeTransform);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.LabelField("Main Options", SS_Common.StyleTitle);

            // AREA NAME
            DrawAreaName();

            // AREA PARENT
            SS_Common.DrawInstancerParentUI(Target.gameObject);


            // SEGMENTS OPTIONS
            DrawTransformOffsetsFoldout();


            Space();
            Space();
            EditorGUILayout.LabelField("Wall Options", SS_Common.StyleTitle);
            // COLLIDER OPTIONS            
            DrawColliderOptions();            

            // SEGMENTS OPTIONS
            DrawSegmentOptions();

            Space();
            Space();

            EditorGUILayout.LabelField("Walls", SS_Common.StyleTitle);


            // PREFAB OPTIONS
            DrawPrefabOptions();

            //DISTRIBUTION OPTIONS
            if (GetWallPrefabArraySize().intValue > 1) DrawDistributionOptions();

            // PREFAB OPTIONS FOLDOUT
            DrawPrefabArrayFoldout();


            Space();
            Space();

            EditorGUILayout.LabelField("Corners", SS_Common.StyleTitle);

            DrawCornerArrayFoldout();


            Space();
            Space();
            ////////////////////////
            // HOLES OPTIONS
            ///////////////////////
            EditorGUILayout.LabelField("Holes", SS_Common.StyleTitle);

            DrawHolesOptions();

            /////////////////////////////////////////////////

            serializedObject.ApplyModifiedProperties();
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

    }
}
