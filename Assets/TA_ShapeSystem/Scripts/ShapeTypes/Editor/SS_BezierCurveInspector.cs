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

        bool ctrlFlag = false;
        bool altFlag = false;
        private float handleSize = 12;

        public SS_BezierCurve Target
        {
            get
            {
                return (SS_BezierCurve)target; ;
            }
        }

        SerializedProperty showResampleUI;

        SerializedProperty numSteps;

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
            Target.DeselectAll();

            showResampleUI = serializedObject.FindProperty("showResampleUI");
            numSteps = serializedObject.FindProperty("numSteps");

            ctrlFlag = false;
            altFlag = false;

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
            CheckKeyboardModifierInput();
        }

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            DrawSelectedKnotInfo();

            DrawResampleUI();

            EditorGUILayout.LabelField(("Length: "+SS_Common.GetCurveLength(Target.elements[0],Target.transform).ToString()));
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Target, "Switch Transform Flags");
            }
            serializedObject.ApplyModifiedProperties();

        }
        
        //////////////////////////
        //////UI DRAWING METHODS
        //////UI DRAWING METHODS
        //////////////////////////

        void DrawSelectedKnotInfo()
        {
            if (Target.selectedKnots.Count == 1)
            {
                serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginVertical("Button");

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Index: "+ Target.selectedKnots[0].myIndex.ToString());
                

                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;

                Target.selectedKnots[0].kType = (KnotType)EditorGUILayout.EnumPopup(Target.selectedKnots[0].kType, GUILayout.Width(120));

                //EditorGUILayout.LabelField(". ");

                
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
      
                EditorGUILayout.EndHorizontal();


                serializedObject.ApplyModifiedProperties();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(Target, "Change Area Name");
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginVertical("Button");



                EditorGUILayout.LabelField("-");


                serializedObject.ApplyModifiedProperties();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(Target, "Change Area Name");
                }
                EditorGUILayout.EndVertical();
            }


            

        }

        void DrawResampleUI()
        {

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical("Button");

            EditorGUILayout.LabelField("Resample Options",SS_Common.StyleTitle);

            EditorGUILayout.PropertyField(numSteps);

            numSteps.intValue = EditorGUILayout.IntSlider(numSteps.intValue, 1, 100);

            EditorGUILayout.PropertyField(showResampleUI);


            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Target, "Change Area Name");
            }
            EditorGUILayout.EndVertical();
            
        }
        
        //////////////////////////
        ////// HANDLES DRAWERS
        ////// HANDLES DRAWERS
        ////// HANDLES DRAWERS
        //////////////////////////

        //////////////////////////
        //////SCENE UI DRAWING METHODS
        //////SCENE UI DRAWING METHODS
        //////////////////////////

        private void DrawKnotMovementHandle(ShapeKnot knot)
        {
            Vector3 SnapVal = Vector3.one * 0.25f;

            float size = (HandleUtility.GetHandleSize(Target.transform.TransformPoint(knot.kPos))) / handleSize;




            Handles.color = Color.red;
            EditorGUI.BeginChangeCheck();
            Vector3 newTargetPosition = Handles.FreeMoveHandle(Target.transform.TransformPoint(knot.kPos), Quaternion.identity, size, SnapVal, Handles.CircleHandleCap);

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
            float size = (HandleUtility.GetHandleSize(Target.transform.TransformPoint(knot.kPos))) / handleSize;

            Handles.color = Color.yellow;

            Vector3 knotWorldPos = knot.KWorldPos(Target.transform);


            if (handleIn)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 handlePos = knotWorldPos + knot.kHandleIn;
                Handles.color = Color.green;

                Vector3 newTargetPosition = Handles.FreeMoveHandle(handlePos, Quaternion.identity, (size/2), SnapVal, Handles.RectangleHandleCap);

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

                Vector3 newTargetPosition = Handles.FreeMoveHandle(handlePos, Quaternion.identity, (size / 2), SnapVal, Handles.RectangleHandleCap);

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

            float size = (HandleUtility.GetHandleSize(Target.transform.TransformPoint(knot.kPos)))/ handleSize;

            if (Handles.Button(Target.transform.TransformPoint(knot.kPos), Quaternion.Euler(90, 0, 0), size, size, Handles.RectangleHandleCap))
            {
                //Debug.Log("Selected Knot " + Target.selectedElementIndex.ToString() + " " + Target.selectedKnotIndex.ToString());

                if (ctrlFlag &&altFlag )    
                {
                    Debug.Log("knot selected with ctrl pressed");
                    CheckForVertexTypeCycle();
                } 
                 if (!altFlag)
                {
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
                }
                else
                {
                    Target.elements[0].RemoveKnot(knot.myIndex);
                    Debug.Log("Deleting Knot");
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

                    if (element.knots[i].kType == KnotType.Bezier || element.knots[i].kType == KnotType.Smooth)
                    {
                        DrawKnotBezierHandles(element.knots[i], true, true);
                    }
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

        void CreateCollider()
        {
            if (Target.BoxCol != null)
            {
                DestroyImmediate(Target.BoxCol);
            }
            Target.BoxCol = Target.gameObject.AddComponent<BoxCollider>();
            Target.BoxCol.size = new Vector3(1000.0f, 0.2f, 1000.0f);
        }

        //////////////////////////
        ////// INPUT METHODS
        ////// INPUT METHODS
        ////// INPUT METHODS
        //////////////////////////

        void StartKnotCreation()
        {
            if(Target.creatingKnot)return;

            Target.creatingKnot = true;

            Debug.Log("Creating knot");

            if (Target.selectedKnots.Count == 0)
            {
                Debug.Log("Creating knot started");

                CreateCollider();

                Target.createdKnot = new ShapeKnot();

                Target.createdKnot.kPos = Vector3.zero;
                Target.createdKnot.kHandleIn = new Vector3(0, 0, -2);
                Target.createdKnot.kHandleOut = new Vector3(0, 0, 2);

                Target.createdKnot.myElementIndex = 0;
                Target.createdKnot.myIndex = Target.elements[0].knots.Length;

                Target.elements[0].AddKnot(Target.createdKnot);

            }
        }

        void FinishKnotCreation()
        {
            Debug.Log("Fisnished creating knot");

            Target.creatingKnot = false;
            Target.createdKnot = null;
            SwitchCollider();            
        }


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

                    KnotType currentType = Target.selectedKnots[0].kType;

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
                    Target.selectedKnots[0].kType = (KnotType)(value++);
                    serializedObject.ApplyModifiedProperties();

                }
            }
        }

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
                            if (!Target.creatingKnot)
                            {
                                StartKnotCreation();
                            }
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

                        if (Target.creatingKnot)
                        {
                            FinishKnotCreation();
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
                case EventType.mouseUp:
                {
                    if (e.button == 1)
                    {
                        Target.DeselectAll();
                    }
                    if (e.button == 2)
                    {
                        if (Target.selectedKnots.Count == 1)
                        {
                            CreateContextMenu();
                                e.Use();
                        }
                    }
                    Debug.Log("Mouse UP "+e.button.ToString());
                    break;
                }
            }
        }
        

        void CreateContextMenu()
        {
            // create the menu and add items to it
            GenericMenu menu = new GenericMenu();

            // forward slashes nest menu items under submenus
            AddMenuItemForKnotType(menu, "Linear", KnotType.Linear);
            AddMenuItemForKnotType(menu, "Smooth", KnotType.Smooth);
            AddMenuItemForKnotType(menu, "Free", KnotType.Bezier);


            /*

            // forward slashes nest menu items under submenus
            AddMenuItemForColor(menu, "RGB/Red", Color.red);
            AddMenuItemForColor(menu, "RGB/Green", Color.green);
            AddMenuItemForColor(menu, "RGB/Blue", Color.blue);

            // an empty string will create a separator at the top level
            menu.AddSeparator("");

            AddMenuItemForColor(menu, "CMYK/Cyan", Color.cyan);
            AddMenuItemForColor(menu, "CMYK/Yellow", Color.yellow);
            AddMenuItemForColor(menu, "CMYK/Magenta", Color.magenta);
            // a trailing slash will nest a separator in a submenu
            menu.AddSeparator("CMYK/");
            AddMenuItemForColor(menu, "CMYK/Black", Color.black);

            menu.AddSeparator("");

            AddMenuItemForColor(menu, "White", Color.white);
            */

            // display the menu
            menu.ShowAsContext();

        }

        // serialize field on window so its value will be saved when Unity recompiles
        [SerializeField]
        Color m_Color = Color.white;


        // a method to simplify adding menu items
        void AddMenuItemForColor(GenericMenu menu, string menuPath, Color color)
        {
            // the menu item is marked as selected if it matches the current value of m_Color
            menu.AddItem(new GUIContent(menuPath), m_Color.Equals(color), OnColorSelected, color);
        }

        // a method to simplify adding menu items
        void AddMenuItemForKnotType(GenericMenu menu, string menuPath, KnotType type)
        {
            // the menu item is marked as selected if it matches the current value of m_Color
            menu.AddItem(new GUIContent(menuPath), Target.selectedKnots[0].kType.Equals(type), OnTypeSelected, type);
        }

        // the GenericMenu.MenuFunction2 event handler for when a menu item is selected
        void OnTypeSelected(object type)
        {
            Target.selectedKnots[0].kType = (KnotType)type;
        }

        // the GenericMenu.MenuFunction2 event handler for when a menu item is selected
        void OnColorSelected(object color)
        {
            m_Color = (Color)color;
        }






    }
}