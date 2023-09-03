using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCollider : MonoBehaviour
{
    public float movSpeed = 0.1f;
    public float timeInterval=1f;
    //private MeshDeformer meshDeformer;
    private float time;
    private bool isMoved = false;
    private Vector3 initpos;
    private float y;
    void Start()
    {
        initpos = transform.position;
        y = initpos.y;
        time = 0;
        StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    {

        if (!isMoved) {
            time += Time.deltaTime;
         y = transform.position.y;
        Debug.Log(y);
            if (y < -30f)
            {
                StopCoroutine(Move());
                isMoved = true;
            }
        }

    }

    private IEnumerator Move()
    {
        while (y > -30f)
        {
            yield return new WaitForSeconds(timeInterval);
            transform.position -= Vector3.up * (movSpeed * time + timeInterval)*timeInterval;
        }
    }

}