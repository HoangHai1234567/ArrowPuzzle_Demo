using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BirdController : MonoBehaviour
{
    private enum BirdState
    {
        Flying,
        Landing,
        Pecking,
        TakingOff
    }

    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Fly Area (World Space)")]
    [SerializeField] private Vector2 flyMin = new Vector2(-8f, 6f);  // góc dưới trái vùng bay
    [SerializeField] private Vector2 flyMax = new Vector2(8f, 10f);  // góc trên phải vùng bay

    [Header("Landing Spots (Ground)")]
    [SerializeField] private Transform[] landingSpots;   // đặt vài empty dưới đất

    [Header("Movement Settings")]
    [SerializeField] private float flySpeed = 3f;
    [SerializeField] private float landSpeed = 4f;

    [Header("Behaviour Settings")]
    [SerializeField, Tooltip("Xác suất sau mỗi lần bay xong thì quyết định đáp xuống")]
    private float landChance = 0.4f;

    [SerializeField] private int minPeckCount = 2;
    [SerializeField] private int maxPeckCount = 5;
    [SerializeField] private float peckInterval = 0.4f;

    private BirdState _state = BirdState.Flying;
    private Tweener _moveTween;

    private void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        StartCoroutine(BirdRoutine());
    }

    private void OnDisable()
    {
        if (_moveTween != null && _moveTween.IsActive())
        {
            _moveTween.Kill();
        }
    }

    IEnumerator BirdRoutine()
    {
        while (true)
        {
            // 1. Đang bay random
            _state = BirdState.Flying;
            animator.SetBool("isFlying", true);

            // bay tới 1 điểm ngẫu nhiên trong vùng bay
            Vector3 flyTarget = GetRandomFlyPoint();
            yield return FlyTo(flyTarget, flySpeed);

            // 2. Quyết định có đáp xuống không
            bool shouldLand = landingSpots != null && landingSpots.Length > 0 &&
                              Random.value < landChance;

            if (!shouldLand)
            {
                // không đáp -> tiếp tục vòng lặp bay
                continue;
            }

            // 3. Landing
            _state = BirdState.Landing;
            Transform landSpot = GetRandomLandingSpot();
            Vector3 landTarget = landSpot.position;

            // tắt cờ bay (để animator có thể chuyển sang state idle trên đất nếu có)
            animator.SetBool("isFlying", false);
            yield return FlyTo(landTarget, landSpeed);

            // 4. Pecking (mổ mổ)
            _state = BirdState.Pecking;
            int peckCount = Random.Range(minPeckCount, maxPeckCount + 1);

            for (int i = 0; i < peckCount; i++)
            {
                animator.SetTrigger("Peck");
                yield return new WaitForSeconds(peckInterval);
            }

            // 5. Takeoff – bay lên lại vùng trên trời
            _state = BirdState.TakingOff;
            animator.SetBool("isFlying", true);

            Vector3 takeoffTarget = GetRandomFlyPoint();
            yield return FlyTo(takeoffTarget, flySpeed);

            // quay lại vòng while -> Flying tiếp
        }
    }

    Vector3 GetRandomFlyPoint()
    {
        float x = Random.Range(flyMin.x, flyMax.x);
        float y = Random.Range(flyMin.y, flyMax.y);
        return new Vector3(x, y, transform.position.z);
    }

    Transform GetRandomLandingSpot()
    {
        if (landingSpots == null || landingSpots.Length == 0)
            return transform; // fallback

        int idx = Random.Range(0, landingSpots.Length);
        return landingSpots[idx];
    }

    IEnumerator FlyTo(Vector3 target, float speed)
    {
        if (_moveTween != null && _moveTween.IsActive())
            _moveTween.Kill();

        float distance = Vector3.Distance(transform.position, target);
        float duration = distance / Mathf.Max(speed, 0.01f);

        // xoay chim theo hướng bay (nếu side-view)
        Vector3 dir = target - transform.position;
        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (dir.x > 0 ? 1 : -1);
            transform.localScale = scale;
        }

        bool done = false;
        _moveTween = transform.DOMove(target, duration)
                              .SetEase(Ease.Linear)
                              .OnComplete(() => done = true);

        while (!done)
            yield return null;
    }
}
