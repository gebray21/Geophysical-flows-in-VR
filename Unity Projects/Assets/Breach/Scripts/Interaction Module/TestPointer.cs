using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class TestPointer : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    [SerializeField] private TMP_Text InfoText;
    private Vector3 groundPt;
    private bool isClicked = false;
    private SimulationManager waterSimulation;
    private Vector3 flowVelocity;
    private float flowHeight;

    private void Start()
    {
        waterSimulation= FindObjectOfType<SimulationManager>();
        flowVelocity = Vector3.zero;
        flowHeight = 0f;
    }

    private void Update()
    {
        if (isClicked)
        {
            RaycastHit res;
             if (rayInteractor.TryGetCurrent3DRaycastHit(out res))
             {
               groundPt = res.point;
                if (res.transform.tag == "Terrain")
                {
                    //Debug.Log($"The ground point  coordinatevis : {groundPt}");
                    flowHeight = GetFlowHeight(groundPt);
                    flowHeight = Mathf.Round(flowHeight * 100) * 0.01f;
                    flowVelocity = GetFlowVelocity(groundPt);
                    flowVelocity = RoundVector(flowVelocity);
                    WriteFlowInfo(flowVelocity, flowHeight);
                }

             }
        }
        


        //Vector3 fromVector = rayInteractor.rayOriginTransform.position;
    }



    // method to be called on selectEnter
    public void ClickOnButtun()
    {
        isClicked = true;
    }

    // method to be called on selectExit
    public void ClickOffButtun()
    {
        isClicked = false;
    }

    //fetch velocity at world position  
    private Vector3 GetFlowVelocity(Vector3 position)
    {
        if (waterSimulation == null) return Vector3.zero;

        var velocity = waterSimulation.GetWaterVelocity(position);
        return velocity.HasValue ? new Vector3(velocity.Value.x, 0f, velocity.Value.y) : Vector3.zero;
    }

    private float GetFlowHeight(Vector3 position)
    {
        if (waterSimulation == null) return 0.0f;

        var elevation = waterSimulation.GetWaterHeightAt(position);
       // return elevation.HasValue ? (elevation.Value - position.y): 0.0f;

      return  elevation.HasValue ? ((elevation.Value - position.y) > 0.0f ? (elevation.Value - position.y) : 0.0f):0.0f;
    }


    private void WriteFlowInfo(Vector3 vel, float height)
    {
        
        InfoText.text = "Point Information: " + '\n' +
           "Flow Height: "+ height.ToString() + '\n'+
          "Flow Velocity: (" + vel.x.ToString() + "," + vel.z.ToString() +","+ vel.y.ToString() +")";
    }

    private Vector3 RoundVector(Vector3 initialVector)
    {
        return new Vector3(Mathf.Round(initialVector.x*100)*0.01f, Mathf.Round(initialVector.y * 100) * 0.01f, Mathf.Round(initialVector.z * 100) * 0.01f);
    }

}
