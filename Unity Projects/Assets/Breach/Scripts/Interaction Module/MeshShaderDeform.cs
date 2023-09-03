using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshShaderDeform : MonoBehaviour
{


    [Header("Deformation Parameters")]
    public float heightMin = 140f;
    public float verticalFactor = 1f;


    [Header("References and Prefabs")]
    public MeshFilter terrainMeshFilter;
    private Mesh terrainMesh;
    private MeshCollider meshCollider;

    //Private Mesh Properties
    private Vector3[] terrainVertices;
    private Vector3[] terrainNormals;
    private Vector3[] Velocity;
    private Vector2[] vel;
    private int numberOfVerts;
    //water simulation
    public float interval = 1f;
    private SimulationManager waterSimulation;
    private float totalTime;
    private float animationTime = 0f;
    public float AnimTime => animationTime;
    void Start()
    {
        InitialiseMeshData();
        waterSimulation = FindObjectOfType<SimulationManager>();
        for (var i = 0; i < numberOfVerts; i++)
        {
            vel[i] = GetUV(terrainVertices[i]);
        }
        terrainMesh.uv3 = vel;
    }

    // Update is called once per frame
    void Update()
    {

        totalTime += Time.deltaTime;
        animationTime = Mathf.Clamp(animationTime, 0f, 1f);
        while (totalTime >= interval)
        {
            for (var i = 0; i < numberOfVerts; i++)
            {
                vel[i] = GetUV(terrainVertices[i]);
            }
            terrainMesh.uv3 = vel;
            //UpdateUV(vel);
            totalTime -= interval;
        }
        Shader.SetGlobalFloat("_AnimationTime", animationTime);
    }

    private void InitialiseMeshData()
    {
        terrainMesh = terrainMeshFilter.mesh;

        //This allows Unity to make background modifications so that it can update the mesh quicker
        terrainMesh.MarkDynamic();
        meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = terrainMesh;
        //The verticies will be reused throughout the life of the program so the Allocator has to be set to Persistent
        terrainVertices = terrainMesh.vertices;
        terrainNormals = terrainMesh.normals;
        
        numberOfVerts = terrainVertices.Length;
        Velocity = new Vector3[numberOfVerts];
        vel = new Vector2[numberOfVerts];
    }

    private void UpdateUV(Vector2[] vel)
    {

                 
        if (vel != null)
            terrainMesh.SetUVs(3, vel);
        // now recalculate bounds & notify users        
        terrainMesh.MarkModified();

        // Make sure data gets uploaded to GPU this frame
        terrainMesh.UploadMeshData(markNoLongerReadable: false);
    }

    private Vector2 GetUV(Vector3 pos)
    {
        Vector3 worldPos= transform.TransformPoint(pos);
        float velMag = GetVelocity(worldPos).magnitude;
      //  Debug.Log("velocity magnitude = " + velMag);
        Vector2 vel2D = new Vector2(velMag, 0f);
        return vel2D;
    }


    private Vector3 GetVelocity(Vector3 position)
    {
        if (waterSimulation == null) return Vector3.zero;

        var velocity = waterSimulation.GetWaterVelocity(position);
        return velocity.HasValue ? new Vector3(velocity.Value.x, 0f, velocity.Value.y) : Vector3.zero;
    }
}
