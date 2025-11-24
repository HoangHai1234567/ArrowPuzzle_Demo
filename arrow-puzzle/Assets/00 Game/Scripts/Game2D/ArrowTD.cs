using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle
{
    public class ArrowTD : MonoBehaviour
    {
        public float Speed = 5f;
        public int ArrowDamage = 5;
        public Rigidbody2D rigidBody2D;
        public bool hasHit = false;

        [Header("Range / Lifetime")]
        [Tooltip("Mũi tên sẽ tự hủy nếu bay xa hơn khoảng cách này (tính từ vị trí spawn).")]
        public float maxTravelDistance = 30f;

        [Tooltip("Thời gian tối đa tồn tại (fallback nếu không trúng gì).")]
        public float maxLifeTime = 5f;

        private bool stuck = false;
        private Vector3 _spawnPosition;
        private float _spawnTime;

        void Start()
        {
            _spawnPosition = transform.position;
            _spawnTime = Time.time;

            if (rigidBody2D == null)
                rigidBody2D = GetComponent<Rigidbody2D>();

            if (rigidBody2D != null)
            {
                rigidBody2D.velocity = transform.right * Speed;
            }
        }

        void Update()
        {
            if (!stuck)
            {
                RotateArrow();
                CheckBulletRangeByDistanceOrTime();
            }
        }

        void CheckBulletRangeByDistanceOrTime()
        {
            // Giới hạn theo khoảng cách
            if (Vector3.Distance(transform.position, _spawnPosition) > maxTravelDistance)
            {
                Destroy(gameObject);
                return;
            }

            // Giới hạn theo thời gian sống
            if (Time.time - _spawnTime > maxLifeTime)
            {
                Destroy(gameObject);
            }
        }

        void RotateArrow()
        {
            if (rigidBody2D == null) return;

            Vector2 direction = rigidBody2D.velocity;
            if (direction.sqrMagnitude <= 0.0001f) return;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        void OnTriggerEnter2D(Collider2D hitInfo)
        {
            if (!hitInfo.CompareTag("Enemy")) return;
            if (stuck) return;

            hasHit = true;

            Skeleton enemy = hitInfo.GetComponent<Skeleton>();
            if (enemy != null)
                enemy.TakeDamage(ArrowDamage);

            if (rigidBody2D != null)
            {
                rigidBody2D.velocity = Vector2.zero;
                rigidBody2D.angularVelocity = 0f;
            }

            Rigidbody2D enemyRb = hitInfo.attachedRigidbody;
            if (enemyRb == null) enemyRb = hitInfo.GetComponentInParent<Rigidbody2D>();

            if (enemyRb != null)
            {
                // Ước lượng điểm găm bằng điểm gần nhất trên collider enemy
                Vector2 worldPoint = hitInfo.ClosestPoint(transform.position);

                var joint = gameObject.AddComponent<FixedJoint2D>();
                joint.connectedBody = enemyRb;
                joint.autoConfigureConnectedAnchor = false;
                joint.anchor = transform.InverseTransformPoint(worldPoint);
                joint.connectedAnchor = enemyRb.transform.InverseTransformPoint(worldPoint);
                joint.enableCollision = false;
            }

            var col = GetComponent<Collider2D>();
            if (col) col.enabled = false;

            stuck = true;
        }
    }
}
