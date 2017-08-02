using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX.ShapeSystem
{
    public class Test_LineIntersect : MonoBehaviour {


        public Transform p0;
        public Transform p1;
        public Transform q0;
        public Transform q1;



        void Start()
        {

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            cube.transform.position = SS_Common.GetLineIntersection(p0.position, p1.position, q0.position, q1.position);


        }




    }
}