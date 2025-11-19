using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArrowsPuzzle.Editor
{
    public class EditorLevelData
    {
        public Dictionary<int, EditorArrow> Arrows;

        public int Width = 10;

        public int Height = 10;

        private int[,] _map;
        public EditorLevelData()
        {
            Arrows = new Dictionary<int, EditorArrow>();
            _map = new int[Height, Width];
        }

        public void Draw(MeshGenerationContext mc)
        {
            foreach (var pair in Arrows)
            {
                pair.Value.Draw(mc);
            }
        }

        public EditorArrow CreateNewArrow(int id, Vector2Int node)
        {
            if (!Arrows.ContainsKey(id))
            {
                EditorArrow arrow = new EditorArrow(id);
                arrow.AddHeadNode(node);
                Arrows.Add(id, arrow);
                _map[node.y, node.x] = id;
                return arrow;
            }

            return null;
        }

        public void ChangeLevelSize(int width, int height)
        {
            /*
            if (width >= Width && height >= Height)
            {
                int minWidth = Mathf.Min(Width, width);
                int minHeight = Mathf.Min(Height, height);

                
                int[,] map = new int[height, width];

                for (int h = 0; h < minHeight; h++)
                {
                    for (int w = 0; w < minWidth; w++)
                    {
                        map[h, w] = _map[h, w];
                    }
                }

                _map = map;
                Width = width;
                Height = height;
            }
            */
            Width = width;
            Height = height;
            _map = new int[Height,Width];
        }

        public void AddHeadNodeToArrow(EditorArrow arrow, Vector2Int node)
        {
            if (Arrows.ContainsKey(arrow.Id))
            {
                arrow.AddHeadNode(node);
                _map[node.y,node.x] = arrow.Id;
            }
        }

        public void AddTailNodeToArrow(EditorArrow arrow, Vector2Int node)
        {
            if (Arrows.ContainsKey(arrow.Id))
            {
                arrow.AddTailNode(node);
                _map[node.y, node.x] = arrow.Id;
            }
        }

        public void RemoveArrowHeadNode(EditorArrow arrow)
        {
            if (arrow.Length > 1 && Arrows.ContainsKey(arrow.Id))
            {
                var head = arrow.Head;
                arrow.RemoveHeadNode();
                _map[head.y, head.x] = 0;
            }
        }

        public void RemoveArrowTailNode(EditorArrow arrow)
        {
            if (arrow.Length > 1 && Arrows.ContainsKey(arrow.Id))
            {
                var tail = arrow.Tail;
                arrow.RemoveTailNode();
                _map[tail.y, tail.x] = 0;
            }
        }

        public void RemoveArrow(EditorArrow arrow)
        {
            if (Arrows.ContainsKey(arrow.Id))
            {
                Arrows.Remove(arrow.Id);
                foreach (var node in arrow.Nodes)
                {
                    _map[node.y, node.x] = 0;
                }
            }
        }

        public EditorArrow GetArrow(int id)
        {
            if (Arrows.ContainsKey(id))
            {
                return Arrows[id];
            }
            return null;
        }

        public void ClearAllArrows()
        {
            Arrows.Clear();
            _map = new int[Height, Width];
        }

        public bool CheckClickArrow(int x, int y, out EditorArrow arrow)
        {
            if (x >= 0 && x<Width && y >= 0 && y<Height)
            {
                int id = _map[y, x];
                if (Arrows.ContainsKey(id))
                {
                    arrow = Arrows[id];
                    return true;
                }
            }

            arrow = null;
            return false;
        }

        public int GetMaxArrowId()
        {
            if (Arrows.Count == 0)
            {
                return 1;
            }

            var max = Arrows.Keys.Max();
            return max + 1;
        }

        public bool IsEmptyNode(Vector2Int node)
        {
            return _map[node.y, node.x] == 0;
        }

        public void Create(int width, int height)
        {
            Width = width;
            Height = height;
            Clear();
        }

        public void Clear()
        {
            _map = new int[Height, Width];
            Arrows.Clear();
        }
    }
}
