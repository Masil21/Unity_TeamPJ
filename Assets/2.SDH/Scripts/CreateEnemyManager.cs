using UnityEngine;

public class CreateEnemyManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public float spawnMinInterval = 4f;
    public float spawnMaxInterval = 8f;
    public float spawnY = 5.5f;
    public float spawnXMin = -2.5f;
    public float spawnXMax = 2.5f;

    private float delta = 0;
    private float nextInterval;

    void Start()
    {
        Application.targetFrameRate = 60;
        nextInterval = Random.Range(spawnMinInterval, spawnMaxInterval);
    }

    void Update()
    {
        delta += Time.deltaTime;
        if (delta > nextInterval)
        {
            SpawnEnemy();
            delta = 0;
            nextInterval = Random.Range(spawnMinInterval, spawnMaxInterval);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        float randomX = Random.Range(spawnXMin, spawnXMax);
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);
        int index = Random.Range(0, enemyPrefabs.Length);
        Instantiate(enemyPrefabs[index], spawnPos, Quaternion.identity);
    }
}
