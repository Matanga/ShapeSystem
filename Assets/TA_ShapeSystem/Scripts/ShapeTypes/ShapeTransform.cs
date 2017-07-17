using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX.ShapeSystem
{


    public class ShapeTransform : MonoBehaviour
    {

        private Transform t;
        public Transform T
        {
            get { return transform; }
        }


        public bool hiddenTransform = false;
        public void SwitchTransformFlags()
        {
            //Debug.Log("SSSSS");
            hiddenTransform = !hiddenTransform;

            if (hiddenTransform)
            {
                T.hideFlags = HideFlags.HideInInspector;
            }
            else
            {
                T.hideFlags = HideFlags.None;

            }

        }


        public List<Vector3> thePoints = new List<Vector3>();
        public float snap =1.0f;

        void Reset()
        {
            thePoints.Clear();
            thePoints.Add(new Vector3(-1, 0, -1));
            thePoints.Add(new Vector3(-1, 0, 1));
            thePoints.Add(new Vector3(1, 0, 1));
            thePoints.Add(new Vector3(1, 0, -1));
            thePoints.Add(new Vector3(1, 0, 0));
        }


        [SerializeField]
        private Vector3 widthGizmoPos;
        public Vector3 WidthGizmoPos
        {
            get
            {
                return widthGizmoPos;
            }
            set
            {

            }
        }

        public void SetPointPos(int pos, Vector3 value)
        {
            Vector3 newVec = new Vector3(value.x, 0, value.z);
            thePoints[pos] = newVec;
        }


        public void AddPointAtSegment(int theSegment,Vector3 thePos)
        {
            thePoints.Insert(theSegment, thePos);

        }



    }
}
