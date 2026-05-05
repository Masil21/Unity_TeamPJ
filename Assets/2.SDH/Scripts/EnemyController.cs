using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 1f;
    public int hp = 16;
    public int scoreValue = 100;

    public Action<Vector3> onDie;

    private Animator animator;
    private bool isHit = false;
    private bool isDead = false;
    private Vector3 moveDirection = Vector3.down;
    private SpawnPoint spawner;
    private int maxHp;

    void Awake()
    {
        animator = GetComponent<Animator>();
        maxHp = hp;
    }

    void OnEnable()  { BoomEffect.OnBoom += BoomKill; }
    void OnDisable() { BoomEffect.OnBoom -= BoomKill; }
    void BoomKill()  { TakeDamage(9999); }

    public void SetSpawner(SpawnPoint sp) { spawner = sp; }

    public void SetDirection(Vector3 dir) { moveDirection = dir; }

    public void ResetEnemy()
    {
        hp = maxHp;
        isHit = false;
        isDead = false;
        StopAllCoroutines();
        if (animator != null) animator.SetInteger("State", 0);
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (true)
        {
            if (!gameObject.activeInHierarchy) yield break;
            transform.Translate(moveDirection * speed * Time.deltaTime);
            yield return null;
            if (!gameObject.activeInHierarchy) yield break;
            Vector3 pos = transform.position;
            if (pos.x < -9.5f || pos.x > 9.5f || pos.y < -5.5f || pos.y > 5.5f)
                break;
        }
        ReturnToPool();
    }

    public void TakeDamage(int damage)
    {
        if (isHit || isDead) return;
        hp -= damage;
        if (hp <= 0)
        {
            isDead = true;
            UIManager.Instance?.AddScore(scoreValue);
            onDie?.Invoke(transform.position);
            ReturnToPool();
            return;
        }
        StartCoroutine(HitRoutine());
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

    void ReturnToPool()
    {
        StopAllCoroutines();
        if (spawner != null)
            spawner.ReturnToPool(gameObject);
        else
            gameObject.SetActive(false);
    }
}
