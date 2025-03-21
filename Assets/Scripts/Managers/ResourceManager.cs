using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static Dictionary<ResourceTypes, int> resources = new Dictionary<ResourceTypes, int>
    {
        { ResourceTypes.Wood, 0 },
        { ResourceTypes.Stone, 0 },
        { ResourceTypes.Food, 0 },
        { ResourceTypes.Gold, 0 }
    };

    public static void AddResource(ResourceTypes resourceType, int amount)
    {
        resources[resourceType] += amount;
    }

    public static void RemoveResource(ResourceTypes resourceType, int amount)
    {
        resources[resourceType] -= amount;
    }

    public static int GetResource(ResourceTypes resourceType)
    {
        return resources[resourceType];
    }

    public static bool CanAfford(int woodCost, int stoneCost, int foodCost, int goldCost)
    {
        return resources[ResourceTypes.Wood] >= woodCost && resources[ResourceTypes.Stone] >= stoneCost && resources[ResourceTypes.Food] >= foodCost && resources[ResourceTypes.Gold] >= goldCost;
    }

    public static void SpendResources(int woodCost, int stoneCost, int foodCost, int goldCost)
    {
        resources[ResourceTypes.Wood] -= woodCost;
        resources[ResourceTypes.Stone] -= stoneCost;
        resources[ResourceTypes.Food] -= foodCost;
        resources[ResourceTypes.Gold] -= goldCost;
    }
}