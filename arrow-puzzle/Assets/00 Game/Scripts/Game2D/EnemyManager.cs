using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("Enemy Prefab")]
        public GameObject defaultEnemyPrefab;

        [Header("Fixed Stand Positions (3 slots)")]
        public Transform[] standPoints = new Transform[3];

        [Header("Portal Settings")]
        public string portalTag = "Portal";
        public float portalOpenDelay = 0.4f;
        public float portalCloseDelay = 0.5f;

        [Header("Tracking")]
        public string enemyTag = "Enemy";
        public float scanInterval = 0.25f;

        private GameObject portal;
        private Transform portalTransform;
        private Transform summonPoint;

        public readonly List<Transform> ActiveEnemies = new List<Transform>();
        public readonly List<Vector3> EnemyPositions = new List<Vector3>();

        private void Start()
        {
            FindPortal();

            if (portal != null)
                portal.SetActive(false);

            // Spawn 1 enemy cho mỗi slot cố định
            for (int i = 0; i < standPoints.Length; i++)
            {
                if (standPoints[i] == null) continue;
                StartCoroutine(SpawnEnemyToSlot(i));
            }

            // vẫn giữ hệ thống scan để GetNearestEnemy hoạt động
            StartCoroutine(ScanEnemiesLoop());
        }

        void FindPortal()
        {
            portal = GameObject.FindGameObjectWithTag(portalTag);

            if (portal == null)
            {
                Debug.LogError("[EnemyManager] Không tìm thấy Portal với tag: " + portalTag);
                return;
            }

            portalTransform = portal.transform;

            summonPoint = portalTransform.Find("SummonPoint");

            if (summonPoint == null)
            {
                Debug.LogError("[EnemyManager] Portal không có child SummonPoint!");
            }
        }

        /// <summary>
        /// Spawn 1 enemy từ portal, đi bộ ra đúng standPoint của slot tương ứng.
        /// </summary>
        IEnumerator SpawnEnemyToSlot(int slotIndex)
        {
            if (defaultEnemyPrefab == null)
            {
                Debug.LogError("[EnemyManager] Chưa gán defaultEnemyPrefab!");
                yield break;
            }

            if (portal == null || summonPoint == null)
            {
                FindPortal();
                if (portal == null || summonPoint == null)
                {
                    Debug.LogError("[EnemyManager] Không thể spawn vì thiếu portal hoặc SummonPoint.");
                    yield break;
                }
            }

            Transform standPoint = standPoints[slotIndex];
            if (standPoint == null)
            {
                Debug.LogWarning("[EnemyManager] standPoints[" + slotIndex + "] chưa được gán.");
                yield break;
            }

            // Bật portal
            portal.SetActive(true);
            yield return new WaitForSeconds(portalOpenDelay);

            // Spawn enemy tại SummonPoint
            Vector3 spawnPos = summonPoint.position;
            GameObject enemyObj = Instantiate(defaultEnemyPrefab, spawnPos, Quaternion.identity);

            // Nếu có hiệu ứng spawn
            EnemySpawnFade fade = enemyObj.GetComponent<EnemySpawnFade>();
            if (fade != null)
                fade.PlaySpawnFX();

            // Thiết lập thông tin slot + điểm đứng cho Skeleton
            Skeleton skeleton = enemyObj.GetComponent<Skeleton>();
            if (skeleton != null)
            {
                skeleton.slotIndex = slotIndex;
                skeleton.standPosition = standPoint.position;
            }

            // Đăng ký enemy mới để PlayerShoot có thể target
            RegisterEnemy(enemyObj.transform);

            // Tắt portal sau 1 lúc
            yield return new WaitForSeconds(portalCloseDelay);
            portal.SetActive(false);
        }

        /// <summary>
        /// Được gọi khi spawn enemy mới → thêm vào hệ thống
        /// và thông báo PlayerShoot để xả băng đạn nếu có.
        /// </summary>
        public void RegisterEnemy(Transform enemy)
        {
            if (!enemy) return;

            ActiveEnemies.Add(enemy);

            // Báo PlayerShoot rằng có enemy → nếu đang có đạn tích trữ thì xả
            PlayerShoot shooter = FindObjectOfType<PlayerShoot>();
            if (shooter != null)
            {
                shooter.TryFireStoredBullets();
            }
        }

        /// <summary>
        /// Được gọi từ Skeleton.Die() khi enemy chết.
        /// Respawn con khác vào đúng slot.
        /// </summary>
        public void OnEnemyDied(Skeleton skeleton)
        {
            if (skeleton == null) return;

            // Xoá khỏi danh sách (scan loop sẽ dọn lại, nhưng xoá sớm cũng ok)
            ActiveEnemies.Remove(skeleton.transform);

            int slot = skeleton.slotIndex;
            if (slot >= 0 && slot < standPoints.Length && standPoints[slot] != null)
            {
                // Respawn enemy mới cho slot này
                StartCoroutine(SpawnEnemyToSlot(slot));
            }
        }

        IEnumerator ScanEnemiesLoop()
        {
            var found = new List<Transform>(64);

            while (true)
            {
                found.Clear();
                EnemyPositions.Clear();

                GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

                foreach (var enemy in enemies)
                {
                    if (enemy == null) continue;
                    found.Add(enemy.transform);
                    EnemyPositions.Add(enemy.transform.position);
                }

                ActiveEnemies.Clear();
                ActiveEnemies.AddRange(found);

                yield return new WaitForSeconds(scanInterval);
            }
        }

        public Transform GetNearestEnemy(Vector3 position)
        {
            if (ActiveEnemies.Count == 0) return null;

            Transform best = null;
            float bestSqr = float.MaxValue;

            foreach (Transform t in ActiveEnemies)
            {
                if (!t) continue;

                float d2 = (t.position - position).sqrMagnitude;
                if (d2 < bestSqr)
                {
                    bestSqr = d2;
                    best = t;
                }
            }
            return best;
        }
    }
}
