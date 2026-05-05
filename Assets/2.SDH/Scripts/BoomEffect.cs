using System.Collections;
using UnityEngine;

public class BoomEffect : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
            animator.SetInteger("State", 1);

        StartCoroutine(BoomRoutine());
    }

    IEnumerator BoomRoutine()
    {
        float elapsed = 0f;

        while (elapsed < 2f)
        {
            DestroyAllEnemies();
            DestroyAllEnemyBullets();
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    void DestroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyController ec = enemy.GetComponent<EnemyController>();
            if (ec != null)
                ec.TakeDamage(9999);
        }
    }

    void DestroyAllEnemyBullets()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (GameObject bullet in bullets)
            Destroy(bullet);
    }
}
