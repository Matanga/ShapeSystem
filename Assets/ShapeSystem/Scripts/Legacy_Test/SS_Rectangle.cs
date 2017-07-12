using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX.ShapeSystem
{

    public class SS_Rectangle : SS_Base
    {

        public System.Action<bool> OnShapeChanged = null;




        public float length=1;
        public float width=1;

        public float snap=0.5f;

        private float lengthMin = 1.0f;
        private float widthMin = 1.0f;




        #region Width Gizmo Properties

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
                Vector3 newVec = new Vector3(value.x, 0, 0);
                UpdateGridWidth(newVec);
            }
        }
        private void UpdateGridWidth(Vector3 newPos)
        {
            Vector3 final = new Vector3();
            if (newPos.x < widthMin)
            {
                final = new Vector3(widthMin, 0, 0);
                width=widthMin;
            }
            else
            {
                final = new Vector3(newPos.x, 0, 0);
                width = newPos.x;
            }
            widthGizmoPos = final;
        }
        
        #endregion

        #region Length Gizmo Properties

        [SerializeField]
        private Vector3 lengthGizmoPos;
        public Vector3 LengthGizmoPos
        {
            get
            {
                return lengthGizmoPos;
            }
            set
            {
                Vector3 newVec = new Vector3(0, 0, value.z);
                UpdateGridLength(newVec);
            }
        }
        private void UpdateGridLength(Vector3 newPos)
        {
            Vector3 final = new Vector3();
            if (newPos.z < lengthMin)
            {
                final = new Vector3(0, 0, lengthMin);
                length = lengthMin;
            }
            else
            {
                final = new Vector3(0, 0, newPos.z);
                length = newPos.z;
            }
            lengthGizmoPos = final;
        }

        #endregion


        void Reset()
        {
            ThePoints = new Vector3[] 
            {            
                new Vector3(0,0,0),
                new Vector3(0,0,length),
                new Vector3(width,0,length),
                new Vector3(width,0,0)            
            };

            widthGizmoPos = new Vector3(width, 0, 0);
            lengthGizmoPos = new Vector3(0, 0, length);

        }



        private void ClampGridSize()
        {
            if (length < lengthMin)
                length = lengthMin;

            if (width < widthMin)
                width = widthMin;
        }


        public void UpdatePointsPos()
        {
            ClampGridSize();

            ThePoints[0]=new Vector3(0,0,0);
            ThePoints[1]=new Vector3(0,0,length);
            ThePoints[2]=new Vector3(width,0,length);
            ThePoints[3] = new Vector3(width, 0, 0);

            WidthGizmoPos = new Vector3(width, 0, 0);
            LengthGizmoPos = new Vector3(0, 0, length);


            if (OnShapeChanged != null)
                OnShapeChanged(true);

        }

    }

}
