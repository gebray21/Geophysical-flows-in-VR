using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOnClick : MonoBehaviour
{
    public Vector3[] postions;
    TerrainCollider terrainCollider;
    Vector3 worldPosition;
    Ray ray;
    
    void Start()
    {
        terrainCollider = Terrain.activeTerrain.GetComponent<TerrainCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        if (terrainCollider.Raycast(ray, out hitData, 1f))
        {
            worldPosition = hitData.point;
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log(worldPosition);
            }
        }
    }
}
