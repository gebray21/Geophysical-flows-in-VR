using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAt : MonoBehaviour
{
    public GameObject infoCanvas;
    public GameObject Player;
    public float distanceFromCamera = 1.5f;
    public float offDistance = 0f;
    public float heightToCamera = 0.5f;
    RectTransform canvasTransform;
    private Camera cam;
    private Transform objectToFollow;
    bool isVisible =false;
    public bool visibleFromStart=false;
    public bool followCamera = false;
    void Start()
    {
        cam = Camera.main;
        if (!Player)
            Player = GameObject.FindGameObjectWithTag("Player");
        if (followCamera)
        {
            objectToFollow = cam.transform;
        }
        else objectToFollow = Player.transform;

        if (infoCanvas != null)
        {
            canvasTransform = infoCanvas.GetComponent<RectTransform>();
            infoCanvas.SetActive(visibleFromStart);
            LookAtCameraStart();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isVisible)
        {
            infoCanvas.SetActive(true);

           // LookAtCameraStart();
        }
        LookAtCameraStart();
    }
    private void LookAtCameraStart()
    {
        Vector3 targetdirection = (infoCanvas.transform.position - objectToFollow.position).normalized;
        Quaternion lookAtRotation = Quaternion.LookRotation(targetdirection, Vector3.up);
        lookAtRotation = Quaternion.Euler(20f, lookAtRotation.eulerAngles.y, lookAtRotation.eulerAngles.z);
        canvasTransform.rotation = lookAtRotation;
        // canvasTransform.rotation = Quaternion.LookRotation(infoCanvas.transform.position - objectToFollow.position, Vector3.up);
        canvasTransform.position = objectToFollow.position + distanceFromCamera * objectToFollow.forward + objectToFollow.up * heightToCamera + objectToFollow.right * offDistance;
    }
    private void LookAtCameraStart1()
    {
        canvasTransform.position = objectToFollow.position + objectToFollow.forward * distanceFromCamera;
        canvasTransform.LookAt(objectToFollow);
    }
    public void LookAtCameraOn()
    {
        isVisible = true;
    }
    public void LookAtCameraOff()
    {
        isVisible = false;
        if (infoCanvas != null)
            infoCanvas.SetActive(false);
        else return;
    }

    
     
    
}