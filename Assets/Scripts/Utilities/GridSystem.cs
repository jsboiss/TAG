using UnityEngine;

public static class GridSystem
{
    public static Vector3 SnapToGrid(Vector3 position, float gridSize)
    {
        return new Vector3(
            Mathf.Round(position.x / gridSize) * gridSize,
            position.y,
            Mathf.Round(position.z / gridSize) * gridSize
        );
    }

    public static void DrawGrid(float width, float height, float gridSize)
    {
        for (float x = -width / 2; x < width / 2; x += gridSize)
        {
            Debug.DrawLine(new Vector3(x, 0, -height / 2), new Vector3(x, 0, height / 2), Color.gray);
        }

        for (float z = -height / 2; z < height / 2; z += gridSize)
        {
            Debug.DrawLine(new Vector3(-width / 2, 0, z), new Vector3(width / 2, 0, z), Color.gray);
        }
    }
}