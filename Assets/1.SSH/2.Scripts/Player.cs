using TMPro;
using UnityEngine;

// 플레이어 프리팹: Assets/1.SSH/3.Prefab/Player.prefab
public class Player : MonoBehaviour
{
    public float speed = 5f;
    public GameObject bulletPrefab;
    public float fireInterval = 0.2f;
    public Transform bulletSpawnOffset;
    private float _fireTimer;
    private float _halfWidth;
    private float _halfHeight;
    private Animator _animator;
    private const int StateIdle = 0;
    private const int StateLeft = 1;
    private const int StateRight = 2;
    public GameObject sideBulletPrefab;
    public GameObject centerBulletPrefab;
    public int power = 1;
    public float sideOffset = 0.25f;

    void Start()
    {
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
            if (UIManager.Instance != null && UIManager.Instance.UseBoom())
            {
                // TODO: SkillBoom 프리팹 생성 — 폭탄 단계에서 구현
            }

        }
    }

    void Fire()
    {
        switch (power)
        {
            case 1:
                SpawnBullet(sideBulletPrefab, Vector3.zero);
                break;
            
            case 2:
                SpawnBullet(sideBulletPrefab,Vector3.left * sideOffset);
                SpawnBullet(sideBulletPrefab,Vector3.right * sideOffset);
                break;
            
            case 3:
                SpawnBullet(centerBulletPrefab, Vector3.zero);
                SpawnBullet(sideBulletPrefab,Vector3.left * sideOffset);
                SpawnBullet(sideBulletPrefab,Vector3.right * sideOffset);
                break;
        }
    }

    void SpawnBullet(GameObject prefab, Vector3 offset)
    {
        Instantiate(prefab, bulletSpawnOffset.position + offset, Quaternion.identity);
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

        // 애니메이션 처리
        if (_animator != null)
        {
            if (h > 0)
            {
                _animator.SetInteger("State", StateRight);
            }
            else if (h < 0)
            {
                _animator.SetInteger("State", StateLeft);
            }
            else
            {
                _animator.SetInteger("State", StateIdle);
            }
        }
    }
}