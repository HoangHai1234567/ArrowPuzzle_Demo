using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle
{
    public class TDGameManager : MonoBehaviour
    {
        public Transform towerPrefabs;
        public Transform enemyManagerPrefabs;

        private Transform _currentTower;
        private Transform _currentEnemyManager;

        public void ResetTDGame()
        {
            if (_currentTower != null)
            {
                Destroy(_currentTower.gameObject);
            }

            if (_currentEnemyManager != null)
            {
                Destroy(_currentEnemyManager.gameObject);
            }

            _currentTower = Instantiate(towerPrefabs, new Vector3(0, 4, 0), Quaternion.identity);
            _currentEnemyManager = Instantiate(enemyManagerPrefabs);
        }
    }
}
