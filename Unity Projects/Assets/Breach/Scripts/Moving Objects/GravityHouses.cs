using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityHouses : MonoBehaviour
{
    public float movSpeed=0.1f;
    private float timeinterval;
    private MeshDeformer meshDeformer;
    private float time;
    public Transform[] gravityHouses;
    void Start()
    {
        timeinterval = meshDeformer.interval;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
