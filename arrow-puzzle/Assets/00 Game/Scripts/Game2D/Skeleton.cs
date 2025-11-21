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

        Rigidbody2D rb;
        Animator animator;
        Transform targetTower;

        bool isGrounded;
        bool isWalking;
        bool isAttacking;
        bool facingRight = true;

        [Header("Death Effects")]
        public float deathJumpforce = 5f;
        public float deathTorque = 10f;
        public float deathGravotuyScale = 2f;
        public float maxDeathTime = 4f;

        [Header("Stand Settings")]
        public Vector3 standPosition;
        public int slotIndex = -1;
        [SerializeField] float reachThreshold = 0.05f;
        bool reachedStandPoint = false;

        [SerializeField] float attackDamage = 20f;
        Tower tower;

        bool isInAttackSequence = false;
        public bool IsInAttackSequence => isInAttackSequence;

        Sequence attackSequence;

        public Transform attackPoint;
        [SerializeField] float attackRange = 0.5f;
        [SerializeField] LayerMask towerLayer;

        public bool CanBeSelectedForAttack()
        {
            return Health > 0f && reachedStandPoint && !isInAttackSequence;
        }

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            targetTower = GameObject.FindGameObjectWithTag("Tower")?.transform;
        }

        void FixedUpdate()
        {
            if (isInAttackSequence)
            {
                FaceTowerOnly();
                return;
            }

            CheckGround();

            if (!reachedStandPoint)
                MoveToStandPoint();
            else
                StandAndFaceTower();

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

        void MoveToStandPoint()
        {
            Vector2 current = rb.position;
            Vector2 target = standPosition;

            Vector2 dir = target - current;
            float dist = dir.magnitude;

            if (dist <= reachThreshold)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
                reachedStandPoint = true;
                isWalking = false;
                isAttacking = false;
                return;
            }

            dir.Normalize();
            rb.velocity = new Vector2(dir.x * Speed, rb.velocity.y);
            isWalking = true;
            isAttacking = false;

            FaceTowerOnly();
        }

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

            if (dx > 0 && !facingRight)
                Flip();
            else if (dx < 0 && facingRight)
                Flip();
        }

        void Flip()
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        public void StartAttackSequence(
            Vector3 attackPosition,
            float moveDuration,
            float stayDuration,
            float returnDuration)
        {
            if (isInAttackSequence || Health <= 0f) return;

            isInAttackSequence = true;
            isWalking = false;
            isAttacking = false;

            if (attackSequence != null && attackSequence.IsActive())
                attackSequence.Kill();

            Vector3 originPos = transform.position;

            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
            }

            if (animator != null)
            {
                animator.enabled = true;
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false);
            }

            attackSequence = DOTween.Sequence();

            Vector3 attackPosX = new Vector3(attackPosition.x, originPos.y, originPos.z);

            attackSequence.Append(
                transform.DOMove(attackPosX, moveDuration).SetEase(Ease.OutQuad)
            );

            attackSequence.AppendCallback(() =>
            {
                isWalking = false;
                isAttacking = false;

                if (animator)
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isAttacking", false);
                }
            });

            float idleBeforeAttack = 0.25f;
            attackSequence.AppendInterval(idleBeforeAttack);

            attackSequence.AppendCallback(() =>
            {
                isAttacking = true;

                if (animator)
                {
                    animator.enabled = true;
                    animator.SetBool("isAttacking", true);
                }
            });

            attackSequence.AppendInterval(stayDuration);

            attackSequence.AppendCallback(() =>
            {
                isAttacking = false;

                if (animator)
                    animator.SetBool("isAttacking", false);
            });

            Vector3 returnPosX = new Vector3(standPosition.x, originPos.y, originPos.z);

            attackSequence.Append(
                transform.DOMove(returnPosX, returnDuration).SetEase(Ease.InQuad)
            );

            attackSequence.OnComplete(() =>
            {
                if (rb != null) rb.isKinematic = false;

                transform.position = new Vector3(standPosition.x, originPos.y, originPos.z);

                reachedStandPoint = true;
                isWalking = false;
                isAttacking = false;
                isInAttackSequence = false;

                if (animator)
                {
                    animator.enabled = true;
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isAttacking", false);
                }
            });
        }

        public void TakeDamage(float damage)
        {
            if (Health <= 0f) return;

            Health -= damage;

            if (Health <= 0f)
                Die();
        }

        void Die()
        {
            isWalking = false;
            isAttacking = false;
            isInAttackSequence = false;

            if (attackSequence != null && attackSequence.IsActive())
                attackSequence.Kill();

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

        bool IsTowerInRange()
        {
            Vector2 center = attackPoint ? (Vector2)attackPoint.position : (Vector2)transform.position;
            Collider2D hitTower = Physics2D.OverlapCircle(center, attackRange, towerLayer);
            return hitTower != null;
        }

        public void Attack()
        {
            if (!isAttacking)
                return;

            if (!IsTowerInRange())
                return;

            Vector2 center = attackPoint ? (Vector2)attackPoint.position : (Vector2)transform.position;
            Collider2D hitTower = Physics2D.OverlapCircle(center, attackRange, towerLayer);

            if (hitTower != null)
            {
                Tower tw = hitTower.GetComponent<Tower>();
                if (tw != null)
                    tw.TakeDamage(attackDamage);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (attackPoint == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
