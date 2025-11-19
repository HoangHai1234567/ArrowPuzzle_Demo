using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace ArrowsPuzzle
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] private float health = 500f;
        // Start is called before the first frame update
        void Start()
        {
            SpawnBounce();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void TakeDamage(float damage)
        {
            // Xử lý khi tháp bị tấn công
            Debug.Log("Dau qua");
            transform.DOShakePosition(
                duration: 0.3f,
                strength: new Vector3(0.1f, 0f, 0f),
                vibrato: 20,
                randomness: 0,
                snapping: false,
                fadeOut: true
            );
        }

        public void SpawnBounce()
        {
            Vector3 originalPos = transform.position;

            // Đẩy tower lên cao 2 units trước khi drop
            transform.position = originalPos + new Vector3(0, 2f, 0);

            // Tạo tween rơi xuống + bounce
            transform.DOMoveY(originalPos.y, 0.5f)
                .SetEase(Ease.OutBounce);
        }
    }
}
