using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle
{
    [System.Serializable]
    public class Wave
    {
        [Tooltip("Prefab enemy sẽ spawn trong wave này")]
        public GameObject enemyPrefab;

        [Tooltip("Số lượng enemy của wave")]
        public int count = 3;

        [Tooltip("Thời gian giữa các lần spawn (giây)")]
        public float spawnInterval = 0.4f;
    }
}
