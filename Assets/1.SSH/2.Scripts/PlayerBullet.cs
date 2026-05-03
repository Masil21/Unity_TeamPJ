using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage = 5;
    public float speed = 10f;
    private float _despawnY;

    private void Start()
    {
        _despawnY = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;

        if (transform.position.y > _despawnY)
        {
            // TODO: 풀링 단계에서 Release로 교체
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.SendMessage("TakeDamage", damage);
            // TODO: 풀링 단계에서 Release로 교체
            Destroy(gameObject);
        }
    }
}