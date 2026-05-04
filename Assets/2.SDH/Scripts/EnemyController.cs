using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 1f;
    public int hp = 16;

    private Animator animator;
    private bool isHit = false;
    private Vector3 moveDirection = Vector3.down;

    public void SetDirection(Vector3 dir)
    {
        moveDirection = dir;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(Move());
    }

    void Update() { }

    IEnumerator Move()
    {
        while (true)
        {
            if (this == null) yield break;
            transform.Translate(moveDirection * speed * Time.deltaTime);
            yield return null;
            if (this == null) yield break;
            Vector3 pos = transform.position;
            if (pos.x < -9.5f || pos.x > 9.5f || pos.y < -5.5f || pos.y > 5.5f)
                break;
        }
        if (this != null)
            Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if (isHit) return;
        hp -= damage;
        if (hp <= 0)
        {
            Destroy(gameObject);
            return;
        }
        StartCoroutine(HitRoutine());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            TakeDamage(5);
        }
    }

    IEnumerator HitRoutine()
    {
        isHit = true;
        animator.SetInteger("State", 1);
        yield return null;
        float hitLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(hitLength);
        animator.SetInteger("State", 0);
        isHit = false;
    }
}
