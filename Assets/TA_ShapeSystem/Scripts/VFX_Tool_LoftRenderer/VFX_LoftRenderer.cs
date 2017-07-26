using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace VFX.ShapeSystem
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteInEditMode]
    public class VFX_LoftRenderer : MonoBehaviour
    {
        private MeshRenderer theMR;
        public MeshRenderer TheMR
        {
            get
            {
                if (theMR == null)
                {
                    theMR = gameObject.GetComponent<MeshRenderer>();
                }
                return theMR;
            }
            set { theMR = value; }
        }

        private MeshFilter theMF;
        public MeshFilter TheMF
        {
            get
            {
                if (theMF == null)
                {
                    theMF = gameObject.GetComponent<MeshFilter>();
                    theMesh = new Mesh();
                    theMF.sharedMesh = theMesh;
                }
                return theMF;
            }
            set { theMF = value; }
        }

        public Mesh theMesh;
        List<Vector3> allVertices = new List<Vector3>();
        List<int[]> allTriangles = new List<int[]>();
        List<Vector2> allUVs = new List<Vector2>();


        public AnimationCurve theLoftCurve = new AnimationCurve(new Keyframe[2]
        {
            new Keyframe(0,0),
            new Keyframe(1,0)
        });

        Vector3[] translatedAnimationCurve;



        public float sizeMultiplier = 1.0f;
        public float radius;
        public int segments;
        public bool flipNormals = true;

        public int skipEveryN = 1;


        private int totalSegments;
        private float angleOffset;

        public Vector3[][] allSegmentsPoints;

        private float[] splineSegmentsLengths;
        private float totalSplineLength;
        private float[] splineIndexUCoords;

        void Start()
        {
            DrawCircleMesh();
        }

        void Update()
        {
            DrawCircleMesh();
        }


        /// <summary>
        /// Returns a key converted into a vector3 where time is X and value is Y
        /// </summary>
        Vector3 GetVector3FromKey(Keyframe theKey)
        {
            return new Vector3(theKey.time * sizeMultiplier, theKey.value * sizeMultiplier, 0);
        }

        /// <summary>
        /// Retuns a normalized direction vector pointing to an angle in a circle
        /// </summary>
        Vector3 GetVector3FromAngle(float theAngle)
        {
            //This inverts theAngle for ease of use with the rest of the script
            if (theAngle > 0)
                theAngle = -theAngle;
            else
                theAngle = Mathf.Abs(theAngle);

            //This is to offset the angle so as to make it start pointing in the z direction
            float myAngle = theAngle + 90.0f;

            myAngle = (myAngle * Mathf.PI) / 180;

            Vector3 newDir = new Vector3((float)Mathf.Cos(myAngle), 0, (float)Mathf.Sin(myAngle));

            return newDir.normalized;
        }

        /// <summary>
        /// Gets the individual position of a point given and angle and a spline point
        /// </summary>
        Vector3 GetSegmentPointPos(float theAngle, Vector3 KeyPos)
        {
            Vector3 theDir = GetVector3FromAngle(theAngle);

            Vector3 theOffset = (theDir * radius) + (theDir * KeyPos.x);
            theOffset.y = theOffset.y + KeyPos.y;

            Vector3 finalPos = theOffset;

            return finalPos;
        }

        /// <summary>
        /// Gets the position of the vertices of this segment based on the current animation curve and a given angle
        /// </summary>
        Vector3[] GetCircleSegmentPoints(float Angle)
        {
            Vector3[] newVector = new Vector3[theLoftCurve.keys.Length];

            for (int i = 0; i < theLoftCurve.keys.Length; i++)
            {
                newVector[i] = GetSegmentPointPos(Angle, GetVector3FromKey(theLoftCurve.keys[i]));

            }
            return newVector;
        }


        /// <summary>
        /// Makes sure the properties are not set to a value that could break
        /// the script
        /// </summary>
        public void ClampValues()
        {
            if (segments <= 2)
                segments = 3;
            if (radius <= 0)
                radius = 0.1f;
            if (skipEveryN <= 0)
                skipEveryN = 1;
        }


        /*
         * A Quad = 2 triangles
         *          4 points
         * 
          p1 ________ p3
            |       /|
            |      / |
            |     /  |
            |    /   |
            |   /    |
            |  /     |
            | /      |
            |/_______|
          p0          p2  
        */


        /// <summary>
        /// Adds a single quad to the list of elements to be build
        /// </summary>
        void AddQuadToLists(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector2[] uvs)
        {
            for (int i = 0; i < uvs.Length; i++)
            {
                allUVs.Add(uvs[i]);
            }

            allVertices.Add(p0);
            allVertices.Add(p1);
            allVertices.Add(p2);
            allVertices.Add(p3);

            int p0Index = allVertices.Count - 4;
            int p1Index = allVertices.Count - 3;
            int p2Index = allVertices.Count - 2;
            int p3Index = allVertices.Count - 1;

            allTriangles.Add(new int[3] {
                p0Index,
                p1Index,
                p3Index
            });
            allTriangles.Add(new int[3] {
                p0Index,
                p3Index,
                p2Index
            });


        }


        /// <summary>
        /// Converts all the created vertices, triangles and uvs lists into a mesh
        /// </summary>
        void ConvertListToMesh()
        {
            Mesh mesh = TheMF.sharedMesh;

            theMesh.Clear();

            List<int> triangleList = new List<int>();
            for (int i = 0; i < allTriangles.Count; i++)
            {
                for (int j = 0; j < allTriangles[i].Length; j++)
                {
                    triangleList.Add(allTriangles[i][j]);
                }
            }


            // Do some calculations...
            theMesh.SetVertices(allVertices);
            theMesh.SetTriangles(triangleList, 0);
            theMesh.SetUVs(0, allUVs);
            theMesh.RecalculateNormals();

            if (flipNormals)
            {
                FlipNormals(theMesh);
            }
            theMesh.RecalculateNormals();


        }

        /// <summary>
        /// Method that flips the normals of a given mesh
        /// </summary>
        void FlipNormals(Mesh mesh)
        {
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;

            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }
        }


        ////SEGMENT STRIP EXAMPLE
        ////SEGMENT STRIP EXAMPLE
        ////SEGMENT STRIP EXAMPLE

        /*
            * A Quad = 2 triangles
            *          4 points
            *       remainder 1
         segmentPoints     p1 ________pn+1_____ plength
                            |       /|       /|
                            |      / |      / |
                            |     /  |     /  |
                            |    /   |    /   |
                            |   /    |   /    |
                            |  /     |  /     |
                            | /      | /      |
                            |/_______|/_______|
         segmentPoints     p0         pn      plength  
                *         remainder 0
        */

        /// <summary>
        /// Gets all the individual vector3 arrays for all the segments ============> CIRCLE LOFT
        /// in the loft
        /// </summary>
        public void GetSegmentsPointsLoop()
        {
            totalSegments = segments + 1;
            angleOffset = 360.0f / segments;

            allSegmentsPoints = new Vector3[segments][];

            float currentAngle = 0.0f;

            //Get all the points
            for (int i = 0; i < allSegmentsPoints.Length; i++)
            {
                allSegmentsPoints[i] = GetCircleSegmentPoints(currentAngle);
                currentAngle += angleOffset;
            }

        }

        



        public bool usePath;
        public SS_BezierCurve myPath;

        private List<Vector3> curveSteps = new List<Vector3>();


        /// <summary>
        /// Gets all the individual vector3 arrays for all the segments 
        /// in the loft
        /// </summary>
        public void GetSegmentsPointsPath()
        {
            totalSegments = myPath.thePoints().Length;

            allSegmentsPoints = new Vector3[totalSegments][];

            Vector3 dir = new Vector3();

            //Get all the points
            for (int i = 0; i < allSegmentsPoints.Length; i++)
            {
                if (i != allSegmentsPoints.Length - 1)
                {
                    dir = Vector3.Normalize(myPath.thePoints()[i + 1] - myPath.thePoints()[i]);
                }
                else
                {
                    dir = Vector3.Normalize(myPath.thePoints()[i] - myPath.thePoints()[i - 1]);
                }
                allSegmentsPoints[i] = GetSegmentPointsAtPos(myPath.thePoints()[i], dir);
            }
        }

        /// <summary>
        /// Gets the position of the vertices of this segment based on the current animation curve and a given angle
        /// </summary>
        Vector3[] GetSegmentPointsAtPos(Vector3 Pos, Vector3 dir)
        {
            Vector3[] newVector = new Vector3[theLoftCurve.keys.Length];

            for (int i = 0; i < theLoftCurve.keys.Length; i++)
            {
                newVector[i] = GetSegmentPointPos2(dir, Pos, GetVector3FromKey(theLoftCurve.keys[i]));

            }
            return newVector;
        }

        /// <summary>
        /// Gets the individual position of a point given and angle and a spline point
        /// </summary>
        Vector3 GetSegmentPointPos2(Vector3 dir, Vector3 Pos, Vector3 KeyPos)
        {

            //Conseguir la direccion entre la camara y el target pero sin diferencia de altura
            //Vector3 cameraGroundPosition = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
            //Vector3 forwardDirection = Vector3.Normalize(transform.position - cameraGroundPosition);
            Vector3 sideDirection = Vector3.Cross(dir, Vector3.up);

            Vector3 theOffsetx = (sideDirection * KeyPos.x);
            Vector3 theOffsety = new Vector3(0, KeyPos.y, 0);



            Vector3 finalPos = Pos + theOffsetx + theOffsety;

            return finalPos;
        }


        Vector3 GetSegmentPointPos3(float theAngle, Vector3 KeyPos)
        {
            Vector3 theDir = GetVector3FromAngle(theAngle);

            Vector3 theOffset = (theDir * radius) + (theDir * KeyPos.x);
            theOffset.y = theOffset.y + KeyPos.y;

            Vector3 finalPos = theOffset;

            return finalPos;
        }








        /// <summary>
        /// Gets two arrays and the total length
        /// One array with all the lengths of the individual segments
        /// the second with the u positions for every index in the segments length
        /// </summary>
        public void GetSegmentsUCoordinates()
        {
            splineSegmentsLengths = new float[theLoftCurve.keys.Length - 1];
            splineIndexUCoords = new float[theLoftCurve.keys.Length];


            for (int x = 0; x < splineSegmentsLengths.Length; x++)
            {
                splineSegmentsLengths[x] = Vector2.Distance(
                                                        GetVector2FromKey(theLoftCurve.keys[x]),
                                                        GetVector2FromKey(theLoftCurve.keys[x + 1])
                                                        );
            }

            totalSplineLength = 0;
            for (int m = 0; m < splineSegmentsLengths.Length; m++)
            {
                totalSplineLength += splineSegmentsLengths[m];
            }


            splineIndexUCoords[0] = 0.0f;
            splineIndexUCoords[splineIndexUCoords.Length - 1] = 1.0f;

            for (int i = 1; i < splineIndexUCoords.Length - 1; i++)
            {
                //  totallength ____  1
                splineIndexUCoords[i] = splineSegmentsLengths[i] / totalSplineLength;
            }

        }

        /// <summary>
        /// Adds the Quads that correspond to this segment to the list that will be used to construct
        /// the final mesh
        /// </summary>
        void GetSegmentQuadsLists(Vector3[] segment1, Vector3[] segment2)
        {
            for (int i = 0; i < segment1.Length - 1; i++)
            {
                Vector2[] theUVs = new Vector2[4]
                {
                        new Vector2(0,splineIndexUCoords[i]),
                        new Vector2(1,splineIndexUCoords[i]),
                        new Vector2(0,splineIndexUCoords[i+1]),
                        new Vector2(1,splineIndexUCoords[i+1])
                };

                AddQuadToLists(segment1[i],
                                segment2[i],
                                segment1[i + 1],
                                segment2[i + 1],
                                theUVs
                    );
            }
        }

        /// <summary>
        /// Process to draw the currenr circular Loft
        /// </summary>
        public void DrawCircleMesh()
        {
            if (!usePath)
                GetSegmentsPointsLoop();
            else
                GetSegmentsPointsPath();

            GetSegmentsUCoordinates();

            allVertices.Clear();
            allTriangles.Clear();
            allUVs.Clear();

            for (int i = 0; i < allSegmentsPoints.Length; i++)
            {
                //Debug.Log(i % 2);
                if (i == 0)
                {
                    GetSegmentQuadsLists(allSegmentsPoints[i], allSegmentsPoints[i + 1]);
                }
                if (i != allSegmentsPoints.Length - 1)
                {
                    if (i != 0 && (i % skipEveryN == 0))
                        GetSegmentQuadsLists(allSegmentsPoints[i], allSegmentsPoints[i + 1]);
                }
                else
                {
                    if (i != 0 && (i % skipEveryN == 0) && !usePath)
                        GetSegmentQuadsLists(allSegmentsPoints[i], allSegmentsPoints[0]);
                }
            }
            ConvertListToMesh();
        }



        /////////////////////////////////
        /////////////////////////////////
        //////////  DEBUG
        //////////  DEBUG
        /////////////////////////////////
        /////////////////////////////////

        #region LineRenderer Properties

        private LineRenderer theLR;
        private LineRenderer TheLR
        {
            get
            {
                if (theLR == null)
                {
                    theLR = gameObject.AddComponent<LineRenderer>();
                    theLR.shadowCastingMode = ShadowCastingMode.Off;
                    theLR.receiveShadows = false;
                    theLR.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                    theLR.useWorldSpace = false;
                    theLR.lightProbeUsage = LightProbeUsage.Off;
                    Material tempMaterial = new Material(Shader.Find("Diffuse"));
                    theLR.sharedMaterial = tempMaterial;
                    theLR.widthMultiplier = theLRWidth;
                    theLR.endColor = theLRColor;
                    theLR.startColor = theLRColor;
                }
                return theLR;
            }
            set
            {
                theLR = value;
            }
        }
        public Color theLRColor = Color.white;
        public float theLRWidth = 1.0f;

        #endregion

        #region LineRenderer Methods

        Vector2 GetVector2FromKey(Keyframe theKey)
        {
            return new Vector2(theKey.time, theKey.value);
        }

        Vector3[] GetLineRendererPointsFromCurve(AnimationCurve theCurve)
        {
            Vector3[] theVectors = new Vector3[theCurve.keys.Length];

            for (int i = 0; i < theCurve.keys.Length; i++)
            {
                theVectors[i] = GetVector3FromKey(theCurve.keys[i]);
            }
            return theVectors;
        }

        public void DrawLineRenderer()
        {
            TheLR.positionCount = theLoftCurve.keys.Length;
            TheLR.SetPositions(GetLineRendererPointsFromCurve(theLoftCurve));
        }

        #endregion

        public void TestDrawQuad()
        {
            allVertices.Clear();
            allTriangles.Clear();
            //AddQuadToLists(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(1, 0, 1));

            //AddQuadToLists(new Vector3(1, 0, 1), new Vector3(1, 0, 2), new Vector3(2, 0, 1), new Vector3(2, 0, 2));

            ConvertListToMesh();
        }

        /// <summary>
        /// Debug Function that creates a sphere at every segment point that exist
        /// </summary>
        public void DrawSegmentsPoints()
        {
            GetSegmentsPointsLoop();

            for (int i = 0; i < allSegmentsPoints.Length; i++)
            {
                for (int j = 0; j < allSegmentsPoints[i].Length; j++)
                {
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.position = allSegmentsPoints[i][j];
                    sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }


    }
}