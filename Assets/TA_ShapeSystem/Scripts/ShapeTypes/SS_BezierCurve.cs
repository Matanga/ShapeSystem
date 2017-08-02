using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace VFX.ShapeSystem
{

    public struct BezierSegment
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;
        public Vector3 D;
    }
    
    [ExecuteInEditMode]
    public class SS_BezierCurve : MonoBehaviour
    {
        /////////////////////////////////////////////////////////////////////
        //////////////////          PROPERTIES              /////////////////
        //////////////////          PROPERTIES              /////////////////
        //////////////////          PROPERTIES              /////////////////
        //////////////////          PROPERTIES              /////////////////
        /////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////
        /// ELEMENTS
        /// ELEMENTS
        ////////////////////////////////////////

        public ShapeElement[] elements;

        ////////////////////////////////////////
        /// SELECTION
        /// SELECTION
        ////////////////////////////////////////

        public List<ShapeKnot> selectedKnots = new List<ShapeKnot>();

        public BoxCollider BoxCol;

        public bool creatingKnot = false;

        public int createdIndex = -1;
        public ShapeKnot createdKnot;


        ////////////////////////////////////////
        /// CURVE PROPERTIES
        /// CURVE PROPERTIES
        ////////////////////////////////////////

        //Lista de todas las posiciones de la curva                                   
        private List<Vector3> curveSteps = new List<Vector3>();

        public bool showResampleUI;
        public int numSteps = 10;

        public Color curveColor = Color.blue;

        public Vector3[] thePoints()
        {
            curveSteps.Clear();
            curveSteps=SS_Common.ResampleCurve(numSteps, elements[0], transform);
            return curveSteps.ToArray();
        }

        //////////////////////////////////////////////////////////////////
        //////////////////          METHODS              /////////////////
        //////////////////          METHODS              /////////////////
        //////////////////          METHODS              /////////////////
        //////////////////          METHODS              /////////////////
        //////////////////////////////////////////////////////////////////


        ////////////////////////////////////////
        /// MONOBEHAVIOUR METHODS
        /// MONOBEHAVIOUR METHODS
        ////////////////////////////////////////

        void Reset()
        {
            CreateDefaultCurve();
        }

        void OnDrawGizmos()
        {
            //The Bezier curve's color
            Gizmos.color = curveColor;

            //Draw Each Element
            for (int i = 0; i < elements.Length; i++)
            {
                //Debug.Log("Element "+ i);
                DrawElement(elements[i]);
            }

            Gizmos.color = Color.green;


            if (showResampleUI)
            {
                DrawResampledCurve();
            }

            if (creatingKnot)
            {
                //Debug.Log("Moving created knot");
                Vector3 newTargetPosition = MousePosRoutine();
                Vector3 newPointPosition = new Vector3(newTargetPosition.x - transform.position.x, 0, newTargetPosition.z - transform.position.z);
                createdKnot.kPos = newPointPosition;
            }
        }

        Vector3 MousePosRoutine()
        {
            Vector2 guiPosition = Event.current.mousePosition;
            Ray ray = UnityEditor.HandleUtility.GUIPointToWorldRay(guiPosition);
            RaycastHit hit;

            Vector3 theVec = new Vector3();

            if (Physics.Raycast(ray, out hit))
            {
                theVec = hit.point;
            }
            return theVec;
        }

        ////////////////////////////////////////
        /// INITIALIZATION
        /// INITIALIZATION
        ////////////////////////////////////////

        void CreateDefaultCurve()
        {
            //Reset the elements Array
            elements = new ShapeElement[1];
            //Add an element to the array
            elements[0] = new ShapeElement();

            //Create the Knot Array
            elements[0].knots = new ShapeKnot[2];

            //Create the first knot
            elements[0].knots[0] = new ShapeKnot();
            //Setup the element Index
            elements[0].knots[0].myElementIndex = 0;
            //Setup knot index
            elements[0].knots[0].myIndex = 0;

            //Position of the first knot 
            elements[0].knots[0].kPos = new Vector3(0, 0, -5);
            //Handle in of the first Knot
            elements[0].knots[0].kHandleIn = new Vector3(0, 0, -2);
            //Handle out of the first Knot
            elements[0].knots[0].kHandleOut = new Vector3(0, 0,2);

            //Create the second knot
            elements[0].knots[1] = new ShapeKnot();
            //Setup the element Index
            elements[0].knots[1].myElementIndex = 0;
            //Setup knot index
            elements[0].knots[1].myIndex = 1;

            //Position of the second knot 
            elements[0].knots[1].kPos = new Vector3(0, 0, 5);
            //Handle in of the second Knot
            elements[0].knots[1].kHandleIn = new Vector3(0, 0, -2);
            //Handle out of the second Knot
            elements[0].knots[1].kHandleOut = new Vector3(0, 0, 2);

        }


        ////////////////////////////////////////
        /// SPLINE UPDATE
        /// SPLINE UPDATE
        ////////////////////////////////////////


        /*   RECALCULATE LINEAR  
            In this mode the Control Points ( tangents) go half-way the distance between them ant their neighbouring control point         
            tangentIn of knot[n] will go half way toward knot[n-1].tangentOut    
            tangentOut of knot[n] will go half way toward knot[n+1].tangentIn             
        */

        void RecalculateLinearKnot(ShapeElement element, int knotIndex)
        {
            //Debug.Log("recalculating index " + knotIndex.ToString());            
            //Local Variables
            Vector3 knotWorldPos = element.knots[knotIndex].KWorldPos(transform);
            Vector3 tempWorldPos = Vector3.zero;

            bool calculateOutHandle = !(knotIndex == element.knots.Length - 1);
            bool calculateInHandle = !(knotIndex == 0) ;

            if (calculateOutHandle)
            {
                Vector3 handleOutTarget = element.knots[knotIndex + 1].KHandleInWorldPos(transform);
                tempWorldPos = Vector3.Lerp(knotWorldPos, handleOutTarget, 0.5f);
                element.knots[knotIndex].kHandleOut = tempWorldPos - knotWorldPos;
            }
            if (calculateInHandle)
            {
                Vector3 handleInTarget = element.knots[knotIndex - 1].KHandleOutWorldPos(transform);
                tempWorldPos = Vector3.Lerp(knotWorldPos, handleInTarget, 0.5f);
                element.knots[knotIndex].kHandleIn = tempWorldPos - knotWorldPos;
            }
        }


        /*   RECALCULATE SMOOTH  
            Algorithm based on : http://www.efg2.com/Lab/Graphics/Jean-YvesQueinecBezierCurves.htm
          
        */





        void RecalculateSmoothKnot(ShapeElement element, int knotIndex)
        {           

            //Debug.Log("recalculating index " + knotIndex.ToString());            
            //Local Variables

            bool isLast = (knotIndex == element.knots.Length - 1);
            bool isFirst = (knotIndex == 0);


            if (!isLast && !isFirst)
            {
                //GET THE POINTS TO BE USED

                Vector3 k0Pos = element.knots[knotIndex-1].KWorldPos(transform);
                Vector3 k1Pos = element.knots[knotIndex].KWorldPos(transform);
                Vector3 k2Pos = element.knots[knotIndex +1].KWorldPos(transform);


                //GET THE OFFSETED LINE

                //Get base Vector
                Vector3 BaseVec = k0Pos - k2Pos;
                float baseVecHalfLenght = Vector3.Magnitude(BaseVec) / 2;

                Vector3 q0 = k1Pos + Vector3.Normalize(BaseVec) * baseVecHalfLenght;
                Vector3 q1 = k1Pos + Vector3.Normalize(-BaseVec) * baseVecHalfLenght;



                //GET THE IN TANGENT

                //GET k2-k1 Vector3

                Vector3 k2k1Vec = k2Pos - k1Pos;

                //Get the smoothed point dist
                float smoothPoint = k2k1Vec.magnitude / 4;

                Vector3 pIn0 = k1Pos + Vector3.Normalize(k2k1Vec) * smoothPoint;
                Vector3 pIn1 = Vector3.Lerp(k0Pos, k1Pos, 0.5f);

                Vector3 inTangent = SS_Common.GetLineIntersection(q0, q1, pIn0, pIn1);

                //ASSIGN THE TANGENT
                element.knots[knotIndex].kHandleIn= inTangent - k1Pos ;



                //GET THE OUT TANGENT

                Vector3 k0k1Vec = k0Pos - k1Pos;

                //Get the smoothed point dist
                smoothPoint = k0k1Vec.magnitude / 4;

                Vector3 pOut0 = k1Pos + Vector3.Normalize(k0k1Vec) * smoothPoint;
                Vector3 pOut1 = Vector3.Lerp(k1Pos, k2Pos, 0.5f);


                Vector3 outTangent = SS_Common.GetLineIntersection(q0, q1, pOut0, pOut1);

                //ASSIGN THE TANGENT
                element.knots[knotIndex].kHandleOut =  k1Pos-outTangent;


            }



            if (isFirst)    //We only calculate the OutTangent
            {
                Vector3 k0Pos = element.knots[0].KWorldPos(transform);
                Vector3 k1Pos = element.knots[1].KWorldPos(transform);
                Vector3 k2Pos = element.knots[2].KWorldPos(transform);

                //FIRST LINE P
                //Midpoint of knot[0] , knot[1]   = "P0"
                Vector3 p0 = Vector3.Lerp(k0Pos, k1Pos, 0.5f);

                //Dir of   knot[2] - knot[1]  = k2k1Dir
                Vector3 k2k1Dir = k2Pos - k1Pos;

                //Half distance between knot[2] - knot[1] = k2k1halfDist
                float k2k1HalfDist = k2k1Dir.magnitude / 2;

                //Get Point starting at "P0" traveling "k2k1halfDist" in "k2k1Dir" direction = "P1"
                Vector3 p1 = p0 + Vector3.Normalize(k2k1Dir) * k2k1HalfDist;

                //SECOND LINE K
                //K0 = knot[0]
                Vector3 q0 = k0Pos;

                //Get vector from knot[0] to knot[2] = "k0k2Vector"
                Vector3 k0k2Vector = k2Pos - k0Pos;

                //Get a quarter of the length of "k0k1Vector" = "smoothLength"
                float smoothLength = Vector3.Magnitude(k0k2Vector) / 4;

                //K1 = Starting at P1 travel "smoothLength" along  "k0k2Vector" direction
                Vector3 q1 = p1 + Vector3.Normalize(k0k2Vector) * smoothLength;

                //GET INTERSECTION
                Vector3 intersection = SS_Common.GetLineIntersection(p0, p1, q0, q1);

                element.knots[0].kHandleOut = intersection - k0Pos;



            }

            if (isLast) //We only calculate the inTangent
            {
                Vector3 k0Pos = element.knots[knotIndex].KWorldPos(transform);
                Vector3 k1Pos = element.knots[knotIndex-1].KWorldPos(transform);
                Vector3 k2Pos = element.knots[knotIndex-2].KWorldPos(transform);

                //FIRST LINE P
                //Midpoint of knot[0] , knot[1]   = "P0"
                Vector3 p0 = Vector3.Lerp(k0Pos, k1Pos, 0.5f);

                //Dir of   knot[2] - knot[1]  = k2k1Dir
                Vector3 k2k1Dir = k2Pos - k1Pos;

                //Half distance between knot[2] - knot[1] = k2k1halfDist
                float k2k1HalfDist = k2k1Dir.magnitude / 2;

                //Get Point starting at "P0" traveling "k2k1halfDist" in "k2k1Dir" direction = "P1"
                Vector3 p1 = p0 + Vector3.Normalize(k2k1Dir) * k2k1HalfDist;

                //SECOND LINE K
                //K0 = knot[0]
                Vector3 q0 = k0Pos;

                //Get vector from knot[0] to knot[2] = "k0k2Vector"
                Vector3 k0k2Vector = k2Pos - k0Pos;

                //Get a quarter of the length of "k0k1Vector" = "smoothLength"
                float smoothLength = Vector3.Magnitude(k0k2Vector) / 4;

                //K1 = Starting at P1 travel "smoothLength" along  "k0k2Vector" direction
                Vector3 q1 = p1 + Vector3.Normalize(k0k2Vector) * smoothLength;

                //GET INTERSECTION
                Vector3 intersection = SS_Common.GetLineIntersection(p0, p1, q0, q1);


                element.knots[knotIndex].kHandleIn = intersection - k0Pos;

            }



        }

        public void UpdateSpline()
        {
            for (int i = 0; i < elements[0].knots.Length; i++)
            {
                switch (elements[0].knots[i].kType)
                {
                    case KnotType.Linear:
                        RecalculateLinearKnot(elements[0], i);
                        break;
                    case KnotType.Smooth:
                        RecalculateSmoothKnot(elements[0], i);
                        break;
                    case KnotType.Bezier:
                        break;
                }
            }

        }

        ////////////////////////////////////////
        /// UTILITIES UI
        /// UTILITIES UI
        ////////////////////////////////////////


        /// Returns a Vector3 in world coordinates
        Vector3 GetPosAtCurvePoint(BezierSegment seg, float dist)
        {

            //Find the total length of the curve
            float totalLength = SS_Common.GetLengthSimpsons(0f, 1f, seg);

            //Use Newton–Raphsons method to find the t value from the start of the curve 
            //to the end of the distance we have
            float t = SS_Common.FindTValue(dist, totalLength, seg);

            //Get the coordinate on the Bezier curve at this t value
            Vector3 pos = SS_Common.DeCasteljausAlgorithm(t, seg);

            return pos;
        }

        public void DeselectAll()
        {
            selectedKnots.Clear();
            for (int i = 0; i < elements.Length; i++)
            {
                for (int x = 0; x < elements[i].knots.Length; x++)
                {
                    elements[i].knots[x].isSelected = false;
                }
            }

        }

        ////////////////////////////////////////
        /// DRAWING UI
        /// DRAWING UI
        /// DRAWING UI
        ////////////////////////////////////////

        ////////////////////////////////////////
        /// BEZIER CURVE DRAWING METHODS
        /// BEZIER CURVE DRAWING METHODS
        ////////////////////////////////////////

        void DrawBezierSegment(BezierSegment seg)
        {
            UnityEditor.Handles.DrawBezier(seg.A, seg.D, seg.B, seg.C, Color.red, null, 1);
        }
                
        void DrawBezierSegmentTest(BezierSegment seg)
        {

            //The start position of the line
            Vector3 lastPos = transform.TransformPoint(seg.A);

            //The resolution of the line
            //Make sure the resolution is adding up to 1, so 0.3 will give a gap at the end, but 0.2 will work
            float resolution = 0.02f;

            //How many loops?
            int loops = Mathf.FloorToInt(1f / resolution);

            for (int i = 1; i < loops; i++)
            {
                //Which t position are we at?
                float t = i * resolution;

                //Find the coordinates between the control points with a Catmull-Rom spline
                Vector3 newPos = SS_Common.DeCasteljausAlgorithm(t, seg);

                //Draw this line segment
                Gizmos.DrawLine(lastPos, newPos);

                //Save this pos so we can draw the next line segment
                lastPos = newPos;
            }

        }

        ////////////////////////////////////////
        /// LINEAR CURVE DRAWING METHODS
        /// LINEAR CURVE DRAWING METHODS
        ////////////////////////////////////////

        void DrawLinearSegment(Vector3 firstPos, Vector3 secondPos)
        {
            Gizmos.DrawLine(firstPos, secondPos);                    
        }

        ////////////////////
        /// DRAWING METHODS
        /// DRAWING METHODS
        ////////////////////

        void DrawElement(ShapeElement element)
        {
            // Debug.Log("Drawing element . Points: " + element.knots.Length);
            for (int i = 0; i < element.knots.Length - 1; i++)
            {
                //CREATE A BEZIER SEGMENT 
                BezierSegment seg = new BezierSegment();
                seg.A = GetScaledVector( element.knots[i].KWorldPos(transform));              //START POINT
                seg.B = GetScaledVector(element.knots[i].KHandleOutWorldPos(transform));     //START TANGENT
                seg.C = GetScaledVector(element.knots[i + 1].KHandleInWorldPos(transform));  //END TANGENT
                seg.D = GetScaledVector(element.knots[i + 1].KWorldPos(transform));          //END POINT

                //NON SCALED
                /*
                //CREATE A BEZIER SEGMENT 
                BezierSegment seg = new BezierSegment();
                seg.A = element.knots[i].KWorldPos(transform);              //START POINT
                seg.B = element.knots[i].KHandleOutWorldPos(transform);     //START TANGENT
                seg.C = element.knots[i + 1].KHandleInWorldPos(transform);  //END TANGENT
                seg.D = element.knots[i + 1].KWorldPos(transform);          //END POINT
                */

                //Draw it
                DrawBezierSegment(seg);
            }
        }

        Vector3 GetScaledVector(Vector3 theVector)
        {
            return new Vector3(theVector.x * transform.lossyScale.x, theVector.y * transform.lossyScale.y, theVector.z * transform.lossyScale.z);
        }

        void DrawResampledCurve()
        {
            //Draws debug segments
            curveSteps.Clear();
            curveSteps = SS_Common.ResampleCurve(numSteps, elements[0], transform);
            for (int i = 0; i < curveSteps.Count - 1; i++)
            {
                if (i % 2 == 0)
                {
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.black;
                }
                DrawLinearSegment(curveSteps[i], curveSteps[i + 1]);
            }
        }

        ////////////////////////////////////////
        /// DEBUG METHODS
        /// DEBUG METHODS
        ////////////////////////////////////////

        /// DEBUG PROPERTIES

        //Lista de todas las posiciones de la curva   
        private List<Vector3> segmentSteps = new List<Vector3>();
        public List<Vector3> SegmentSteps
        {
            get { return segmentSteps; }
        }

        //Divide the curve into equal steps
        public void DivideSegmentIntoSteps(BezierSegment seg)
        {
            //Find the total length of the curve
            float totalLength = SS_Common.GetLengthSimpsons(0f, 1f, seg);

            //How many sections do we want to divide the curve into
            int parts = numSteps;

            //reset the curve steps Array                                               <======= EDIT MIO
            segmentSteps.Clear();
            segmentSteps.Add(transform.TransformPoint(seg.A));


            //What's the length of one section?
            float sectionLength = totalLength / (float)parts;

            //Init the variables we need in the loop
            float currentDistance = 0f + sectionLength;

            //The curve's start position
            Vector3 lastPos = seg.A;

            for (int i = 1; i <= parts; i++)
            {
                //Use Newton–Raphsons method to find the t value from the start of the curve 
                //to the end of the distance we have
                float t = SS_Common.FindTValue(currentDistance, totalLength, seg);

                //Get the coordinate on the Bezier curve at this t value
                Vector3 pos = SS_Common.DeCasteljausAlgorithm(t, seg);


                //Save the last position
                lastPos = pos;

                //Add current pos to vector list                                                        <======= EDIT MIO
                segmentSteps.Add(transform.TransformPoint(pos));

                //Add to the distance traveled on the line so far
                currentDistance += sectionLength;
            }
        }
        
    }

}