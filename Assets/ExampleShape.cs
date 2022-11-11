using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ExampleShape : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public Color color; 
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateShape()
    {
        vertices = new Vector3[]
        {
            new Vector3(1,0,0),
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0)
        };

        triangles = new int[]
        {
            0,1,2
        };


        GetComponent<MeshRenderer>().material.color = color;
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}


