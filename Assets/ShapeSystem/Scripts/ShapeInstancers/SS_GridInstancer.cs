using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VFX.ShapeSystem
{
    [System.Serializable]
    public class SS_GridInstanceObject
    {
        public GameObject thePrefab;
        public Vector3 translateOffset;
    }
    
    public class SS_GridInstancer : MonoBehaviour
    {        
        //              width
        //              columns
        //     3_____________________2
        //      |                   |
        //      |                   |
        //      |                   |
        //      |                   |   rows  
        //      |                   |   depth
        //      |                   |
        //      |                   |
        //      |___________________|
        //     0                     1 
        //
        //      ROWS     = 0---3
        //      COLUMNS  = 0---1
        //

        public SS_GridInstanceObject[] thePrefabs;

        public string myName;

        #region Main Options

        public bool useBorders=false;
        
        public bool useInterior=true;

        public int gridRows=2;
        
        public int gridColumns=2;

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

        /// <summary>
        /// Get the list of points from the RectTransform component of the parent of this transform
        /// </summary>
        public Vector3[] rectShape
        {
            get
            {
                Vector3[] thePoints = new Vector3[4];
                RectTransform.GetWorldCorners(thePoints);
                return thePoints;
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

        #region Distribution Options

        /// <summary>
        /// Distribution type for the prefabs
        /// </summary>
        public DistributionType myDistrType = DistributionType.Random;
        public int randSeed;


        int currSequenceIndex = 0;
        int randNum = 0;


        #endregion

        #region Offset Options

        public Vector3 offsetTranslate;
        public Vector3 offsetRotate;

        #endregion

        #region Random Values

        public bool useRandomRotation;

        public bool useRandomProbability;

        public float rndProbValue=100.0f;

        #endregion

        #region Temp Properties

        List<Vector3> gridPoints = new List<Vector3>();

        List<Vector3> borderPoints = new List<Vector3>();

        #endregion

        #if UNITY_EDITOR

        ///////////////////////////
        /////////////////////////// 
        ////   METHODS
        ////   METHODS
        ////   METHODS 
        /////////////////////////// 
        ///////////////////////////
        
        #region CALLBACKS
        public void OnRectUpdateCallback()
        {
            //Debug.Log("Changed");
            GeneratePrefabs();
        }
        #endregion

        #region Grid Points Methods

        public Vector3[] GetColumnRows(Vector3 columnStart)
        { 
            return new Vector3[0];
        }

        void GetGridPoints()
        {
            float widthDistance = Vector3.Distance(rectShape[0], rectShape[1]);
            float depthDistance = Vector3.Distance(rectShape[0], rectShape[3]);

            float widthSeparation = widthDistance / (gridColumns + 1);
            float depthSeparation = depthDistance / (gridRows + 1);


            List<Vector3> columnPoints = new List<Vector3>();
            for (int i = 0; i < gridColumns; i++)
            {
                float newDist = ((1 + i) * widthSeparation) / widthDistance;
                columnPoints.Add(Vector3.Lerp(rectShape[0], rectShape[1], newDist));
            }

            for (int x = 0; x < gridRows; x++)
            {
                float newDist = ((1 + x) * depthSeparation) / depthDistance;
                Vector3 currPoint = (Vector3.Lerp(rectShape[0], rectShape[3], newDist));
                Vector3 endPoint = rectShape[1] + (currPoint - rectShape[0]);

                for (int r = 0; r < gridColumns; r++)
                {
                    float newDist2 = ((1 + r) * widthSeparation) / widthDistance;
                    gridPoints.Add(Vector3.Lerp(currPoint, endPoint, newDist2));
                }
            }
        }

        void GetBorderPoints()
        {
            float widthDistance = Vector3.Distance(rectShape[0], rectShape[1]);
            float depthDistance = Vector3.Distance(rectShape[0], rectShape[3]);

            float widthSeparation = widthDistance / (gridColumns + 1);
            float depthSeparation = depthDistance / (gridRows + 1);

            for (int i = 0; i < rectShape.Length; i++)
            {
                borderPoints.Add(rectShape[i]);
            }
            for (int i = 0; i < gridColumns; i++)
            {
                float newDist = ((1+ i) * widthSeparation) / widthDistance;
                borderPoints.Add(Vector3.Lerp(rectShape[0], rectShape[1], newDist));
            }

            for (int i = 0; i < gridColumns; i++)
            {
                float newDist = ((1 + i) * widthSeparation) / widthDistance;
                borderPoints.Add(Vector3.Lerp(rectShape[3], rectShape[2], newDist));
            }

            for (int i = 0; i < gridRows; i++)
            {
                float newDist = ((1 + i) * depthSeparation) / depthDistance;
                borderPoints.Add(Vector3.Lerp(rectShape[0], rectShape[3], newDist));
            }
            for (int i = 0; i < gridRows; i++)
            {
                float newDist = ((1 + i) * depthSeparation) / depthDistance;
                borderPoints.Add(Vector3.Lerp(rectShape[1], rectShape[2], newDist));
            }
        }
        

        #endregion

        #region Utility Methods

        public void SetChildrenVisibility(bool theVal)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(theVal);
            }
        }

        /// <summary>
        /// Positions and rotates the given gameObject into the given position
        /// </summary>
        /// <param name="clone"></param>
        /// <param name="thePos"></param>
        void ProcessGO(GameObject clone,Vector3 thePos)
        {
            clone.transform.SetParent(transform);
            clone.transform.position = thePos;

            if (useRandomRotation)
            {
                clone.transform.Rotate(Vector3.up, (90 * Random.Range(0, 4)));
            }
        }

        public void ClampRNDProbability()
        {
            rndProbValue = Mathf.Clamp(rndProbValue, 0.0f, 100.0f);
        }

        #endregion

        #region Shared Methods

        public void AddPrefab()
        {
            SS_GridInstanceObject[] newPrefabs = new SS_GridInstanceObject[thePrefabs.Length + 1];

            for (int i = 0; i < thePrefabs.Length; i++)
            {
                newPrefabs[i] = thePrefabs[i];
            }

            newPrefabs[newPrefabs.Length - 1] = new SS_GridInstanceObject();
            newPrefabs[newPrefabs.Length - 1].thePrefab = SS_Common.SS_DefaultGridPrefab();
            thePrefabs = newPrefabs;
        }

        public void RemovePrefab()
        {
            SS_GridInstanceObject[] newPrefabs = new SS_GridInstanceObject[thePrefabs.Length - 1];
            for (int i = 0; i < thePrefabs.Length - 1; i++)
            {
                newPrefabs[i] = thePrefabs[i];
            }
            thePrefabs = newPrefabs;
        }

        GameObject GetNextPrefab()
        {
            if (thePrefabs == null) return new GameObject("Temp");
            if (thePrefabs.Length == 0) return new GameObject("Temp");
            bool createPrefab = true;
            if (useRandomProbability)
            {
                //Random.InitState(randSeed);
                float theVal = Random.Range(0f, 100f);
                if (theVal > rndProbValue)
                {
                    createPrefab = false;
                }
            }
            if(createPrefab)
            {
                GameObject clone = UnityEditor.PrefabUtility.InstantiatePrefab(thePrefabs[0].thePrefab as GameObject) as GameObject;
                return  clone;
            }
            else 
            { 
                return null;
            }
        }

        public void RemovePreviousPrefabGrid()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        #endregion
        
        public void GeneratePrefabs()
        {
            RemovePreviousPrefabGrid();

            ClampRNDProbability();

            gridPoints.Clear();

            GetGridPoints();

            for (int b = 0; b < gridPoints.Count; b++)
            {
                GameObject clone = GetNextPrefab();
                if (clone != null)ProcessGO(clone,gridPoints[b]);
            }
            if (useBorders)
            {
                borderPoints.Clear();
                GetBorderPoints();

                for (int b = 0; b < borderPoints.Count; b++)
                {
                    GameObject clone = GetNextPrefab();
                    if (clone != null) ProcessGO(clone, borderPoints[b]);
                }                
            }            
        }

        #endif

    }

}