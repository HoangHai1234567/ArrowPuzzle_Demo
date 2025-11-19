using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace ArrowsPuzzle
{
    public class EnemySpawnFade : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float fadeDuration = 0.5f;

        [Header("Movement Scripts")]
        [Tooltip("Các script sẽ được bật lại sau khi fade xong (ví dụ: Skeleton).")]
        [SerializeField] private MonoBehaviour[] scriptsToEnableAfterFade;

        private bool hasPlayed = false;

        void Awake()
        {
            if (!spriteRenderer)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            // Tắt toàn bộ script movement ban đầu
            if (scriptsToEnableAfterFade != null)
            {
                foreach (var s in scriptsToEnableAfterFade)
                {
                    if (s != null)
                        s.enabled = false;
                }
            }

            // Đảm bảo alpha = 0 khi mới spawn
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = 0f;
                spriteRenderer.color = c;
            }
        }

        public void PlaySpawnFX()
        {
            if (hasPlayed) return;
            hasPlayed = true;

            if (spriteRenderer == null)
            {
                EnableMovement();
                return;
            }

            // Đặt lại alpha = 0 cho chắc
            Color c = spriteRenderer.color;
            c.a = 0f;
            spriteRenderer.color = c;

            // Fade alpha từ 0 → 1
            spriteRenderer
                .DOFade(1f, fadeDuration)
                .SetUpdate(false) // chạy theo Time.timeScale bình thường
                .OnComplete(EnableMovement);
        }

        private void EnableMovement()
        {
            if (scriptsToEnableAfterFade != null)
            {
                foreach (var s in scriptsToEnableAfterFade)
                {
                    if (s != null)
                        s.enabled = true;
                }
            }
        }
    }
}
