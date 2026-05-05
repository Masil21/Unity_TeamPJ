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
            yield return new WaitForSeconds(2f); // 2초 스캔

            GameObject playerGo = GameObject.FindWithTag("Player");
            if (playerGo == null) continue;

            Vector3 dir = (playerGo.transform.position - firePoint.position).normalized;
            Fire(dir);
        }
    }

    void Fire(Vector3 dir)
    {
        if (bulletPrefab == null || firePoint == null) return;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        EnemyBullitController ctrl = bullet.GetComponent<EnemyBullitController>();
        if (ctrl != null) ctrl.SetDirection(dir);
    }
}
