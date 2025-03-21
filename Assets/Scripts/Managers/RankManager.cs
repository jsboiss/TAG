using System.Collections.Generic;
using UnityEngine;

public class RankManager : MonoBehaviour
{
    public static RankManager Instance { get; private set; }

    [System.Serializable]
    public class RankChance
    {
        public Rank rank;
        [Range(0f, 100f)] public float percentage;
    }

    [SerializeField] private static List<RankChance> rankChances = new()
    {
        new RankChance { rank = Rank.S, percentage = 0.01f },
        new RankChance { rank = Rank.A, percentage = 0.25f },
        new RankChance { rank = Rank.B, percentage = 0.75f },
        new RankChance { rank = Rank.C, percentage = 2f },
        new RankChance { rank = Rank.D, percentage = 5f },
        new RankChance { rank = Rank.E, percentage = 15f },
        new RankChance { rank = Rank.F, percentage = 75f }
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private static void NormalizeProbabilities()
    {
        float total = 0f;
        foreach (var rankChance in rankChances)
        {
            total += rankChance.percentage;
        }

        if (total <= 0) return; // Avoid division by zero

        for (int i = 0; i < rankChances.Count; i++)
        {
            rankChances[i].percentage = (rankChances[i].percentage / total) * 100f;
        }
    }

    public static Rank GetRandomRank()
    {
        NormalizeProbabilities(); // Ensure the values sum to 100%

        float randomValue = Random.value * 100f;
        float cumulative = 0f;

        foreach (var rankChance in rankChances)
        {
            cumulative += rankChance.percentage;
            if (randomValue < cumulative)
                return rankChance.rank;
        }

        return Rank.F;
    }
}
