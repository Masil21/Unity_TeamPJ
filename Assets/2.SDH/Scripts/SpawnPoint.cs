using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject[] enemyPrefabs;

    [Header("Top Spawn Points (Point_0~4) - 아래로 이동")]
    public Transform[] topPoints;
    public float topSpawnMinInterval = 1.5f;
    public float topSpawnMaxInterval = 3f;

    [Header("대각선 시작 포인트 (Point_5, Point_7)")]
    public Transform[] diagStartPoints;

    [Header("대각선 목표 포인트 (Point_6, Point_8)")]
    public Transform[] diagEndPoints;
    public float diagSpawnMinInterval = 3f;
    public float diagSpawnMaxInterval = 6f;

    private float topDelta;
    private float topNextInterval;
    private float diagDelta;
    private float diagNextInterval;

    void Start()
    {
        Application.targetFrameRate = 60;
        topNextInterval = Random.Range(topSpawnMinInterval, topSpawnMaxInterval);
        diagNextInterval = Random.Range(diagSpawnMinInterval, diagSpawnMaxInterval);
    }

    void Update()
    {
        topDelta += Time.deltaTime;
        if (topDelta > topNextInterval)
        {
            SpawnFromTop();
            topDelta = 0;
            topNextInterval = Random.Range(topSpawnMinInterval, topSpawnMaxInterval);
        }

        diagDelta += Time.deltaTime;
        if (diagDelta > diagNextInterval)
        {
            SpawnDiagonal();
            diagDelta = 0;
            diagNextInterval = Random.Range(diagSpawnMinInterval, diagSpawnMaxInterval);
        }
    }

    void SpawnFromTop()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (topPoints == null || topPoints.Length == 0) return;

        int idx = Random.Range(0, topPoints.Length);
        int enemyIdx = Random.Range(0, enemyPrefabs.Length);

        GameObject enemy = Instantiate(enemyPrefabs[enemyIdx], topPoints[idx].position, Quaternion.identity);
        EnemyController ec = enemy.GetComponent<EnemyController>();
        if (ec != null) ec.SetDirection(Vector3.down);
    }

    void SpawnDiagonal()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (diagStartPoints == null || diagStartPoints.Length == 0) return;

        int diagIdx = Random.Range(0, diagStartPoints.Length);
        int enemyIdx = Random.Range(0, enemyPrefabs.Length);

        Vector3 spawnPos = diagStartPoints[diagIdx].position;
        Vector3 dir = (diagEndPoints[diagIdx].position - spawnPos).normalized;

        GameObject enemy = Instantiate(enemyPrefabs[enemyIdx], spawnPos, Quaternion.identity);
        EnemyController ec = enemy.GetComponent<EnemyController>();
        if (ec != null) ec.SetDirection(dir);
    }
}
