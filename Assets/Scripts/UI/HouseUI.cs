using UnityEngine;
using TMPro;

public class HouseUI : MonoBehaviour
{
    public GameObject housePanel; // Reference to the ResidentsPanel
    public GameObject residentTextTemplate; // Reference to the ResidentTextTemplate
    public float verticalSpacing = 80f; // Space between each resident text

    private House selectedHouse; // Currently selected house

    void Start()
    {
        // Hide the residents panel initially
        housePanel.SetActive(false);
    }

    // Show the residents of a selected house
    public void ShowResidents(House house)
    {
        selectedHouse = house;

        // Clear the previous resident list
        foreach (Transform child in residentTextTemplate.transform.parent)
        {
            if (child != residentTextTemplate.transform)
            {
                Destroy(child.gameObject);
            }
        }

        // Populate the UI with the residents
        for (int i = 0; i < selectedHouse.residents.Count; i++)
        {
            // Instantiate the resident text
            GameObject residentText = Instantiate(residentTextTemplate, residentTextTemplate.transform.parent);
            TMP_Text textComponent = residentText.GetComponent<TMP_Text>();

            // Set the text
            textComponent.text = 
            $"Rank: {selectedHouse.residents[i].UnitData.rank}, Job: {selectedHouse.residents[i].UnitData.job}, State: {selectedHouse.residents[i].state}";

            // Adjust the position of the resident text
            RectTransform rectTransform = residentText.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Move the text down based on its index and spacing
                var pos = i + 1;
                rectTransform.anchoredPosition = new Vector2(0, -pos * verticalSpacing);
            }

            residentText.SetActive(true);
        }

        // Show the residents panel
        housePanel.SetActive(true);
    }

    // Hide the residents panel
    public void HideResidents()
    {
        housePanel.SetActive(false);
    }
}