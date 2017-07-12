using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VFX.ShapeSystem;

public class Test_SnapRectTransforms : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    public GameObject theFloor;

    public RectTransform theParentTransform;



    [ContextMenu("Test Snap Rect Transform")]
    void Doit()
    {
        GameObject newFloor = Instantiate(theFloor, theParentTransform.position, Quaternion.Euler(90,0,0)) as GameObject;

        RectTransform theRect = newFloor.GetComponent<RectTransform>();

        newFloor.transform.SetParent(theParentTransform);
        theRect.anchorMin = new Vector2(0, 0);
        theRect.anchorMax = new Vector2(1, 1);
        theRect.pivot = new Vector2(0.5f, 0.5f);
        theRect.offsetMin = new Vector2(0, 0);
        theRect.offsetMax = new Vector2(0, 0);

        SS_LevelCanvas theCanvas = theParentTransform.gameObject.GetComponent<SS_LevelCanvas>();
        theCanvas.UpdateChildren();

    }

}
