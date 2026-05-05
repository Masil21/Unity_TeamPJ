using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public int poolSizePerEnemy = 8;

    [Header("상단 하강 (Point 0~4)")]
    public Transform[] topPoints;
    public float topSpawnMinInterval = 1.5f;
    public float topSpawnMaxInterval = 3f;

    [Header("좌→우 수평 (Point 5, 6)")]
    public Transform[] leftToRightPoints;
    public float lrSpawnMinInterval = 2f;
    public float lrSpawnMaxInterval = 4f;

    [Header("우→좌 수평 (Point 7, 8)")]
    public Transform[] rightToLeftPoints;
    public float rlSpawnMinInterval = 2f;
    public float rlSpawnMaxInterval = 4f;

    [Header("좌중단 → 우하단 대각선 (Point 9, 11)")]
    public Transform[] diagLeftPoints;
    public float diagLeftSpawnMinInterval = 3f;
    public float diagLeftSpawnMaxInterval = 6f;

    [Header("우중단 → 좌하단 대각선 (Point 10, 12)")]
    public Transform[] diagRightPoints;
    public float diagRightSpawnMinInterval = 3f;
    public float diagRightSpawnMaxInterval = 6f;

    private Dictionary<GameObject, Queue<GameObject>> pools = new();
    private Dictionary<GameObject, GameObject> instanceToPrefab = new();

    private float topDelta, topNext;
    private float lrDelta, lrNext;
    private float rlDelta, rlNext;
    private float diagLeftDelta, diagLeftNext;
    private float diagRightDelta, diagRightNext;

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
        topNext = Random.Range(topSpawnMinInterval, topSpawnMaxInterval);
        lrNext = Random.Range(lrSpawnMinInterval, lrSpawnMaxInterval);
        rlNext = Random.Range(rlSpawnMinInterval, rlSpawnMaxInterval);
        diagLeftNext = Random.Range(diagLeftSpawnMinInterval, diagLeftSpawnMaxInterval);
        diagRightNext = Random.Range(diagRightSpawnMinInterval, diagRightSpawnMaxInterval);
    }

    void Update()
    {
        Tick(ref topDelta, ref topNext, topSpawnMinInterval, topSpawnMaxInterval, SpawnFromTop);
        Tick(ref lrDelta, ref lrNext, lrSpawnMinInterval, lrSpawnMaxInterval, SpawnLeftToRight);
        Tick(ref rlDelta, ref rlNext, rlSpawnMinInterval, rlSpawnMaxInterval, SpawnRightToLeft);
        Tick(ref diagLeftDelta, ref diagLeftNext, diagLeftSpawnMinInterval, diagLeftSpawnMaxInterval, SpawnDiagLeft);
        Tick(ref diagRightDelta, ref diagRightNext, diagRightSpawnMinInterval, diagRightSpawnMaxInterval, SpawnDiagRight);
    }

    void Tick(ref float delta, ref float next, float min, float max, System.Action spawn)
    {
        delta += Time.deltaTime;
        if (delta > next) { spawn(); delta = 0; next = Random.Range(min, max); }
    }

    void SpawnFromTop()    => SpawnAt(topPoints, Vector3.down);
    void SpawnLeftToRight() => SpawnAt(leftToRightPoints, Vector3.right);
    void SpawnRightToLeft() => SpawnAt(rightToLeftPoints, Vector3.left);
    void SpawnDiagLeft()   => SpawnAt(diagLeftPoints, new Vector3(1f, -1f, 0f).normalized);
    void SpawnDiagRight()  => SpawnAt(diagRightPoints, new Vector3(-1f, -1f, 0f).normalized);

    void SpawnAt(Transform[] points, Vector3 dir)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (points == null || points.Length == 0) return;

        int idx = Random.Range(0, points.Length);
        int enemyIdx = Random.Range(0, enemyPrefabs.Length);

        var obj = GetFromPool(enemyPrefabs[enemyIdx], points[idx].position);
        if (obj == null) return;

        var ec = obj.GetComponent<EnemyController>();
        if (ec != null)
        {
            ec.SetSpawner(this);
            ec.ResetEnemy();
            ec.SetDirection(dir);
            if (ItemManager3.Instance != null)
                ec.onDie = ItemManager3.Instance.CreateItem;
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
        if (!pools.TryGetValue(prefab, out var queue)) return null;
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
}
