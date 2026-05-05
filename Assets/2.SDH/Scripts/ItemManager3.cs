using System.Collections.Generic;
using UnityEngine;

public class ItemManager3 : MonoBehaviour
{
    public static ItemManager3 Instance;

    public GameObject coinPrefab;
    public GameObject powerPrefab;
    public GameObject boomPrefab;
    public int poolSize = 5;

    private Dictionary<GameObject, Queue<GameObject>> _pools = new();
    private Dictionary<GameObject, GameObject> _instanceToPrefab = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitPool(coinPrefab, poolSize);
        InitPool(powerPrefab, poolSize);
        InitPool(boomPrefab, poolSize);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void InitPool(GameObject prefab, int size)
    {
        if (prefab == null) return;
        var q = new Queue<GameObject>();
        for (int i = 0; i < size; i++)
            q.Enqueue(CreatePooled(prefab));
        _pools[prefab] = q;
    }

    GameObject CreatePooled(GameObject prefab)
    {
        var obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        _instanceToPrefab[obj] = prefab;
        return obj;
    }

    public void CreateItem(Vector3 pos)
    {
        int rand = Random.Range(0, 10);
        GameObject prefab = null;

        if (rand <= 2)
            return;
        else if (rand <= 5)
            prefab = coinPrefab;
        else if (rand <= 7)
            prefab = powerPrefab;
        else
            prefab = boomPrefab;

        if (prefab == null) return;
        GetItem(prefab, pos);
    }

    void GetItem(GameObject prefab, Vector3 pos)
    {
        if (!_pools.TryGetValue(prefab, out var q)) return;
        var obj = q.Count > 0 ? q.Dequeue() : CreatePooled(prefab);
        obj.transform.SetPositionAndRotation(pos, Quaternion.identity);
        obj.SetActive(true);
    }

    public void ReturnItem(GameObject obj)
    {
        if (obj == null || !obj.activeSelf) return;
        if (_instanceToPrefab.TryGetValue(obj, out var prefab) && _pools.ContainsKey(prefab))
        {
            obj.SetActive(false);
            _pools[prefab].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}
