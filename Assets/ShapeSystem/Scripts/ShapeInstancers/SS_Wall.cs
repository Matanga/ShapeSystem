using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX.ShapeSystem
{
    [System.Serializable]
    public class SS_WallObjects
    {
        public GameObject thePrefab;
        public bool flip;
    }

    [System.Serializable]
    public class SS_CornerObjects
    {
        public GameObject thePrefab;

    }


    [System.Serializable]
    public class SS_WallHole
    {
        public int segmentIndex;
        public SS_LevelArea theShape;
        public int theHoleSegment;
        public float width;
        public float startDistance;

        public void SetParameters(Vector3[] thePoints)
        {         
            //find out wich sides of the faces are aligned
            if (theShape == null) return;

            Vector3[] myPoints=new Vector3[4];
            theShape.myRect.GetWorldCorners(myPoints);

            Vector3[] wallSegment= new Vector3[2];
            //Debug.Log(thePoints.Length);
            //Debug.Log(wallSegment.Length);

            wallSegment[0] = myPoints[theHoleSegment];

            if(theHoleSegment==3)
            {
                wallSegment[1] = myPoints[0];
            }
            else
            {
                wallSegment[1] = myPoints[theHoleSegment + 1];
            }

            float totalDist= Vector3.Distance(thePoints[0],thePoints[1]);

            float segDist = Vector3.Distance(wallSegment[0], wallSegment[1]);



            startDistance = Vector3.Distance(thePoints[0], wallSegment[1]);

            //width = (startDistance + segDist) / totalDist;
            width = Vector3.Distance(wallSegment[0], wallSegment[1]);


            //Vector3 HoleEnd = Vector3.Lerp(thePoints[0], thePoints[1], width);
            //width = Vector3.Distance(thePoints[0], HoleEnd);
            
                    
        }

    }
    
    public class SS_Wall : MonoBehaviour
    {
        /// <summary>
        /// The Data Holders of the Wall Segments. These contain the prefab reference,
        /// distribution options and more.
        /// </summary>
        public SS_WallObjects[] theWallPrefabs;
        
        /// <summary>
        /// The Rectangle shape that will be used to create the walls along.
        /// </summary>        
        public SS_Rectangle theShape;

        /// <summary>
        /// The Data Holders of the Corner Segments. These contain the prefab reference, and other info.
        /// </summary>        
        public SS_CornerObjects[] theCornerPrefabs;




        #region Distribution Options

        /// <summary>
        /// Distribution type for the prefabs
        /// </summary>
        public DistributionType myDistrType = DistributionType.Sequence;
        public int randSeed;


        int currSequenceIndex = 0;
        int randNum = 0;


        #endregion

        #region RectTransform Properties

        /// <summary>
        /// The Reference to this transforms parent´s RectTransform
        /// </summary>
        public RectTransform rectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = transform.parent.GetComponent<RectTransform>();
                }
                return rectTransform;
            }
        }


        public SS_BezierCurve theShapeTransform;


        /// <summary>
        /// Get the list of points from the RectTransform component of the parent of this transform
        /// </summary>
        public Vector3[] rectShape
        {
            get
            {
                if (theShapeTransform != null)
                {
                    return theShapeTransform.thePoints();
                }
                else
                {                
                    Vector3[] thePoints = new Vector3[4];
                    RectTransform.GetWorldCorners(thePoints);
                    return thePoints;
                }

            }
        }

        /// <summary>
        /// The length value of the RectTransform (property height in RectTransform)
        /// </summary>
        public float rectLength
        {
            get
            {
                Vector3[] thePoints = new Vector3[4];
                RectTransform.GetLocalCorners(thePoints);
                return Vector3.Distance(thePoints[0], thePoints[1]);
            }
        }

        /// <summary>
        /// The Width value of the RectTransform 
        /// </summary>        
        public float rectWidth
        {
            get
            {
                Vector3[] thePoints = new Vector3[4];
                RectTransform.GetLocalCorners(thePoints);
                return Vector3.Distance(thePoints[0], thePoints[3]);
            }
        }



        #endregion

        public string myName;

        #region Collider Options

        /// <summary>
        /// Flag to enable or disable collider generation
        /// </summary>
        public bool generateCollider;
        /// <summary>
        /// Collider Height
        /// </summary>
        public float colliderHeight = 0.25f;
        /// <summary>
        /// Flag to know enable /disable Static navigation
        /// </summary>
        [Tooltip("Will this floor collision be static for the navigation mesh calculation?")]
        public bool navigationStatic;


        #endregion

        #region Offset Options

        public Vector3 moduleSize;
        public Vector3 offsetTranslate;
        public Vector3 offsetRotate;

        #endregion

        public bool showPrefabOptionsUI;

        /// <summary>
        /// Information Data holders for holes in the wall
        /// </summary>
        public SS_WallHole[] theHoles;

        #region Segment Properties  
        public bool[] segmentFlag=new bool[4]{true,true,true,true};
        public int[] selectedSegments;
        private int[] currSegments;
        public int[] CurrSegments
        {
            get {
                    List<int> segments = new List<int>();
                    for (int i = 0; i < segmentFlag.Length; i++)
                    {
                        if (segmentFlag[i] == true) segments.Add(i);
                    }
                    return segments.ToArray();

                }

            set { currSegments = value; }
        }
        #endregion
        

        #region Current Segment Temp Values

        Vector3[] theSegment;
        float segmentRemainder;
        int segmentCount;
        float segmentSeparation;
        float segmentScale; 
  
        #endregion

        private List<Vector3[]> myShapeVersion= new List<Vector3[]>();
        private int currentPrefab = 0;

        #if UNITY_EDITOR
        

        public void OnRectUpdateCallback()
        {
            GeneratePrefabs();
        }

        public void RemovePreviousInstances()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        void GenerateCollider(Vector3[] thePoints)
        {
            GameObject newGo = new GameObject("SS_Wall_Collider");

            newGo.transform.SetParent(transform);
            Vector3 midpoint = Vector3.Lerp(thePoints[0], thePoints[1], 0.5f);


            newGo.transform.position = midpoint;


            BoxCollider theCollider = newGo.AddComponent<BoxCollider>();

            theCollider.size = new Vector3(rectWidth, colliderHeight, rectLength);

            //theCollider.center = new Vector3(0, -(colliderHeight / 2), 0);

            if (navigationStatic)
            {
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(newGo, UnityEditor.StaticEditorFlags.NavigationStatic);
            }
        }


        // INFO 
        // INFO
        ////////////////////////////////////////////////////////////////////////////
        //
        //   A List of Vector3 will be created for  each original segment
        //   --When the list has 0 values it means that this segment wont be drawn
        //   --When there are 2 values it is the original segment
        //
        //      0------------------------------0
        //
        //   -- When there are more that 2 values then the line will be interpeted as a series 
        //      of alternating lines
        //
        //      0---0    0----0    0-----------0
        //
        //    --Lines will always start with a valid segment
        //      in the cases where a hole cuts the begining of the segment
        //      two points with identical positions will be placed at the 
        //      begining
        //
        //    --  
        //

        #region Instancer Helper Methods


        /// <summary>
        /// Returns the index of the closest point on the Vector3[](interpreted as a line) that is below the provided
        /// </summary>
        int GetClosestIndex(Vector3[] thePoints, float theDist)
        {
            float closestDist = 10000;
            int closestIndex = -1;
            for (int i = 0; i < thePoints.Length; i++)
            {
                float currDist = Vector3.Distance(thePoints[0], thePoints[i]);
                if (currDist < theDist)
                {
                    if (currDist <= closestDist)
                    {
                        closestDist = currDist;
                        closestIndex = i;
                    }
                }
            }
            return closestIndex;
        }

        /// <summary>
        /// Does the provided distance along a line fall inside a pre exisiting hole
        /// </summary>
        bool IsPointOnSegmentHole(Vector3[] thePoints,float theDist)
        {
            int closestIndex = GetClosestIndex( thePoints, theDist);
            //Now we know which is the closest index to the given dist
            if (closestIndex % 2 != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates an internal version of the original shape 
        /// This version contemplates wheter a segment is being used or not
        /// and also adds holdes whenever necessary
        /// </summary>
        void SetupShapeInterpretation()
        {
            myShapeVersion.Clear();
            //Debug.Log("starting shape interpretation");

            if (theShapeTransform != null)
            {
                //Create an array for each original Segment
                for (int i = 0; i < theShapeTransform.thePoints().Length-1; i++)
                {
                    //Debug.Log("Adding empty array at index " + i);
                    myShapeVersion.Add(new Vector3[0]);
                }

            }
            else
            {
                //Create an array for each original Segment
                for (int i = 0; i < rectShape.Length; i++)
                {
                    //Debug.Log("Adding empty array at index " + i);
                    myShapeVersion.Add(new Vector3[0]);
                }
            }




            if (theShapeTransform != null)
            {
                //Now fill the point data for the segments that are currently active
                for (int o = 0; o < myShapeVersion.Count; o++)
                {
                    //Debug.Log("Adding points to current segment used index, " + currSegIndex);

                    myShapeVersion[o] = new Vector3[2];

                    //Add the first point
                    myShapeVersion[o][0] = rectShape[o];
                    //Debug.Log("Adding points to current segment used index, " + currSegIndex + " new point " + theShape.ThePoints[CurrSegments[currSegIndex]]);

                    //Add the second point depending on the position of the current point
                    if (o == rectShape.Length - 1)
                    {
                        myShapeVersion[o][1] = rectShape[0];
                        //Debug.Log("Adding points to current segment used index, " + currSegIndex + " new point " + theShape.ThePoints[0]);

                    }
                    else
                    {
                        myShapeVersion[o][1] = rectShape[o + 1];
                        //Debug.Log("Adding points to current segment used index, " + currSegIndex + " new point " + theShape.ThePoints[CurrSegments[currSegIndex] + 1]);

                    }
                }

            }
            else
            {
                //Now fill the point data for the segments that are currently active
                for (int currSegIndex = 0; currSegIndex < CurrSegments.Length; currSegIndex++)
                {
                    //Debug.Log("Adding points to current segment used index, " + currSegIndex);

                    myShapeVersion[CurrSegments[currSegIndex]] = new Vector3[2];

                    //Add the first point
                    myShapeVersion[CurrSegments[currSegIndex]][0] = rectShape[CurrSegments[currSegIndex]];
                    //Debug.Log("Adding points to current segment used index, " + currSegIndex + " new point " + theShape.ThePoints[CurrSegments[currSegIndex]]);

                    //Add the second point depending on the position of the current point
                    if (CurrSegments[currSegIndex] == rectShape.Length - 1)
                    {
                        myShapeVersion[CurrSegments[currSegIndex]][1] = rectShape[0];
                        //Debug.Log("Adding points to current segment used index, " + currSegIndex + " new point " + theShape.ThePoints[0]);

                    }
                    else
                    {
                        myShapeVersion[CurrSegments[currSegIndex]][1] = rectShape[CurrSegments[currSegIndex] + 1];
                        //Debug.Log("Adding points to current segment used index, " + currSegIndex + " new point " + theShape.ThePoints[CurrSegments[currSegIndex] + 1]);

                    }
                }

                //Now we fill the hole information for the 

                //  Adding holes to a segment logic
                //
                //  -holes have a dist and a width
                //  -holes can never extend beyond the length of the original segment
                //  -holes can never have a negative start value
                SetupHoles();
            }




        }

        void SetupHoles()
        {
            if (theHoles != null && theHoles.Length > 0)
            {
                for (int holeIndex = 0; holeIndex < theHoles.Length; holeIndex++)
                {
                    //If this segment has more than 0 values it means that it is being used
                    if (myShapeVersion[theHoles[holeIndex].segmentIndex].Length != 0)
                    {
                        Vector3 startPos = myShapeVersion[theHoles[holeIndex].segmentIndex][0];
                        Vector3 endPos = myShapeVersion[theHoles[holeIndex].segmentIndex][myShapeVersion[theHoles[holeIndex].segmentIndex].Length - 1];


                        Vector3[] newSegment = new Vector3[2];
                        newSegment[0]=startPos;
                        newSegment[1]=endPos;
                        theHoles[holeIndex].SetParameters(newSegment);



                        float totalDist = Vector3.Distance(startPos, endPos);

                        bool isHoleValid = true;

                        //Si el start position caer en un agujero el Hole es invalido
                        isHoleValid = !(IsPointOnSegmentHole(myShapeVersion[theHoles[holeIndex].segmentIndex], theHoles[holeIndex].startDistance));

                        //Si el final del agujero cae dentro de los limites del hole es invalido
                        isHoleValid = (theHoles[holeIndex].startDistance + theHoles[holeIndex].width < totalDist);

                        //Si hay ningun otro nodo entre el start point y el end point del hole es invalido

                        int startClosestIndex = GetClosestIndex(myShapeVersion[theHoles[holeIndex].segmentIndex], theHoles[holeIndex].startDistance);
                        int endClosestIndex = GetClosestIndex(myShapeVersion[theHoles[holeIndex].segmentIndex], theHoles[holeIndex].startDistance + theHoles[holeIndex].width);
                        //Debug.Log( endClosestIndex);
                        //Debug.Log(startClosestIndex );
                        isHoleValid = (startClosestIndex == endClosestIndex);

                        //El Hole es valid
                        if (isHoleValid)
                        {
                            List<Vector3> tempList = new List<Vector3>();
                            for (int m = 0; m <= startClosestIndex; m++)
                            {
                                tempList.Add(myShapeVersion[theHoles[holeIndex].segmentIndex][m]);
                                //Debug.Log("adding" + myShapeVersion[theHoles[holeIndex].segmentIndex][m]);
                            }

                            //      totalDist _______ 1
                            //        distX __________ X  
                            //

                            float normalizedStartPos = theHoles[holeIndex].startDistance / totalDist;
                            float normalizedEndPos = (theHoles[holeIndex].startDistance + theHoles[holeIndex].width) / totalDist;

                            tempList.Add(Vector3.Lerp(startPos, endPos, normalizedStartPos));
                            //Debug.Log("adding" + Vector3.Lerp(startPos, endPos, normalizedStartPos));

                            tempList.Add(Vector3.Lerp(startPos, endPos, normalizedEndPos));
                            //Debug.Log("adding" + Vector3.Lerp(startPos, endPos, normalizedEndPos));


                            for (int w = startClosestIndex + 1; w < myShapeVersion[theHoles[holeIndex].segmentIndex].Length; w++)
                            {
                                tempList.Add(myShapeVersion[theHoles[holeIndex].segmentIndex][w]);
                                //Debug.Log("adding" + myShapeVersion[theHoles[holeIndex].segmentIndex][w]);
                            }

                            Vector3[] newArr = tempList.ToArray();

                            myShapeVersion[theHoles[holeIndex].segmentIndex] = newArr;
                        }
                    }
                }
            }
        }

        #endregion

        #region Shared Methods 
        
        public void SetChildrenVisibility(bool theVal)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(theVal);
            }
        }

        public void AddPrefab()
        {
            SS_WallObjects[] newPrefabs = new SS_WallObjects[theWallPrefabs.Length + 1];

            for (int i = 0; i < theWallPrefabs.Length; i++)
            {
                newPrefabs[i] = theWallPrefabs[i];
            }

            newPrefabs[newPrefabs.Length - 1] = new SS_WallObjects();
            newPrefabs[newPrefabs.Length - 1].thePrefab = SS_Common.SS_DefaultWallPrefab();
            theWallPrefabs = newPrefabs;
        }

        public void RemovePrefab()
        {
            SS_WallObjects[] newPrefabs = new SS_WallObjects[theWallPrefabs.Length - 1];
            for (int i = 0; i < theWallPrefabs.Length-1; i++)
            {
                newPrefabs[i] = theWallPrefabs[i];
            }
            theWallPrefabs = newPrefabs;
        }

        #endregion

        /// <summary>
        /// Builds a segment wall using a segment given by the index
        /// </summary>
        /// <param name="index"></param>
        void BuildSegment(Vector3[] currSegment)
        {
            if (currSegment.Length == 0) return;

            if (currSegment.Length == 2)
            {
                GetSegmentDistributionParameters(currSegment);
                InstantiateCurrentSegmentPrefabs();
            }
            if (currSegment.Length>2)
            {
                if (currSegment[0] != currSegment[1])
                {
                    Vector3[] tempSegment = new Vector3[] { currSegment[0], currSegment[1] };
                    GetSegmentDistributionParameters(tempSegment);
                    InstantiateCurrentSegmentPrefabs();
                }

                for (int i = 2; i < currSegment.Length; i=i+2)
                {
                    if (i != currSegment.Length - 1)
                    {
                        Vector3[] tempSegment = new Vector3[] { currSegment[i], currSegment[i+1] };
                        GetSegmentDistributionParameters(tempSegment);
                        InstantiateCurrentSegmentPrefabs();
                    }
                }
                //Debug.Log("Holed Wall!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }

        }

        void GetSegmentDistributionParameters(Vector3[] currSegment)
        {
            float segmentLength = Vector3.Distance(currSegment[0], currSegment[1]);

            theSegment = new Vector3[] { currSegment[0], currSegment[1] };

            segmentRemainder = segmentLength % moduleSize.x;
            segmentCount = 0;
            segmentSeparation = 0;
            segmentScale = 0;

            if (segmentRemainder != 0)
            {
                segmentCount = Mathf.FloorToInt(segmentLength / moduleSize.x);

                //  thePrefabs[0].objectSize.x ________  1
                //  remainder                  ________  x
                float siseRelative = segmentRemainder / moduleSize.x;

                segmentScale = 1 + (siseRelative / segmentCount);


                segmentSeparation = moduleSize.x + (segmentRemainder / segmentCount);
                // segmentLength ________ 1
                // segmentSeparation_____ x
                segmentSeparation = segmentSeparation / segmentLength;

            }
            else
            {
                segmentCount = Mathf.FloorToInt(segmentLength / moduleSize.x);
                segmentScale = 1;
                segmentSeparation = moduleSize.x;
                segmentSeparation = segmentSeparation / segmentLength;
            }

        }

        /// <summary>
        /// Return the next prefab with all valid modifications applied
        /// </summary>
        /// <returns></returns>
        GameObject GetNextPrefab()
        {
            GameObject returnGO;
            if (theWallPrefabs.Length > 1)
            {
                switch (myDistrType)
                {
                    case (DistributionType.Random):
                        currSequenceIndex = Random.Range(0, theWallPrefabs.Length);
                        returnGO = UnityEditor.PrefabUtility.InstantiatePrefab(theWallPrefabs[currSequenceIndex].thePrefab as GameObject) as GameObject;
                        break;
                    case (DistributionType.Sequence):
                        int theIndex = currSequenceIndex;
                        currSequenceIndex++;
                        if (currSequenceIndex == theWallPrefabs.Length)
                            currSequenceIndex = 0;
                        returnGO = UnityEditor.PrefabUtility.InstantiatePrefab(theWallPrefabs[theIndex].thePrefab as GameObject) as GameObject;
                        break;
                    default:
                        returnGO = UnityEditor.PrefabUtility.InstantiatePrefab(theWallPrefabs[0].thePrefab as GameObject) as GameObject;
                        break;
                }
            }
            else
            {
                currSequenceIndex = 0;
                returnGO = UnityEditor.PrefabUtility.InstantiatePrefab(theWallPrefabs[0].thePrefab as GameObject) as GameObject;
            }
            if (returnGO == null)
            {
                returnGO = SS_Common.SS_DefaultWallPrefab();
            }

            //Assign the parent
            returnGO.transform.SetParent(transform);

            //Assign the scale
            returnGO.transform.localScale = new Vector3(segmentScale, 1, segmentScale);


            return returnGO;
        }

        void InstantiateCurrentSegmentPrefabs()
        { 


            for (int i = 0; i < segmentCount; i++)
            {
                GameObject clone = GetNextPrefab();
                                
                clone.transform.position = (Vector3.Lerp(theSegment[0], theSegment[1], i * segmentSeparation) /*+ offsetTranslate*/);

                Vector3 theDir = Vector3.Normalize(theSegment[1] - theSegment[0]);

                Vector3 theCross = Vector3.Cross(theDir, clone.transform.up);

                clone.transform.LookAt(clone.transform.position + theCross);
        

                //Apply x and y offsets in world space
                Vector3 newPos = new Vector3(clone.transform.position.x+offsetTranslate.x, clone.transform.position.y + offsetTranslate.y, clone.transform.position.z);

                clone.transform.position = newPos;                

                clone.transform.Translate(new Vector3(0, 0, offsetTranslate.z), Space.Self);




                if (theWallPrefabs[currSequenceIndex].flip)
                {
                    clone.transform.GetChild(0).Rotate(Vector3.forward, 180);
                }
            } 

            if (generateCollider)
            {
                Vector3 theDir = Vector3.Normalize(theSegment[1] - theSegment[0]);


                Vector3 midPoint = Vector3.Lerp(theSegment[0], theSegment[1], 0.5f);
                GameObject newGo = new GameObject("SS_Wall_Collider");
                newGo.transform.position = midPoint+Vector3.up*(moduleSize.y/2.0f);


                Vector3 theCross = Vector3.Cross(theDir, newGo.transform.up);


                newGo.transform.LookAt(newGo.transform.position + theCross);


                newGo.transform.SetParent(transform);

                BoxCollider coll = newGo.AddComponent<BoxCollider>();

                coll.size = new Vector3(Vector3.Distance(theSegment[0], theSegment[1]), moduleSize.y, 0.2f);
                    
            }

           // if (generateCollider) GenerateCollider(theSegment);
       
        }

        /// <summary>
        /// Main Function that builds the wall
        /// </summary>
        public void GeneratePrefabs()
        {
            Random.InitState(randSeed);
    
            RemovePreviousInstances();

            SetupShapeInterpretation();

            //Debug.Log("Setting up " + myShapeVersion.Count + " Segments");
            for (int i = 0; i < myShapeVersion.Count; i++)
            {
                if (myShapeVersion[i].Length != 0)
                {
                    //Debug.Log("Setting up segment" + i);

                    //BuildShapeSegment(i);
                    BuildSegment(myShapeVersion[i]);

                }
            }

        }



        [ContextMenu("Generate")]
        void Generate()
        {
            GeneratePrefabs();
        }


        #endif
        
        



    }
}
