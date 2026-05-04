using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public float spawnMinInterval = 4f;
    public float spawnMaxInterval = 8f;

    [Header("Top Spawn Points (Point_0~4) - 아래로 이동")]
    public Transform[] topPoints;

    [Header("대각선 시작 포인트 (Point_5, Point_7)")]
    public Transform[] diagStartPoints;

    [Header("대각선 목표 포인트 (Point_6, Point_8)")]
    public Transform[] diagEndPoints;

    private float delta;
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

        int topCount = topPoints != null ? topPoints.Length : 0;
        int diagCount = diagStartPoints != null ? diagStartPoints.Length : 0;
        int total = topCount + diagCount;
        if (total == 0) return;

        int idx = Random.Range(0, total);
        int enemyIdx = Random.Range(0, enemyPrefabs.Length);
        Vector3 spawnPos;
        Vector3 dir;

        if (idx < topCount)
        {
            spawnPos = topPoints[idx].position;
            dir = Vector3.down;
        }
        else
        {
            int diagIdx = idx - topCount;
            spawnPos = diagStartPoints[diagIdx].position;
            dir = (diagEndPoints[diagIdx].position - spawnPos).normalized;
        }

        GameObject enemy = Instantiate(enemyPrefabs[enemyIdx], spawnPos, Quaternion.identity);
        EnemyController ec = enemy.GetComponent<EnemyController>();
        if (ec != null) ec.SetDirection(dir);
    }
}
