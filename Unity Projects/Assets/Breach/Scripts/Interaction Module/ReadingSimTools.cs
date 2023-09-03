using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ReadingSimTools : MonoBehaviour
{
    [SerializeField] InputActionReference measurementAction;
    [SerializeField] XRRayInteractor rayInteractor; //right or left hand
    [SerializeField] private GameObject measurementCanvas;
    [SerializeField] private TMP_Text InfoText;
    private Vector3 groundPt;
    private bool isClicked = false;
    private SimulationManager waterSimulation;
    private Vector3 flowVelocity;
    private float flowHeight;

    private RaycastHit res;
    private bool pressed = false;
    private bool isReadOn = false;
    private float triggerTreshold = 0.5f;
    private int triggerCounter = 0;
    void Start()
    {
        waterSimulation = FindObjectOfType<SimulationManager>();
        flowVelocity = Vector3.zero;
        flowHeight = 0f;
        measurementCanvas.SetActive(false);
        rayInteractor.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
       // CheckMeasurementTool();
        if (isReadOn)
            GetHitPoint();
    }

    private void GetHitPoint()
    {
        if (isReadOn == false)
            return;
        float triggerPressed = measurementAction.action.ReadValue<float>();
        if (triggerPressed >= triggerTreshold && pressed == false)
        {
            pressed = true;
            rayInteractor.enabled = true;
            if (rayInteractor.TryGetCurrent3DRaycastHit(out res) && res.transform.tag == "Terrain")
            {
                triggerCounter++;

                // Save hit vector3 and instantiate marker
                if (triggerCounter % 2 == 1)
                {
                    measurementCanvas.SetActive(true);
                    groundPt = res.point;
                    flowHeight = GetFlowHeight(groundPt);
                    flowHeight = Mathf.Round(flowHeight * 100) * 0.01f;
                    flowVelocity = GetFlowVelocity(groundPt);
                    flowVelocity = RoundVector(flowVelocity);
                    WriteFlowInfo(flowVelocity, flowHeight);
                }
                else
                {
                    measurementCanvas.SetActive(false);
                }
            }
            
        }
        else if (triggerPressed < triggerTreshold && pressed)
        {
            pressed = false;
            rayInteractor.enabled = false;
        }
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

        return elevation.HasValue ? ((elevation.Value - position.y) > 0.0f ? (elevation.Value - position.y) : 0.0f) : 0.0f;
    }


    private void WriteFlowInfo(Vector3 vel, float height)
    {

        InfoText.text = "Flow Information: " + '\n' +
           "Height(m): " + height.ToString() + '\n' +
          "Velocity(m/s): (" + vel.x.ToString() + "," + vel.z.ToString() + "," + vel.y.ToString() + ")";
    }

    private Vector3 RoundVector(Vector3 initialVector)
    {
        return new Vector3(Mathf.Round(initialVector.x * 100) * 0.01f, Mathf.Round(initialVector.y * 100) * 0.01f, Mathf.Round(initialVector.z * 100) * 0.01f);
    }

    private void CheckMeasurementTool()
    {
        if (isReadOn)
            rayInteractor.enabled = true;
        else
            rayInteractor.enabled = false;
    }
    public void SwitchOnRead()
    {
        //isReadOn = !isReadOn;
        isReadOn = true;
    }
    public void SwitchOffRead()
    {
        isReadOn = false;
        rayInteractor.enabled = false;
        measurementCanvas.SetActive(false);
    }
}
