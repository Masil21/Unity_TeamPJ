using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public float spawnMinInterval = 4f;
    public float spawnMaxInterval = 8f;

    private Transform[] points;
    private float delta;
    private float nextInterval;

    void Start()
    {
        points = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            points[i] = transform.GetChild(i);

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
        if (points == null || points.Length == 0) return;

        int pointIndex = Random.Range(0, points.Length);
        int enemyIndex = Random.Range(0, enemyPrefabs.Length);
        Instantiate(enemyPrefabs[enemyIndex], points[pointIndex].position, Quaternion.identity);
    }
}
