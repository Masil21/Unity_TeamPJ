# SSH ↔ SDH 협업 가이드

> Cursor AI 참고용. SDH(신동혁)가 만든 Enemy_A 시스템과 연동하기 위해  
> SSH(팀원)가 **플레이어 총알 쪽에서 맞춰야 할 것**을 정리한 문서.

---

## 1. 현재 SDH가 만든 것

### Enemy A 프리팹 (`Assets/2.SDH/Prefabs/Enemy A`)
| 컴포넌트 | 설정 |
|---------|------|
| SpriteRenderer | Enemy A 스프라이트 |
| Animator | Idle(State=0) / Hit(State=1) 전환 |
| Rigidbody2D | Gravity Scale = 0 |
| CapsuleCollider2D | Is Trigger = ON |
| EnemyController.cs | 이동 + HP + 피격 처리 |

### EnemyController.cs 핵심 내용
```csharp
public int hp = 16;          // Enemy 체력

public void TakeDamage(int damage)  // 총알에서 이걸 호출해줘
{
    hp -= damage;
    if (hp <= 0) Destroy(gameObject);  // HP 0이면 Enemy 파괴
    // 아니면 Hit 애니메이션 재생 후 Idle 복귀
}
```

### CreateEnemyManager 프리팹
- x축 랜덤(-2.5 ~ 2.5), y=5.5 위치에서 Enemy A 스폰
- 4~8초 간격 랜덤 생성
- y ≤ -5.5 아래로 떨어지면 자동 파괴

---

## 2. SSH가 맞춰야 할 것

### ① 플레이어 총알 프리팹 Tag → `PlayerBullet`
```
총알 프리팹 선택 → Inspector → Tag → PlayerBullet
(없으면 Edit → Tags & Layers → Tags에서 추가)
```

### ② 총알 프리팹에 Collider2D 필요
- CircleCollider2D 또는 CapsuleCollider2D 아무거나
- Trigger 여부 상관없음
- **Enemy A가 Rigidbody2D를 가지고 있으므로 총알엔 Rigidbody2D 없어도 됨**

### ③ 총알 스크립트에 `PlayerBullet` 클래스 + `damage` 변수 추가

총알 스크립트 이름을 **`PlayerBullet`** 으로 만들어줘.

```csharp
public class PlayerBullet : MonoBehaviour
{
    public int damage = 5;   // 데미지 값 (조절 가능)

    // 총알 이동 로직 ...

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>()?.TakeDamage(damage);
            // 총알 처리 (Destroy or 풀 반환)
            Destroy(gameObject);
        }
    }
}
```

---

## 3. 충돌 조건 체크리스트

| 항목 | SDH(Enemy A) | SSH(총알) |
|------|-------------|----------|
| Tag | `Enemy` ✅ 설정됨 | `PlayerBullet` ← **설정 필요** |
| Collider2D | CapsuleCollider2D(Trigger) ✅ | ← **추가 필요** |
| Rigidbody2D | ✅ 있음 | 없어도 됨 |
| 스크립트 | EnemyController.cs ✅ | PlayerBullet.cs ← **클래스명 맞춰줘** |

---

## 4. 전체 흐름

```
[SSH 총알] 발사
    └─ OnTriggerEnter2D ("Enemy" 태그 감지)
         └─ EnemyController.TakeDamage(damage) 호출
              ├─ hp > 0 → Hit 애니메이션 → Idle 복귀
              └─ hp <= 0 → Enemy 파괴
```

---

## 5. 수치 참고

| 항목 | 값 |
|------|-----|
| Enemy A HP | 16 |
| 총알 damage 추천 | 5 (히트 3발이면 처치) |
| Enemy 이동 속도 | 1f |
| Enemy 스폰 범위 x | -2.5 ~ 2.5 |
| 화면 밖 파괴 y | -5.5 이하 |
| 해상도 | 1920×1080 Portrait |
