using System.Collections;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingData data;
    public float currentHealth;

    private GameObject outlineObject;
    private Renderer outlineRenderer;
    public Material outlineMaterial;

    void Awake()
    {
        CreateOutline();
    }
    private void CreateOutline()
    {
        outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localRotation = Quaternion.identity;
        outlineObject.transform.localScale = Vector3.one * 1.02f;

        MeshFilter originalMeshFilter = GetComponent<MeshFilter>();
        MeshRenderer originalRenderer = GetComponent<MeshRenderer>();

        MeshFilter outlineMeshFilter = outlineObject.AddComponent<MeshFilter>();
        MeshRenderer outlineMeshRenderer = outlineObject.AddComponent<MeshRenderer>();

        outlineMeshFilter.sharedMesh = originalMeshFilter.sharedMesh;
        outlineMeshRenderer.sharedMaterial = outlineMaterial;
        outlineMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        outlineObject.SetActive(false);
        outlineRenderer = outlineMeshRenderer;
    }
    public void SetOutline(bool enable)
    {
        outlineObject.SetActive(enable);
    }

    public virtual void Construct()
    {
        if (ResourceManager.CanAfford(data.woodCost, data.stoneCost, data.foodCost, data.goldCost))
        {
            ResourceManager.SpendResources(data.woodCost, data.stoneCost, data.foodCost, data.goldCost);
        }
        else
        {
            Debug.LogWarning("Not enough resources to build " + data.buildingName);
            return;
        }

        if (data.constructionTime > 0) StartCoroutine(ConstructionTimer());
        else OnConstructionComplete();
    }

    private IEnumerator ConstructionTimer()
    {
        Debug.Log($"Constructing {data.buildingName}... Time: {data.constructionTime} seconds");
        yield return new WaitForSeconds(data.constructionTime);
        OnConstructionComplete();
    }

    protected virtual void OnConstructionComplete()
    {
        Debug.Log($"{data.buildingName} construction complete!");
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        ShowFloatingDamage(amount, gameObject.transform.position);

        if (currentHealth <= 0)
        {
            DestroyBuilding();
        }
    }

    private void ShowFloatingDamage(float amount, Vector3 position)
    {
        position.y += 10;
        FloatingTextManager.Instance.SpawnFloatingText(amount.ToString(), position);
    }

    private void DestroyBuilding() 
    {
        BuildingManager.Instance.buildings.Remove(this);
        Destroy(gameObject);
    }
}