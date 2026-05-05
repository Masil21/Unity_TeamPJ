using System.Collections;
using UnityEngine;

public class Item3 : MonoBehaviour
{
    public enum ItemType { None = -1, Coin, Boom, Power }
    public ItemType itemType;

    private float speed = 1f;

    void OnEnable()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (true)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            yield return null;
            if (transform.position.y <= -5.5f)
            {
                ReturnToPool();
                yield break;
            }
        }
    }

    void ReturnToPool()
    {
        if (ItemManager3.Instance != null)
            ItemManager3.Instance.ReturnItem(gameObject);
        else
            Destroy(gameObject);
    }
}
