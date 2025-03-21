using System;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance { get; private set; }
    public List<UnitController> units = new();
    public GameObject unitPrefab;

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

    public UnitController AddUnit(Rank rank, House house)
    {
        GameObject unitObject = Instantiate(unitPrefab, house.transform.position, Quaternion.identity);
        
        if (!unitObject.TryGetComponent<UnitController>(out var unitBehaviour))
        {
            Debug.LogError("Unit prefab is missing UnitBehaviour component!");
            return null;
        }

        unitBehaviour.Initialize(new Unit(rank), house);
        units.Add(unitBehaviour);

        Debug.Log($"Unit spawned: {unitBehaviour.UnitData.unitName}, Rank = {unitBehaviour.UnitData.rank}, Age = {unitBehaviour.UnitData.age}");

        return unitBehaviour;
    }

    public void RemoveUnit(UnitController unitBehaviour)
    {
        if (units.Contains(unitBehaviour))
        {
            units.Remove(unitBehaviour);
            Destroy(unitBehaviour.gameObject);
        }
    }

    public void AssignJob(UnitController unitBehaviour, Job job, Transform workplace)
    {
        if (!units.Contains(unitBehaviour))
        {
            Debug.LogWarning($"Could not find Unit GameObject for {unitBehaviour.UnitData.unitName}");
            return;
        }

        unitBehaviour.AssignJob(workplace, job);
    }

    public int GetPopulationCount()
    {
        return units.Count;
    }

    public void AssignJob(Unit unit, Job job)
    {
        unit.job = job;
    }

    public static int GetEffectiveness(Rank rank)
    {
        switch (rank)
        {
            case Rank.S: return 5;
            case Rank.A: return 4;
            case Rank.B: return 3;
            case Rank.C: return 2;
            case Rank.D: return 1;
            case Rank.E: return 0;
            case Rank.F: return -1;
            default: return 0;
        }
    }

    public List<UnitController> GetUnassignedUnits()
    {
        List<UnitController> unassignedUnits = new();
        
        foreach (UnitController unit in units)
        {
            if (unit.UnitData.job == Job.Unassigned)
            {
                unassignedUnits.Add(unit);
            }
        }

        return unassignedUnits;
    }
}