using System.Collections.Generic;
using UnityEngine;

public class Heatmap : MonoBehaviour
{
    public int width = 50;
    public int height = 50;
    public float cellSize = 1f;
    public float decayRate = 5f;
    public float npcHeatAmount = 20f; 
    public float npcUpdateInterval = 0.5f; 
    public float roadThreshold = 50f; 
    public float clusteringDistance = 15f;
    public int splineResolution = 10;

    private float[,] heatmap;

    private void Start()
    {
        heatmap = new float[width, height];
        InvokeRepeating(nameof(UpdateNPCHeat), 0, npcUpdateInterval);
        //InvokeRepeating(nameof(GenerateRoads), 0, 5f);
    }

    private void Update()
    {
        DecayHeatmap();
    }

    private void DecayHeatmap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                heatmap[x, z] = Mathf.Max(0, heatmap[x, z] - decayRate * Time.deltaTime);
            }
        }
    }

    public void AddHeat(Vector3 position, float amount)
    {
        int x = Mathf.FloorToInt(position.x / cellSize);
        int z = Mathf.FloorToInt(position.z / cellSize);
        
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            heatmap[x, z] += amount;
        }
    }

    private void UpdateNPCHeat()
    {
        //Debug.Log($"🟡 NPC Position: {npc.transform.position}");
        foreach (var unit in PopulationManager.Instance.units) {
            AddHeat(unit.transform.position, npcHeatAmount);
        }
    }

    public List<Vector3> GetHighHeatPoints()
    {
        List<Vector3> highHeatPoints = new();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (heatmap[x, z] >= roadThreshold)
                {
                    highHeatPoints.Add(new Vector3(x * cellSize, 0, z * cellSize));
                }
            }
        }
        return highHeatPoints;
    }

    public List<List<Vector3>> ClusterHighHeatPoints()
    {
        List<Vector3> points = GetHighHeatPoints();
        List<List<Vector3>> clusters = new();

        while (points.Count > 0)
        {
            List<Vector3> cluster = new();
            Vector3 seed = points[0];
            cluster.Add(seed);
            points.RemoveAt(0);

            for (int i = points.Count - 1; i >= 0; i--)
            {
                if (Vector3.Distance(seed, points[i]) <= clusteringDistance)
                {
                    cluster.Add(points[i]);
                    points.RemoveAt(i);
                }
            }

            // Sort the cluster into a continuous path
            cluster = SortPointsIntoPath(cluster);

            clusters.Add(cluster);
        }
        return clusters;
    }

    private List<Vector3> SortPointsIntoPath(List<Vector3> points)
    {
        if (points.Count < 2) return points;

        List<Vector3> sorted = new();
        Vector3 current = points[0];
        sorted.Add(current);
        points.Remove(current);

        while (points.Count > 0)
        {
            Vector3 next = FindNearestPoint(current, points);
            sorted.Add(next);
            points.Remove(next);
            current = next;
        }

        return sorted;
    }

    private Vector3 FindNearestPoint(Vector3 from, List<Vector3> points)
    {
        Vector3 nearest = points[0];
        float minDist = Vector3.Distance(from, nearest);

        foreach (var point in points)
        {
            float dist = Vector3.Distance(from, point);
            if (dist < minDist)
            {
                nearest = point;
                minDist = dist;
            }
        }

        return nearest;
    }

    public List<Vector3> GenerateSplinePath(List<Vector3> points)
    {
        List<Vector3> splinePath = new();
        if (points.Count < 2) return splinePath;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p0 = (i == 0) ? points[i] : points[i - 1];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = (i + 2 < points.Count) ? points[i + 2] : points[i + 1];
            for (int t = 0; t < splineResolution; t++)
            {
                float s = t / (float)splineResolution;
                Vector3 interpolated = 0.5f * (
                    2 * p1 +
                    (-p0 + p2) * s +
                    (2 * p0 - 5 * p1 + 4 * p2 - p3) * (s * s) +
                    (-p0 + 3 * p1 - 3 * p2 + p3) * (s * s * s)
                );
                splinePath.Add(interpolated);
            }
        }
        return splinePath;
    }

    public void GenerateRoads()
    {
        RoadMeshGenerator roadMeshGenerator = FindFirstObjectByType<RoadMeshGenerator>();
        if (roadMeshGenerator == null)
        {
            Debug.LogError("No RoadMeshGenerator found in the scene!");
            return;
        }

        List<List<Vector3>> clusters = ClusterHighHeatPoints();
        foreach (var cluster in clusters)
        {
            List<Vector3> splinePath = GenerateSplinePath(cluster);
            roadMeshGenerator.GenerateRoadMesh(splinePath);
        }
    }

    private void OnDrawGizmos()
    {
        if (heatmap == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float intensity = heatmap[x, z] / 100f; // Normalize for color
                Gizmos.color = new Color(intensity, 0, 0, intensity);
                Vector3 pos = new Vector3(x * cellSize, 0, z * cellSize);
                Gizmos.DrawCube(pos, Vector3.one * (cellSize * 0.9f));
            }
        }

        // Draw spline paths
        var clusters = ClusterHighHeatPoints();
        Gizmos.color = Color.yellow;
        foreach (var cluster in clusters)
        {
            List<Vector3> splinePath = GenerateSplinePath(cluster);
            for (int i = 0; i < splinePath.Count - 1; i++)
            {
                Gizmos.DrawLine(splinePath[i], splinePath[i + 1]);
            }
        }
    }
}
