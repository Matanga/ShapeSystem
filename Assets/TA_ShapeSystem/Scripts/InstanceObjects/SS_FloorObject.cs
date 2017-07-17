using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX.ShapeSystem
{
    public class SS_FloorObject : MonoBehaviour {



        public GameObject myPrefab;
        
        #if UNITY_EDITOR

            public void RebuildPrefab()
            {
                GameObject returnGO = UnityEditor.PrefabUtility.InstantiatePrefab(myPrefab as GameObject) as GameObject;
                returnGO.transform.SetParent(transform);
                returnGO.transform.localPosition = new Vector3(0, 0, 0);
                returnGO.transform.localRotation = Quaternion.identity;

                returnGO.transform.localScale = transform.GetChild(0).localScale;
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

        #endif


    }
}
