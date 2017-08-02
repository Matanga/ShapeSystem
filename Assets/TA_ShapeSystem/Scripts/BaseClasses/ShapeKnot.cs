using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX.ShapeSystem
{
    [System.Serializable]
    public class ShapeKnot
    {
        public int myElementIndex;
        public int myIndex;

        public bool isSelected;

        public KnotType kType = KnotType.Linear;

        public Vector3 kPos;
        public Vector3 KWorldPos(Transform t)
        {
            return (t.TransformPoint(kPos));
        }

        public Vector3 kScale;
        public Vector3 kRotation;

        public Vector3 kHandleIn;
        public Vector3 KHandleInWorldPos(Transform t)
        {
            return (t.TransformPoint(kPos) + kHandleIn);
        }

        public Vector3 kHandleOut;
        public Vector3 KHandleOutWorldPos(Transform t)
        {
            return (t.TransformPoint(kPos) + kHandleOut);
        }

    }
}