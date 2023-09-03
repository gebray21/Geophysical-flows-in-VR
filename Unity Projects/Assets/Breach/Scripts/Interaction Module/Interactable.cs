using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class Interactable : MonoBehaviour
{
    //public Canvas infoCanvas;
    public GameObject infoCanvas;
    public Transform xRPlayerTransform;
    private XRSimpleInteractable xRSimpleInteractable;
    RectTransform canvasTransform;
    private Camera cam;
    private RaycastHit hit;
    bool isHoverable = false;
   // private XRRayInteractor rayInteractor;
    void Start()
    {
        Physics.IgnoreCollision(xRPlayerTransform.GetComponent<Collider>(), GetComponent<Collider>());
        if (infoCanvas != null)
        {
            canvasTransform = infoCanvas.GetComponent<RectTransform>();
            infoCanvas.SetActive(false);
        }
        xRSimpleInteractable = GetComponent<XRSimpleInteractable>();
        cam = Camera.main;
    }

    private void Update()
    {
      //  if (xRSimpleInteractable.isHovered && infoCanvas != null)
      if(isHoverable)
        {
            infoCanvas.SetActive(true);
            // infoCanvas.transform.LookAt(infoCanvas.transform.position- xRPlayerTransform.position, Vector3.up);

            Vector3 targetdirection = (infoCanvas.transform.position - cam.transform.position).normalized;
            Quaternion lookAtRotation = Quaternion.LookRotation(targetdirection, Vector3.up);
            lookAtRotation = Quaternion.Euler(0f, lookAtRotation.eulerAngles.y, lookAtRotation.eulerAngles.z);
            canvasTransform.rotation = lookAtRotation;         
            canvasTransform.rotation= Quaternion.LookRotation(infoCanvas.transform.position - cam.transform.position, Vector3.up);
            var interactor = xRSimpleInteractable.interactorsHovering[0] as XRRayInteractor;//XRBaseInteractor;
            if (interactor != null)
            {
                if (interactor.TryGetCurrent3DRaycastHit(out hit))
                {
                    Vector3 hitPt = hit.point + Vector3.up * 1.0f;
                    canvasTransform.position = hitPt;
                }
            }
        }

       // Vector3 targetdirection = (RightHandCanvas.transform.position - Player.transform.position).normalized;
      //  RightHandCanvas.transform.rotation = Quaternion.LookRotation(targetdirection, Vector3.up);

    }


    public void HoverOver()
    {
        isHoverable = true;
        /*
        if (infoCanvas != null)
        {
            infoCanvas.SetActive(true);
           // canvasTransform.rotation = xRPlayerTransform.rotation;
            infoCanvas.transform.LookAt(cam.transform.position, Vector3.up);
            // canvasTransform.rotation = Quaternion.LookRotation(canvasTransform.position - xRPlayerTransform.position);
            canvasTransform.position = xRPlayerTransform.position + 2f * xRPlayerTransform.forward + new Vector3(0, 1, 0);
        }
        else return;
        */
    }
    public void HoverEnd()
    {
        isHoverable = false;
        if (infoCanvas != null)
            infoCanvas.SetActive(false);
        else return;
    }
   
}
