using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoModelScript : MonoBehaviour
{

    [SerializeField] private GameObject LayerInfo;
    [SerializeField] private float moveDistance = 1.5f;
    [SerializeField] private float totalMovementTime = 10f; //the amount of time you want the movement to take
    private Vector3 Origin, Destination;
    void Start()
    {
        Origin = transform.localPosition;
        Destination = Origin + transform.up * moveDistance;
        if (LayerInfo)
        {
            LayerInfo.SetActive(false);
        }
    }

    public void ShowSoilModel()
    {
        StartCoroutine(moveObject());
    }

    private IEnumerator moveObject()
    {
        if (LayerInfo)
        {
            LayerInfo.SetActive(true);
        }
        float currentMovementTime = 0f;//The amount of time that has passed

        while (Vector3.Distance(transform.localPosition, Destination) > 0)
        {
            currentMovementTime += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(Origin, Destination, currentMovementTime / totalMovementTime);
            yield return null;
        }
        
    }

    public void ResetPosition()
    {
        transform.localPosition = Origin;
    }
}
