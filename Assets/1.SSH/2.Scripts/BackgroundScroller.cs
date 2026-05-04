using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Header("Scroll")]
    [Tooltip("낮을수록 느리게 내려감. Bottom < Middle < Top 으로 다르게 두면 패럴랙스")]
    public float scrollSpeed = 2f;

    [Tooltip("0이면 자식 SpriteRenderer bounds 높이 사용. 스프라이트 없을 때만 수동 입력")]
    [SerializeField] float manualLoopHeight;

    Transform _t;
    float _loopHeight;
    Vector3 _startWorldPos;
    bool _valid;

    void Awake()
    {
        _t = transform;
    }

    void Start()
    {
        _startWorldPos = _t.position;
        _loopHeight = ResolveLoopHeight();
        _valid = _loopHeight > 0.001f;
    }

    float ResolveLoopHeight()
    {
        if (manualLoopHeight > 0f)
            return manualLoopHeight;

        var sr = GetComponentInChildren<SpriteRenderer>();
        return sr != null ? sr.bounds.size.y : 0f;
    }

    void OnValidate()
    {
        if (manualLoopHeight < 0f)
            manualLoopHeight = 0f;
    }

    void Update()
    {
        if (!_valid)
            return;

        _t.Translate(Vector3.down * (scrollSpeed * Time.deltaTime), Space.World);

        if (_t.position.y <= _startWorldPos.y - _loopHeight)
            _t.position = _startWorldPos;
    }
}
