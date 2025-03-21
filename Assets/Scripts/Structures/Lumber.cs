using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Lumber : Building
{
    public LumberData lumberData; // Reference to the ScriptableObject
    public List<UnitController> workers = new(); // Assigned workers
    public int maxWorkers = 3; // Max number of workers allowed

    private void OnMouseDown()
    {
        if (LumberUI.Instance != null)
        {
            LumberUI.Instance.Show(this);
        }
        else
        {
            Debug.LogError("❌ LumberUI is missing from the scene!");
        }
    }

    public void AssignWorker(UnitController unit)
    {
        if (workers.Count >= maxWorkers)
        {
            Debug.Log("🔴 No available worker slots at this Lumber Camp!");
            return;
        }

        if (unit.UnitData.job != Job.Unassigned)
        {
            Debug.Log("🔴 This unit already has a job!");
            return;
        }

        unit.UnitData.job = Job.Lumberjack;
        workers.Add(unit);
        unit.AssignJob(transform, Job.Lumberjack);
    }

    public void RemoveWorker(LumberUI ui, UnitController unit)
    {
        if (workers.Contains(unit))
        {
            workers.Remove(unit);
            unit.UnitData.job = Job.Unassigned;
            unit.AssignJob(unit.GetHomeTransform(), Job.Unassigned);
            ui.RefreshUI();
        }
    }

    private IEnumerator ProduceWood()
    {
        while (true)
        {
            yield return new WaitForSeconds(lumberData.productionInterval);

            ResourceManager.AddResource(ResourceTypes.Wood, lumberData.productionRate);
            Debug.Log($"{lumberData.buildingName} produced {lumberData.productionRate} wood.");
        }
    }
}
