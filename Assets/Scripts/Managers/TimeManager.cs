using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private int currentYear = 0;

    void Start()
    {
        // Simulate one year every 10 seconds (for testing)
        InvokeRepeating("AdvanceYear", 10f, 10f);
    }

    void AdvanceYear()
    {
        currentYear++;
        Debug.Log($"Year {currentYear}");
    }
}