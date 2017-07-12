using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace VFX.ShapeSystem
{
    [CustomEditor(typeof(SS_Base))]
    public class SS_BaseEditor : Editor
    {
        private SS_Base theTarget;
        public SS_Base TheTarget
        {
            get {
                if (theTarget == null)
                    theTarget = (SS_Base)target;
                return theTarget; }
            set { theTarget = value; }
        }




        public void SceneGUI_DrawShape()
        {
            if (TheTarget.ThePoints.Length != 0)
            {
                for (int i = 0; i < TheTarget.ThePoints.Length; i++)
                {
                    if (i == TheTarget.ThePoints.Length - 1)
                    {
                        Handles.DrawLine(TheTarget.transform.TransformPoint(TheTarget.ThePoints[i]), TheTarget.transform.TransformPoint(TheTarget.ThePoints[0]));
                    }
                    else
                    {
                        Handles.DrawLine(TheTarget.transform.TransformPoint(TheTarget.ThePoints[i]), TheTarget.transform.TransformPoint(TheTarget.ThePoints[i + 1]));
                    }
                }
            }
        }




    }
}
