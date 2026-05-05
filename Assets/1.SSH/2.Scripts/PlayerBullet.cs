using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage = 5;
    public float speed = 10f;
    private float _despawnY;

    void Start()
    {
        _despawnY = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
    }

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;

        if (transform.position.y > _despawnY)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController ec = other.GetComponent<EnemyController>();
            if (ec != null) ec.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
