using UnityEngine;

public class EnemyBullitController : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 moveDir = Vector3.down;

    void OnEnable()  { BoomEffect.OnBoom += ReturnToPool; }
    void OnDisable() { BoomEffect.OnBoom -= ReturnToPool; }

    public void SetDirection(Vector3 dir)
    {
        moveDir = dir;
    }

    void Update()
    {
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);

        Vector3 pos = transform.position;
        if (pos.x < -9.5f || pos.x > 9.5f || pos.y < -5.5f || pos.y > 5.5f)
            ReturnToPool();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null) player.TakeDamage();
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        if (EnemyBulletPool.Instance != null)
            EnemyBulletPool.Instance.Return(gameObject);
        else
            Destroy(gameObject);
    }
}
