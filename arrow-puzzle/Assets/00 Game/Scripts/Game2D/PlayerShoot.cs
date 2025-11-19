using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle
{
    public class PlayerShoot : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public Transform firePos;

        [Header("Auto Shooting Settings")]
        public EnemyManager enemyManager;
        public float autoShootInterval = 1f;  

        private float nextShootTime = 0f;

        void Update()
        {
            
            // if (Input.GetButtonDown("Fire1"))
            // {
            //     Shoot();
            // }

            if (Time.time >= nextShootTime)
            {
                AutoShootNearestEnemy();
                nextShootTime = Time.time + autoShootInterval;
            }
        }

        void Shoot()
        {
            if (bulletPrefab && firePos)
            {
                Instantiate(bulletPrefab, firePos.position, firePos.rotation);
            }
        }

        void AutoShootNearestEnemy()
        {
            if (!enemyManager || !bulletPrefab || !firePos) return;

            // Lấy enemy gần nhất từ EnemyManager
            Transform nearest = enemyManager.GetNearestEnemy(transform.position);
            if (nearest == null) return;

            // Xác định hướng bắn
            Vector2 dir = (nearest.position - firePos.position).normalized;
            Quaternion rot = Quaternion.FromToRotation(Vector2.right, dir);

            // Sinh mũi tên theo hướng tới enemy gần nhất
            Instantiate(bulletPrefab, firePos.position, rot);
        }
    }
}
