using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreColliders : MonoBehaviour
{
    public List<Collider> CollidersToIgnore;
    void Start()
    {
        var thisCol = GetComponent<Collider>();
        if (CollidersToIgnore != null)
        {
            foreach (var col in CollidersToIgnore)
            {
                if (col && col.enabled)
                {
                    Physics.IgnoreCollision(thisCol, col, true);
                }
            }
        }
    }


   
    
}
