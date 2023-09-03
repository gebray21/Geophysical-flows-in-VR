
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Breach.WildWaters.WaterSystems
{
    public class MeshDeformation : MonoBehaviour
    {
        public float verticalFactor = 1f;
        public float HeightBelowTerrain = 0.5f;
        private SimulationManager waterSimulation;
        public Vector3 Velocity { get; private set; }
        public float startingTime = 0f;

        [SerializeField] private float interval = 1f;
        Mesh mesh;
        private new MeshCollider collider;
        Vector3[] vertices, displacedVertices;
        Vector3[] normals;
        Vector3 testVect;
        int lens;
        List<int> vertList = new List<int>();
        int vertCount;
        private bool isMovable = true;
        float time;
        float totalTime;

       private Terrain terrain;
        
        // Start is called before the first frame update
        void Start()
        {
            terrain = Terrain.activeTerrain;
            time = 0f;
            totalTime = 0f;
            // getVelocityProvider(this);
            waterSimulation = FindObjectOfType<SimulationManager>();
           // water = FindObjectOfType<Water>();
            mesh = GetComponent<MeshFilter>().mesh;
            collider = GetComponent<MeshCollider>();
            mesh.Optimize();
            vertices = mesh.vertices;
            lens = vertices.Length;

            displacedVertices = new Vector3[lens];
            for (int i = 0; i < lens; i++)
            {
                displacedVertices[i] = vertices[i];
                vertList.Add(i);

             //Debug.Log("the vertices " + i + ": " + transform.TransformPoint(vertices[i]));
            }
            vertCount = vertList.Count;
           // Debug.Log(vertCount);

        // StartCoroutine(UpdateMesh());
        }

        // Update is called once per frame
        void Update()
        {
            totalTime += Time.deltaTime;
            if (totalTime < startingTime)
            {
                return;
            }
            time += Time.deltaTime;
            
            while (time >= interval)
            {
                    MoveMesh();
                    time -= interval;                  
            }

          /*  if (isMovable)
            {
                MoveMesh();
               
                CheckMove();
            }
          
           */

        }
        
        void MoveMesh()
        {

            //vertList.Clear();
            vertList = new List<int>();
            var somethingDeformed = false;
            for (var i = 0; i < lens; i++)
            {
                Vector3 worldPt = transform.TransformPoint(vertices[i]);
                Vector3 Velocity = GetWaterVelocity(worldPt);
                // Debug.Log(" velocity : " + Velocity);

                if (Velocity.magnitude >= 0.05f && worldPt.y > 140f) //(terrain.SampleHeight(worldPt)- HeightBelowTerrain)
                {
                    vertices[i] -= (Velocity.magnitude * Vector3.up * verticalFactor) * interval*interval;
                    vertList.Add(i);
                    somethingDeformed = true;
                }
            }
            vertCount = vertList.Count;
            if (!somethingDeformed)
            {
                return;
            }
            // Debug.Log("the number of movable : " + vertCount);
            // Destroy(GetComponent<MeshCollider>());
            mesh.vertices = vertices;
 
           mesh.RecalculateNormals();
           if(collider!=null) collider.sharedMesh = mesh;
          //  gameObject.AddComponent<MeshCollider>();
        }

        private IEnumerator UpdateMesh()
        {
            while (isMovable)
            {              
                DeformMesh();
                yield return null; 
            }
            yield return new WaitForSeconds(1f); 
        }

        void DeformMesh()
        {
            vertList = new List<int>();
            for (var i = 0; i < lens; i++)
            {
                Vector3 worldPt = transform.TransformPoint(vertices[i]);
                Vector3 Velocity = GetWaterVelocity(worldPt);
                // Debug.Log(" velocity : " + Velocity);

                if (Velocity.magnitude >= 0.05f && worldPt.y > (Terrain.activeTerrain.SampleHeight(worldPt) - HeightBelowTerrain))
                {
                    vertices[i] -= (Velocity.magnitude * Vector3.up * verticalFactor)*Time.deltaTime;
                    vertList.Add(i);
                }
                mesh.vertices = vertices;
                mesh.RecalculateNormals();
            }
        }




        void CheckMove()
        {
            if (vertCount < (int)(0.2f * vertCount))
                isMovable = false;
            else
                isMovable = true;
        }



        private Vector3 GetWaterVelocity(Vector3 position)
        {
            if (waterSimulation == null) return Vector3.zero;

            var velocity = waterSimulation.GetWaterVelocity(position);
            return velocity.HasValue ? new Vector3(velocity.Value.x, 0f, velocity.Value.y) : Vector3.zero;
        }
    }
}