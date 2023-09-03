using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class InteractableGameObjectName : MonoBehaviour
{
    // private XRGrabInteractable grabable;
    public GameObject Player; //for adjusting the direction of the canvases
  
    private XRBaseInteractable grabable;
    private GameObject grabableObject;
    private Material soilMaterial;
    private MeshRenderer meshRenderer;
    private string layerName;
    public GameObject LeftHandCanvas;
    public TMP_Text leftText;
    private IXRSelectInteractor leftHand, rightHand;
    private Camera cam;
    void Awake()
    {
        if (!Player)
            Player = GameObject.FindGameObjectWithTag("Player");
        cam = Camera.main;
        grabable = GetComponent<XRGrabInteractable>();
        grabableObject = grabable.gameObject;
        meshRenderer = GetComponent<MeshRenderer>();
        soilMaterial = meshRenderer.sharedMaterial;
        layerName = soilMaterial.name;
        LeftHandCanvas.SetActive(false);
       

    }

    // Update is called once per frame
    void Update()
    {
        var interactor = grabable.GetOldestInteractorSelecting();
       if (grabable.isSelected)
      // if(grabable.IsSelectableBy(leftHand)|| grabable.IsSelectableBy(rightHand))
        {
            LeftHandCanvas.SetActive(true);
            LeftHandCanvas.transform.position = transform.position + transform.right * 0.1f-transform.forward*0.1f;
           // Vector3 targetdirection = (LeftHandCanvas.transform.position - Player.transform.position).normalized;
            Vector3 targetdirection = (LeftHandCanvas.transform.position - cam.transform.position).normalized;
            Quaternion lookAtRotation = Quaternion.LookRotation(targetdirection, Vector3.up);
            lookAtRotation = Quaternion.Euler(0f, lookAtRotation.eulerAngles.y, lookAtRotation.eulerAngles.z);
            LeftHandCanvas.transform.rotation = lookAtRotation;
            LeftHandCanvas.transform.rotation = Quaternion.LookRotation(LeftHandCanvas.transform.position - cam.transform.position, Vector3.up);
            leftText.text = layerName + " Layer";                     
        }
       
    }
}
