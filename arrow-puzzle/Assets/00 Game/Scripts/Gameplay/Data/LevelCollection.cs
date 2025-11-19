using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ArrowsPuzzle
{
    [CreateAssetMenu(fileName = "LevelCollection",menuName = "ArrowPuzzle/Data/Create Level Collection")]
    public class LevelCollection : ScriptableObject
    {
        [Required]
        [SerializeField] private LevelAssetData _defaultLevel;

        [SerializeField] private LevelAssetData[] _levels;

        public int Count => _levels.Length;

        public LevelAssetData GetLevel(int index)
        {
            index--;
            if (index >= 0 && index < _levels.Length)
            {
                return _levels[index];
            }

            return _defaultLevel;
        }
    }

    [Serializable]
    public class LevelAssetData
    {
        public AssetReference LevelAsset;

        public string Name;

        public string Description;
    }
}
