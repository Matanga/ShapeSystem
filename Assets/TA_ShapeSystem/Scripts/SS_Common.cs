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

        [MenuItem("GameObject/Create Other/SS_Spline")]
        static void SS_CreateSSSpline()
        {
            SS_CreateSpline();
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

        public static void SS_CreateSpline()
        {
            GameObject newGO = new GameObject("SS_Spline");
            SS_BezierCurve SSCurve = newGO.AddComponent<SS_BezierCurve>();
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

        #region Bezier Base Methods

        //The De Casteljau's Algorithm
        public static Vector3 DeCasteljausAlgorithm(float t, BezierSegment seg)
        {
            //Linear interpolation = lerp = (1 - t) * A + t * B
            //Could use Vector3.Lerp(A, B, t)

            //To make it faster
            float oneMinusT = 1f - t;

            //Layer 1
            Vector3 Q = oneMinusT * seg.A + t * seg.B;
            Vector3 R = oneMinusT * seg.B + t * seg.C;
            Vector3 S = oneMinusT * seg.C + t * seg.D;

            //Layer 2
            Vector3 P = oneMinusT * Q + t * R;
            Vector3 T = oneMinusT * R + t * S;

            //Final interpolated position
            Vector3 U = oneMinusT * P + t * T;

            return U;
        }

        //The derivative of cubic De Casteljau's Algorithm
        public static Vector3 DeCasteljausAlgorithmDerivative(float t, BezierSegment seg)
        {
            Vector3 dU = t * t * (-3f * (seg.A - 3f * (seg.B - seg.C) - seg.D));

            dU += t * (6f * (seg.A - 2f * seg.B + seg.C));

            dU += -3f * (seg.A - seg.B);

            return dU;
        }

        //Get and infinite small length from the derivative of the curve at position t
        public static float GetArcLengthIntegrand(float t, BezierSegment seg)
        {
            //The derivative at this point (the velocity vector)
            Vector3 dPos = DeCasteljausAlgorithmDerivative(t, seg);

            //This the how it looks like in the YouTube videos
            //float xx = dPos.x * dPos.x;
            //float yy = dPos.y * dPos.y;
            //float zz = dPos.z * dPos.z;

            //float integrand = Mathf.Sqrt(xx + yy + zz);

            //Same as above
            float integrand = dPos.magnitude;

            return integrand;
        }

        //Get the length of the curve between two t values with Simpson's rule
        public static float GetLengthSimpsons(float tStart, float tEnd, BezierSegment seg)
        {
            //This is the resolution and has to be even
            int n = 20;

            //Now we need to divide the curve into sections
            float delta = (tEnd - tStart) / (float)n;

            //The main loop to calculate the length

            //Everything multiplied by 1
            float endPoints = GetArcLengthIntegrand(tStart, seg) + GetArcLengthIntegrand(tEnd, seg);

            //Everything multiplied by 4
            float x4 = 0f;
            for (int i = 1; i < n; i += 2)
            {
                float t = tStart + delta * i;

                x4 += GetArcLengthIntegrand(t, seg);
            }

            //Everything multiplied by 2
            float x2 = 0f;
            for (int i = 2; i < n; i += 2)
            {
                float t = tStart + delta * i;

                x2 += GetArcLengthIntegrand(t, seg);
            }

            //The final length
            float length = (delta / 3f) * (endPoints + 4f * x4 + 2f * x2);

            return length;
        }

        //Use Newton–Raphsons method to find the t value at the end of this distance d
        public static float FindTValue(float d, float totalLength, BezierSegment seg)
        {
            //Need a start value to make the method start
            //Should obviously be between 0 and 1
            //We can say that a good starting point is the percentage of distance traveled
            //If this start value is not working you can use the Bisection Method to find a start value
            //https://en.wikipedia.org/wiki/Bisection_method
            float t = d / totalLength;

            //Need an error so we know when to stop the iteration
            float error = 0.001f;

            //We also need to avoid infinite loops
            int iterations = 0;

            while (true)
            {
                //Newton's method
                float tNext = t - ((GetLengthSimpsons(0f, t, seg) - d) / GetArcLengthIntegrand(t, seg));

                //Have we reached the desired accuracy?
                if (Mathf.Abs(tNext - t) < error)
                {
                    break;
                }

                t = tNext;

                iterations += 1;

                if (iterations > 1000)
                {
                    break;
                }
            }

            return t;
        }







        #endregion

        #region Bezier Helper Methods


        //Divide the curve into equal steps
        public static List<Vector3> ResampleCurve(int numSteps, ShapeElement element,Transform theT)
        {
            float totalLenght = GetCurveLength(element,theT);
            float sectionLength = totalLenght / numSteps;


            List<Vector3> curveSteps = new List<Vector3>();


            /*   
                    20          8          35                 17                -segmentLenghts     N
                    
             X -------------X--------X-------------------X-----------X          -theSegs            N
              
             0              20      28                   63          80         -knotDistances      N+1    
             
             */

            //Create A list of Bezier Segment
            List<BezierSegment> theSegs = new List<BezierSegment>();
            //Create A list of Bezier Segment Lengths
            List<float> segmentLenghts = new List<float>();


            for (int i = 0; i < element.knots.Length - 1; i++)
            {
                //CREATE A BEZIER SEGMENT 
                BezierSegment seg = new BezierSegment();
                seg.A = element.knots[i].KWorldPos(theT);              //START POINT
                seg.B = element.knots[i].KHandleOutWorldPos(theT);     //START TANGENT
                seg.C = element.knots[i + 1].KHandleInWorldPos(theT);  //END TANGENT
                seg.D = element.knots[i + 1].KWorldPos(theT);          //END POINT

                theSegs.Add(seg);

                segmentLenghts.Add(GetLengthSimpsons(0f, 1f, seg));
            }
            

            //Create a list of knot Distances
            List<float> knotDistances = new List<float>();
            float tempDist = 0;
            knotDistances.Add(tempDist);
            for (int i = 0; i < segmentLenghts.Count; i++)
            {
                tempDist = tempDist + segmentLenghts[i];
                knotDistances.Add(tempDist);
            }

            //Add the first point
            curveSteps.Clear();
            curveSteps.Add(theSegs[0].A);

            //For each step
            for (int i = 1; i < numSteps; i++)
            {
                //Get this point dist
                float distAlong = sectionLength * i;

                //Find out in wich segment it falls
                int currSegIndex = 0;
                for (int f = 0; f < knotDistances.Count - 1; f++)
                {
                    if (distAlong > knotDistances[f])
                    {
                        if (distAlong < knotDistances[f + 1])
                        {
                            currSegIndex = f;
                        }
                    }
                }

                //Find out at which distance along that segment the point falls
                float segPointDist = distAlong - knotDistances[currSegIndex];

                //Use Newton–Raphsons method to find the t value from the start of the curve 
                //to the end of the distance we have
                float t = FindTValue(segPointDist, segmentLenghts[currSegIndex], theSegs[currSegIndex]);

                //Get the coordinate on the Bezier curve at this t value
                Vector3 pos = DeCasteljausAlgorithm(t, theSegs[currSegIndex]);

                //Add the next point to the curveSteps list
                curveSteps.Add(pos);
            }
            return curveSteps;
        }

        /// <summary>
        /// Gets the sum of all individuals segments in this curve
        /// </summary>
        /// <returns></returns>
        public static float GetCurveLength(ShapeElement element, Transform theT)
        {
            float TotalLenghtSum = 0;
            //Draw Each Element

            for (int i = 0; i < element.knots.Length - 1; i++)
            {
                //CREATE A BEZIER SEGMENT 
                BezierSegment seg = new BezierSegment();
                seg.A = element.knots[i].KWorldPos(theT);              //START POINT
                seg.B = element.knots[i].KHandleOutWorldPos(theT);     //START TANGENT
                seg.C = element.knots[i + 1].KHandleInWorldPos(theT);  //END TANGENT
                seg.D = element.knots[i + 1].KWorldPos(theT);          //END POINT

                float totalLength = GetLengthSimpsons(0f, 1f, seg);
                TotalLenghtSum = TotalLenghtSum + totalLength;
            }
            
            return TotalLenghtSum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>a Vector3 in world coordinates </returns>
        public static Vector3 GetWorldPosAtCurvePoint(BezierSegment seg, float dist)
        {

            //Find the total length of the curve
            float totalLength = SS_Common.GetLengthSimpsons(0f, 1f, seg);

            //Use Newton–Raphsons method to find the t value from the start of the curve 
            //to the end of the distance we have
            float t = SS_Common.FindTValue(dist, totalLength, seg);

            //Get the coordinate on the Bezier curve at this t value
            Vector3 pos = SS_Common.DeCasteljausAlgorithm(t, seg);

            return pos;
        }



        #endregion


        #region Line Methods    

        public static float[] GetLineData(Vector3 p1, Vector3 p2)
        {
            float[] temp = new float[2];




            return temp;
        }

        public static Vector3 GetLineIntersection(Vector3 p0, Vector3 p1, Vector3 q0, Vector3 q1)
        {
            Vector3 returnVec = Vector3.zero;
            //Direction Vectors
            Vector3 DP = p1 - p0;
            Vector3 DQ = q1 - q0;
            //Start difference
            Vector3 PQ = q0 - p0;
            //Find Values
            float a = Vector3.Dot(DP, DP);
            float b = Vector3.Dot(DP, DQ);
            float c = Vector3.Dot(DQ, DQ);
            float d = Vector3.Dot(DP, PQ);
            float e = Vector3.Dot(DQ, PQ);
            //Find discriminant
            float DD =( a * c) -( b * b);

            //Debug.Log("DD " + DD.ToString());
            //If DD == 0 then segments are parallel
            if (DD != 0)
            {
                //Find parameters for the closest points on lines 
                float tt =Mathf.Abs( (b * e - c * d) / DD);
                float uu = Mathf.Abs((a * e - b * d) / DD);

                bool intersect = true;

                if (tt < 0 || tt > 1)
                    intersect = false;

                if (uu < 0 || uu > 1)
                    intersect = false;
                //Debug.Log("tt " + tt.ToString()+" uu "+uu.ToString());

                //Debug.Log("Intersect " + intersect.ToString());

                if (intersect)
                {
                    //Find distance between points

                    Vector3 P = p0 + tt * DP;
                    Vector3 Q = q0 + uu * DQ;

                    float dist = Vector3.Distance(P, Q);
                    //Debug.Log("dist " + dist.ToString());

                    if (dist < 0.1f)
                    {
                        returnVec = P;
                        //Debug.Log("returnVec " + returnVec.ToString());
                    }
                }
            }
            return returnVec;

        }




        #endregion

    }
}