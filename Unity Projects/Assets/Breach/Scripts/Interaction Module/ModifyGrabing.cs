using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class ModifyGrabing : MonoBehaviour
{
    public Transform[] soilLayers;
    private Vector3[] orginalPos;
    private Quaternion[] orginalRot;
    bool reset = false;
    
    

    void Start()
    {
        orginalPos = new Vector3[soilLayers.Length];
        orginalRot = new Quaternion[soilLayers.Length];
        for (int i = 0; i < soilLayers.Length; i++)
        {
            Vector3 childPos = soilLayers[i].localPosition;
            Quaternion childRot= soilLayers[i].localRotation;
            orginalPos[i]= childPos;
            orginalRot[i] = childRot;
        }                              
    }

   
    void Update()
    {
        if (reset)
        {
            for (int i = 0; i < soilLayers.Length; i++)
            {
                soilLayers[i].localPosition = orginalPos[i];
                soilLayers[i].localRotation = orginalRot[i];
            }
            reset = false;
        }
        
    }

    public void ResetPosition()
    {
        reset = true;
    }
    
}
