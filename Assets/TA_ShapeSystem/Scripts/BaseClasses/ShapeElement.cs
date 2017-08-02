using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX.ShapeSystem
{
    [System.Serializable]
    public class ShapeElement
    {
        public int myIndex;

        public ShapeKnot[] knots;

        public void AddKnot(int theIndex, Vector3 pos)
        {
            Debug.Log("Adding knot");

            ShapeKnot[] newKnots = new ShapeKnot[knots.Length + 1];


            ShapeKnot theKnot = new ShapeKnot();
            theKnot.myIndex = theIndex;
            theKnot.myElementIndex = myIndex;
            //Handle in of the first Knot
            theKnot.kHandleIn = new Vector3(0, 0, -1);
            //Handle out of the first Knot
            theKnot.kHandleOut = new Vector3(0, 0, 1);

            for (int i = 0; i < newKnots.Length; i++)
            {

                if (i < theIndex)
                {
                    newKnots[i] = knots[i];
                }
                if (i == theIndex)
                {
                    newKnots[i] = theKnot;
                }
                if (i > theIndex)
                {
                    newKnots[i] = knots[i - 1]; ;
                }
            }
            knots = newKnots;

        }

        public void AddKnot(ShapeKnot theKnot)
        {
            Debug.Log("Adding knot");

            ShapeKnot[] newKnots = new ShapeKnot[knots.Length + 1];

            for (int i = 0; i < knots.Length; i++)
            {
                newKnots[i] = knots[i];
            }
            newKnots[newKnots.Length - 1] = theKnot;

            knots = newKnots;
            UpdateKnots();
        }

        public void RemoveKnot(int index)
        {
            if (knots.Length > 2)
            {
                List<ShapeKnot> newKnots = new List<ShapeKnot>();

                for (int i = 0; i < knots.Length; i++)
                {
                    if (i != index)
                    {
                        newKnots.Add(knots[i]);
                    }
                }
                knots = newKnots.ToArray();
                UpdateKnots();
            }
        }

        void UpdateKnots()
        {
            for (int i = 0; i < knots.Length; i++)
            {
                knots[i].myIndex = i;
            }
        }

    }

}