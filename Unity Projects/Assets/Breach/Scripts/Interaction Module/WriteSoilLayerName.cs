using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using UnityEngine.Events;

public class WriteSoilLayerName : MonoBehaviour
{
    public GameObject LeftHandCanvas;
    public GameObject RightHandCanvas;
    public TMP_Text leftText;
    public TMP_Text rightText;
    public GameObject Player; //for adjusting the direction of the canvases

    // private XRGrabInteractable grabable;
    private XRBaseInteractable grabable;
    private GameObject grabableObject;
    private Material soilMaterial;
    private MeshRenderer meshRenderer;
    private string layerName;
    private XRDirectInteractor rHand;
    private XRDirectInteractor lHand;
    void Awake()
    {
        if (!Player)
            Player = GameObject.FindGameObjectWithTag("Player");
        meshRenderer = GetComponent<MeshRenderer>();
        soilMaterial = meshRenderer.sharedMaterial;
        layerName= soilMaterial.name;
        LeftHandCanvas.SetActive(false);
        RightHandCanvas.SetActive(false);
        grabable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabable.selectEntered.AddListener(SelectEntered);
        grabable.selectExited.AddListener(SelectExited);
    }

    private void OnDisable()
    {
        grabable.selectEntered.RemoveListener(SelectEntered);
        grabable.selectExited.RemoveListener(SelectExited);
    }

    

    public void ShowSoilLayerName(XRGrabInteractable Grabbable)
    //public void ShowSoilLayerName()
    {
       // Grabbable = grabable;
        //grabableObject = grabbable.gameObject;  
        if (!LeftHandCanvas.activeSelf)
        {
           LeftHandCanvas.SetActive(true);
           LeftHandCanvas.transform.position = transform.position+transform.right*0.1f;
            Vector3 targetdirection = (LeftHandCanvas.transform.position - Player.transform.position).normalized;
           LeftHandCanvas.transform.rotation = Quaternion.LookRotation(targetdirection,Vector3.up);
           leftText.text = layerName;
        }
        else
        {
           RightHandCanvas.SetActive(true);
           RightHandCanvas.transform.position = transform.position + transform.right * 0.1f;
            Vector3 targetdirection = (RightHandCanvas.transform.position - Player.transform.position).normalized;
            RightHandCanvas.transform.rotation = Quaternion.LookRotation(targetdirection, Vector3.up);
            rightText.text = layerName;
        }
    }

    public void HideSoilLayerName(XRGrabInteractable Grabbable)
    {
       // Grabbable = grabable;
        if (LeftHandCanvas.activeSelf)
            LeftHandCanvas.SetActive(false);
        if (RightHandCanvas.activeSelf)
            LeftHandCanvas.SetActive(false);
    }

    protected virtual void SelectEntered(SelectEnterEventArgs arg0) => ShowName();
    protected virtual void SelectExited(SelectExitEventArgs arg0) => HideName();

    protected void ShowName()
    {
        if (!LeftHandCanvas.activeSelf)
        {
            LeftHandCanvas.SetActive(true);
            LeftHandCanvas.transform.position = transform.position + transform.right * 0.1f;
            Vector3 targetdirection = (LeftHandCanvas.transform.position - Player.transform.position).normalized;
            Quaternion lookAtRotation = Quaternion.LookRotation(targetdirection, Vector3.up);
            lookAtRotation = Quaternion.Euler(0f, lookAtRotation.eulerAngles.y, lookAtRotation.eulerAngles.z);
            LeftHandCanvas.transform.rotation = lookAtRotation;
            leftText.text = layerName + " Layer";
        }
        else
        {
            RightHandCanvas.SetActive(true);
            RightHandCanvas.transform.position = transform.position + transform.right * 0.1f;
            Vector3 targetdirection = (RightHandCanvas.transform.position - Player.transform.position).normalized;
            Quaternion lookAtRotation = Quaternion.LookRotation(targetdirection, Vector3.up);
            lookAtRotation = Quaternion.Euler(0f, lookAtRotation.eulerAngles.y, lookAtRotation.eulerAngles.z);
            RightHandCanvas.transform.rotation = lookAtRotation;
            rightText.text = layerName + " Layer"; 
        }
    }

    protected void HideName()
    {
        if (LeftHandCanvas.activeSelf)
            LeftHandCanvas.SetActive(false);
        if (RightHandCanvas.activeSelf)
            LeftHandCanvas.SetActive(false);
    }
}
