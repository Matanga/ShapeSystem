using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace VFX.ShapeSystem
{
    public class SS_Common : Editor
    {
        #region Creation Menu Buttons   

        [MenuItem("GameObject/Create Other/LevelCanvas")]
        static void SS_CreateLevelCanvas()
        {
            //Create the canvas object
            GameObject newGo = new GameObject("LevelCanvas");

            RectTransform rectT = newGo.AddComponent<RectTransform>();
            rectT.Rotate(90, 0, 0);
            rectT.sizeDelta = new Vector2(25, 25);
            Canvas theCanvas = newGo.AddComponent<Canvas>();
            theCanvas.renderMode = RenderMode.WorldSpace;
            newGo.AddComponent<SS_LevelCanvas>();

            //Create a starting area

            GameObject newArea = new GameObject("SS_Area_Start");

            RectTransform newRect = newArea.AddComponent<RectTransform>();
            newRect.sizeDelta = new Vector2(rectT.rect.width, rectT.rect.height);

            //IMAGE STUFF
            Image newImage = newArea.AddComponent<Image>();
            newImage.color = Color.gray;

            //SS_RectTransformShape STUFF
            newArea.AddComponent<SS_LevelArea>();

            //POSITIONING STUFF
            newArea.transform.position = newGo.transform.position;
            newArea.transform.rotation = newGo.transform.rotation;
            newRect.SetParent(newGo.transform);

            Selection.activeGameObject = newGo;

        }

        [MenuItem("GameObject/Create Other/LevelArea")]
        static void SS_CreateSSArea()
        {
            if ((Selection.activeGameObject) == null) return;

            bool isValid = false;
            SS_LevelArea theRect = (Selection.activeGameObject).GetComponent<SS_LevelArea>();
            if (theRect != null)
                isValid = true;

            SS_LevelCanvas theLevel = (Selection.activeGameObject).GetComponent<SS_LevelCanvas>();
            if (theLevel != null)
                isValid = true;

            if (isValid)
            {
                SS_CreateLevelArea(Selection.activeGameObject, "" , Random.ColorHSV());
            }
            else
            {
                Debug.Log("Not valid");
            }

        }
        
        [MenuItem("GameObject/Create Other/SS_Wall")]
        static void SS_CreateMenuItemWall()
        {
            if ((Selection.activeGameObject) == null) return;
            SS_CreateWall((Selection.activeGameObject));
        }

        [MenuItem("GameObject/Create Other/SS_Floor")]
        static void SS_CreateMenuItemFloor()
        {
            if ((Selection.activeGameObject) == null) return;

            SS_CreateFloor(Selection.activeGameObject);

        }

        [MenuItem("GameObject/Create Other/SS_GridInstancer")]
        static void SS_CreateSSGridInstancer()
        {
            if ((Selection.activeGameObject) == null) return;
            SS_CreateGridInsts(Selection.activeGameObject);
        }

        [MenuItem("GameObject/Create Other/SS_GridScatter")]
        static void SS_CreateSSGridScatter()
        {
            if ((Selection.activeGameObject) == null) return;
            SS_CreateGridInsts(Selection.activeGameObject);
        }



        #endregion


        #region Editor Object Creation Methods

        public static void SS_CreateWall(GameObject theParent)
        {
            bool isValid = false;
            SS_LevelArea theRect = (theParent).GetComponent<SS_LevelArea>();
            if (theRect != null)
                isValid = true;

            if (isValid)
            {
                GameObject newGO = new GameObject("SS_Wall");
                SS_Wall SSWall = newGO.AddComponent<SS_Wall>();


                //Setup Placeholder Wall Prefab
                SSWall.theWallPrefabs = new SS_WallObjects[1];
                SSWall.theWallPrefabs[0] = new SS_WallObjects();
                SSWall.theWallPrefabs[0].thePrefab = SS_DefaultWallPrefab();

               // SSWall.moduleSize = new Vector3(3, 0.4f, 3);
                SSWall.moduleSize = GetPrefabSize(SSWall.theWallPrefabs[0].thePrefab.name, '_');


                //Setup Placeholder Corner Prefab

                SSWall.theCornerPrefabs = new SS_CornerObjects[1];

                SSWall.theCornerPrefabs[0] = new SS_CornerObjects();

                SSWall.theCornerPrefabs[0].thePrefab = SS_DefaultCornerPrefab();



                newGO.transform.position = theRect.transform.position;
                newGO.transform.rotation = theRect.transform.rotation;
                newGO.transform.SetParent(theRect.transform);

                theRect.UpdateLevelCanvas();
            }

        }
 
        public static void SS_CreateFloor(GameObject theParent)
        {
            bool isValid = false;
            SS_LevelArea theRect = (theParent).GetComponent<SS_LevelArea>();
            if (theRect != null)
                isValid = true;

            if (isValid)
            {
                GameObject newGO = new GameObject("SS_Floor");
                SS_Floor SSFloor = newGO.AddComponent<SS_Floor>();

       
                SSFloor.thePrefabs = new SS_FloorInstanceObject[1];

                SSFloor.thePrefabs[0] = new SS_FloorInstanceObject();
                SSFloor.thePrefabs[0].thePrefab = SS_DefaultFloorPrefab();

                //SSFloor.moduleSize = new Vector3(3, 0.25f, 3);

                SSFloor.moduleSize = GetPrefabSize(SSFloor.thePrefabs[0].thePrefab.name, '_');

                newGO.transform.position = theRect.transform.position;
                newGO.transform.rotation = theRect.transform.rotation;
                newGO.transform.SetParent(theRect.transform);

                theRect.UpdateLevelCanvas();
            }

        }

        public static void SS_CreateGridInsts(GameObject theParent)
        {
            bool isValid = false;
            SS_LevelArea theRect = (Selection.activeGameObject).GetComponent<SS_LevelArea>();
            if (theRect != null)
                isValid = true;

            if (isValid)
            {
                GameObject newGO = new GameObject("SS_Grid");
                SS_GridInstancer SSGrid = newGO.AddComponent<SS_GridInstancer>();

                SSGrid.thePrefabs = new SS_GridInstanceObject[1];

                SSGrid.thePrefabs[0] = new SS_GridInstanceObject();
                SSGrid.thePrefabs[0].thePrefab = SS_DefaultGridPrefab();

                newGO.transform.position = theRect.transform.position;
                newGO.transform.rotation = theRect.transform.rotation;
                newGO.transform.SetParent(theRect.transform);

                theRect.UpdateLevelCanvas();
            }


        }

        public static void SS_CreateGridScatters(GameObject theParent)
        {
            bool isValid = false;
            SS_LevelArea theRect = (Selection.activeGameObject).GetComponent<SS_LevelArea>();
            if (theRect != null)
                isValid = true;

            if (isValid)
            {
                GameObject newGO = new GameObject("SS_Scatter");
                SS_GridScatter SSGrid = newGO.AddComponent<SS_GridScatter>();

                SSGrid.thePrefabs = new SS_GridObjects[1];

                SSGrid.thePrefabs[0] = new SS_GridObjects();
                SSGrid.thePrefabs[0].thePrefab = SS_DefaultScatterPrefab();

                newGO.transform.position = theRect.transform.position;
                newGO.transform.rotation = theRect.transform.rotation;
                newGO.transform.SetParent(theRect.transform);

                theRect.UpdateLevelCanvas();
            }


        }

        public static void SS_CreateLevelArea(GameObject theParent, string theName,Color theColor)
        {
            bool isValid = false;
            RectTransform theRect = theParent.GetComponent<RectTransform>();
            if (theRect != null)
                isValid = true;
            
            if (isValid)
            {
                GameObject newGO = new GameObject(theName.Length==0?"Level_Area":("Level_Area_"+theName));

                //RECT TRANSFORM STUFF
                RectTransform parentRect = theParent.GetComponent<RectTransform>();
                RectTransform newRect = newGO.AddComponent<RectTransform>();
                newRect.sizeDelta = new Vector2(parentRect.rect.width, parentRect.rect.height);
                //IMAGE STUFF
                Image newImage = newGO.AddComponent<Image>();
                newImage.color = theColor;

                //SS_RectTransformShape STUFF
                newGO.AddComponent<SS_LevelArea>();

                //POSITIONING STUFF
                newGO.transform.position = theParent.transform.position;
                newGO.transform.rotation = theParent.transform.rotation;
                newRect.SetParent(theParent.transform);
            }


        }

        #endregion


        #region System Default Prefabs

        public static GameObject SS_DefaultWallPrefab()
        {
            GameObject newGO = (Resources.Load("SS_WallPlaceHolder_3.0x0.4x3.0", typeof(GameObject))) as GameObject;
            return newGO;        
        }

        public static GameObject SS_DefaultFloorPrefab()
        {
            GameObject newGO = (Resources.Load("SS_FloorPlaceHolder_3.0x3.0x0.25", typeof(GameObject))) as GameObject;
            return newGO;
        }

        public static GameObject SS_DefaultGridPrefab()
        {
            GameObject newGO = (Resources.Load("SS_GridPrefab_0.3x0.3x3.0", typeof(GameObject))) as GameObject;
            return newGO;
        }

        public static GameObject SS_DefaultScatterPrefab()
        {
            GameObject newGO = (Resources.Load("SS_ScattterPrefab_0.3x0.3x3.0", typeof(GameObject))) as GameObject;
            return newGO;
        }

        public static GameObject SS_DefaultCornerPrefab()
        {
            GameObject newGO = (Resources.Load("SS_CornerPlaceHolder_0.3x0.3x3.0", typeof(GameObject))) as GameObject;
            return newGO;
        }




        #endregion


        #region String Utils

        public static Vector3 GetPrefabSize(string theName, char theChar)
        {
            float xVal = 0;
            float yVal = 0;
            float zVal = 0;

            // ... This will separate all the words.
            string[] words = theName.Split(theChar);

            if (words != null && words.Length > 1)
            {
                string dd = words[words.Length - 1];

                string[] theSizes = dd.Split('x');

                if (theSizes.Length == 3)
                {
                    xVal = float.Parse(theSizes[0]);
                    yVal = float.Parse(theSizes[2]);
                    zVal = float.Parse(theSizes[1]);
                }
            }
            return new Vector3(xVal, yVal, zVal);
        }
    
        #endregion

        #region GUIStyles

        public static GUIStyle StyleTitle
        {
            get
            {
                GUIStyle styleTitle = new GUIStyle();
                styleTitle.fontStyle = FontStyle.Bold;
                styleTitle.fontSize = 14;
                return styleTitle;
            }
        }
             
        #endregion

        #region UI Utils

        public static void CheckForSwitchDisplayInput(GameObject theArea)
        {
            Event e = Event.current;
            if (EventType.KeyDown == e.type && KeyCode.R == e.keyCode)
            {
                if (e.modifiers == EventModifiers.Shift)
                {
                    SS_LevelCanvas theLevelCanvas = theArea.gameObject.GetComponentInParent<SS_LevelCanvas>();

                    if (theLevelCanvas == null)
                        theLevelCanvas = theArea.gameObject.GetComponent<SS_LevelCanvas>();

                    theLevelCanvas.SwitchLevelCanvasDebugDisplay();
                }
            }
        }

        public static void CheckForLevelCanvasUpdateInput(GameObject theGO)
        {
            if (Event.current.type == EventType.MouseUp)
            {
                SS_LevelCanvas theLevelCanvas = theGO.gameObject.GetComponentInParent<SS_LevelCanvas>();

                if (theLevelCanvas == null)
                    theLevelCanvas = theGO.gameObject.GetComponent<SS_LevelCanvas>();

                if (theLevelCanvas != null)
                    theLevelCanvas.UpdateChildren();
            }

        }

        public static void UIButtonGameObjectSelect(GameObject theGO)
        {
            if (GUILayout.Button(new GUIContent("S", "Select the object"), GUILayout.Width(20))) Selection.activeGameObject = theGO;
        }

        public static void UIButtonGameObjectDelete(GameObject theGO)
        {
            if (GUILayout.Button(new GUIContent("X", "Delete the object"), GUILayout.Width(20))) DestroyImmediate(theGO);
        }

        public static void DrawInstancerParentUI(GameObject theInstancer)
        {
            EditorGUILayout.BeginVertical("Button");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Parent:", GUILayout.Width(50));

            EditorGUILayout.LabelField(theInstancer.transform.parent.name, GUILayout.Width(145));

            SS_Common.UIButtonGameObjectSelect(theInstancer.transform.parent.gameObject);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

        }

        public static void RenameArea(string baseName,string newName, Transform t)
        {
            if (newName.Length == 0)
            {
                t.name = baseName;
            }
            else
            {
                t.name = baseName+ "_" + newName;
            }
        }

        public static void IndentMultiple(bool add, int amount)
        {
            if (add)
            {
                for (int i = 0; i < amount; i++)
                {
                    EditorGUI.indentLevel++;
                }
            }
            else
            {
                EditorGUI.indentLevel--;
            }

        }


        #endregion

        #region Editor Scene Utils
        
        public static void SetChildrenActiveInHierarchy(Transform theParent, bool val)
        {
            for (int i = 0; i < theParent.childCount; i++)
            {
                theParent.GetChild(i).gameObject.SetActive(val);
            }
        }

        #endregion

    }
}