using UnityEngine;
using UnityEngine.AI;

public class TerrainPainter : MonoBehaviour
{
    public static TerrainPainter Instance { get; set; } 
    public Terrain terrain;

    [Header("Painting")]
    public float paintStepSize = 0.5f;
    public float paintStrength = 0.1f;
    public float paintRadius = 1f;

    [Header("Other")]
    public int grassTextureIndex = 0; // The index of the grass texture
    public int dirtTextureIndex = 1;  // The index of the dirt texture
    private float[,,] originalSplatmap; // Stores the original splatmap

    private void Awake()
    {
        if (Instance == null) Instance = this;
        terrain = Terrain.activeTerrain;
        
        if (terrain != null)
        {
            originalSplatmap = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
        }
    }

    public void ResetSplatmap()
    {
        if (originalSplatmap != null && terrain != null)
        {
            terrain.terrainData.SetAlphamaps(0, 0, originalSplatmap);
        }
    }

    private void Paint(Vector3 worldPosition, float radius, float strength, int targetTexture, int fadeTexture)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        int splatRes = terrainData.alphamapResolution;

        // Convert world position to terrain splatmap coordinates
        int mapX = Mathf.RoundToInt((worldPosition.x - terrainPos.x) / terrainData.size.x * splatRes);
        int mapZ = Mathf.RoundToInt((worldPosition.z - terrainPos.z) / terrainData.size.z * splatRes);
        int mapRadius = Mathf.RoundToInt(radius / terrainData.size.x * splatRes);

        float[,,] splatmap = terrainData.GetAlphamaps(0, 0, splatRes, splatRes);

        for (int x = -mapRadius; x <= mapRadius; x++)
        {
            for (int z = -mapRadius; z <= mapRadius; z++)
            {
                int newX = Mathf.Clamp(mapX + x, 0, splatRes - 1);
                int newZ = Mathf.Clamp(mapZ + z, 0, splatRes - 1);

                float distance = Vector2.Distance(new Vector2(x, z), Vector2.zero) / mapRadius;
                float blend = Mathf.Clamp01(1 - distance) * strength;

                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {
                    if (i == targetTexture)
                        splatmap[newZ, newX, i] += blend; // Increase the target texture
                    else if (i == fadeTexture)
                        splatmap[newZ, newX, i] -= blend; // Decrease the fading texture
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmap);
    }

    public void PaintPathway(NavMeshPath path) 
    {
        if (path == null || path.corners.Length < 2) return;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Vector3 start = path.corners[i];
            Vector3 end = path.corners[i + 1];
            float distance = Vector3.Distance(start, end);
            int steps = Mathf.CeilToInt(distance / paintStepSize);

            for (int j = 0; j < steps; j++)
            {
                Vector3 point = Vector3.Lerp(start, end, j / (float)steps);
                TerrainPainter.Instance.Paint(point, paintRadius, paintStrength, 1, 0);
            }
        }
    }
}
