using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX.ShapeSystem
{
    public class SS_RectInstancer : MonoBehaviour
    {


        #region RectTransform Properties

            /// <summary>
            /// The Reference to this transforms parent´s RectTransform
            /// </summary>
            public static RectTransform rectTransform;
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


            public static void GeneratePrefabs()
        { 

        }


        #if UNITY_EDITOR

            #region Shared Methods

            public void Reset()
            {
                if (RectTransform != null)
                {
                    RectTransform.reapplyDrivenProperties += ReapplyDrivenProperties;
                }
            }

            public void ReapplyDrivenProperties(RectTransform driven)
            {
                Debug.Log("Updateando SS_Floor");
            }

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

            #endregion


        #endif



    }
}