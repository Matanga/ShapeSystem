using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VFX.ShapeSystem
{
    /// <summary>
    /// The type of distribution that will be used when there are 
    /// more than 1 prefab to use
    /// </summary>
    public enum DistributionType
    {
        Random,
        Sequence,
        CustomSequence
    };


    [System.Serializable]
    public class SS_InstanceObject
    {
        public GameObject thePrefab;
        public Vector3 objectSize;
    }

    [System.Serializable]
    public class SS_FloorInstanceObject
    {
        public GameObject thePrefab;
        public bool useRandomRotation;
    }

    public class SS_Floor : MonoBehaviour
    {
        public SS_FloorInstanceObject[] thePrefabs;

        #region Distribution Options

            /// <summary>
            /// Distribution type for the prefabs
            /// </summary>
            public DistributionType myDistrType= DistributionType.Sequence;        
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
                    rectTransform=transform.parent.GetComponent<RectTransform>();
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
            get {
                Vector3[] thePoints= new Vector3[4];
                RectTransform.GetLocalCorners(thePoints);
                return Vector3.Distance(thePoints[0], thePoints[3]);
            }
        }
        
        #endregion
        
        
        #region Collider Options
        
        /// <summary>
        /// Flag to enable or disable collider generation
        /// </summary>
        public bool generateCollider;
        /// <summary>
        /// Collider Height
        /// </summary>
        public float colliderHeight=0.25f;
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

        public string myName;



        #region Dynamic Floor Creation Parameters
        
        /// <summary>
        /// The difference in distance that will have to be corrected by scaling the prefabs
        /// </summary>
        float widthRemainder;
        /// <summary>
        /// The count of width prefabs
        /// </summary>
        int widthCount;
        /// <summary>
        /// The Distance that each prefab will be from another
        /// </summary>
        float widthSeparation;
        /// <summary>
        /// the scale of each prefab along the width axis
        /// </summary>
        float widthScale;


        /// <summary>
        /// The difference in distance that will have to be corrected by scaling the prefabs
        /// </summary>
        float lengthRemainder;
        /// <summary>
        /// The count of width prefabs
        /// </summary>
        int lengthCount;
        /// <summary>
        /// The Distance that each prefab will be from another
        /// </summary>
        float lengthSeparation;
        /// <summary>
        /// the scale of each prefab along the width axis
        /// </summary>
        float lengthScale;
        /// <summary>
        /// Size difference in the length axis. must be stored to offset the modules on this axis ||======= TO DO:find better solution =======||
        /// </summary>
        float lengthOffset;


        #endregion

        #if UNITY_EDITOR
        
        #region Shared Methods  

        public void OnRectUpdateCallback()
        {
            //Debug.Log("floor update");
            GeneratePrefabs();
        }

        public void RemovePreviousInstances()
        {
            for (int i = transform.childCount-1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        public void AddPrefab()
        {
            SS_FloorInstanceObject[] newPrefabs = new SS_FloorInstanceObject[thePrefabs.Length + 1];

            for (int i = 0; i < thePrefabs.Length; i++)
            {
                newPrefabs[i] = thePrefabs[i];
            }

            newPrefabs[newPrefabs.Length - 1] = new SS_FloorInstanceObject();
            newPrefabs[newPrefabs.Length - 1].thePrefab = SS_Common.SS_DefaultFloorPrefab();
            thePrefabs = newPrefabs;
        }

        public void RemovePrefab()
        {
            SS_FloorInstanceObject[] newPrefabs = new SS_FloorInstanceObject[thePrefabs.Length - 1];
            for (int i = 0; i < thePrefabs.Length - 1; i++)
            {
                newPrefabs[i] = thePrefabs[i];
            }
            thePrefabs = newPrefabs;
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



        #endregion



        /// <summary>
        /// Main Method in charge of generating the grid
        /// </summary>
        public void GeneratePrefabs()
        {
            //Debug.Log("Generating Prefabs");

            //Reset Sequence index
            currSequenceIndex = 0;
            //Set Random seed
            Random.InitState(randSeed);

            //Remove old Prefabs
            RemovePreviousInstances(); 

            //Get Width parameters
            GetWidthDistributionParameters();

            //Get Length parameters
            GetLengthDistributionParameters();

            //Generate Colliders if necessary
            if (generateCollider) GenerateCollider();

            //Bidimensional for loop
            for (int x = 0; x < lengthCount; x++)
            {
                for (int i = 0; i < widthCount; i++)
                {
                    GameObject clone = GetNextPrefab();
                    //Debug.Log(clone);

                    GameObject tempGO = GetOffsetedTransformGO();

                    clone.transform.position = tempGO.transform.TransformPoint(new Vector3(i * widthSeparation, 0, (x) * lengthSeparation + lengthOffset) +offsetTranslate);
                    
                    GameObject finalGO = new GameObject("SS_FloorInstance");

                    finalGO.transform.position = clone.transform.position;
                    finalGO.transform.SetParent(clone.transform.parent);
                    SS_FloorObject theObject = finalGO.AddComponent<SS_FloorObject>();

                    switch (myDistrType)
                    {
                        case DistributionType.Random:
                            theObject.myPrefab = thePrefabs[currSequenceIndex].thePrefab;
                            break;
                        case DistributionType.Sequence:
                            if(currSequenceIndex==0)
                                theObject.myPrefab = thePrefabs[thePrefabs.Length-1].thePrefab;
                            else
                                theObject.myPrefab = thePrefabs[currSequenceIndex-1].thePrefab;
                            break;
                        case DistributionType.CustomSequence:
                            break;
                        default:
                            break;
                    }


                    clone.transform.SetParent(finalGO.transform);

                    DestroyImmediate(tempGO);
                }
            }
        }

        #region Dynamic Floor Creation Methods
        
        /// <summary>
        /// Gets all dynamic  parameters necessary for Length distribution
        /// </summary>
        void GetLengthDistributionParameters()
        {
            //lengthRemainder = theShape.length % thePrefabs[0].objectSize.z;
            lengthRemainder = rectLength % moduleSize.z;

            lengthCount = 0;
            lengthSeparation = 0;
            lengthScale = 0;

            if (lengthRemainder != 0)
            {
                //lengthCount = Mathf.FloorToInt(theShape.length / thePrefabs[0].objectSize.z);
                lengthCount = Mathf.FloorToInt(rectLength / moduleSize.z);


                //  thePrefabs[0].objectSize.x ________  1
                //  remainder                  ________  x
                float sizeRelative = lengthRemainder / moduleSize.z;

                lengthScale = 1 + (sizeRelative / lengthCount);

                lengthOffset= (lengthRemainder / lengthCount);

                lengthSeparation = moduleSize.z + (lengthRemainder / lengthCount);

            }
            else
            {
                //lengthCount = Mathf.FloorToInt(theShape.length / thePrefabs[0].objectSize.z);
                lengthCount = Mathf.FloorToInt(rectLength / moduleSize.z);

                lengthScale = 1;
                lengthSeparation = moduleSize.z;
            }
        }
        
        /// <summary>
        /// Gets all dynamic  parameters necessary for Width distribution
        /// </summary>
        void GetWidthDistributionParameters()
        { 
             //widthRemainder =theShape.width % thePrefabs[0].objectSize.x;
            widthRemainder = rectWidth % moduleSize.x;


             widthCount=0;
             widthSeparation = 0;
             widthScale = 0;

            if (widthRemainder != 0)
            {
                //widthCount = Mathf.FloorToInt(theShape.width / thePrefabs[0].objectSize.x);
                widthCount = Mathf.FloorToInt(rectWidth / moduleSize.x);

                //  thePrefabs[0].objectSize.x ________  1
                //  remainder                  ________  x
                float siseRelative = widthRemainder / moduleSize.x;

                widthScale = 1 + (siseRelative / widthCount);

                widthSeparation = moduleSize.x + (widthRemainder / widthCount);


            }
            else
            {
                //widthCount = (int)(theShape.width / thePrefabs[0].objectSize.x);
                widthCount = (int)(rectWidth / moduleSize.x);

                widthScale = 1;
                widthSeparation = moduleSize.x;
            }        
        }

        /// <summary>
        /// Return the next prefab with all valid modifications applied
        /// </summary>
        /// <returns></returns>
        GameObject GetNextPrefab()
        {
            GameObject returnGO;
            if(thePrefabs.Length>1)
            {
                switch(myDistrType)
                {
                    case(DistributionType.Random):
                            currSequenceIndex = Random.Range(0, thePrefabs.Length);
                            returnGO = UnityEditor.PrefabUtility.InstantiatePrefab(thePrefabs[currSequenceIndex].thePrefab as GameObject) as GameObject;
                            break;
                    case (DistributionType.Sequence):
                            int theIndex = currSequenceIndex;
                            currSequenceIndex++;
                            if (currSequenceIndex == thePrefabs.Length)
                                currSequenceIndex = 0;
                            returnGO = UnityEditor.PrefabUtility.InstantiatePrefab(thePrefabs[theIndex].thePrefab as GameObject) as GameObject;
                            break;
                    default:
                            returnGO = UnityEditor.PrefabUtility.InstantiatePrefab(thePrefabs[0].thePrefab as GameObject) as GameObject;
                            break;
                }
            }
            else
            {
                returnGO= UnityEditor.PrefabUtility.InstantiatePrefab(thePrefabs[0].thePrefab as GameObject) as GameObject;
            }

            //Assign the parent
            returnGO.transform.SetParent(transform);

            //Assign the scale
            returnGO.transform.localScale = new Vector3(widthScale, 1, lengthScale);

            //Check if the prefab can be rotated
            if (thePrefabs[currSequenceIndex].useRandomRotation)
            {
                returnGO.transform.GetChild(0).Rotate(Vector3.up, (90 * Random.Range(0, 4)));
            }
            return returnGO;
        }

        /// <summary>
        /// Generates the collider
        /// </summary>
        void GenerateCollider()
        {
            GameObject newGo = new GameObject("SS_Floor_Collider");
            
            newGo.transform.SetParent(transform);
            Vector3 midpointWidth = Vector3.Lerp(rectShape[0], rectShape[3],0.5f);
            Vector3 midpointLength = Vector3.Lerp(rectShape[0], rectShape[1],0.5f);


            newGo.transform.position = new Vector3(midpointWidth.x, RectTransform.transform.position.y, midpointLength.z);


            BoxCollider theCollider = newGo.AddComponent<BoxCollider>();
            theCollider.size = new Vector3(rectWidth, colliderHeight, rectLength);
            theCollider.center = new Vector3(0, -(colliderHeight/2), 0);
            if (navigationStatic)
            { 
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(newGo, UnityEditor.StaticEditorFlags.NavigationStatic);
            }
        }

        /// <summary>
        /// The Original Canvas object was meant to be used standing up 
        /// The original version of my shape system scripts assumed grids were layed down
        /// This method creates a temp object that offsets the transform of the RectTransform 
        /// to simulate it being layed down
        /// </summary>
        GameObject GetOffsetedTransformGO()
        {
            GameObject tempGO = new GameObject();
            tempGO.transform.SetParent(RectTransform.transform.parent);
            tempGO.transform.localPosition = RectTransform.transform.localPosition;
            tempGO.transform.localScale = RectTransform.transform.localScale;
            tempGO.transform.localRotation = RectTransform.transform.localRotation;

            tempGO.transform.Rotate(tempGO.transform.right, 90);

            tempGO.transform.position = rectShape[1] - new Vector3(0, 0, moduleSize.x);

            return tempGO;
        }


        #endregion


        #endif

    }

}
