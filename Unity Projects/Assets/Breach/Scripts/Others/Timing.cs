using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timing : MonoBehaviour
{
    public float maxTime =220f; //220 seconds 
    public GameObject ScreenCover;
    [SerializeField] float distanceFromCamera = 2f;
    RectTransform canvasTransform;
    private float timeCount;
    private Camera cam;
    void Start()
    {
        ScreenCover.SetActive(false);
        canvasTransform = ScreenCover.GetComponent<RectTransform>();
        cam = Camera.main;
        timeCount = 0f; 
    }

    void Update()
    {
        timeCount += Time.deltaTime;
        if (timeCount > maxTime)
        {
            ScreenCover.SetActive(true);
            canvasTransform.position = cam.transform.position + cam.transform.forward * distanceFromCamera;
            Vector3 targetdirection = (canvasTransform.position - cam.transform.position).normalized;
            Quaternion lookAtRotation = Quaternion.LookRotation(targetdirection, Vector3.up);
            lookAtRotation = Quaternion.Euler(0f, lookAtRotation.eulerAngles.y, lookAtRotation.eulerAngles.z);
            canvasTransform.rotation = lookAtRotation;
            canvasTransform.rotation = Quaternion.LookRotation(canvasTransform.position - cam.transform.position, Vector3.up);
        }
        
    }
}
