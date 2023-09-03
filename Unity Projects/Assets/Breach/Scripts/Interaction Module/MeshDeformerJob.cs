using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public struct MeshDeformerJob : IJobParallelFor
{
    [ReadOnly] public float deltaTime;
    [ReadOnly] public Vector3 velocity;
    [ReadOnly] public float heightDiff;
    [ReadOnly] public float heightToDeform;
    [ReadOnly] public float force;
    [ReadOnly] public NativeArray<Vector3> normals;
     public NativeArray<Vector3> vertices;

    public void Execute(int index)
    {
        // Due to the lack of ref returns, it is not possible to directly change the content of a NativeContainer.
        // For example, nativeArray[0]++; is the same as writing var temp = nativeArray[0]; temp++; which does not update the value in nativeArray.
        // Instead, you must copy the data from the index into a local temporary copy, modify that copy, and save it back.
        Vector3 vertex = vertices[index];
        // Check if the vertex is has velocity
        float a = velocity.magnitude;
        if (a >=0.05f && heightToDeform > heightDiff)
        {
            vertex += normals[index] * force * deltaTime;

            // Save it back.
            vertices[index] = vertex;
        }
    }
}

