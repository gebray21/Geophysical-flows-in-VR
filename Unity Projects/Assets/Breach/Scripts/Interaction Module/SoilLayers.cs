using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;


public class SoilLayers : MonoBehaviour
{
   // public Transform layerParent;
    public Transform layer;
    public XRRayInteractor rayInteractor; //right or left hand
    public float speed = 5f;
     Vector3 Origin;
    Vector3 Destination;
    RaycastHit res;
    bool isPressed=false;
    public GameObject layerInfoCanvas;
    void Start()
    {
        layerInfoCanvas.SetActive(false);
    }


    void Update()
    {
        /*
        if(isPressed)
        {
            var isHit = RayHit();
            if (isHit)
            {
                var startTime = Time.time;
                var hit = GetHit();
                originPosition = hit.transform.position;
                transform.position = Vector3.Lerp(originPosition, originPosition + 25f * Vector3.up, speed*Time.deltaTime);
               // Debug.Log("Clicked on " + originPosition);
            }
           // isPressed = false;
        } */
    }

    private bool RayHit()
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out res))
        {
            if (res.collider.gameObject.tag == "SoilLayer")
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

    IEnumerator MoveLayer(Vector3 target, float Duration)
    {
        var startTime = Time.time;
        var t = 0f;
        var isHit = RayHit();
        var hit = GetHit();
       var originPosition = hit.transform.position;
        var newTransform = hit.transform;
        while (t <= 1 && isHit)
        {
            layerInfoCanvas.SetActive(true);
            layerInfoCanvas.transform.rotation = rayInteractor.transform.rotation;
            layerInfoCanvas.transform.position = rayInteractor.transform.position+ (2f * rayInteractor.transform.forward) + new Vector3(0, 1, 0);
            yield return new WaitForSeconds(0.5f);
            t = (Time.time - startTime) / Duration;
            newTransform.position = Vector3.Lerp(originPosition, target, t);
            transform.position = newTransform.position;
            yield return null;
        }
        
    }

    public void SelectOn()
    {
        // isPressed= true;
       // var destination = transform.position + 25f * Vector3.up;
       // StartCoroutine(MoveLayer(destination, 5f));
        ON();
    }

    // method to be called on selectExit
    public void SelectOff()
    {
        // isPressed = false;
       // var destination = transform.position + 25f * Vector3.up;
       // StopCoroutine(MoveLayer(destination, 1f));
        // layerInfoCanvas.SetActive(false);
        OFF();
    }

   private void ON()
    {
        if (layerInfoCanvas != null)
        {
            layerInfoCanvas.SetActive(true);
            layerInfoCanvas.transform.rotation = rayInteractor.transform.rotation;
            layerInfoCanvas.transform.position = rayInteractor.transform.position + (2f * rayInteractor.transform.forward) + new Vector3(0, 1, 0);
        }
        else return;
    }
    private void OFF()
    {
        if (layerInfoCanvas != null)
            layerInfoCanvas.SetActive(false);
        else return;
    }

    public IEnumerator moveObject()
    {
        float totalMovementTime = 5f; //the amount of time you want the movement to take
        float currentMovementTime = 0f;//The amount of time that has passed
     
        while (Vector3.Distance(transform.localPosition, Destination) > 0)
        {
            currentMovementTime += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(Origin, Destination, currentMovementTime / totalMovementTime);
            yield return null;
        }
    }
}
