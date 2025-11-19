using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle
{
    public class Bow : MonoBehaviour
    {
        [Header("Auto Rotation Settings")]
        public Transform firePos;               
        public EnemyManager enemyManager;       
        public Transform player;                

        private void Update()
        {
            AutoRotateBow();
        }

        private void AutoRotateBow()
        {
            if (!enemyManager || !firePos || !player) return;

            // Lấy enemy gần nhất quanh player
            Transform nearestEnemy = enemyManager.GetNearestEnemy(player.position);
            if (nearestEnemy == null) return;

            // Tính hướng từ cung tới enemy
            Vector2 direction = nearestEnemy.position - firePos.position;

            // Tính góc quay
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Gán rotation cho cung
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
