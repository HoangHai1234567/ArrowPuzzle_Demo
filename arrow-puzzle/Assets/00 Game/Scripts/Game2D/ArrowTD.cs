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

        bool stuck = false;

        void Start()
        {
            rigidBody2D.velocity = transform.right * Speed;
        }

        void Update()
        {
            CheckBulletRange();
            if (!stuck) RotateArrow();
        }

        void CheckBulletRange()
        {
            if (Camera.main == null) return;
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
            if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1)
            {
                Destroy(gameObject);
            }
        }

        void RotateArrow()
        {
            Vector2 direction = rigidBody2D.velocity;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        void OnTriggerEnter2D(Collider2D hitInfo)
        {
            if (!hitInfo.CompareTag("Enemy")) return;
            if (stuck) return;

            hasHit = true;


            Skeleton enemy = hitInfo.GetComponent<Skeleton>();
            if (enemy != null) enemy.TakeDamage(ArrowDamage);


            rigidBody2D.velocity = Vector2.zero;
            rigidBody2D.angularVelocity = 0f;


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
