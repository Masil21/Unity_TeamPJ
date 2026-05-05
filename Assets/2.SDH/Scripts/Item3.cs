using System.Collections;
using UnityEngine;

public class Item3 : MonoBehaviour
{
    public enum ItemType { None = -1, Coin, Boom, Power }
    public ItemType itemType;

    private float speed = 1f;

    void Start()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (true)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            if (transform.position.y <= -5.5f)
            {
                Destroy(gameObject);
                yield break;
            }
            yield return null;
        }
    }
}
