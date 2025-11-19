using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("Waves")]
        public GameObject defaultEnemyPrefab;
        public Wave[] waves = new Wave[3];

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


        void Start()
        {
            FindPortal();

            if (portal != null)
                portal.SetActive(false);

            StartCoroutine(RunWaves());
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


        IEnumerator RunWaves()
        {
            for (int i = 0; i < waves.Length; i++)
            {
                yield return StartCoroutine(SpawnWave(waves[i]));
                yield return new WaitUntil(() => ActiveEnemies.Count == 0);
            }
        }


        IEnumerator SpawnWave(Wave wave)
        {
            GameObject enemyPrefab = wave.enemyPrefab != null ? wave.enemyPrefab : defaultEnemyPrefab;
            if (enemyPrefab == null) yield break;

            if (portal == null || summonPoint == null)
            {
                FindPortal();
                if (portal == null || summonPoint == null)
                {
                    Debug.LogError("[EnemyManager] Không thể spawn vì thiếu portal hoặc SummonPoint.");
                    yield break;
                }
            }

            // 1) Bật cổng
            portal.SetActive(true);

            yield return new WaitForSeconds(portalOpenDelay);

            // 2) Spawn enemy tại SummonPoint
            for (int i = 0; i < wave.count; i++)
            {
                Vector3 spawnPos = summonPoint.position;

                GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                // fade-in nếu có script DOTween
                EnemySpawnFade fade = enemyObj.GetComponent<EnemySpawnFade>();
                if (fade != null)
                    fade.PlaySpawnFX();

                yield return new WaitForSeconds(wave.spawnInterval);
            }

            // 3) Tắt portal
            yield return new WaitForSeconds(portalCloseDelay);
            portal.SetActive(false);
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
