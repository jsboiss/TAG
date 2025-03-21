using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; set; }
    public GameObject enemyPrefab;
    public Vector3 spawnLocation;
    public int spreadAmount = 1;
    public int enemyPoolSize = 10;
    private Queue<GameObject> enemyPool = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InitializePool();
        Debug.Log($"Pool count is: {enemyPool.Count()}");
        StartCoroutine(SpawnWaveTimer());
    }

    private void InitializePool()
    {
        for (int i = 0; i < enemyPoolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
        }
    }

    private void SpawnWave()
    {
        int enemyCount = Mathf.RoundToInt(2);

        for (int i = 0; i < enemyCount; i++)
        {
            if (enemyPool.Count > 0)
            {
                GameObject enemy = enemyPool.Dequeue();
                enemy.SetActive(true);
                enemy.transform.position = SpreadOutSpawnLocation(spawnLocation);
            }
        }
    }

    private IEnumerator SpawnWaveTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            SpawnWave();
        }
    }

    public void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }

    private Vector3 SpreadOutSpawnLocation(Vector3 location)
    {
        location.x = Random.Range(location.x - spreadAmount, location.x + spreadAmount);
        location.z = Random.Range(location.z - spreadAmount, location.z + spreadAmount);
        return location;
    } 
}
