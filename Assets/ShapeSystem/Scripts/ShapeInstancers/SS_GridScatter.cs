using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX.ShapeSystem
{
    [System.Serializable]
    public class SS_GridObjects
    {
        public GameObject thePrefab;
    }

    public class SS_GridScatter : MonoBehaviour
    {

        public SS_GridObjects[] thePrefabs;

        public string myName;




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

        #region Offset Options

        public Vector3 moduleSize;
        public Vector3 offsetTranslate;
        public Vector3 offsetRotate;

        #endregion

        #region Scatter Options

        public int scatterCount=100;
       
        public int randSeed;

        public float wallOffset=0;

        public bool randomRotation = false;


        public bool avoidSelfCollision = false;

        #endregion

        #if UNITY_EDITOR

        public void OnRectUpdateCallback()
        {
            GeneratePrefabs();
        }
        
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
            SS_GridObjects[] newPrefabs = new SS_GridObjects[thePrefabs.Length + 1];

            for (int i = 0; i < thePrefabs.Length; i++)
            {
                newPrefabs[i] = thePrefabs[i];
            }

            newPrefabs[newPrefabs.Length - 1] = new SS_GridObjects();
            newPrefabs[newPrefabs.Length - 1].thePrefab = SS_Common.SS_DefaultScatterPrefab();
            thePrefabs = newPrefabs;
        }

        public void RemovePrefab()
        {
            SS_GridObjects[] newPrefabs = new SS_GridObjects[thePrefabs.Length - 1];
            for (int i = 0; i < thePrefabs.Length - 1; i++)
            {
                newPrefabs[i] = thePrefabs[i];
            }
            thePrefabs = newPrefabs;
        }

        #endregion



        public Vector3 GetNextPointOnGrid()
        {
            return new Vector3(Random.Range(rectShape[0].x + wallOffset, rectShape[3].x - wallOffset), transform.parent.position.y, Random.Range(rectShape[0].z + wallOffset, rectShape[1].z - wallOffset));
        
        }


        public void RemovePreviousInstances()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }


        void DoCollisionAvoidance(GameObject clone)
        {
            //flag to know if a valid place has been found
            bool validPlaceFound = false;

            //An arbitrary amount of tries to avoid an infinite loop of not finding a valid position for the prefab
            for (int r = 0; r < 5; r++)
            {
                //Only do the loop if a valid place hasnte been found
                if (!validPlaceFound)
                {
                    //Get all colliders for this object
                    Collider[] hitColliders = Physics.OverlapSphere(clone.transform.position, 0,5);

                    //Flag to know if we are colliding with a sibling
                    bool collidesWithSibling = false;

                    //for every collided object
                    for (int t = 0; t < hitColliders.Length; t++)
                    {
                        //If we haven collides with a sibling
                        if (!collidesWithSibling)
                        {
                            //if the collider is a child or a deep child of this transform it means it is an instances object
                            collidesWithSibling = hitColliders[t].transform.IsChildOf(transform);
                        }
                    }
                    //If we  havent collides with a sibling it means that this place is ok and we can 
                    //set the valid Place flag to true
                    if (!collidesWithSibling)
                    {
                        validPlaceFound = true;
                    }
                    // If not we get a new position and try the process again
                    else
                    {
                        clone.transform.position = GetNextPointOnGrid();                        
                    }

                }
            } 
            //If we have reached the max try and havent found a valid place we destroy the object 
            if (!validPlaceFound)
            { 
                DestroyImmediate(clone);           
            }

        }


        public void GeneratePrefabs()
        {
            Random.InitState(randSeed);
            RemovePreviousInstances();

            for (int i = 0; i < scatterCount; i++)
            {
                GameObject clone = UnityEditor.PrefabUtility.InstantiatePrefab(thePrefabs[0].thePrefab as GameObject) as GameObject;

                if(randomRotation)
                {
                    clone.transform.Rotate(Vector3.up, Random.Range(0, 360));             
                }
                clone.transform.SetParent(transform);

                clone.transform.position = GetNextPointOnGrid();

                //If the flag to avoid collisions is set
                if (avoidSelfCollision)
                {
                    DoCollisionAvoidance(clone);
                }
            }
        }

        #endif


    }
}