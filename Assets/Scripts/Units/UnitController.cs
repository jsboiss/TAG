using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class UnitController : MonoBehaviour
{
    [Header("Unit")]
    public Unit UnitData { get; private set; }
    public UnitState state;
    private NavMeshAgent agent;
    private House home;
    private Transform workplace;
    private NavMeshPath currentPath;

    private readonly float workTime = 3f; // Time spent working per cycle

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(Unit unitData, House homeTransform)
    {
        UnitData = unitData;
        home = homeTransform;
        transform.position = home.transform.position; // Spawn at home
    }

    public void SetHome(House newHome) => home = newHome;

    public void AssignJob(Transform workLocation, Job newJob)
    {
        /// Assigns the unit to a job and moves them to the workplace.
        if (newJob == Job.Unassigned)
        {
            Debug.Log($"❌ {UnitData.unitName} was unassigned from work. Walking home.");
            workplace = null; // Clear previous workplace to prevent accidental reassignments
            UnitData.job = Job.Unassigned;
            SetState(UnitState.WalkingHome);
            return;
        }

        Debug.Log($"🛠️ {UnitData.unitName} assigned to {newJob}");
        UnitData.job = newJob;
        workplace = workLocation;

        // Move unit first before checking distance
        agent.SetDestination(workplace.position);
        StartCoroutine(VerifyWalkingToWork());
    }

    private IEnumerator VerifyWalkingToWork()
    {
        /// Verifies if the unit should walk to work or immediately start working.
        yield return new WaitForSeconds(0.2f); // Allow Unity to process movement

        if (!agent.pathPending && agent.remainingDistance > 1f)
            SetState(UnitState.WalkingToWork);
        else
            SetState(UnitState.Working);
    }
    
    private void SetState(UnitState newState)
    {
        state = newState;
        currentPath = agent.path;
        switch (state)
        {
            case UnitState.WalkingToWork:
                agent.SetDestination(workplace.position);
                StartCoroutine(WaitUntilArrived(() => 
                {
                    TerrainPainter.Instance.PaintPathway(currentPath);
                    SetState(UnitState.Working);
                }));
                break;
            case UnitState.Working:
                StartCoroutine(WorkCycle());
                break;
            case UnitState.WalkingHome:
                if (home == null) 
                {
                    Debug.Log("🔴 No home found.");
                    break;
                }
                
                agent.SetDestination(home.transform.position);
                StartCoroutine(WaitUntilArrived(() => 
                {
                    TerrainPainter.Instance.PaintPathway(currentPath);
                    SetState(UnitState.Idle);
                }));
                break;
            default:
                state = UnitState.Idle;
                break;
        }
    }

    private IEnumerator WorkCycle()
    {
        while (UnitData.job != Job.Unassigned && UnitData.isAlive)
        {
            yield return new WaitForSeconds(workTime);
            if (workplace != null) 
            {
                agent.SetDestination(workplace.position);
                yield return new WaitUntil(() => agent.remainingDistance < 0.5f);
            }
            
            ResourceManager.AddResource(ResourceTypes.Wood, 1);
        }
    }

    private Transform FindTaskLocation()
    {
        GameObject[] taskObjects = GameObject.FindGameObjectsWithTag("Tree");
        return taskObjects.Length > 0 ? taskObjects[Random.Range(0, taskObjects.Length)].transform : workplace;
    }

    public Transform GetHomeTransform() => home?.transform;

    /// Waits until the unit arrives at its destination before triggering an action.
    private IEnumerator WaitUntilArrived(System.Action onArrived)
    {
        yield return new WaitUntil(() => agent.remainingDistance < 0.5f);
        onArrived?.Invoke();
    }
}
