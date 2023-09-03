using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectGroup : MonoBehaviour
{
    /// <summary>
    /// This component gives you a convenient way to group together GameObjects and toggle them on / off
    /// </summary>
        public List<GameObject> BoreHoleInfo;
        public List<GameObject> BoreHole;
       // public List<GameObject> BoreHoleSign;
        public List<GameObject> DeskInfo;

    public void ActivateDeactivateGO(int GoIndex)
        {
            for (int x = 0; x < BoreHoleInfo.Count; x++)
            {
                if (BoreHoleInfo[x] != null)
                {
                BoreHoleInfo[x].SetActive(false);
                BoreHole[x].SetActive(x == GoIndex);
               // BoreHoleSign[x].SetActive(x == GoIndex);
                DeskInfo[x].SetActive(x == GoIndex);
            }

            }
        }
 }
