using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class MeasurementTools : MonoBehaviour
{
    public InputActionReference measurementAction;
    public XRRayInteractor rayInteractor; //right or left hand
    public GameObject measurementMarker;
    [SerializeField] private TMP_Text measuredInfo;
    [SerializeField] private GameObject measurementCanvas;

    private RaycastHit hit;
    private bool pressed = false;
    private bool isMeasureOn = false;
    private float triggerTreshold = 0.5f;
    private int triggerCounter = 0;

    List<GameObject> hitMarkers = new List<GameObject>();
    List<Vector3> hits = new List<Vector3>();

    private float measuredHeight;
    private float measuredSlope;
    private float measuredlength;

    [SerializeField] private List<GameObject>  lineGameObjects= new List<GameObject>();
    private List<LineRenderer> lineRenderers; 

    
    void Start()
    {
        measurementCanvas.SetActive(false);
        lineRenderers = new List<LineRenderer>(lineGameObjects.Count);
        CreatLineRenderers();
        //disabable the ray interactor you are using for measurement
        rayInteractor.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckMeasurementTool();
        if (isMeasureOn)
        castRay();
    }


    public void castRay()
    {
        if (isMeasureOn == false)

            return;
       // CheckMeasurementTool();
        float triggerPressed = measurementAction.action.ReadValue<float>();       
        if (triggerPressed >= triggerTreshold && pressed == false)
        {
            
            pressed = true;

            if (rayInteractor.TryGetCurrent3DRaycastHit(out hit) && hit.transform.tag == "Terrain")
            {
                
                triggerCounter++;

                // Save hit vector3 and instantiate marker
                if (triggerCounter % 3 != 0)
                {
                    GameObject hitMarker = Instantiate(measurementMarker, hit.point, Quaternion.identity);
                    hitMarkers.Add(hitMarker);
                    hits.Add(hit.point);

                    // Calculate and display distance
                    if (triggerCounter % 3 == 2)
                    {
                        

                        // reticle.gameObject.SetActive(false);
                        DrawLines();
                        WriteMeasuredInfo();
                    }
                }
                // Delete markers and remove previous hits in list
                else
                {
                    // Remove measurement markers, clear hit an marker lists, disable distance canvas and remove measurment line
                    DestroyGameObjects(hitMarkers);
                    ClearList(hits);
                   // displaying = false;
                   foreach(LineRenderer lineRenderer in lineRenderers)
                    {
                        lineRenderer.enabled = false;
                    }
                    measurementCanvas.SetActive(false);                   
                }
            }
        }
        else if (triggerPressed < triggerTreshold && pressed)
        {
            pressed = false;
        }
    }


        private Vector3 GetP1Horizontal(Vector3 P1, Vector3 P2)
        {
            var yMax = P2.y > P1.y ? P2.y : P1.y;
            Vector3 P1h = new Vector3(P1.x, yMax, P1.z);
            return P1h;
        }
        private Vector3 GetP2Horizontal(Vector3 P1, Vector3 P2)
        {
            var yMax = P2.y > P1.y ? P2.y : P1.y;
            Vector3 P2h = new Vector3(P2.x, yMax, P2.z);
            return P2h;
        }

        private Vector3 GetP1Vertical(Vector3 P1, Vector3 P2)
        {
            Vector3 lowPt = P2.y < P1.y ? P2 : P1;
            Vector3 P1V = lowPt;
            return P1V;
        }
        private Vector3 GetP2Vertical(Vector3 P1, Vector3 P2)
        {
            Vector3 lowPt = P2.y < P1.y ? P2 : P1;
            Vector3 P2V = (P2 == lowPt) ? new Vector3(lowPt.x, P1.y, lowPt.z) : new Vector3(lowPt.x, P2.y, lowPt.z);
            return P2V;
        }

        private float CalculateDistance(Vector3 startPt, Vector3 endPt)
        {
            return Vector3.Distance(startPt, endPt);
        }

        private float ComputeLength(Vector3 startPt, Vector3 endPt)
        {
            float dx = startPt.x - endPt.x;
            float dz = startPt.z - endPt.z;
            return Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dz, 2));
        }
        private float ComputeHeight(Vector3 startPt, Vector3 endPt)
        {
            return Mathf.Abs(startPt.y - endPt.y);
        }
        private float ComputeSlope(Vector3 startPt, Vector3 endPt)
        {
            float length = ComputeLength(startPt, endPt);
            float height = ComputeHeight(startPt, endPt);
            return height / length;
        }
        private void WriteMeasuredInfo()
        {
        float reverseSlope = Mathf.Round(100f / measuredSlope)*0.01f;
        
        measuredInfo.text = "Measurements: " + '\n' +
               "Height: " + measuredHeight.ToString() + '\n' +
               "length: " + measuredlength.ToString() + '\n' +
               "Slope: 1:" + reverseSlope.ToString();
        }

    private void DestroyGameObjects(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i]);
        }
        list.Clear();
    }

    private void ClearList(List<Vector3> hits)
    {
        hits.Clear();
    }

    private void CreatLineRenderers()
    {
       // lineRenderers[0] = new LineRenderer(); //space holding 
        for (int i = 0; i < lineGameObjects.Count; i++)
        {
            LineRenderer lineRenderer = lineGameObjects[i].GetComponent<LineRenderer>();
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.enabled = false;
            lineRenderers.Add(lineRenderer);
        }      
    }

    private void DrawLines()
    {
        measurementCanvas.SetActive(true);
        Vector3 P1 = hits[0];
        Vector3 P2 = hits[1];
       
        measuredlength = Mathf.Round(ComputeLength(P1, P2) * 100) * 0.01f;
        measuredHeight = Mathf.Round(ComputeHeight(P1, P2) * 100) * 0.01f;
        measuredSlope = Mathf.Round(ComputeSlope(P1, P2) * 100) * 0.01f;

        Vector3 P1h = GetP1Horizontal(P1, P2);
        Vector3 P2h = GetP2Horizontal(P1, P2);
        Vector3 P1v = GetP1Vertical(P1, P2);
        Vector3 P2v = GetP2Vertical(P1, P2);
        //diagonal line
        var lineRenderer1 = lineRenderers[0];
        lineRenderer1.SetPositions(new Vector3[] { P1, P2 });
        lineRenderer1.enabled = true;
         //horizontal line 
         var lineRendererH = lineRenderers[1];
        lineRendererH.SetPositions(new Vector3[] { P1h, P2h });
        lineRendererH.enabled = true;
        //vertical line 
        var lineRendererV = lineRenderers[2];
        lineRendererV.SetPositions(new Vector3[] { P1v, P2v });
        lineRendererV.enabled = true;
    }

    public void SwitchOnMeasure()
    {
        isMeasureOn = !isMeasureOn;
    }

    private void CheckMeasurementTool()
    {
        if (isMeasureOn)
            rayInteractor.enabled = true;
        else
            rayInteractor.enabled = false;
    }

}
