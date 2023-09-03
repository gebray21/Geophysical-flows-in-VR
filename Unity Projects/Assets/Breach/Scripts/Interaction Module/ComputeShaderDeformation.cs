using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ComputeShaderDeformation : MonoBehaviour
{

    public struct VertexData
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;
       // public float velocity;
    }
    // deformation input

      public float verticalFactor = 1f;
    //The mesh
    private Mesh mesh;

    //Compute shader
    [SerializeField] private ComputeShader computeShader;
    private int _kernel;
    private int dispatchCount = 0;
    private ComputeBuffer computeBuffer;
    private NativeArray<VertexData> vertexData;
    private AsyncGPUReadbackRequest request;
    private MeshCollider meshCollider;
    private bool isDispatched;
    //simulation 
    private SimulationManager waterSimulation;

    private void Awake()
    {
        if (!SystemInfo.supportsAsyncGPUReadback)
        {
            gameObject.SetActive(false);
            return;
        }
        var meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        mesh = meshFilter.mesh;

        SetKernel();
        CreateVertexData();
        SetMeshVertexBufferParams();
        computeBuffer = CreateComputeBuffer();
        SetComputeShaderValues();
    }

    private void Update()
    {
        Dispatch();
    }

    private void LateUpdate()
    {
        GatherResult();
    }


    private void SetKernel()
    {
        _kernel = computeShader.FindKernel("CSMain");
        computeShader.GetKernelThreadGroupSizes(_kernel, out var threadX, out _, out _);
        dispatchCount = Mathf.CeilToInt(mesh.vertexCount / threadX + 1);
    }


    private void CreateVertexData()
    {
        var meshVertexCount = mesh.vertexCount;
        vertexData = new NativeArray<VertexData>(meshVertexCount, Allocator.Temp);
        var meshVertices = mesh.vertices;
        var meshNormals = mesh.normals;
        var meshUV = mesh.uv;
        for (var i = 0; i < meshVertexCount; ++i)
        {
            var v = new VertexData
            {
                position = meshVertices[i],
                normal = meshNormals[i],
                uv = meshUV[i],
               // velocity = 0.1f
        };
            vertexData[i] = v;
        }
    }

    private void SetMeshVertexBufferParams()
    {
        var layout = new[]
        {
                new VertexAttributeDescriptor(VertexAttribute.Position, mesh.GetVertexAttributeFormat(VertexAttribute.Position), 3),
                new VertexAttributeDescriptor(VertexAttribute.Normal, mesh.GetVertexAttributeFormat(VertexAttribute.Normal), 3),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, mesh.GetVertexAttributeFormat(VertexAttribute.TexCoord0), 2),
            };
        mesh.SetVertexBufferParams(mesh.vertexCount, layout);
    }

    private void SetComputeShaderValues()
    {
        computeShader.SetBuffer(_kernel, "vertexBuffer", computeBuffer);
        computeShader.SetFloat("_Force", verticalFactor);    
    }

    private ComputeBuffer CreateComputeBuffer()
    {
        var computeBuffer = new ComputeBuffer(mesh.vertexCount, 32);
        if (vertexData.IsCreated)
        {
            computeBuffer.SetData(vertexData);
        }

        return computeBuffer;
    }

    private void Dispatch()
    {
        
        isDispatched = true;

        computeShader.SetFloat("_Time", Time.time);
        computeShader.Dispatch(_kernel, dispatchCount, 1, 1);
        request = AsyncGPUReadback.Request(computeBuffer);
    }

    private void GatherResult()
    {
        if (!isDispatched || !request.done || request.hasError)
        {
            return;
        }

        isDispatched = false;
        vertexData = request.GetData<VertexData>();

        mesh.MarkDynamic();
        mesh.SetVertexBufferData(vertexData, 0, 0, vertexData.Length);
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;

        request = AsyncGPUReadback.Request(computeBuffer);
    }

    private void OnDestroy()
    {
        CleanUp();
    }
    private void CleanUp()
    {
        computeBuffer?.Release();
    }

    

}


