using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletPool : MonoBehaviour
{
    public static EnemyBulletPool Instance;
    public GameObject bulletPrefab;
    public int poolSize = 20;

    private Queue<GameObject> _pool = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        for (int i = 0; i < poolSize; i++)
            _pool.Enqueue(Create());
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    GameObject Create()
    {
        var obj = Instantiate(bulletPrefab, transform);
        obj.SetActive(false);
        return obj;
    }

    public GameObject Get(Vector3 position)
    {
        var obj = _pool.Count > 0 ? _pool.Dequeue() : Create();
        obj.transform.SetPositionAndRotation(position, Quaternion.identity);
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        if (!obj.activeSelf) return;
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}
