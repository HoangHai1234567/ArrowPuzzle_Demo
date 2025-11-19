using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle
{
    [Serializable]
    public class LevelData
    {
        public int Width;

        public int Height;

        public int Time;

        public Dictionary<int, Vector2Int[]> Arrows;
    }
}
