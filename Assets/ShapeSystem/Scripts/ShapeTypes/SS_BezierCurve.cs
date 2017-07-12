using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace VFX.ShapeSystem
{
        public enum KnotType {Linear,Smooth,Bezier }

        [System.Serializable]
        public class ShapeKnot
            {
                public KnotType kType= KnotType.Linear;

                public Vector3 kPos;
                public Vector3 kScale;
                public Vector3 kRotation;

                public Vector3 kHandleIn;
                public Vector3 kHandleOut;

            }

        [System.Serializable]
        public class ShapeElement
        {
            public ShapeKnot[] knots;

            public void AddKnot(int index, Vector3 pos)
            {

            }

            public void RemoveKnot(int index)
            {

            }

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

        public ShapeKnot selectedKnot=null;

        ////////////////////////////////////////
        /// CURVE PROPERTIES
        /// CURVE PROPERTIES
        ////////////////////////////////////////

        [Header("Num Steps")]
        public int numSteps = 10;

        ////////////////////////////////////////
        /// LEGACY PROPERTIES
        /// LEGACY PROPERTIES
        ////////////////////////////////////////

        //Easier to use ABCD for the positions of the points so they are the same as in the tutorial image
        Vector3 A, B, C, D;

        //An array with colors to display the line segments that make up the final curve
        Color[] colorsArray = { Color.white, Color.red, Color.blue, Color.magenta, Color.black };

        //////////////////////////////////////////////////////////////////
        //////////////////          METHODS              /////////////////
        //////////////////          METHODS              /////////////////
        //////////////////          METHODS              /////////////////
        //////////////////          METHODS              /////////////////
        //////////////////////////////////////////////////////////////////


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
            //Position of the first knot 
            elements[0].knots[0].kPos = new Vector3(0, 0, -1);
            //Handle in of the first Knot
            elements[0].knots[0].kHandleIn = new Vector3(0, 0, -1);
            //Handle out of the first Knot
            elements[0].knots[0].kHandleOut = new Vector3(0, 0, 1);


            //Create the second knot
            elements[0].knots[1] = new ShapeKnot();
            //Position of the second knot 
            elements[0].knots[1].kPos = new Vector3(0, 0, 1);
            //Handle in of the second Knot
            elements[0].knots[1].kHandleIn = new Vector3(0, 0, -1);
            //Handle out of the second Knot
            elements[0].knots[1].kHandleOut = new Vector3(0, 0, 1);            
        }

        void Reset()
        {
            CreateDefaultCurve();
        }

        ////////////////////////////////////////
        /// BEZIER CALCULATION METHODS
        /// BEZIER CALCULATION METHODS
        ////////////////////////////////////////

        //The De Casteljau's Algorithm
        public Vector3 DeCasteljausAlgorithm(float t)
        {
            //Linear interpolation = lerp = (1 - t) * A + t * B
            //Could use Vector3.Lerp(A, B, t)

            //To make it faster
            float oneMinusT = 1f - t;

            //Layer 1
            Vector3 Q = oneMinusT * A + t * B;
            Vector3 R = oneMinusT * B + t * C;
            Vector3 S = oneMinusT * C + t * D;

            //Layer 2
            Vector3 P = oneMinusT * Q + t * R;
            Vector3 T = oneMinusT * R + t * S;

            //Final interpolated position
            Vector3 U = oneMinusT * P + t * T;

            return U;
        }

        //The derivative of cubic De Casteljau's Algorithm
        Vector3 DeCasteljausAlgorithmDerivative(float t)
        {
            Vector3 dU = t * t * (-3f * (A - 3f * (B - C) - D));

            dU += t * (6f * (A - 2f * B + C));

            dU += -3f * (A - B);

            return dU;
        }

        //Get and infinite small length from the derivative of the curve at position t
        float GetArcLengthIntegrand(float t)
        {
            //The derivative at this point (the velocity vector)
            Vector3 dPos = DeCasteljausAlgorithmDerivative(t);

            //This the how it looks like in the YouTube videos
            //float xx = dPos.x * dPos.x;
            //float yy = dPos.y * dPos.y;
            //float zz = dPos.z * dPos.z;

            //float integrand = Mathf.Sqrt(xx + yy + zz);

            //Same as above
            float integrand = dPos.magnitude;

            return integrand;
        }

        //Get the length of the curve between two t values with Simpson's rule
        float GetLengthSimpsons(float tStart, float tEnd)
        {
            //This is the resolution and has to be even
            int n = 20;

            //Now we need to divide the curve into sections
            float delta = (tEnd - tStart) / (float)n;

            //The main loop to calculate the length

            //Everything multiplied by 1
            float endPoints = GetArcLengthIntegrand(tStart) + GetArcLengthIntegrand(tEnd);

            //Everything multiplied by 4
            float x4 = 0f;
            for (int i = 1; i < n; i += 2)
            {
                float t = tStart + delta * i;

                x4 += GetArcLengthIntegrand(t);
            }

            //Everything multiplied by 2
            float x2 = 0f;
            for (int i = 2; i < n; i += 2)
            {
                float t = tStart + delta * i;

                x2 += GetArcLengthIntegrand(t);
            }

            //The final length
            float length = (delta / 3f) * (endPoints + 4f * x4 + 2f * x2);

            return length;
        }

        //Use Newton–Raphsons method to find the t value at the end of this distance d
        float FindTValue(float d, float totalLength)
        {
            //Need a start value to make the method start
            //Should obviously be between 0 and 1
            //We can say that a good starting point is the percentage of distance traveled
            //If this start value is not working you can use the Bisection Method to find a start value
            //https://en.wikipedia.org/wiki/Bisection_method
            float t = d / totalLength;

            //Need an error so we know when to stop the iteration
            float error = 0.001f;

            //We also need to avoid infinite loops
            int iterations = 0;

            while (true)
            {
                //Newton's method
                float tNext = t - ((GetLengthSimpsons(0f, t) - d) / GetArcLengthIntegrand(t));

                //Have we reached the desired accuracy?
                if (Mathf.Abs(tNext - t) < error)
                {
                    break;
                }

                t = tNext;

                iterations += 1;

                if (iterations > 1000)
                {
                    break;
                }
            }

            return t;
        }

        //Lista de todas las posiciones de la curva                                     <======= EDIT MIO
        private List<Vector3> curveSteps = new List<Vector3>();
        public List<Vector3> CurveSteps
        {
            get { return curveSteps; }
        }

        //Divide the curve into equal steps
        public void DivideCurveIntoSteps()
        {
            //Find the total length of the curve
            float totalLength = GetLengthSimpsons(0f, 1f);

            //How many sections do we want to divide the curve into
            int parts = numSteps;

            //reset the curve steps Array                                               <======= EDIT MIO
            curveSteps.Clear();
            curveSteps.Add(transform.TransformPoint(A));


            //What's the length of one section?
            float sectionLength = totalLength / (float)parts;

            //Init the variables we need in the loop
            float currentDistance = 0f + sectionLength;

            //The curve's start position
            Vector3 lastPos = A;

            //The Bezier curve's color
            //Need a seed or the line will constantly change color
            Random.InitState(12345);

            int lastRandom = Random.Range(0, colorsArray.Length);

            for (int i = 1; i <= parts; i++)
            {
                //Use Newton–Raphsons method to find the t value from the start of the curve 
                //to the end of the distance we have
                float t = FindTValue(currentDistance, totalLength);

                //Get the coordinate on the Bezier curve at this t value
                Vector3 pos = DeCasteljausAlgorithm(t);


                //Draw the line with a random color
                int newRandom = Random.Range(0, colorsArray.Length);

                //Get a different random number each time
                while (newRandom == lastRandom)
                {
                    newRandom = Random.Range(0, colorsArray.Length);
                }

                lastRandom = newRandom;

                Gizmos.color = colorsArray[newRandom];

                Gizmos.DrawLine(lastPos, pos);


                //Save the last position
                lastPos = pos;

                //Add current pos to vector list                                                        <======= EDIT MIO
                curveSteps.Add(transform.TransformPoint(pos));

                //Add to the distance traveled on the line so far
                currentDistance += sectionLength;
            }
        }
        
        ////////////////////////////////////////
        /// DRAWING UI
        /// DRAWING UI
        ////////////////////////////////////////

        void DrawSegment(ShapeKnot firstKnot, ShapeKnot secondKnot)
        {
            //Debug.Log("Drawing Segment");
            if (firstKnot.kType == KnotType.Linear && secondKnot.kType == KnotType.Linear)
            {
                Vector3 firstKnotWorldPos = transform.TransformPoint(firstKnot.kPos);
                Vector3 secondKnotWorldPos = transform.TransformPoint(secondKnot.kPos);

                //Debug.Log("First Pos: " + firstKnotWorldPos);
                //Debug.Log("Second Pos: " + secondKnotWorldPos);



                Gizmos.DrawLine(firstKnotWorldPos, secondKnotWorldPos);
            }
        }

        void DrawElement(ShapeElement element)
        {
           // Debug.Log("Drawing element . Points: " + element.knots.Length);

            for (int i = 0; i < element.knots.Length - 1; i++)
            {
                //Debug.Log("Drawing point : " + i );
                DrawSegment(element.knots[i], element.knots[i + 1]);
            }
        }

        void OnDrawGizmos()
        {
            //Debug.Log("Drawing");

            //The Bezier curve's color
            Gizmos.color = Color.white;

            //Draw Segments

            for (int i = 0; i < elements.Length; i++)
            {
                //Debug.Log("Element "+ i);

                DrawElement(elements[i]);
            }

            //Also draw lines between the control points and endpoints
            Gizmos.color = Color.green;

            //Draw Handles
            //Gizmos.DrawLine(A, B);
            //Gizmos.DrawLine(C, D);

            //DivideCurveIntoSteps();
        }
        
    }


}