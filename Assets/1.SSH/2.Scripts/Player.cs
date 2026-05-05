using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public int hp = 3;
    public int bulletDamage = 5;
    public float fireInterval = 0.2f;
    public Transform bulletSpawnOffset;
    public GameObject sideBulletPrefab;
    public GameObject centerBulletPrefab;
    public GameObject boomAnimationPrefab;
    public int power = 1;
    public float sideOffset = 0.25f;

    [SerializeField] float respawnDelay = 1f;

    private int _maxHp;
    private int _boomCount = 0;
    private Vector3 _startPos;
    private float _fireTimer;
    private float _halfWidth;
    private float _halfHeight;
    private Animator _animator;
    private bool _isInvincible = false;

    private const int StateIdle = 0;
    private const int StateLeft = 1;
    private const int StateRight = 2;

    void Start()
    {
        _maxHp = hp;
        _startPos = transform.position;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        _halfWidth = sr.bounds.extents.x;
        _halfHeight = sr.bounds.extents.y;
        _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        MovePlayer();

        if (Input.GetMouseButton(0))
        {
            _fireTimer += Time.deltaTime;
            if (_fireTimer >= fireInterval)
            {
                Fire();
                _fireTimer = 0f;
            }
        }
        else
        {
            _fireTimer = 0f;
        }

        if (Input.GetMouseButtonUp(1))
        {
            bool used = UIManager.Instance != null
                ? UIManager.Instance.UseBoom()
                : (_boomCount > 0 && --_boomCount >= 0);
            if (used && boomAnimationPrefab != null)
                Instantiate(boomAnimationPrefab, transform.position, Quaternion.identity);
        }
    }

    void Fire()
    {
        if (bulletSpawnOffset == null) return;

        switch (power)
        {
            case 1:
                SpawnBullet(sideBulletPrefab, Vector3.zero);
                break;
            case 2:
                SpawnBullet(sideBulletPrefab, Vector3.left * sideOffset);
                SpawnBullet(sideBulletPrefab, Vector3.right * sideOffset);
                break;
            case 3:
                SpawnBullet(centerBulletPrefab, Vector3.zero);
                SpawnBullet(sideBulletPrefab, Vector3.left * sideOffset);
                SpawnBullet(sideBulletPrefab, Vector3.right * sideOffset);
                break;
        }
    }

    void SpawnBullet(GameObject prefab, Vector3 offset)
    {
        if (prefab == null) return;
        GameObject bullet = Instantiate(prefab, bulletSpawnOffset.position + offset, Quaternion.identity);
        PlayerBullet pb = bullet.GetComponent<PlayerBullet>();
        if (pb != null) pb.damage = bulletDamage;
    }

    public void TakeDamage()
    {
        if (_isInvincible) return;
        hp--;

        if (hp <= 0)
        {
            hp = _maxHp;
            power = 1;
            StartCoroutine(RespawnRoutine());
        }
        else
        {
            StartCoroutine(InvincibleRoutine(1.5f));
        }
    }

    IEnumerator RespawnRoutine()
    {
        _isInvincible = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
        yield return new WaitForSeconds(respawnDelay);
        transform.position = _startPos;
        if (sr != null) sr.enabled = true;
        yield return new WaitForSeconds(2f);
        _isInvincible = false;
    }

    IEnumerator InvincibleRoutine(float duration)
    {
        _isInvincible = true;
        yield return new WaitForSeconds(duration);
        _isInvincible = false;
    }

    public void PowerUp()
    {
        if (power < 3) power++;
    }

    public void SetInvincible(bool value)
    {
        _isInvincible = value;
    }

    public void AddBoom()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.AddBoom();
        else
            _boomCount = Mathf.Min(_boomCount + 1, 3);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Item3 item = other.GetComponent<Item3>();
        if (item == null) return;

        switch (item.itemType)
        {
            case Item3.ItemType.Coin:
                if (UIManager.Instance != null) UIManager.Instance.AddScore(100);
                break;
            case Item3.ItemType.Power:
                PowerUp();
                if (UIManager.Instance != null) UIManager.Instance.AddScore(200);
                break;
            case Item3.ItemType.Boom:
                AddBoom();
                if (UIManager.Instance != null) UIManager.Instance.AddScore(300);
                break;
        }
        Destroy(other.gameObject);
    }

    void MovePlayer()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(h, v, 0f).normalized;
        transform.position += moveDir * (speed * Time.deltaTime);

        Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, min.x + _halfWidth, max.x - _halfWidth);
        pos.y = Mathf.Clamp(pos.y, min.y + _halfHeight, max.y - _halfHeight);
        transform.position = pos;

        if (_animator != null)
        {
            if (h > 0) _animator.SetInteger("State", StateRight);
            else if (h < 0) _animator.SetInteger("State", StateLeft);
            else _animator.SetInteger("State", StateIdle);
        }
    }
}
