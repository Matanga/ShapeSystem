using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VFX.ShapeSystem
{

    [CustomEditor(typeof(SS_BezierCurve))]
    public class SS_BezierCurveInspector : Editor
    {
        //////////////////////////
        //////PROPERTIES
        //////PROPERTIES
        //////PROPERTIES
        //////////////////////////

        public SS_BezierCurve Target
        {
            get
            {
                return (SS_BezierCurve)target; ;
            }
        }

        SerializedProperty selectedKnot;

        //////////////////////////
        //////EDITOR METHODS
        //////EDITOR METHODS
        //////EDITOR METHODS
        //////////////////////////

        void OnDisable()
        {
            //Debug.Log("Deselecting");
            Target.selectedKnot = null;
        }

        void OnEnable()
        {
            Target.selectedKnot = null;
            selectedKnot = serializedObject.FindProperty("selectedKnot");
        }

        void OnSceneGUI()
        {
            for (int i = 0; i < Target.elements.Length; i++)
            {
                //Debug.Log("Element "+ i);
                DrawElement(Target.elements[i]);
            }
            if (Target.selectedKnot != null)
            {
                //Debug.Log("a knot is selected");
                CheckForVertexTypeCycle();
            }

        }
        

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            if (selectedKnot.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(selectedKnot);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Target, "Switch Transform Flags");
            }
            serializedObject.ApplyModifiedProperties();
        }

        //////////////////////////
        ////// HANDLES DRAWERS
        ////// HANDLES DRAWERS
        ////// HANDLES DRAWERS
        //////////////////////////

        private void DrawKnotMovementHandle(ShapeKnot knot)
        { 
            Vector3 SnapVal = Vector3.one * 0.25f;

            Handles.color = Color.red;
            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.FreeMoveHandle(Target.transform.TransformPoint(knot.kPos), Quaternion.identity, 0.15f, SnapVal, Handles.CircleHandleCap);

            //float remainder = newTargetPosition.x % SnapVal.x;
            //newTargetPosition = new Vector3(newTargetPosition.x+remainder, newTargetPosition.y, newTargetPosition.z);


            if (EditorGUI.EndChangeCheck())
            {
                Vector3 newPointPosition = new Vector3(newTargetPosition.x - Target.transform.position.x, 0, newTargetPosition.z - Target.transform.position.z);

                Undo.RecordObject(Target, "Change Look At Target Position");

                knot.kPos = newPointPosition;
            }
            
        }

        private void DrawKnotBezierHandles(ShapeKnot knot,bool handleIn, bool handleOut)
        {
            Vector3 SnapVal = Vector3.one * 0.25f;

            Handles.color = Color.yellow;

            Vector3 knotWorldPos = Target.transform.TransformPoint(knot.kPos);

            if (handleIn)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 handlePos = knotWorldPos + knot.kHandleIn;
                Handles.color = Color.green;

                Vector3 newTargetPosition = Handles.FreeMoveHandle(handlePos, Quaternion.identity, 0.1f, SnapVal, Handles.RectangleHandleCap);

                if (EditorGUI.EndChangeCheck())
                {
                    //Vector3 newPointPosition = new Vector3(newTargetPosition.x - handlePos.x, 0, newTargetPosition.z - handlePos.z);
                    Vector3 newPointPosition = newTargetPosition - knotWorldPos;
                    newPointPosition.y = 0;

                    Undo.RecordObject(Target, "Change Look At Target Position");

                    knot.kHandleIn = newPointPosition;
                }


                Handles.color = Color.yellow;
                Handles.DrawLine(knotWorldPos, (knotWorldPos + knot.kHandleIn));




            }
            if (handleOut)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 handlePos = knotWorldPos + knot.kHandleOut;

                Handles.color = Color.green;

                Vector3 newTargetPosition = Handles.FreeMoveHandle(handlePos, Quaternion.identity, 0.1f, SnapVal, Handles.RectangleHandleCap);

                if (EditorGUI.EndChangeCheck())
                {
                    //Vector3 newPointPosition = new Vector3(newTargetPosition.x - handlePos.x, 0, newTargetPosition.z - handlePos.z);
                    Vector3 newPointPosition = newTargetPosition - knotWorldPos;
                    newPointPosition.y = 0;

                    Undo.RecordObject(Target, "Change Look At Target Position");

                    knot.kHandleOut = newPointPosition;
                }



                Handles.color = Color.yellow;
                Handles.DrawLine(knotWorldPos, (knotWorldPos + knot.kHandleOut));
            }
        }

        private void DrawKnotSelector(ShapeKnot knot)
        {
            Handles.color = Color.green;
            Vector3 SnapVal = Vector3.one * 0.25f;

            //Vector3 newTargetPosition = Handles.FreeMoveHandle(Target.transform.TransformPoint(knot.kPos), Quaternion.identity, 0.25f, SnapVal, Handles.SphereHandleCap);

            if (Handles.Button(Target.transform.TransformPoint(knot.kPos), Quaternion.Euler(90,0,0), 0.2f, 0.2f, Handles.RectangleHandleCap))
            {
                Target.selectedKnot = knot;
                //Debug.Log("The button was pressed!");
            }

        }
        
        //////////////////////////
        ////// SEGMENT DRAWERS
        ////// SEGMENT DRAWERS
        ////// SEGMENT DRAWERS
        //////////////////////////

        void DrawSegment(ShapeKnot firstKnot, ShapeKnot secondKnot)
        {
            //Debug.Log("Drawing Segment");
            if (firstKnot.kType == KnotType.Linear && secondKnot.kType == KnotType.Linear)
            {
                Vector3 firstKnotWorldPos = Target.transform.TransformPoint(firstKnot.kPos);
                Vector3 secondKnotWorldPos = Target.transform.TransformPoint(secondKnot.kPos);

                //Debug.Log("First Pos: " + firstKnotWorldPos);
                //Debug.Log("Second Pos: " + secondKnotWorldPos);



                //Gizmos.DrawLine(firstKnotWorldPos, secondKnotWorldPos);
            }
        }
        
        void DrawElement(ShapeElement element)
        {
            // Debug.Log("Drawing element . Points: " + element.knots.Length);

            //DRAW KNOTS
            for (int i = 0; i < element.knots.Length ; i++)
            {
                if (element.knots[i] == Target.selectedKnot)
                {
                    DrawKnotMovementHandle(element.knots[i]);
                    DrawKnotBezierHandles(element.knots[i], true, true);
                }
                else
                {
                    DrawKnotSelector(element.knots[i]);
                }
            }
        }
        

        //////////////////////////
        ////// INPUT METHODS
        ////// INPUT METHODS
        ////// INPUT METHODS
        //////////////////////////

        void CheckForVertexTypeCycle()
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                //Debug.Log("Key down");
                //Debug.Log(e.keyCode);

                if (e.keyCode == KeyCode.V)
                {
                    Debug.Log("Switiching selected vertex Type");

                    KnotType currentType = Target.selectedKnot.kType;
                    int value = (int)currentType;

                    int myEnumMemberCount = KnotType.GetNames(typeof(KnotType)).Length;

                    Debug.Log(myEnumMemberCount);

                    Debug.Log(value);

                    if (value == myEnumMemberCount - 1)
                    {
                        value = 0;
                    }
                    else
                    {
                        value++;
                    }

                    Debug.Log(value);

                    serializedObject.Update();
                    Target.selectedKnot.kType = (KnotType)(value++);
                    serializedObject.ApplyModifiedProperties();

                }
            }
        }




        


    }
}