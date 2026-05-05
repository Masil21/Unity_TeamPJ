using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletPool : MonoBehaviour
{
    public static PlayerBulletPool Instance;
    public GameObject sideBulletPrefab;
    public GameObject centerBulletPrefab;
    public int sidePoolSize = 20;
    public int centerPoolSize = 10;

    private Dictionary<GameObject, Queue<GameObject>> _pools = new();
    private Dictionary<GameObject, GameObject> _instanceToPrefab = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        InitPool(sideBulletPrefab, sidePoolSize);
        InitPool(centerBulletPrefab, centerPoolSize);
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
        {
            var obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            _instanceToPrefab[obj] = prefab;
            q.Enqueue(obj);
        }
        _pools[prefab] = q;
    }

    public GameObject Get(GameObject prefab, Vector3 position)
    {
        if (!_pools.TryGetValue(prefab, out var q)) return null;
        GameObject obj;
        if (q.Count > 0)
            obj = q.Dequeue();
        else
        {
            obj = Instantiate(prefab, transform);
            _instanceToPrefab[obj] = prefab;
        }
        obj.transform.SetPositionAndRotation(position, Quaternion.identity);
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        if (!obj.activeSelf) return;
        if (!_instanceToPrefab.TryGetValue(obj, out var prefab)) return;
        obj.SetActive(false);
        _pools[prefab].Enqueue(obj);
    }
}
