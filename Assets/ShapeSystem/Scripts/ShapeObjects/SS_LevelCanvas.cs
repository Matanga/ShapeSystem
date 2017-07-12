using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VFX.ShapeSystem
{
    public class SS_LevelCanvas : MonoBehaviour
    {
        /// <summary>
        /// Flag to know which Display mode to draw
        /// </summary>
        public bool debugMode;
        
        /// <summary>
        /// Flag to display the Create new Window in Editor mode
        /// </summary>
        public bool showCreateNew;
          
        #region Reference Arrays

        /// <summary>
        /// Reference to all SS_LevelArea scripts down the hierarchy
        /// </summary>
        public SS_LevelArea[] allRects ;
        /// <summary>
        /// Reference to all Image scripts down the hierarchy
        /// </summary>
        public Image[] allImages ;
        /// <summary>
        /// Reference to all SS_Floor scripts down the hierarchy
        /// </summary>
        public SS_Floor[] allFloors;
        /// <summary>
        /// Reference to all SS_Wall scripts down the hierarchy
        /// </summary>
        public SS_Wall[] allWalls;
        /// <summary>
        /// Reference to all SS_GridInstancer scripts down the hierarchy
        /// </summary>
        public SS_GridInstancer[] allGrids;

        /// <summary>
        /// Reference to all SS_GridInstancer scripts down the hierarchy
        /// </summary>
        public SS_FloorRepeater[] allFloorRepeaters;



        #endregion

        #region Switch Display Mode Methods

        /// <summary>
        /// Switches the Display mode and updates the level canvas
        /// </summary>
        public void SwitchLevelCanvasDebugDisplay()
        {
            GetDebugReferences();
            //Debug.Log("Switching");
            debugMode = !debugMode;

            if (allImages.Length != 0)
            {
                for (int i = 0; i < allImages.Length; i++)
                {
                    allImages[i].enabled = debugMode;
                }
            }

            if (allWalls != null && allWalls.Length != 0)
            {
                for (int i = 0; i < allWalls.Length; i++)
                {
                    allWalls[i].SetChildrenVisibility(!debugMode);
                }
            }
            if (allFloors != null && allFloors.Length != 0)
            {
                for (int i = 0; i < allFloors.Length; i++)
                {
                    allFloors[i].SetChildrenVisibility(!debugMode);
                }
            }

            if (allGrids != null && allGrids.Length != 0)
            {
                for (int i = 0; i < allGrids.Length; i++)
                {
                    allGrids[i].SetChildrenVisibility(!debugMode);
                }
            }

            if (allFloorRepeaters != null && allFloorRepeaters.Length != 0)
            {
                for (int i = 0; i < allFloorRepeaters.Length; i++)
                {
                    allFloorRepeaters[i].SetChildrenVisibility(!debugMode);
                }
            }

            /*
            if (allRects.Length != 0)
            {
                for (int i = 0; i < allRects.Length; i++)
                {
                    allRects[i].SetChildrenVisibility(!debugMode);
                }
            }*/

        }
       
        /// <summary>
        /// Sets the display mode to the given value and updates the level canvas
        /// </summary>
        /// <param name="theVal"></param>
        public void SetLevelCanvasDebugDisplay(bool theVal)
        {
            debugMode = theVal;
            if (allImages.Length != 0)
            {
                for (int i = 0; i < allImages.Length; i++)
                {
                    allImages[i].enabled=theVal;
                }
            }
            if (allRects.Length != 0)
            {
                for (int i = 0; i < allRects.Length; i++)
                {
                    allRects[i].SetChildrenVisibility(!theVal);
                }                
            }

        }

 
       #endregion

        #region Update Child Instancers 

       /// <summary>
       /// Updates all children that have an Instancer type(wall,floor,grid,etc) of this Level canvas
       /// </summary>
       public void UpdateChildren()
        {
           if (debugMode) return;
           GetDebugReferences();

           UpdateFloors();
           UpdateWalls();
           UpdateGridInstancers();
           UpdateGridScatters();
           UpdateFloorRepeaters();

        }

       /// <summary>
       /// Gets all the pertinent components down the hierarchy
       /// </summary>
       void GetDebugReferences()
        { 
            allRects = transform.GetComponentsInChildren<SS_LevelArea>();
            allImages = transform.GetComponentsInChildren<Image>();

            allFloors = transform.GetComponentsInChildren<SS_Floor>();
            allWalls = transform.GetComponentsInChildren<SS_Wall>();
            allGrids = transform.GetComponentsInChildren<SS_GridInstancer>();
            allFloorRepeaters = transform.GetComponentsInChildren<SS_FloorRepeater>();

        }


       void UpdateFloors()
        { 
            SS_Floor[] allFloors = transform.GetComponentsInChildren<SS_Floor>();
            if (allFloors != null)
            {
                for (int i = 0; i < allFloors.Length; i++)
                {
                    allFloors[i].OnRectUpdateCallback();
                }   
            }
        }
       void UpdateWalls()
        { 
            SS_Wall[] allWalls = transform.GetComponentsInChildren<SS_Wall>();
            if (allWalls != null)
            {
                for (int i = 0; i < allWalls.Length; i++)
                {
                    allWalls[i].OnRectUpdateCallback();
                }
            }
        }
       void UpdateGridInstancers()
        { 
            SS_GridInstancer[] allGridInsts = transform.GetComponentsInChildren<SS_GridInstancer>();
            if (allGridInsts != null)
            {
                for (int i = 0; i < allGridInsts.Length; i++)
                {
                    allGridInsts[i].OnRectUpdateCallback();
                }
            }
        }
       void UpdateGridScatters()
       {
           SS_GridScatter[] allGridInsts = transform.GetComponentsInChildren<SS_GridScatter>();
           if (allGridInsts != null)
           {
               for (int i = 0; i < allGridInsts.Length; i++)
               {
                   allGridInsts[i].OnRectUpdateCallback();
               }
           }
       }
       void UpdateFloorRepeaters()
        {
            SS_FloorRepeater[] allinstances = transform.GetComponentsInChildren<SS_FloorRepeater>();
            if (allinstances != null)
            {
                for (int i = 0; i < allinstances.Length; i++)
                {
                    allinstances[i].OnRectUpdateCallback();
                }
            }
        }




        #endregion

    }
}