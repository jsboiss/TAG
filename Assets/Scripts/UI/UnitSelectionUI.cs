using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionUI : MonoBehaviour
{
    public static UnitSelectionUI Instance { get; private set; }
    public Transform unitListContainer;
    public GameObject unitButtonPrefab;

    private System.Action<UnitController> onUnitSelected;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        gameObject.SetActive(false); // Hide at start
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Hide();
        }
    }

    public void Show(System.Action<UnitController> onSelect)
    {
        gameObject.SetActive(true);
        onUnitSelected = onSelect;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(List<UnitController> availableUnits, System.Action<UnitController> onSelect)
    {
        Debug.Log($"📋 Showing Unit Selection UI... Units available: {availableUnits.Count}");

        ClearUI(); // Make sure old buttons are removed
        onUnitSelected = onSelect;
        gameObject.SetActive(true);

        foreach (UnitController unit in availableUnits)
        {
            GameObject button = Instantiate(unitButtonPrefab, unitListContainer);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

            if (buttonText == null)
            {
                Debug.LogError("❌ TMP_Text component not found in unitButtonPrefab!");
                return;
            }

            buttonText.text = $"Rank: {unit.UnitData.rank}";
            button.GetComponent<Button>().onClick.AddListener(() => SelectUnit(unit));
        }
    }


    void ClearUI()
    {
        foreach (Transform child in unitListContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void SelectUnit(UnitController unit)
    {
        onUnitSelected?.Invoke(unit);
        gameObject.SetActive(false);
    }
}
