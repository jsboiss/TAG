using UnityEngine;

[CreateAssetMenu(fileName = "NewBuilding", menuName = "Game Data/Building")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public int woodCost;
    public int stoneCost;
    public int foodCost;
    public int goldCost;
    public float constructionTime;
    public GameObject prefab;
    public float health;
    public int unitsProvided;
    public int productionRate;
    public ResourceTypes productionType;
    public float productionInterval;
}
