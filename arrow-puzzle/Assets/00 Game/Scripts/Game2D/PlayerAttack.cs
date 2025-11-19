using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private Animator animator;    
        [SerializeField] private float attackDuration = 0.8f; 
        [SerializeField] private float attackCooldown = 2f; 

        private bool isAttacking = false;
        private float lastAttackTime = 0f;

        void Update()
        {
            HandleAttackInput();
        }

        private void HandleAttackInput()
        {
            if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastAttackTime + attackCooldown)
            {
                StartAttack();
            }

            // Tự động tắt trạng thái sau khi hết thời gian animation
            if (isAttacking && Time.time >= lastAttackTime + attackDuration)
            {
                StopAttack();
            }
        }

        private void StartAttack()
        {
            isAttacking = true;
            lastAttackTime = Time.time;

            // Kích hoạt animation tấn công
            animator.SetBool("isAttacking", true);
        }

        private void StopAttack()
        {
            isAttacking = false;
            animator.SetBool("isAttacking", false);
        }
    }
}
