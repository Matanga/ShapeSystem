using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace VFX.ShapeSystem
{
    [CustomEditor(typeof(SS_LevelArea))]
    public class SS_LevelAreaEditor : Editor
    {
        public SS_LevelArea Target
        {
            get
            {
                return (SS_LevelArea)target;
            }
        }

        #region Serialized Properties
        
        SerializedProperty areaName;

        SerializedProperty areaType;



        SerializedProperty showCreateNew;

        SerializedProperty showExistingSubAreas;

        SerializedProperty showExistingInstancers;
        
        SerializedProperty showCreationUI;
        SerializedProperty currInstancerType;

        #endregion

        #region Temp Properties
        
        /// <summary>
        /// The temp string to have the name for the new area
        /// </summary>
        string newAreaName = "";

        /// <summary>
        /// The temp color to have the name for the new area
        /// </summary>  
        Color newColor = Color.grey;

        #endregion

        #region Editor Methods  

        void OnEnable()
        {
            areaName = serializedObject.FindProperty("areaName");

            areaType = serializedObject.FindProperty("areaType");

            showCreateNew = serializedObject.FindProperty("showCreateNew");

            showExistingSubAreas = serializedObject.FindProperty("showExistingSubAreas");

            showExistingInstancers = serializedObject.FindProperty("showExistingInstancers");

            showCreationUI = serializedObject.FindProperty("showCreationUI");

            currInstancerType = serializedObject.FindProperty("currInstancerType");
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawAreaName();
            GUILayout.Space(2);

            DrawAreaType();

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Sub Areas", SS_Common.StyleTitle);

            DrawCreateAreaGUI();
            GUILayout.Space(2);

            DrawExistingSubAreas();
            GUILayout.Space(2);

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Instancers", SS_Common.StyleTitle); DrawCreateInstancer();
            GUILayout.Space(2);

            DrawExistingInstancers();

            serializedObject.ApplyModifiedProperties();

        }
        
        private void OnSceneGUI()
        {
            SS_Common.CheckForSwitchDisplayInput(Target.gameObject);

            SS_Common.CheckForLevelCanvasUpdateInput(Target.gameObject);

            SceneGUIDrawShapeSegments();
        }

        #endregion

        #region Inspector GUI  Methods

        /// <summary>
        /// Draw Name
        /// </summary>
        void DrawAreaName() 
        {
            EditorGUILayout.BeginVertical("Button");
            GUILayout.Space(7);
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(areaName);

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Target, "Change Area Name");
                Target.RenameArea();
            }
            GUILayout.Space(7);
            EditorGUILayout.EndVertical();

        }

        /// <summary>
        /// Draw Area Type
        /// </summary>
        void DrawAreaType()
        {
            EditorGUILayout.BeginVertical("Button");
            GUILayout.Space(7);
            serializedObject.Update();


            EditorGUILayout.PropertyField(areaType);

            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(7);
            EditorGUILayout.EndVertical();

        }
        /// <summary>
        /// CREATE AREA
        /// </summary>
        void DrawCreateAreaGUI()
        {
            EditorGUILayout.BeginVertical("Button");

            EditorGUI.indentLevel++;
            showCreateNew.boolValue = EditorGUILayout.Foldout(showCreateNew.boolValue, "Create Area");
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
                    SS_Common.SS_CreateLevelArea(Target.gameObject, newAreaName, newColor);
                    newAreaName = "";
                }

            }
            EditorGUILayout.EndVertical();
        }

        void DrawExistingSubAreasGUI(SS_LevelArea[] allAreas)
        {
            if (allAreas != null && allAreas.Length > 0)
            {
                for (int i = 0; i < allAreas.Length; i++)
                {
                    EditorGUILayout.BeginVertical("Button");

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(allAreas[i].name);

                    SS_Common.UIButtonGameObjectSelect(allAreas[i].gameObject);
                    SS_Common.UIButtonGameObjectDelete(allAreas[i].gameObject);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }
            }
        }
        
        void DrawExistingSubAreas()
        {
            SS_LevelArea[] theAreas = UpdateSubAreasInfo();

            if (theAreas == null || theAreas.Length == 0) return;

            EditorGUILayout.BeginVertical("Button");

            EditorGUI.indentLevel++;
            showExistingSubAreas.boolValue = EditorGUILayout.Foldout(showExistingSubAreas.boolValue, "Show Sub Areas");
            EditorGUI.indentLevel--;

            if (showExistingSubAreas.boolValue)
            {
                DrawExistingSubAreasGUI(theAreas);
            }

            EditorGUILayout.EndVertical();

        }

        /// <summary>
        /// CREATE INSTANCER
        /// </summary>
        void DrawCreateInstancer()
        {
            EditorGUILayout.BeginVertical("Button");
            //GUILayout.Space(7);


            EditorGUI.indentLevel++;
            showCreationUI.boolValue = EditorGUILayout.Foldout(showCreationUI.boolValue, "Create Instancer");
            EditorGUI.indentLevel--;

            if (showCreationUI.boolValue)
            {
                 EditorGUILayout.PropertyField(currInstancerType,new GUIContent("Type"));

                 if (GUILayout.Button("Create"))
                 {
                     //Debug.Log("Creating");
                     string theType= currInstancerType.enumNames[currInstancerType.enumValueIndex];
                     //Debug.Log(theType);
                     switch (theType)
                     {
                         case "Floor":
                             SS_Common.SS_CreateFloor(Target.gameObject);
                             break;
                         case "Wall":
                             //Debug.Log("Creating Wall");
                             SS_Common.SS_CreateWall(Target.gameObject);
                             break;
                         case "GridInstance":
                             SS_Common.SS_CreateGridInsts(Target.gameObject);
                             break;
                         case "Scatter":
                             SS_Common.SS_CreateGridScatters(Target.gameObject);
                             break;
                         case "Shape":
                             break;
                         default:
                             break;
                     }

                 }
            }

            //GUILayout.Space(7);
            EditorGUILayout.EndVertical();
        
        }

        void DrawExistingInstancers()
        {
            SS_Floor[] allFloor = UpdateFloorInstancersInfo();

            SS_Wall[] allWalls = UpdateWallInsatncersInfo();

            SS_GridInstancer[] allGrids = UpdateGridInstancersInfo();

            SS_GridScatter[] allScatters = UpdateGridScattersInfo();


            

            bool isValid = false;

            if (allFloor != null)
                if(allFloor.Length!=0)      
                    isValid = true;

            if (allWalls != null)
                if (allWalls.Length != 0)
                    isValid = true;

            if (allGrids != null)
                if (allGrids.Length != 0)
                    isValid = true;

            if (allScatters != null)
                if (allScatters.Length != 0)
                    isValid = true;




            if (!isValid) return;

            EditorGUILayout.BeginVertical("Button");

            EditorGUI.indentLevel++;
            showExistingInstancers.boolValue = EditorGUILayout.Foldout(showExistingInstancers.boolValue, "Show Instancers");
            EditorGUI.indentLevel--;

            if (showExistingInstancers.boolValue)
            {
                DrawFloorsGUI(allFloor);

                DrawWallsGUI(allWalls);

                DrawGridInstsGUI(allGrids);

                DrawGridScattersGUI(allScatters);

            }

            EditorGUILayout.EndVertical();

        }

        void DrawFloorsGUI(SS_Floor[] allFloor)
        {
            if (allFloor != null && allFloor.Length > 0)
            {
                for (int i = 0; i < allFloor.Length; i++)
                {
                    EditorGUILayout.BeginVertical("Button");

                    EditorGUILayout.BeginHorizontal();

                    EditorGUI.indentLevel++;
                    showCurrentFloor[i] = EditorGUILayout.Foldout(showCurrentFloor[i], allFloor[i].name);
                    EditorGUI.indentLevel--;


                    if (GUILayout.Button("Select"))Selection.activeGameObject = allFloor[i].gameObject;

                    
                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;
                    if (GUILayout.Button("X"))DestroyImmediate(allFloor[i].gameObject);                    
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;

                    EditorGUILayout.EndHorizontal();

                    if (showCurrentFloor[i])
                    {
                        for (int x = 0; x < allFloor[i].thePrefabs.Length; x++)
                        {
                            EditorGUILayout.BeginVertical("Button");
                            string thePrefabName = allFloor[i].thePrefabs[x].thePrefab.name;

                            EditorGUILayout.LabelField("Prefab: " + thePrefabName);

                            int thePrefabCount = Target.GetChildCountByName(allFloor[i].transform, thePrefabName);
                            EditorGUILayout.LabelField("Count: " + thePrefabCount);

                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }

        void DrawWallsGUI(SS_Wall[] allWalls)
        {
            if (allWalls != null && allWalls.Length > 0)
            {
                for (int i = 0; i < allWalls.Length; i++)
                {
                    EditorGUILayout.BeginVertical("Button");

                    EditorGUILayout.BeginHorizontal();

                    EditorGUI.indentLevel++;
                    showCurrentWall[i] = EditorGUILayout.Foldout(showCurrentWall[i], allWalls[i].name);
                    EditorGUI.indentLevel--;


                    if (GUILayout.Button("Select")) Selection.activeGameObject = allWalls[i].gameObject;
                    

                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;
                    if (GUILayout.Button("X")) DestroyImmediate(allWalls[i].gameObject);                    
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;

                    EditorGUILayout.EndHorizontal();

                    if (showCurrentWall[i])
                    {
                        for (int x = 0; x < allWalls[i].theWallPrefabs.Length; x++)
                        {
                            EditorGUILayout.BeginVertical("Button");
                            string thePrefabName = allWalls[i].theWallPrefabs[x].thePrefab.name;

                            EditorGUILayout.LabelField("Prefab: " + thePrefabName);

                            int thePrefabCount = Target.GetChildCountByName(allWalls[i].transform, thePrefabName);
                            EditorGUILayout.LabelField("Count: " + thePrefabCount);

                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }

        void DrawGridInstsGUI(SS_GridInstancer[] allGrids)
        {
            if (allGrids != null && allGrids.Length > 0)
            {
                for (int i = 0; i < allGrids.Length; i++)
                {
                    EditorGUILayout.BeginVertical("Button");

                    EditorGUILayout.BeginHorizontal();

                    EditorGUI.indentLevel++;
                    showCurrentGridInst[i] = EditorGUILayout.Foldout(showCurrentGridInst[i], allGrids[i].name);
                    EditorGUI.indentLevel--;

                    //SelectObject
                    if (GUILayout.Button("Select")) Selection.activeGameObject = allGrids[i].gameObject;
                    
                    //Destroy Object 
                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;
                    if (GUILayout.Button("X")) DestroyImmediate(allGrids[i].gameObject);                    
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;

                    EditorGUILayout.EndHorizontal();

                    if (showCurrentGridInst[i])
                    {
                        for (int x = 0; x < allGrids[i].thePrefabs.Length; x++)
                        {
                            EditorGUILayout.BeginVertical("Button");
                            string thePrefabName = allGrids[i].thePrefabs[x].thePrefab.name;

                            EditorGUILayout.LabelField("Prefab: " + thePrefabName);

                            int thePrefabCount = Target.GetChildCountByName(allGrids[i].transform, thePrefabName);
                            EditorGUILayout.LabelField("Count: " + thePrefabCount);

                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }


        void DrawGridScattersGUI(SS_GridScatter[] allInstancers)
        {
            if (allInstancers != null && allInstancers.Length > 0)
            {
                for (int i = 0; i < allInstancers.Length; i++)
                {
                    EditorGUILayout.BeginVertical("Button");

                    EditorGUILayout.BeginHorizontal();

                    EditorGUI.indentLevel++;
                    showCurrentGridScatters[i] = EditorGUILayout.Foldout(showCurrentGridScatters[i], allInstancers[i].name);
                    EditorGUI.indentLevel--;

                    //SelectObject
                    if (GUILayout.Button("Select")) Selection.activeGameObject = allInstancers[i].gameObject;

                    //Destroy Object 
                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;
                    if (GUILayout.Button("X")) DestroyImmediate(allInstancers[i].gameObject);
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;

                    EditorGUILayout.EndHorizontal();

                    if (showCurrentGridScatters[i])
                    {
                        for (int x = 0; x < allInstancers[i].thePrefabs.Length; x++)
                        {
                            EditorGUILayout.BeginVertical("Button");
                            string thePrefabName = allInstancers[i].thePrefabs[x].thePrefab.name;

                            EditorGUILayout.LabelField("Prefab: " + thePrefabName);

                            int thePrefabCount = Target.GetChildCountByName(allInstancers[i].transform, thePrefabName);
                            EditorGUILayout.LabelField("Count: " + thePrefabCount);

                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }




        #endregion

        #region Update Elements Info & Methods

        bool[] showCurrentFloor=new bool[1];

        bool[] showCurrentWall = new bool[1];

        bool[] showCurrentGridInst = new bool[1];

        bool[] showCurrentGridScatters = new bool[1];




        SS_LevelArea[] UpdateSubAreasInfo()
        {
            SS_LevelArea[] theAreas = Target.gameObject.GetComponentsInChildren<SS_LevelArea>();

            List<SS_LevelArea> finalAreas = new List<SS_LevelArea>();

            if (theAreas != null && theAreas.Length != 0)
            {
                for (int i = 0; i < theAreas.Length; i++)
                {
                    if (theAreas[i].transform != Target.transform)
                    {
                        finalAreas.Add(theAreas[i]);
                    }
                }
            }
            return finalAreas.ToArray();
        }

        SS_Floor[] UpdateFloorInstancersInfo()
        {
            SS_Floor[] allFloor = Target.gameObject.GetComponentsInChildren<SS_Floor>();

            if (showCurrentFloor.Length != allFloor.Length)
            {
                showCurrentFloor = new bool[allFloor.Length];
            }

            return allFloor;
        }

        SS_Wall[] UpdateWallInsatncersInfo()
        {
            SS_Wall[] theWalls = Target.gameObject.GetComponentsInChildren<SS_Wall>();

            if (showCurrentWall.Length != theWalls.Length)
            {
                showCurrentWall = new bool[theWalls.Length];
            }
            return theWalls;
        }

        SS_GridInstancer[] UpdateGridInstancersInfo()
        {
            SS_GridInstancer[] theGrids = Target.gameObject.GetComponentsInChildren<SS_GridInstancer>();

            if (showCurrentGridInst.Length != theGrids.Length)
            {
                showCurrentGridInst = new bool[theGrids.Length];
            }
            return theGrids;
        }

        SS_GridScatter[] UpdateGridScattersInfo()
        {
            SS_GridScatter[] theGrids = Target.gameObject.GetComponentsInChildren<SS_GridScatter>();

            if (showCurrentGridScatters.Length != theGrids.Length)
            {
                showCurrentGridScatters = new bool[theGrids.Length];
            }
            return theGrids;
        }




        #endregion

        #region Scene GUI  Methods

        void SceneGUIDrawShapeSegments()
        { 
               Vector3[] thePoints= new Vector3[4];
            Target.myRect.GetWorldCorners(thePoints);

            Handles.color = Color.red;

            Vector3 pos1 = Vector3.Lerp(thePoints[0], thePoints[1], 0.5f) + Vector3.up * 0.5f;
            Handles.Label(pos1,"0");

            Vector3 pos2 = Vector3.Lerp(thePoints[1], thePoints[2], 0.5f) + Vector3.up * 0.5f;
            Handles.Label(pos2, "1");

            Vector3 pos3 = Vector3.Lerp(thePoints[2], thePoints[3], 0.5f) + Vector3.up * 0.5f;
            Handles.Label(pos3, "2");

            Vector3 pos4 = Vector3.Lerp(thePoints[3], thePoints[0], 0.5f) + Vector3.up * 0.5f;
            Handles.Label(pos4, "3");     
        
        }
       
        #endregion

    }
}