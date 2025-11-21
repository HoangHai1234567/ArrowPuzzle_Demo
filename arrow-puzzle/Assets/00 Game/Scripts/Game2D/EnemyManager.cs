using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("Enemy Spawn")]
        public GameObject defaultEnemyPrefab;

        [Tooltip("3 vị trí cố định mà enemy sẽ đứng trên scene")]
        public Transform[] standPoints = new Transform[3];

        [Header("Portal Settings")]
        public string portalTag = "Portal";
        public float portalOpenDelay = 0.4f;
        public float portalCloseDelay = 0.5f;

        [Header("Tracking")]
        public string enemyTag = "Enemy";
        public float scanInterval = 0.25f;

        [Header("Attack Sequence Settings")]
        [Tooltip("Điểm enemy sẽ float tới khi tấn công (đặt 1 empty gần tower)")]
        public Transform towerAttackPoint;
        public float attackMoveDuration = 0.4f;
        public float attackStayDuration = 0.4f;
        public float attackReturnDuration = 0.4f;

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

            for (int i = 0; i < standPoints.Length; i++)
            {
                if (standPoints[i] == null) continue;
                StartCoroutine(SpawnEnemyToSlot(i));
            }

            StartCoroutine(ScanEnemiesLoop());
        }

        void FindPortal()
        {
            portal = GameObject.FindGameObjectWithTag(portalTag);
            if (portal == null) return;

            portalTransform = portal.transform;
            summonPoint = portalTransform.Find("SummonPoint");
        }

        IEnumerator SpawnEnemyToSlot(int slotIndex)
        {
            if (defaultEnemyPrefab == null)
                yield break;

            if (portal == null || summonPoint == null)
            {
                FindPortal();
                if (portal == null || summonPoint == null)
                    yield break;
            }

            Transform standPoint = standPoints[slotIndex];
            if (standPoint == null)
                yield break;

            portal.SetActive(true);
            yield return new WaitForSeconds(portalOpenDelay);

            Vector3 spawnPos = summonPoint.position;
            GameObject enemyObj = Instantiate(defaultEnemyPrefab, spawnPos, Quaternion.identity);

            var fade = enemyObj.GetComponent<EnemySpawnFade>();
            if (fade != null)
                fade.PlaySpawnFX();

            Skeleton skeleton = enemyObj.GetComponent<Skeleton>();
            if (skeleton != null)
            {
                skeleton.slotIndex = slotIndex;
                skeleton.standPosition = standPoint.position;
            }

            RegisterEnemy(enemyObj.transform);

            yield return new WaitForSeconds(portalCloseDelay);
            portal.SetActive(false);
        }

        public void RegisterEnemy(Transform enemy)
        {
            if (!enemy) return;
            if (!ActiveEnemies.Contains(enemy))
                ActiveEnemies.Add(enemy);
        }

        public void OnEnemyDied(Skeleton skeleton)
        {
            if (skeleton == null) return;

            ActiveEnemies.Remove(skeleton.transform);

            int slot = skeleton.slotIndex;
            if (slot >= 0 && slot < standPoints.Length && standPoints[slot] != null)
                StartCoroutine(SpawnEnemyToSlot(slot));
        }

        public void TriggerRandomEnemyAttack()
        {
            if (towerAttackPoint == null)
                return;

            List<Skeleton> candidates = new List<Skeleton>();

            foreach (Transform t in ActiveEnemies)
            {
                if (!t) continue;
                Skeleton s = t.GetComponent<Skeleton>();
                if (s != null && s.CanBeSelectedForAttack())
                    candidates.Add(s);
            }

            if (candidates.Count == 0)
                return;

            Skeleton chosen = candidates[Random.Range(0, candidates.Count)];

            chosen.StartAttackSequence(
                towerAttackPoint.position,
                attackMoveDuration,
                attackStayDuration,
                attackReturnDuration
            );
        }

        IEnumerator ScanEnemiesLoop()
        {
            var found = new List<Transform>(8);

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

                Skeleton s = t.GetComponent<Skeleton>();
                if (s != null && s.IsInAttackSequence)
                    continue; // đang float lên / float về → không cho target

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
