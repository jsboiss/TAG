using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Enemy EnemyData { get; private set; } 
    public float attackRange = 50f;
    public float attackCooldown = 1f;

    private NavMeshAgent agent;
    private EnemyState currentState;
    private float currentHealth;
    private Building currentTarget;
    private float lastAttackTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        InitializeEnemy();
    }

    void InitializeEnemy()
    {
        EnemyData = ScriptableObject.CreateInstance<Enemy>();
        currentHealth = EnemyData.health;
        currentState = EnemyState.Moving;
        FindNewTarget();
    }

    void Update()
    {
        if (currentState == EnemyState.Dead) return;

        switch (currentState)
        {
            case EnemyState.Moving:
                MoveToTarget();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
            case EnemyState.Idle:
                FindNewTarget();
                break;
        }
    }

    void FindNewTarget()
    {
        var allBuildings = BuildingManager.Instance.buildings;
        currentTarget = FindClosestAccessibleBuilding(allBuildings);
        
        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.transform.position);
            currentState = EnemyState.Moving;
        }
        else
        {
            currentState = EnemyState.Idle;
        }
    }

    void MoveToTarget()
    {
        if (currentTarget == null) 
        {
            FindNewTarget();
            return;
        }

        if (Vector3.Distance(transform.position, currentTarget.transform.position) <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
    }

    void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;
        if (currentTarget != null)
        {
            currentTarget.TakeDamage(EnemyData.attack);
            
            if (currentTarget.IsDestroyed())
            {
                Debug.Log("Target Destroyed, Finding new target");
                FindNewTarget();
            }
            return;
        }
        currentState = EnemyState.Idle;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        currentState = EnemyState.Dead;
        gameObject.SetActive(false); // Return to pool instead of destroying
    }

    Building FindClosestAccessibleBuilding(List<Building> buildings)
    {
        Building bestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (var building in buildings)
        {
            // if (building.buildingType == BuildingType.Wall || building.buildingType == BuildingType.Tower)
            //     continue; 

            if (CanReach(building.transform.position))
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    bestTarget = building;
                }
            }
        }

        return bestTarget;
    }

    bool CanReach(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        return agent.CalculatePath(targetPosition, path) && path.status == NavMeshPathStatus.PathComplete;
    }
}
