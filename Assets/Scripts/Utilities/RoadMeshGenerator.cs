using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoadMeshGenerator : MonoBehaviour
{
    public float roadWidth = 2f;
    public float heightOffset = 0.1f;
    public Terrain terrain;
    public Material roadMaterial;
    
    public void GenerateRoadMesh(List<Vector3> pathPoints)
    {
        if (pathPoints == null || pathPoints.Count < 2 || terrain == null)
        {
            Debug.LogError("Invalid path or missing terrain reference!");
            return;
        }

        Mesh mesh = new();
        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();

        for (int i = 0; i < pathPoints.Count; i++)
        {
            Vector3 point = pathPoints[i];
            float height = terrain.SampleHeight(point) + heightOffset;
            Vector3 adjustedPoint = new(point.x, height, point.z);
            
            Vector3 forward = (i < pathPoints.Count - 1) ? pathPoints[i + 1] - pathPoints[i] : pathPoints[i] - pathPoints[i - 1];
            forward.Normalize();
            Vector3 right = Vector3.Normalize(Vector3.Cross(Vector3.up, forward)) * (roadWidth * 0.5f);
            
            vertices.Add(adjustedPoint + right);
            vertices.Add(adjustedPoint - right);
            
            uvs.Add(new Vector2(0, i / (float)pathPoints.Count));
            uvs.Add(new Vector2(1, i / (float)pathPoints.Count));
        }
        
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            int index = i * 2;
            triangles.Add(index);
            triangles.Add(index + 1);
            triangles.Add(index + 2);

            triangles.Add(index + 1);
            triangles.Add(index + 3);
            triangles.Add(index + 2);
        }
        
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        meshFilter.mesh = mesh;

        mesh.RecalculateBounds();
        Debug.Log("RoadMeshGenerator Run ++++++++++");

        // Apply the material
        if (roadMaterial != null)
        {
            meshRenderer.material = roadMaterial;
        }
        else
        {
            Debug.LogWarning("No material assigned to road! Assign one in the Inspector.");
        }
    }
}