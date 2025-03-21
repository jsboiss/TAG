using TMPro;
using UnityEngine;

public class StatisticsUI : MonoBehaviour
{
    public TextMeshProUGUI populationComponent;
    public TextMeshProUGUI goldComponent;
    public TextMeshProUGUI foodComponent;
    public TextMeshProUGUI woodComponent;

    private void Update()
    {
        int populationCount = PopulationManager.Instance.GetPopulationCount();
        populationComponent.text = $"{populationCount:D5}";

        goldComponent.text = $"{ResourceManager.GetResource(ResourceTypes.Gold):D5}";
        foodComponent.text = $"{ResourceManager.GetResource(ResourceTypes.Food):D5}";
        woodComponent.text = $"{ResourceManager.GetResource(ResourceTypes.Wood):D5}";
    }
}