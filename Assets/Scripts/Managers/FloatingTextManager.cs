using UnityEngine;
using TMPro;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance;
    public GameObject floatingTextPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnFloatingText(string text, Vector3 position)
    {
        GameObject textObject = Instantiate(this.floatingTextPrefab, RandomisePosition(position), Quaternion.identity);
        textObject.GetComponentInChildren<TextMeshProUGUI>().text = text;

        Destroy(textObject, 0.5f);
    }

    private Vector3 RandomisePosition(Vector3 position)
    {
        position.x = Random.Range(position.x - 1, position.x + 1);
        position.y = Random.Range(position.y - 1, position.y + 1);
        
        return position;
    }
}
