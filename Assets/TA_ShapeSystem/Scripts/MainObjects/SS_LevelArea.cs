using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace VFX.ShapeSystem
{
    public enum SS_AreaType { Room, Floor };

    public enum SS_InstancerType {Floor,Wall,GridInstance,Scatter,Shape };

    public class SS_LevelArea : MonoBehaviour
    {
        private RectTransform rectTransform;
        public RectTransform myRect
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = gameObject.GetComponent<RectTransform>();
                return rectTransform; 
            }
        }

        public string areaName="";

        public SS_AreaType areaType;



        #region UI Properties

        /// <summary>
        /// Flag to display the Create new Window in Editor mode
        /// </summary>
        public bool showCreateNew;

        /// <summary>
        /// Show Creation UI Flag in Editor mode
        /// </summary>
        public bool showCreationUI;

        /// <summary>
        /// Show Existing elements flag in Editor mode
        /// </summary>
        public bool showExistingSubAreas;

        /// <summary>
        /// Show Existing elements flag in Editor mode
        /// </summary>
        public bool showExistingInstancers;

        /// <summary>
        /// The type of element that is currently selected 
        /// </summary>
        public SS_InstancerType currInstancerType;
        

        #endregion


        #region Editor Utilities

        /// <summary>
        /// Renames an area based on the base name + area name
        /// </summary>
        public void RenameArea()
        {
            if (areaName.Length == 0)
            {
                transform.name = "Level_Area";
            }
            else 
            {
                transform.name = "Level_Area_" + areaName;
            }
        }

        /// <summary>
        /// Action that gets called from child elements to update the level canvas hierarchy
        /// </summary>
        public void UpdateLevelCanvas()
        {
            SS_LevelCanvas theLevelCanvas = gameObject.GetComponentInParent<SS_LevelCanvas>();
            theLevelCanvas.UpdateChildren();
        }
        
        /// <summary>
        /// Action that gets called from child elements to update the level canvas hierarchy
        /// </summary>
        public void SwitchCanvasDisplayMode()
        {
            SS_LevelCanvas theLevelCanvas = gameObject.GetComponentInParent<SS_LevelCanvas>();
            theLevelCanvas.SwitchLevelCanvasDebugDisplay();
        }

        /// <summary>
        /// Action that gets called from child elements to update the level canvas hierarchy
        /// </summary>
        public void SetChildrenVisibility(bool newVal)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                SS_LevelArea theShape = transform.GetChild(i).GetComponent<SS_LevelArea>();
                if(theShape==null)
                    transform.GetChild(i).gameObject.SetActive(newVal);
            }        
        }

        /// <summary>
        /// Retunrs the current amount of direct children with the given 
        /// name under this transforms
        /// </summary>
        public int GetChildCountByName(Transform theParent, string theName)
        {
            int theCount = 0;
            for (int i = 0; i < theParent.childCount; i++)
            {
                if (theParent.GetChild(i).name == theName)
                    theCount++;
            }
            return theCount;
        }

        #endregion


    }
}
