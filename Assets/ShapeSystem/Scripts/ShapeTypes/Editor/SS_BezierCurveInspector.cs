using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VFX.ShapeSystem
{

    [CustomEditor(typeof(SS_BezierCurve))]
    public class SS_BezierCurveInspector : Editor
    {

        public bool creatingKnot;
        bool ctrlFlag = false;
        bool altFlag = false;

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

        //////////////////////////
        //////EDITOR METHODS
        //////EDITOR METHODS
        //////EDITOR METHODS
        //////////////////////////

        void OnDisable()
        {
        }

        void OnEnable()
        {

        }

        void OnSceneGUI()
        {
            for (int i = 0; i < Target.elements.Length; i++)
            {
                //Debug.Log("Element "+ i);
                DrawElementHandles(Target.elements[i]);
            }
            /*
            //VERTEX TYPE INPUT
            if (Target.selectedElementIndex >=0)
            {
                //Debug.Log("a knot is selected");
                CheckForVertexTypeCycle();
            }*/
            if (creatingKnot)
            {
                Debug.Log("Moving Sphere");

                Handles.CubeHandleCap(00021,MousePosRoutine(),Quaternion.identity,0.5f,EventType.repaint);

                //Debug.Log(hit.point);
            }
            CheckKeyboardModifierInput();
        }

        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField(("Length: "+ Target.GetCurveTotalLenght().ToString()));
            


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

        private void DrawKnotBezierHandles(ShapeKnot knot, bool handleIn, bool handleOut)
        {
            Vector3 SnapVal = Vector3.one * 0.25f;

            Handles.color = Color.yellow;

            Vector3 knotWorldPos = knot.KWorldPos(Target.transform);


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

            if (Handles.Button(Target.transform.TransformPoint(knot.kPos), Quaternion.Euler(90, 0, 0), 0.2f, 0.2f, Handles.RectangleHandleCap))
            {

                //Debug.Log("Selected Knot " + Target.selectedElementIndex.ToString() + " " + Target.selectedKnotIndex.ToString());

                //Deselect previous knots
                if (Target.selectedKnots.Count != 0)
                {
                    for (int i = Target.selectedKnots.Count - 1; i >= 0; i--)
                    {
                        Target.selectedKnots[i].isSelected = false;
                        Target.selectedKnots.RemoveAt(i);
                    }                
                }
                knot.isSelected = true;
                Target.selectedKnots.Add(knot);


                //Debug.Log("The button was pressed!");

                if (altFlag) Debug.Log("knot selected with alt pressed");
                if (ctrlFlag)
                {
                    //Debug.Log("knot selected with ctrl pressed");
                    SwitchCollider();
                    creatingKnot = true;
                    //Target.elements[Target.selectedElementIndex].AddKnot((Target.elements[Target.selectedElementIndex].knots.Length), Vector3.zero);
                }
            }
        }

        //////////////////////////
        ////// SEGMENT DRAWERS
        ////// SEGMENT DRAWERS
        ////// SEGMENT DRAWERS
        //////////////////////////

        void DrawSegmentHandles(ShapeKnot firstKnot, ShapeKnot secondKnot)
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

        void DrawElementHandles(ShapeElement element)
        {
            // Debug.Log("Drawing element . Points: " + element.knots.Length);

            //DRAW KNOTS
            for (int i = 0; i < element.knots.Length; i++)
            {

                if (element.knots[i].isSelected)
                //if (element.knots[i] == Target.selectedKnot)
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
        ////// HELPER METHODS
        ////// HELPER METHODS
        ////// HELPER METHODS
        //////////////////////////

        void SwitchCollider()
        {
            if (Target.BoxCol == null)
            {
                Target.BoxCol = Target.gameObject.AddComponent<BoxCollider>();
                Target.BoxCol.size = new Vector3(1000.0f, 0.2f, 1000.0f);
            }
            else
            {
                DestroyImmediate(Target.BoxCol);
            }
        }

        //////////////////////////
        ////// INPUT METHODS
        ////// INPUT METHODS
        ////// INPUT METHODS
        //////////////////////////

        void CheckKeyboardModifierInput()
        {
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.keyDown:
                    {
                        if (e.control)
                        {
                            if (!ctrlFlag)
                            {
                                Debug.Log("CTRL down");

                                ctrlFlag = true;
                            }
                        }
                        if (e.alt)
                        {
                            if (!altFlag)
                            {
                                Debug.Log("ALT down");
                                altFlag = true;
                            }
                        }
                        break;
                    }
                case EventType.keyUp:
                    {
                        if (ctrlFlag)
                        {
                            ctrlFlag = false;
                            if (creatingKnot)
                            {
                                creatingKnot = false;
                                SwitchCollider();
                            }
                            Debug.Log("Ctrl up");
                        }
                        if (altFlag)
                        {
                            Debug.Log("Alt Up");
                            altFlag = false;
                        }
                        break;
                    }
            }
        }

        Vector3 MousePosRoutine()
        {
            Vector2 guiPosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
            RaycastHit hit;

            Vector3 theVec = new Vector3();

            if (Physics.Raycast(ray, out hit))
            {
                theVec = hit.point;
            }
            return theVec;
        }

        /*
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
        */

    }
}