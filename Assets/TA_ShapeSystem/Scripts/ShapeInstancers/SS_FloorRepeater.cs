using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX.ShapeSystem
{

    public class SS_FloorRepeater : MonoBehaviour
    {
        List<Mesh> theMeshes = new List<Mesh>();
        
        public SS_LevelArea theFloor;

        public float floorHeight;

        public int floorCount;

        private int previousSiblingIndex;

        public void RemovePreviousInstances()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        public void OnRectUpdateCallback()
        {
            GeneratePrefabs();
        }

        public void SetChildrenVisibility(bool theVal)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(theVal);
            }
        }


        int GetPreviousChildIndex()
        {
            int previousChild=-1;
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                if (transform.parent.GetChild(i) == transform)
                {
                    previousChild = i - 1;
                }
            }
            return previousChild;
        }
        bool IsPreviousChildFloor()
        {
            previousSiblingIndex = GetPreviousChildIndex();

            SS_LevelArea theArea = transform.parent.GetChild(previousSiblingIndex).GetComponent<SS_LevelArea>();

            if (theArea.areaType == SS_AreaType.Floor)      {return true; }            
                                                        else{ return false; }
        }


        void ProcessMeshBounds(MeshFilter theMeshFilter)
        {
            Vector3 thePos = theMeshFilter.transform.position;
            Bounds theBound = theMeshFilter.sharedMesh.bounds;
            Debug.Log(theMeshFilter.transform.name);

            //Debug.Log(thePos);
            //Debug.Log (theBound.center);

            Debug.Log(new Vector3(thePos.x,thePos.y+ theBound.extents.y, thePos.z));
            Debug.Log(theFloor.transform.position.y+( floorHeight * (floorCount + 1)));
        }

        [ContextMenu("World Height ")]
        public float GetWorldHeight()
        {
            theMeshes.Clear();

            Transform lastChild = transform.GetChild(transform.childCount - 1);

            MeshFilter[] allRenderers = lastChild.GetComponentsInChildren<MeshFilter>();

            foreach (MeshFilter renderer in allRenderers)
            {
                if (!theMeshes.Contains(renderer.sharedMesh))
                {
                    theMeshes.Add(renderer.sharedMesh);
                    ProcessMeshBounds(renderer);
                }
            }
            return 1.0f;
        }


        /// <summary>
        /// Main Function that builds the wall
        /// </summary>
        public void GeneratePrefabs()
        {
            RemovePreviousInstances();

            if (theFloor.areaType==SS_AreaType.Floor)
            {
                for (int i = 0; i < floorCount; i++)
                {
                    GameObject newFloor = Instantiate(theFloor.gameObject, theFloor.transform.position, theFloor.transform.rotation) as GameObject;

                    newFloor.transform.position = newFloor.transform.position + new Vector3(0, floorHeight*(i+1), 0);

                    newFloor.transform.SetParent(transform);

                    SS_LevelArea[] allareas = newFloor.GetComponentsInChildren<SS_LevelArea>();
                    for (int x = allareas.Length - 1; x >= 0; x--)
                    {
                        DestroyImmediate(allareas[x]);
                    }

                    SS_Floor[] allfloors = newFloor.GetComponentsInChildren<SS_Floor>();
                    for (int x = allfloors.Length - 1; x >= 0; x--)
                    {
                        DestroyImmediate(allfloors[x]);
                    }

                    SS_Wall[] allwalls = newFloor.GetComponentsInChildren<SS_Wall>();
                    for (int x = allwalls.Length - 1; x >= 0; x--)
                    {
                        DestroyImmediate(allwalls[x]);
                    }

                    SS_GridInstancer[] allinstancers = newFloor.GetComponentsInChildren<SS_GridInstancer>();
                    for (int x = allinstancers.Length - 1; x >= 0; x--)
                    {
                        DestroyImmediate(allinstancers[x]);
                    }
                }
            }  
        }

        [ContextMenu("Generate Floors")]
        void Doit()
        {
            GeneratePrefabs();

        }

    }

}