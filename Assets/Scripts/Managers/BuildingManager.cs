using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }
    public List<BuildingData> buildingTypes;
    public List<Building> buildings;
    public LayerMask layer;
    public bool isPlacing;

    private GameObject tempBuilding;
    private RaycastHit hit;
    private Vector3 pos;
    private float rotationAngle = 0f;
    private BuildingData selectedBuildingData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update() 
    {
        if (tempBuilding != null) 
        {
            tempBuilding.transform.position = pos;

            if (Input.GetKeyDown(KeyCode.R))
            {
                rotationAngle = (rotationAngle + 90f) % 360f;
                tempBuilding.transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
            }

            if (Input.GetMouseButtonDown(0)) 
            {
                PlaceObject();
            }
            
            if (Input.GetMouseButtonDown(1)) 
            {
                CancelPlacement();
            }
        }
    }

    public void SelectBuilding(int index) 
    {
        if (index < 0 || index >= buildingTypes.Count) return;

        selectedBuildingData = buildingTypes[index];
        tempBuilding = Instantiate(selectedBuildingData.prefab, pos, Quaternion.identity);
        isPlacing = true;
    }

    public void PlaceObject() 
    {
        var buildingObject = Instantiate(tempBuilding, pos, tempBuilding.transform.rotation);

        if (buildingObject.TryGetComponent<Building>(out var building))
        {
            building.data = selectedBuildingData; 

            AssignBuildingData(building, selectedBuildingData);

            building.Construct();
            buildings.Add(building);
        }

        Destroy(tempBuilding);
        isPlacing = false;
    }

    private void AssignBuildingData(Building building, BuildingData data)
    {
        // Find a matching field in the building that can hold the data type
        var buildingType = building.GetType();
        var dataType = data.GetType();

        var field = buildingType.GetFields()
            .FirstOrDefault(f => f.FieldType == dataType);

        if (field != null)
        {
            field.SetValue(building, data);
        }
        else
        {
            Debug.LogWarning($"⚠️ No matching field found for {dataType.Name} in {buildingType.Name}");
        }
    }

    private void CancelPlacement()
    {
        Destroy(tempBuilding);
        isPlacing = false;
    }

    private void FixedUpdate() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, layer)) 
        {
            pos = hit.point;
        }
    }
}
