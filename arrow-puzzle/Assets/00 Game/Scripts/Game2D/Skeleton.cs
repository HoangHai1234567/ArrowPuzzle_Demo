using System.Collections;
using System.Collections.Generic;
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
        private bool facingRight = true;

        [Header("Death Effects")]
        public float deathJumpforce = 5f;
        public float deathTorque = 10f;
        public float deathGravotuyScale = 2f;
        public float maxDeathTime = 4f;

        [Header("Attack")]
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackRange = 0.5f;
        [SerializeField] private float attackCooldown = 6f;
        [SerializeField] private float attackDuration = 0.7f;
        public Transform attackPoint;
        [SerializeField] private LayerMask towerLayer;

        private float nextAttackTime = 0f;
        private bool isAttacking = false;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            targetTower = GameObject.FindGameObjectWithTag("Tower")?.transform;
        }

        void FixedUpdate()
        {
            // 🔍 ONLY DEBUG HERE
            // Debug.Log(
            //     $"[Skeleton] {name} | t={Time.time:F2} | " +
            //     $"Grounded={isGrounded} | Walking={isWalking} | Attacking={isAttacking} | " +
            //     $"InRange={IsTowerInRange()} | Vel={rb.velocity}"
            // );

            CheckGround();

            if (targetTower == null)
            {
                isWalking = false;
                isAttacking = false;

                if (animator)
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isAttacking", false);
                }
                return;
            }

            if (isGrounded && !isAttacking)
            {
                if (!IsTowerInRange())
                {
                    MoveTowardsTower();
                }
                else
                {
                    TryAttackInRange();
                }
            }
            else if (!isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
                isWalking = false;
            }

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

        void MoveTowardsTower()
        {
            Vector2 direction = (targetTower.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * Speed, rb.velocity.y);

            isWalking = true;

            if (direction.x > 0 && !facingRight)
                Flip();
            else if (direction.x < 0 && facingRight)
                Flip();
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
            float tourque = Random.Range(-deathTorque, deathTorque);
            rb.AddTorque(tourque, ForceMode2D.Impulse);

            var col = GetComponent<Collider2D>();
            if (col) col.isTrigger = true;

            rb.gravityScale = deathGravotuyScale;

            float t = 0f;
            while (t < maxDeathTime)
            {
                t += Time.deltaTime;

                if (Camera.main != null)
                {
                    Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
                    if (screenPoint.y < 0 || screenPoint.x < 0 || screenPoint.x > 1)
                        break;
                }

                yield return null;
            }

            Destroy(gameObject);
        }

        void TryAttackInRange()
        {
            if (Time.time < nextAttackTime)
            {
                isWalking = false;
                return;
            }

            rb.velocity = new Vector2(0f, rb.velocity.y);
            isWalking = false;
            isAttacking = true;

            StartCoroutine(ResetAttackState());

            nextAttackTime = Time.time + attackCooldown;
        }

        IEnumerator ResetAttackState()
        {
            yield return new WaitForSeconds(attackDuration);
            isAttacking = false;
        }

        void Attack()  // Animation Event
        {
            if (attackPoint == null) return;

            Collider2D hitTower = Physics2D.OverlapCircle(attackPoint.position, attackRange, towerLayer);

            if (hitTower != null)
            {
                Tower tower = hitTower.GetComponent<Tower>();
                if (tower != null)
                    tower.TakeDamage(attackDamage);
            }
        }

        bool IsTowerInRange()
        {
            Vector2 p = attackPoint ? (Vector2)attackPoint.position : (Vector2)transform.position;
            return Physics2D.OverlapCircle(p, attackRange, towerLayer) != null;
        }
    }
}
