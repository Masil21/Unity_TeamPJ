using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public int poolSizePerEnemy = 8;

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

    private Dictionary<GameObject, Queue<GameObject>> pools = new();
    private Dictionary<GameObject, GameObject> instanceToPrefab = new();

    private float topDelta;
    private float topNextInterval;
    private float diagDelta;
    private float diagNextInterval;

    void Awake()
    {
        foreach (var prefab in enemyPrefabs)
        {
            if (prefab == null) continue;
            var queue = new Queue<GameObject>();
            for (int i = 0; i < poolSizePerEnemy; i++)
                queue.Enqueue(CreatePooled(prefab));
            pools[prefab] = queue;
        }
    }

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

    GameObject CreatePooled(GameObject prefab)
    {
        var obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        instanceToPrefab[obj] = prefab;
        return obj;
    }

    GameObject GetFromPool(GameObject prefab, Vector3 position)
    {
        if (!pools.TryGetValue(prefab, out var queue))
            return null;

        var obj = queue.Count > 0 ? queue.Dequeue() : CreatePooled(prefab);
        obj.transform.SetPositionAndRotation(position, Quaternion.identity);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (instanceToPrefab.TryGetValue(obj, out var prefab) && pools.ContainsKey(prefab))
        {
            obj.SetActive(false);
            pools[prefab].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }

    void SpawnFromTop()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (topPoints == null || topPoints.Length == 0) return;

        int idx = Random.Range(0, topPoints.Length);
        int enemyIdx = Random.Range(0, enemyPrefabs.Length);

        var obj = GetFromPool(enemyPrefabs[enemyIdx], topPoints[idx].position);
        if (obj == null) return;

        var ec = obj.GetComponent<EnemyController>();
        if (ec != null)
        {
            ec.SetSpawner(this);
            ec.ResetEnemy();
            ec.SetDirection(Vector3.down);
            ec.onDie = ItemManager3.Instance.CreateItem;
        }
    }

    void SpawnDiagonal()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (diagStartPoints == null || diagStartPoints.Length == 0) return;

        int diagIdx = Random.Range(0, diagStartPoints.Length);
        int enemyIdx = Random.Range(0, enemyPrefabs.Length);

        Vector3 spawnPos = diagStartPoints[diagIdx].position;
        Vector3 dir = (diagEndPoints[diagIdx].position - spawnPos).normalized;

        var obj = GetFromPool(enemyPrefabs[enemyIdx], spawnPos);
        if (obj == null) return;

        var ec = obj.GetComponent<EnemyController>();
        if (ec != null)
        {
            ec.SetSpawner(this);
            ec.ResetEnemy();
            ec.SetDirection(dir);
            ec.onDie = ItemManager3.Instance.CreateItem;
        }
    }
}
