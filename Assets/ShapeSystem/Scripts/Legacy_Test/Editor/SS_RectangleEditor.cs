using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace VFX.ShapeSystem
{


    [CustomEditor(typeof(SS_Rectangle))]
    public class SS_RectangleEditor : SS_BaseEditor
    {
        public SS_Rectangle Target
        {
            get
            {
                return  (SS_Rectangle)target;
            }
        }

        SerializedProperty snap;

        void OnEnable()
        {
            snap = serializedObject.FindProperty("snap");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            serializedObject.Update();
            

            EditorGUILayout.PropertyField(serializedObject.FindProperty("length"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("width"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("snap"));

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Target, "Moved Point");
                Target.UpdatePointsPos();
            }

        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
        static void RenderCustomGizmo(Transform objectTransform, GizmoType gizmoType)
        {
            SS_Rectangle TheTarget = objectTransform.GetComponent<SS_Rectangle>();

            if (TheTarget == null) return;

            if (TheTarget.ThePoints == null) return;
            
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

        protected virtual void OnSceneGUI()
        {
            //float size = HandleUtility.GetHandleSize(example.targetPosition) * example.buttonSize;



            /*
             * 
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


            SceneGUI_DrawShape();


            DrawWidthGizmo();
            DrawLengthGizmo();
        }
    
        private void DrawWidthGizmo()
        {
            Vector3 SnapVal = Vector3.one  *snap.floatValue;

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

        private void DrawLengthGizmo()
        {
            Vector3 SnapVal = Vector3.one * snap.floatValue;

            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.FreeMoveHandle(Target.transform.TransformPoint(Target.LengthGizmoPos), Quaternion.identity, Target.buttonSize, SnapVal, Handles.SphereHandleCap);

            if (EditorGUI.EndChangeCheck())
            {
                Vector3 lengthTargetPosition = new Vector3(0, 0, newTargetPosition.z - Target.transform.position.z);
                Undo.RecordObject(Target, "Change Look At Target Position");
                Target.LengthGizmoPos = lengthTargetPosition;
                Target.UpdatePointsPos();
            }
        }    


    }



}