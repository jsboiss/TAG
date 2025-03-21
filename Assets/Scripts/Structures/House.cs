using UnityEngine;
using System.Collections.Generic;

public class House : Building
{
    public HouseData houseData; // A reference to a House-specific ScriptableObject
    public List<UnitController> residents = new();

    public override void Construct()
    {
        base.Construct();
        currentHealth = houseData.health;
        Debug.Log($"🟢 House Health is: {currentHealth}");

        for (int i = 0; i < houseData.unitsProvided; i++)
        {
            Rank unitRank = RankManager.GetRandomRank();
            UnitController newUnit = PopulationManager.Instance.AddUnit(unitRank, this);
            AddResident(newUnit);
        }
    }
    
    public void AddResident(UnitController unit)
    {
        residents.Add(unit);
        unit.SetHome(this);
    }

    public void RemoveResident(UnitController unit)
    {
        residents.Remove(unit);
        unit.SetHome(null);
    }
}