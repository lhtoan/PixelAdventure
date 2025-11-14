using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;

    [Header("Spawn Weights")]
    public float[] spawnWeights;

    [Header("Spawn Counts (per enemy type)")]
    public int[] spawnCounts;     // ⭐ Số lượng spawn mỗi loại

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;
    public bool spawning = false;

    [Header("References")]
    public Transform npc;
    public Transform enemyParent;

    private int totalSpawned = 0; // tổng số enemy đã spawn
    private int totalToSpawn = 0; // tổng số cần spawn

    private void Awake()
    {
        // Tính tổng số lượng enemy cần spawn
        foreach (int c in spawnCounts)
            totalToSpawn += c;
    }

    public void StartSpawning()
    {
        if (!spawning)
            StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        spawning = true;

        while (totalSpawned < totalToSpawn)
        {
            SpawnEnemy();
            totalSpawned++;

            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("Spawn finished! Total enemies spawned: " + totalSpawned);
    }

    void SpawnEnemy()
    {
        int prefabIndex = GetRandomEnemyIndex();

        // Nếu loại đó đã spawn đủ số lượng → chọn loại khác
        while (spawnCounts[prefabIndex] <= 0)
        {
            prefabIndex = GetRandomEnemyIndex();
        }

        // Giảm số lượng còn lại
        spawnCounts[prefabIndex]--;

        // Spawn enemy
        GameObject enemy = Instantiate(enemyPrefabs[prefabIndex], transform.position, Quaternion.identity);
        enemy.SetActive(true);

        var movement = enemy.GetComponent<UniversalEnemyMovement>();
        if (movement != null)
            movement.target = npc;

        if (enemyParent != null)
            enemy.transform.SetParent(enemyParent);
    }

    // Random theo trọng số → trả về index
    int GetRandomEnemyIndex()
    {
        float total = 0;
        foreach (float w in spawnWeights)
            total += w;

        float r = Random.value * total;
        float sum = 0;

        for (int i = 0; i < spawnWeights.Length; i++)
        {
            sum += spawnWeights[i];
            if (r <= sum)
                return i;
        }

        return 0;
    }
}
