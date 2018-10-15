using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkewModel : MonoBehaviour {
    public Vector3 skewDirection;
    public Vector3 skewPosition;

    Vector3[] startVertices;
    Vector3[] vertices;
    
    private MeshFilter meshFilter;
    private Mesh mesh;
    
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = Util.CopyMesh(meshFilter.mesh);
        startVertices = mesh.vertices;
        vertices = startVertices;
    }

    void Start () {
        mesh.vertices = startVertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = startVertices[i] + (skewDirection * startVertices[i].y);
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        vertices = startVertices;

        meshFilter.mesh = mesh;
    }
}
