using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingComponents : MonoBehaviour
{
    private SimulationManager waterSimulation;
    private Vector3 Velocity;
    private GameObject[] houseParts;
    private Rigidbody[] rigidBodies;
    private Vector3 pos;
    private bool isDemolished = false;
    int numparts;
    private Terrain terrain;

    private void Awake()
    {
        
    }

    
    void Start()
    {
        pos = transform.position;
        List<GameObject> parts = new List<GameObject>();
        List<Rigidbody> bodies = new List<Rigidbody>();
        waterSimulation = FindObjectOfType<SimulationManager>();
        terrain = Terrain.activeTerrain;
        for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i);
            if (c.gameObject.activeSelf)
            {
                var rb = transform.GetChild(i).GetComponent<Rigidbody>();
                rb.isKinematic = true;
                parts.Add(c.gameObject);
                bodies.Add(rb);
            }
            houseParts = parts.ToArray();
            rigidBodies = bodies.ToArray();
        }
        numparts = houseParts.Length;

        //Demolish();
    }

    private void Update()
    {
        // Demolish();
        // if(!isDemolished)
        //{
        Vector3 Velocity = GetVelocity(pos);
        if (Velocity.magnitude > 0.0001f)
        {
            for (int i = 0; i < numparts; i++)
            {
              if (houseParts[i].transform.position.y>140f) //terrain.SampleHeight(houseParts[i].transform.position)
                    rigidBodies[i].isKinematic = false;
              else
                    rigidBodies[i].isKinematic = true;
            }
        }
            //isDemolished = true;
        //}
    }

    public void Demolish()
    {
        StartCoroutine(DoDemolish());
    }

    private IEnumerator DoDemolish()
    {
        foreach (var h in houseParts)
        {
            //Vector3 worldPt = transform.TransformPoint(h.transform.position);
            //Debug.Log("pos " + h.transform.position);
            Vector3 Velocity = GetVelocity(pos);
            if (Velocity.magnitude > 0.0001f)
            {
                h.GetComponent<Rigidbody>().isKinematic = false;
            }
            yield return null;
        }
    }

    private Vector3 GetVelocity(Vector3 position)
    {
        if (waterSimulation == null) return Vector3.zero;

        var velocity = waterSimulation.GetWaterVelocity(position);
        return velocity.HasValue ? new Vector3(velocity.Value.x, 0f, velocity.Value.y) : Vector3.zero;
    }
}
