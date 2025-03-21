using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/Enemy Data")]
public class Enemy : ScriptableObject
{
    public float health = 100f;
    public float attack = 5f;
    public EnemyState state = EnemyState.Idle;
    public GameObject prefab;
}