using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectGameObject : MonoBehaviour
{
    public GameObject[] gameObjectGroup;
    
   public void DeactivateAll()
    {
        for ( int x=0; x < gameObjectGroup.Length; x++)
        {
            gameObjectGroup[x].SetActive(false);
        }
    }

    public void ActivateAll()
    {
        for (int x = 0; x < gameObjectGroup.Length; x++)
        {
            gameObjectGroup[x].SetActive(true);
        }
    }
}
