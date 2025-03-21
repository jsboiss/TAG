using UnityEngine;

public class HouseSelection : MonoBehaviour
{
    public HouseUI houseUI; // Reference to the HouseUI script
    private House selectedHouse;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !BuildingManager.Instance.isPlacing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                House house = hit.collider.GetComponent<House>();

                if (house != null)
                {
                    // Deselect the previous house
                    if (selectedHouse != null && selectedHouse != house)
                    {
                        selectedHouse.SetOutline(false);
                    }

                    // Select the new house
                    selectedHouse = house;
                    Debug.Log("Selected house: " + selectedHouse);
                    selectedHouse.SetOutline(true);

                    // Show the residents
                    houseUI.ShowResidents(house);
                }
                else
                {
                    // Deselect the currently selected house
                    if (selectedHouse != null)
                    {
                        selectedHouse.SetOutline(false);
                        selectedHouse = null;
                    }

                    // Hide UI
                    houseUI.HideResidents();
                }
            }
        }
    }
}
