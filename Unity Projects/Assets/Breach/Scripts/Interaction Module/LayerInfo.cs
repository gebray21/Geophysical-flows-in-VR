using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerInfo : MonoBehaviour
{
    [SerializeField] private GameObject soilLayersContainer;
    [SerializeField] private Transform playerTransform;
    
    [SerializeField] private float scale=0.5f;
    [SerializeField] private float verticalPos=10f;
    private bool isShown = false;

    void Start()
    {
        soilLayersContainer.SetActive(false);
    }

   
    void Update()
    {
        
    }

    public void ShowLayers()
    {
        if (isShown == false)
        {
            StartCoroutine(MoveSoilLayer());
            isShown = true;
        }
        else soilLayersContainer.SetActive(false);

    }

    IEnumerator MoveSoilLayer()
    {
        soilLayersContainer.SetActive(true);
        // soilLayersContainer.transform.position += Vector3.up * 20f;
        //yield return new WaitForSeconds(0.5f);
        soilLayersContainer.transform.localScale = Vector3.one * scale;
        var layerPos = playerTransform.position;
        soilLayersContainer.transform.rotation = playerTransform.rotation;
        soilLayersContainer.transform.position = layerPos + 2f * playerTransform.forward + new Vector3(0, 1, 0)*verticalPos;
       // yield return new WaitForSeconds(0.5f);
        
       yield return null;
    }

    
}
