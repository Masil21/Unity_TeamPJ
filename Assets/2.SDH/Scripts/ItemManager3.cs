using UnityEngine;

public class ItemManager3 : MonoBehaviour
{
    public static ItemManager3 Instance;

    public GameObject coinPrefab;
    public GameObject powerPrefab;
    public GameObject boomPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // SpawnPoint에서 onDie에 등록하는 메서드
    public void CreateItem(Vector3 pos)
    {
        // None:30% Coin:30% Power:20% Boom:20%
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

        if (prefab != null)
            Instantiate(prefab, pos, Quaternion.identity);
    }
}
