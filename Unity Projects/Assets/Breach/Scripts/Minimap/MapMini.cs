using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMini : MonoBehaviour
{
    public RectTransform playerInMap;
    public RectTransform map2dEnd;
    public Transform map3dParent;
    public Transform map3dEnd;

    private Vector3 normalized, mapped;

    void Start()
    {
       // Debug.Log(playerInMap.localPosition);

    }

    private void Update()
    {
        normalized = Divide(
                map3dParent.InverseTransformPoint(this.transform.position),
                map3dEnd.position - map3dParent.position
            );

       // Debug.Log(normalized);
        normalized.y = normalized.z;
        mapped = Multiply(normalized, map2dEnd.localPosition);
        mapped.z = 0;
        Debug.Log(mapped);
        playerInMap.localPosition = mapped;
        playerInMap.localEulerAngles = new Vector3(0, 0, this.transform.eulerAngles.y);
    }

    private static Vector3 Divide(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    private static Vector3 Multiply(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    private static Vector2 TranslateAndScale(Vector2 a, Vector2 b, Vector2 c) // a = x,y coordinate, b = world coordinate of center of the 3d terrain, c =scale 
    {
        return new Vector3((a.x -0.5f*b.x)*c.x, (a.y - 0.5f*b.y)*c.y);
    }
}