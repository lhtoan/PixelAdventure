using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;

    [Header("Spawn Weights")]
    public float[] spawnWeights;

    [Header("Spawn Counts (per enemy type)")]
    public int[] spawnCounts;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f;  // khoảng delay mặc định giữa các lần spawn
    public bool spawning = false;

    [Header("Acceleration Settings")]
    public float totalSpawnTime = 20f;      // tổng thời gian để tính mốc tăng tốc
    public float minSpawnInterval = 0.2f;   // tốc độ spawn tối đa
    public float accelerationFactor = 0.98f; // hệ số giảm interval sau mỗi lần spawn

    [Header("Spawn Points")]
    public Transform[] groundPoints;
    public Transform[] flyPoints;
    public Transform[] bossPoints;

    [Header("References")]
    public Transform npc;
    public Transform enemyParent;

    private int totalSpawned = 0;
    private int totalToSpawn = 0;
    private float elapsedTime = 0f;

    private void Awake()
    {
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

            // tăng thời gian trôi qua
            elapsedTime += spawnInterval;

            // ✔ bắt đầu tăng tốc sau 1/4 tổng thời gian
            if (elapsedTime >= totalSpawnTime * 0.25f)
            {
                // giảm spawn interval mỗi lần spawn
                spawnInterval *= accelerationFactor;

                // giới hạn không nhỏ hơn minSpawnInterval
                if (spawnInterval < minSpawnInterval)
                    spawnInterval = minSpawnInterval;
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("Spawn finished! Total enemies spawned: " + totalSpawned);
    }

    void SpawnEnemy()
    {
        int prefabIndex = GetRandomEnemyIndex();

        while (spawnCounts[prefabIndex] <= 0)
            prefabIndex = GetRandomEnemyIndex();

        spawnCounts[prefabIndex]--;

        EnemyInfo info = enemyPrefabs[prefabIndex].GetComponent<EnemyInfo>();
        EnemyType type = info.enemyType;

        Transform spawnPoint = GetSpawnPoint(type);

        GameObject enemy = Instantiate(enemyPrefabs[prefabIndex], spawnPoint.position, Quaternion.identity);
        enemy.SetActive(true);

        var movement = enemy.GetComponent<UniversalEnemyMovement>();
        if (movement != null)
            movement.target = npc;

        if (enemyParent != null)
            enemy.transform.SetParent(enemyParent);
    }

    Transform GetSpawnPoint(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Ground:
                return groundPoints[Random.Range(0, groundPoints.Length)];
            case EnemyType.Fly:
                return flyPoints[Random.Range(0, flyPoints.Length)];
            case EnemyType.Boss:
                return bossPoints[Random.Range(0, bossPoints.Length)];
        }

        return transform;
    }

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

    public void StopSpawning()
    {
        spawning = false;
        StopAllCoroutines();
        Debug.Log("Enemy Spawning STOPPED!");
    }

    public void ClearAllEnemies()
    {
        if (enemyParent == null) return;

        foreach (Transform child in enemyParent)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("Tất cả enemy đã bị xóa");
    }


}
