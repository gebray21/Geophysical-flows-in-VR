using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class DrawLine : MonoBehaviour
{
    public XRRayInteractor rayInteractor; //right or left hand
    public InputActionReference drawReference = null;
    public Material lineMat;
    [SerializeField] private TMP_Text terrainInfo;
    public float lineWidth = 0.01f;
   // public Color lineColor; 
    private RaycastHit res;
    private Vector3 P1, P2;
    private Vector3? lStart = null;
    private bool isFirstPtStored = false;
    private bool isSecondPtStored = false;
    private Vector3 groundPt;
    private float distance;
    int cli = 0;
    private GameObject lineGameObject;
    private GameObject lineGameObjectH;
    private GameObject lineGameObjectV;
    private float terrainHeight;
    private float terrainSlope;
    private float terrainlength;

    void Start()
    {
        drawReference.action.started += Draw;
       // lineRenderer = GetComponent<LineRenderer>();
    }

 /*  
    void Update()
    {
        float value = drawReference.action.ReadValue<float>();
        if(value >0.99f)
        {
            if (RayHit())
            {
                //Trial1();
               // Trial2();
                StartCoroutine(MeasureDistance());
            }
        }
    }
*/
    private void Draw(InputAction.CallbackContext ctx)
    {
        if (RayHit())
        {
            Trial1();
        }
    }

    private void OnDestroy()
    {
        drawReference.action.started -= Draw;
    }


    private IEnumerator MeasureDistance()
    {
        Trial2();
        yield return null;
    }
    

   private void Trial1()
    {
        if (lineGameObject)
            Destroy(lineGameObject);
        if (lineGameObjectH)
            Destroy(lineGameObjectH);
        if (lineGameObjectV)
            Destroy(lineGameObjectV);

        // groundPt = GetHitpoint();
        if (isFirstPtStored == false)
        {
            P1 = GetHitpoint();
            isFirstPtStored = true;
        }
        else
        {
            P2 = GetHitpoint();
            isSecondPtStored = true;
            isFirstPtStored = false;
        }

        if (isSecondPtStored)
        {
            var yMax = P2.y > P1.y ? P2.y : P1.y;
            Vector3 lowPt = P2.y < P1.y ? P2 : P1;
            Vector3 P1h = new Vector3(P1.x, yMax, P1.z);
            Vector3 P2h = new Vector3(P2.x, yMax, P2.z);
            Vector3 P1v = lowPt;
            Vector3 P2v = (P2 == lowPt) ? new Vector3(lowPt.x, P1.y, lowPt.z) : new Vector3(lowPt.x,P2.y,lowPt.z);
            lineGameObject = new GameObject();
            lineGameObjectH = new GameObject();
            lineGameObjectV = new GameObject();
            //diagonal line
            var lineRenderer1 = lineGameObject.AddComponent<LineRenderer>();           
            lineRenderer1.SetPositions(new Vector3[] { P1, P2 });
            SetLinerenderer(lineRenderer1);
            //horizontal line 
            var lineRendererH = lineGameObjectH.AddComponent<LineRenderer>();
            lineRendererH.SetPositions(new Vector3[] { P1h, P2h });
            SetLinerenderer(lineRendererH);
            //vertical line 
            var lineRendererV = lineGameObjectV.AddComponent<LineRenderer>();
            lineRendererV.SetPositions(new Vector3[] { P1v, P2v });
            SetLinerenderer(lineRendererV);

            terrainlength = Mathf.Round(ComputeLength(P1, P2)*100)*0.01f;
            terrainHeight = Mathf.Round(ComputeHeight(P1, P2)*100)*0.01f;
            terrainSlope = Mathf.Round(ComputeSlope(P1, P2)*100)*0.01f;
            WriteTerrainInfo();
            //Debug.Log("distance between the points is" + distance.ToString());
            isSecondPtStored = false;
        }
        
    }

    private void SetLinerenderer(LineRenderer myLineR)
    {
        myLineR.material = lineMat;
        myLineR.startWidth = lineWidth;
        myLineR.endWidth = lineWidth;
        myLineR.startColor = Color.red;
        myLineR.endColor = Color.red;
    }
    private void Trial2()
    {
        if (cli == 0)
        {
            cli = 1;
            P1 = GetHitpoint();
            lStart = GetHitpoint();
            return;
        }
        if (cli == 1)
        {
            cli = 2;
            P2 = GetHitpoint();
            Vector3 lEnd = GetHitpoint();
            
            distance = Vector3.Distance(lStart.Value, lEnd);
           // Debug.Log("distance between the points is" + distance.ToString());
            return;
        }
        if(cli==2)
        {
            cli = 0;
            lStart = null;
            return;
        }
        
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


    private bool RayHit()
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out res))
        {
            if (res.collider.gameObject.tag == "Terrain")
            {
                return true;
            }
        }
        return false;
    }

    private RaycastHit GetHit()
    {
        return res;
    }

    private Vector3 GetHitpoint()
    {   
            var hit = GetHit();     
           return hit.point;      
    }

    private void WriteTerrainInfo()
    {
        terrainInfo.text = "Measurements: " + '\n' +
           "Height: " + terrainHeight.ToString() + '\n' +
           "length: " + terrainlength.ToString() + '\n' +
           "Slope: " + terrainSlope.ToString();        
    }
}
