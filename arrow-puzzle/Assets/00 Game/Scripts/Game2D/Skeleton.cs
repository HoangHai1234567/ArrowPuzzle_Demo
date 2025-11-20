using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace ArrowsPuzzle
{
    public class Skeleton : MonoBehaviour
    {
        [Header("Stats")]
        public float Health = 100f;
        public float Speed = 2f;

        [Header("Ground Check")]
        public Transform groundCheck;
        public float groundCheckRadius = 0.2f;
        public LayerMask groundLayer;

        private Rigidbody2D rb;
        private Animator animator;
        private Transform targetTower;
        private bool isGrounded;
        private bool isWalking;
        private bool isAttacking = false;
        private bool facingRight = true;

        [Header("Death Effects")]
        public float deathJumpforce = 5f;
        public float deathTorque = 10f;
        public float deathGravotuyScale = 2f;
        public float maxDeathTime = 4f;

        [Header("Stand Settings")]
        public Vector3 standPosition;
        public int slotIndex = -1;
        private bool reachedStandPoint = false;
        private float reachThreshold = 0.5f;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            targetTower = GameObject.FindGameObjectWithTag("Tower")?.transform;
        }

        void FixedUpdate()
        {
            CheckGround();

            if (!reachedStandPoint)
            {
                MoveToStandPoint();
            }
            else
            {
                StandAndFaceTower();
            }

            // UPDATE ANIMATOR
            if (animator)
            {
                animator.SetBool("isWalking", isWalking);
                animator.SetBool("isAttacking", isAttacking);
            }
        }

        void CheckGround()
        {
            if (groundCheck == null) return;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        /// <summary>
        /// Enemy đi từ portal → standPosition
        /// </summary>
        void MoveToStandPoint()
        {
            Vector2 current = rb.position;
            Vector2 target = standPosition;

            float dist = (target - current).magnitude;

            if (dist <= reachThreshold)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
                reachedStandPoint = true;
                isWalking = false;
                isAttacking = false;
                return;
            }

            Vector2 dir = (target - current).normalized;
            rb.velocity = new Vector2(dir.x * Speed, rb.velocity.y);

            isWalking = true;
            isAttacking = false;

            FaceTowerOnly();
        }

        /// <summary>
        /// Sau khi đến vị trí → đứng yên, idle, nhìn tower
        /// </summary>
        void StandAndFaceTower()
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            isWalking = false;
            isAttacking = false;
            FaceTowerOnly();
        }

        void FaceTowerOnly()
        {
            if (targetTower == null) return;

            float dx = targetTower.position.x - transform.position.x;
            if (dx > 0 && !facingRight) Flip();
            if (dx < 0 && facingRight) Flip();
        }

        void Flip()
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;
            if (Health <= 0)
                Die();
        }

        void Die()
        {
            isWalking = false;
            isAttacking = false;

            if (animator)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false);
            }

            gameObject.tag = "Untagged";

            EnemyManager manager = FindObjectOfType<EnemyManager>();
            if (manager != null)
                manager.OnEnemyDied(this);

            this.enabled = false;
            StartCoroutine(DeathFallRoutine());
        }

        IEnumerator DeathFallRoutine()
        {
            if (rb == null) yield break;

            rb.isKinematic = false;
            rb.simulated = true;
            rb.velocity = Vector2.zero;

            rb.constraints = RigidbodyConstraints2D.None;

            rb.AddForce(Vector2.up * deathJumpforce, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-deathTorque, deathTorque), ForceMode2D.Impulse);

            var col = GetComponent<Collider2D>();
            if (col) col.isTrigger = true;

            rb.gravityScale = deathGravotuyScale;

            float t = 0f;
            while (t < maxDeathTime)
            {
                t += Time.deltaTime;

                Vector3 sp = Camera.main.WorldToViewportPoint(transform.position);
                if (sp.y < 0 || sp.x < 0 || sp.x > 1)
                    break;

                yield return null;
            }

            Destroy(gameObject);
        }

        // Attack block — vẫn giữ cho animator nhưng không dùng gameplay
        void Attack() { }
    }
}
