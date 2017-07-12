using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VFX.ShapeSystem
{ 
    [CustomEditor(typeof(ShapeTransform))]
    public class ShapeTransformEditor : Editor
    {

        public ShapeTransform Target
        {
            get
            {
                return (ShapeTransform)target; ;
            }
        }

        SerializedProperty hiddenTransform;
        SerializedProperty thePoints;
        SerializedProperty snap;

        void OnEnable()
        {
            hiddenTransform = serializedObject.FindProperty("hiddenTransform");
            thePoints = serializedObject.FindProperty("thePoints");
            snap = serializedObject.FindProperty("snap");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(hiddenTransform);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Target, "Switch Transform Flags");
                Target.SwitchTransformFlags();
            }

            serializedObject.ApplyModifiedProperties();

        }



        public void SceneGUI_DrawShape()
        {
            if (Target.thePoints.Count != 0)
            {
                for (int i = 0; i < Target.thePoints.Count; i++)
                {
                    if (i == Target.thePoints.Count - 1)
                    {
                        Handles.DrawLine(Target.transform.TransformPoint(Target.thePoints[i]), Target.transform.TransformPoint(Target.thePoints[0]));
                    }
                    else
                    {
                        Handles.DrawLine(Target.transform.TransformPoint(Target.thePoints[i]), Target.transform.TransformPoint(Target.thePoints[i + 1]));
                    }
                }
            }
        }




        protected virtual void OnSceneGUI()
        {
            //float size = HandleUtility.GetHandleSize(example.targetPosition) * example.buttonSize;

            /*             
            EditorGUI.BeginChangeCheck();
             
            for (int i = 0; i < TheTarget.ThePoints.Length; i++)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPos = Handles.FreeMoveHandle(TheTarget.transform.TransformPoint(TheTarget.ThePoints[i]), Quaternion.identity, TheTarget.buttonSize, snap, Handles.SphereHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(TheTarget, "Moved Point");
                    TheTarget.SetPointPos(TheTarget.transform.InverseTransformPoint(newTargetPos), i);
                }
            }*/
            //CheckForCtrlPressed();

            SceneGUI_DrawShape();

            
            for (int i = 0; i < Target.thePoints.Count; i++)
            {
                DrawPointGizmo(i);
            }

            for (int i = 0; i < Target.thePoints.Count; i++)
            {
                DrawInnerPoints(i);
            }
            





            //DrawWidthGizmo();
        }

        void CheckForCtrlPressed()
        {
            Event e = Event.current;
            if (EventType.KeyDown == e.type && KeyCode.LeftControl == e.keyCode)
            {
                Debug.Log("Pressed Left Control");
            }

        }
        
        private void DrawPointGizmo(int Pos)
        {
            Vector3 SnapVal = Vector3.one * snap.floatValue;

            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.FreeMoveHandle(Target.transform.TransformPoint(Target.thePoints[Pos]), Quaternion.identity, 0.25f, SnapVal, Handles.SphereHandleCap);

            //float remainder = newTargetPosition.x % SnapVal.x;
            //newTargetPosition = new Vector3(newTargetPosition.x+remainder, newTargetPosition.y, newTargetPosition.z);


            if (EditorGUI.EndChangeCheck())
            {
                Vector3 newPointPosition = new Vector3(newTargetPosition.x - Target.transform.position.x, 0, newTargetPosition.z - Target.transform.position.z);

                Undo.RecordObject(Target, "Change Look At Target Position");

                Target.SetPointPos(Pos, newPointPosition);

                //Target.WidthGizmoPos = widthTargetPosition;
                //Target.UpdatePointsPos();

            }
        }

        private void DrawInnerPoints(int Pos)
        {
            Vector3 SnapVal = Vector3.one * snap.floatValue;
            Vector3 midPoint = new Vector3();

            bool isLast=false;

            if (Pos == Target.thePoints.Count - 1)
            {
                //Handles.DrawLine(Target.transform.TransformPoint(Target.thePoints[Pos]), Target.transform.TransformPoint(Target.thePoints[0]));
                midPoint = Vector3.Lerp(Target.thePoints[Pos], Target.thePoints[0], 0.5f);
                isLast = true;
            }
            else
            {
                //Handles.DrawLine(Target.transform.TransformPoint(Target.thePoints[Pos]), Target.transform.TransformPoint(Target.thePoints[Pos + 1]));
                midPoint = Vector3.Lerp(Target.thePoints[Pos], Target.thePoints[Pos + 1], 0.5f);
            }
            Vector3 TransformedPoint = Target.transform.TransformPoint(midPoint);

            bool pressed = Handles.Button(TransformedPoint, Quaternion.Euler(90, 0, 90), 0.25f, 0.25f, Handles.RectangleCap);
            if (pressed)
            {
                if (isLast)
                {
                    Target.AddPointAtSegment(0, midPoint);
                }
                else
                {
                    Target.AddPointAtSegment(Pos+1, midPoint);
                }
                Debug.Log("Point Added");
            }
            

            //Handles.CircleCap(0,midPoint,Quaternion.Euler(90,0,90),0.25f);

        }





        /*
        private void DrawWidthGizmo()
        {
            Vector3 SnapVal = Vector3.one * snap.floatValue;

            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.FreeMoveHandle(Target.transform.TransformPoint(Target.WidthGizmoPos), Quaternion.identity, Target.buttonSize, SnapVal, Handles.SphereHandleCap);

            //float remainder = newTargetPosition.x % SnapVal.x;
            //newTargetPosition = new Vector3(newTargetPosition.x+remainder, newTargetPosition.y, newTargetPosition.z);


            if (EditorGUI.EndChangeCheck())
            {
                Vector3 widthTargetPosition = new Vector3(newTargetPosition.x - Target.transform.position.x, 0, 0);
                Undo.RecordObject(Target, "Change Look At Target Position");
                Target.WidthGizmoPos = widthTargetPosition;
                Target.UpdatePointsPos();
            }
        }
        */



    }

}