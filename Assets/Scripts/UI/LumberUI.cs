using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LumberUI : MonoBehaviour
{
    public static LumberUI Instance { get; private set; }
    public Lumber lumberCamp; // Reference to the selected Lumber Camp
    public Transform slotContainer; // Parent object for slots
    public GameObject slotPrefab; // Prefab for UI slot button

    private List<UnitController> availableUnits = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gameObject.SetActive(false); // Ensure it starts hidden
    }

    private void Start()
    {
        gameObject.SetActive(false); // Hide the UI when the game starts
    }

    public void Show(Lumber camp)
    {
        lumberCamp = camp;
        gameObject.SetActive(true); // Ensure the UI becomes visible again
        RefreshUI();
        Debug.Log($"🟢 Showing UI: {gameObject} is {gameObject.activeInHierarchy}");
    }

    public void Hide()
    {
        Debug.Log("🔴 Hiding UI");
        gameObject.SetActive(false);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Hide();
        }
    }

    public void RefreshUI()
    {
        ClearUI();
        availableUnits = PopulationManager.Instance.GetUnassignedUnits();
        Debug.Log($"🟡 There are {availableUnits.Count} unasigned units found.");

        for (int i = 0; i < lumberCamp.maxWorkers; i++)
        {
            GameObject slot = Instantiate(slotPrefab, slotContainer);
            Button slotButton = slot.GetComponent<Button>();
            Debug.Log($"🟡 Slot button is: {slotButton}");

            if (i < lumberCamp.workers.Count)
            {
                // If a worker is already assigned, show their name
                UnitController assignedWorker = lumberCamp.workers[i];
                slotButton.GetComponentInChildren<TMP_Text>().text = $"{assignedWorker.UnitData.rank}";
                slotButton.onClick.AddListener(() => lumberCamp.RemoveWorker(this, assignedWorker));
            }
            else
            {
                // Show "Assign Worker" if the slot is empty
                var buttonText = slotButton.GetComponentInChildren<TMP_Text>();
                Debug.Log($"🟡 Text Component of slot button is: {buttonText}");
                buttonText.text = "Assign";
                slotButton.onClick.AddListener(() => ShowUnitSelection(i));
            }
        }
    }

    void ClearUI()
    {
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void ShowUnitSelection(int slotIndex)
    {
        //Debug.Log($"🟡 Opening UnitSelectionUI for slot {slotIndex} with {availableUnits.Count} available units.");

        UnitSelectionUI.Instance.Show(availableUnits, (selectedUnit) =>
        {
            lumberCamp.AssignWorker(selectedUnit);
            RefreshUI(); // Refresh worker slots after assignment
        });
    }
}
