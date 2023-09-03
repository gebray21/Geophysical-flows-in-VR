using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using System.Threading.Tasks;


public class MeshDeformer : MonoBehaviour
{

    [Header("Deformation Parameters")]
    public float heightMin = 140f;
    public float verticalFactor = 1f;


    [Header("References and Prefabs")]
    public MeshFilter terrainMeshFilter;
    private Mesh terrainMesh;
    private MeshCollider meshCollider;

    //Private Mesh Job Properties
    NativeArray<Vector3> terrainVertices;
    NativeArray<Vector3> terrainNormals;
    NativeArray<Vector3> Velocity;

    //Job Handles
    UpdateMeshJob meshDeformationJob;
    JobHandle meshDeformationJobHandle;
    private Vector3[] vertPos;
    private int numberOfVerts;
    //water simulation
    public float interval = 1f;
    private SimulationManager waterSimulation;
    private float totalTime;

    private void Start()
    {
        InitialiseMeshData();
        waterSimulation = FindObjectOfType<SimulationManager>();
    }

    private void InitialiseMeshData()
    {
        terrainMesh = terrainMeshFilter.mesh;

        //This allows Unity to make background modifications so that it can update the mesh quicker
        terrainMesh.MarkDynamic();
        meshCollider = gameObject.GetComponent<MeshCollider>();

        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = terrainMesh;
        }
        //The verticies will be reused throughout the life of the program so the Allocator has to be set to Persistent
        terrainVertices = new NativeArray<Vector3>(terrainMesh.vertices, Allocator.Persistent);
        terrainNormals = new NativeArray<Vector3>(terrainMesh.normals, Allocator.Persistent);
        vertPos = terrainMesh.vertices;
        numberOfVerts = vertPos.Length;
        Velocity = new NativeArray<Vector3>(numberOfVerts, Allocator.Persistent);        
    }



    private void Update()
    {
        totalTime += Time.deltaTime;
        while (totalTime >= interval)
        {
            for (var i = 0; i < numberOfVerts; i++)
            {
                Vector3 worldPt = transform.TransformPoint(vertPos[i]);
                Velocity[i] = GetVelocity(worldPt);
            }
            totalTime -= interval;
        }

            //Creating a job and assigning the variables within the Job
            meshDeformationJob = new UpdateMeshJob()
            {
                vertices = terrainVertices,
                normals = terrainNormals,
                velocity = Velocity,
                hMin = heightMin,
                //time = Time.time,
                time = Time.deltaTime,
                force = verticalFactor
            };

            //Setup of the job handle
            meshDeformationJobHandle = meshDeformationJob.Schedule(terrainVertices.Length, 64);
           
    }    




    private void LateUpdate()
    {
        //Ensuring the completion of the job
        meshDeformationJobHandle.Complete();
        
            

            //Set the vertices directly
            terrainMesh.SetVertices(meshDeformationJob.vertices);

        //Most expensive
        terrainMesh.RecalculateBounds();
        terrainMesh.RecalculateNormals();
       // meshCollider.enabled = false;
       // meshCollider.enabled = true;
    }

    private void OnDestroy()
    {
        // make sure to Dispose any NativeArrays when you're done
        terrainVertices.Dispose();
        terrainNormals.Dispose();
        Velocity.Dispose();
    }



    [BurstCompile]
    private struct UpdateMeshJob : IJobParallelFor
    {
        [ReadOnly] public float time;
        [ReadOnly] public float hMin;
        [ReadOnly] public float force;
        [ReadOnly] public NativeArray<Vector3> normals;
        [ReadOnly] public NativeArray<Vector3> velocity;
       // public object simulation;
        public NativeArray<Vector3> vertices;

       // public SimulationManager flowSimulation { get; private set; }

        public void Execute(int i)
        {
            var vertex = vertices[i];
            var vel = velocity[i];
            if (vel.magnitude >= 0.05f && vertex.y > hMin)
            {
                vertex -= (vel.magnitude * Vector3.up * force) * time;
                vertices[i] = vertex;
            }


            /*
            var num = 1 - vertex.x / 484f - vertex.z / 1264f;
            var velMag = force*Mathf.Pow(num,time);   
            if (vertex.y> hMin )
            {
                vertex -= Vector3.up* velMag;

                // Save it back.
                vertices[i] = vertex;
            }
           
             */
        }
      
        /*
        private Vector3 FlowVelocity(Vector3 pos)
        {
            if (simulation == null) return Vector3.zero;

            var velocity = simulation.GetWaterVelocity(pos);
            return velocity.HasValue ? new Vector3(velocity.Value.x, 0f, velocity.Value.y) : Vector3.zero;
        }
        */
    }

    private Vector3 GetVelocity(Vector3 position)
    {
        if (waterSimulation == null) return Vector3.zero;

        var velocity = waterSimulation.GetWaterVelocity(position);
        return velocity.HasValue ? new Vector3(velocity.Value.x, 0f, velocity.Value.y) : Vector3.zero;
    }

}



                