using System.Collections;
using UnityEngine;

public class EnemyBullit : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    void OnEnable()
    {
        StartCoroutine(FireRoutine());
    }

    IEnumerator FireRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            if (Player.Instance == null) continue;

            Vector3 dir = (Player.Instance.transform.position - firePoint.position).normalized;
            Fire(dir);
        }
    }

    void Fire(Vector3 dir)
    {
        if (firePoint == null) return;
        GameObject bullet = EnemyBulletPool.Instance != null
            ? EnemyBulletPool.Instance.Get(firePoint.position)
            : Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        if (bullet == null) return;
        EnemyBullitController ctrl = bullet.GetComponent<EnemyBullitController>();
        if (ctrl != null) ctrl.SetDirection(dir);
    }
}
