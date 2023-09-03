using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoreHoleScript : MonoBehaviour
{
    //[SerializeField] private Transform player;
    [SerializeField] private Transform BoreHole;
    [SerializeField] private GameObject boreHoleInfo;
    private Vector3 Origin, Destination;
    [SerializeField] private float boreHoleHeight = 1.9f;
    private float totalMovementTime = 10f; //the amount of time you want the movement to take
    void Start()
    {
        Origin = transform.position;
        Destination = Origin + transform.up * boreHoleHeight;
        if (boreHoleInfo)
        {
            boreHoleInfo.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowSoilLayer()
    {
        StartCoroutine(moveObject());
    }
   
    private IEnumerator moveObject()
    {
        
        float currentMovementTime = 0f;//The amount of time that has passed
        while (Vector3.Distance(transform.localPosition, Destination) > 0)
        {
            currentMovementTime += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(Origin, Destination, currentMovementTime / totalMovementTime);
            yield return null;
        }
        if (boreHoleInfo)
        {
            boreHoleInfo.SetActive(true);
        }       
    }

    public void ResetPosition()
    {
        transform.position=Origin;
    }
}
