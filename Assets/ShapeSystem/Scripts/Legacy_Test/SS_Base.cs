using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace VFX.ShapeSystem
{

    public class SS_Base : MonoBehaviour
    {

        public float buttonSize=0.25f;


        [SerializeField]
        private Vector3[] thePoints;
        public Vector3[] ThePoints
        {
            set {  thePoints=value; }
            get { return thePoints; }
        }


        public void SetPointPos(Vector3 newPos, int index)
        {

            //thePoints[index] = new Vector3(newPos.x, 0, newPos.z);
            thePoints[index] = newPos;
        }







    }
}