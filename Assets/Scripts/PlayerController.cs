using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Heatmap heatmap;
    private NavMeshAgent agent;

    void Awake()
    {
        if (gameObject.TryGetComponent<NavMeshAgent>(out var navMesh))
        {
            this.agent = navMesh;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var movePosition = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(movePosition, out var hit))
            {
                this.agent.SetDestination(hit.point);
            }
        }

        if (Input.GetKeyDown(KeyCode.G)) 
        {
            Debug.Log($"🟢 NPC Position: {this.transform.position}");
            heatmap.GenerateRoads();
        }
    }
}
