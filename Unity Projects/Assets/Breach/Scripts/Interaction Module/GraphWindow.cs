using Breach;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphWindow : MonoBehaviour
{
    
    public float updateInterval = 5.0f;
    public Transform bridgePier;
    public Sprite circleSprite;
    //public Transform parentTransform;
    public RectTransform graphContainer;
    public RectTransform dashContainer;
    public RectTransform labelXTemplate;
    public RectTransform labelYTemplate;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;

    private float timer;
    private float timeValue;
    private float flowDepth;
    private SimulationManager waterSimulation;
    private Vector3 flowVelocity;

    private float yMax = 6f;
    private float xMax = 300f;
    private float graphHeight;
    private float graphWidth;

    void Start()
    {
        //graphContainer = transform.Find("Container").GetComponent<RectTransform>();
        dashTemplateX = dashContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = dashContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        waterSimulation = FindObjectOfType<SimulationManager>();
        flowVelocity = Vector3.zero;
        flowDepth= 0;
        timer = 0f;
        timeValue= 0f;
        graphHeight = graphContainer.sizeDelta.y;
        graphWidth = graphContainer.sizeDelta.x;

        labelXis(11, labelXTemplate,-5f);
        labelYis(10, labelYTemplate,-5f);

    }

    void Update()
    {
        timer += Time.deltaTime;
        timeValue+= Time.deltaTime;

        if (timer >= updateInterval)
        {
            timer = 0f;
            drawGraph(timeValue);
            
        }

        
    }


    private void createCircle(Vector2 anchorposition)
    {
        GameObject gameObject= new GameObject("circle",typeof(Image));
        gameObject.transform.SetParent(graphContainer,false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        
        RectTransform rectTransform= gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition=anchorposition;
        rectTransform.sizeDelta = new Vector2(1.005f, 1.005f);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
    }
    private void drawGraph(float t)
    {
       
        flowVelocity = GetFlowVelocity(bridgePier.position);
        flowDepth = GetFlowHeight(bridgePier.position);
        float yPos = (flowDepth / yMax) * graphHeight;
        float xPos = (t / xMax) * graphWidth;
        createCircle(new Vector2(xPos, yPos));
        
    }
    private void labelXis(int n,RectTransform rect,float yLocation)
    {
        float dX = xMax/(n-1);
        float yScale = graphHeight / yMax;
        for (int i=0; i<n; i++)
        {
            float xPosition = graphWidth * i/(n);
            RectTransform labelXY = Instantiate(rect);
            labelXY.SetParent(graphContainer,false);
            labelXY.gameObject.SetActive(true);
            labelXY.anchoredPosition = new Vector2(xPosition, yLocation);
            //float zValue = -parentTransform.position.z;
           // labelXY.gameObject.transform.localPosition = new Vector3(labelXY.gameObject.transform.localPosition.x, xlabelXY.gameObject.transform.localPosition.y, zValue);
            labelXY.GetComponent<TextMeshPro>().text=(dX*i).ToString();
            // Duplicate the x dash template
            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(dashContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition-graphWidth/2, 0);
        }

    }

    private void labelYis(int m, RectTransform rectY, float xLocation)
    {
        float yMin = 0f;
        float dY = yMax/(m-1);
        for (int i = 0; i < m; i++)
        {
            // Duplicate the label template
            RectTransform labelY = Instantiate(rectY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / m;
            labelY.anchoredPosition = new Vector2(xLocation, normalizedValue * graphHeight);
            float yValue = yMin + (normalizedValue * (yMax - yMin));
            labelY.GetComponent<TextMeshPro>().text = yValue.ToString();
            // Duplicate the dash template
            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(dashContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(0f, normalizedValue * graphHeight- graphHeight/2);

        }
    }

    private Vector3 GetFlowVelocity(Vector3 position)
    {
        if (waterSimulation == null) return Vector3.zero;

        var velocity = waterSimulation.GetWaterVelocity(position);
        return velocity.HasValue ? new Vector3(velocity.Value.x, 0f, velocity.Value.y) : Vector3.zero;
    }

    private float GetFlowVelocityMagnitude(Vector3 position, float time)
    {
        if (waterSimulation == null) return 0f;

        var velocity = waterSimulation.GetWaterVelocity(position + flowVelocity * time);
        return velocity.HasValue ? velocity.Value.magnitude : 0f;
    }

    private float GetFlowHeight(Vector3 position)
    {
        if (waterSimulation == null) return 0.0f;

        var elevation = waterSimulation.GetWaterHeightAt(position);
        // return elevation.HasValue ? (elevation.Value - position.y): 0.0f;

        return elevation.HasValue ? ((elevation.Value - position.y) > 0.0f ? (elevation.Value - position.y) : 0.0f) : 0.0f;
    }


}
