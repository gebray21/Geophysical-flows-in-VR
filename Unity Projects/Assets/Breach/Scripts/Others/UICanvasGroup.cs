using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /// This component gives you a convenient way to group together GameObjects and toggle them on / off
    
    public class UICanvasGroup : MonoBehaviour {

        public List<GameObject> CanvasObjects;
        private int mapsCount=0;
        
        public void ActivateCanvas(int CanvasIndex) {
            for(int x = 0; x < CanvasObjects.Count; x++) {
                if(CanvasObjects[x] != null) {
                    CanvasObjects[x].SetActive(x == CanvasIndex);
                }
            }
        }

        public void ShowNext()
        {
            CanvasObjects[mapsCount].SetActive(false);
            mapsCount++;
            if (mapsCount >= CanvasObjects.Count)
            {
                mapsCount = mapsCount % CanvasObjects.Count;
            }

            CanvasObjects[mapsCount].SetActive(true);
        }
    }


