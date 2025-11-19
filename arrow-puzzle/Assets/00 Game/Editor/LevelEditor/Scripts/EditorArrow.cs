using System;
using System.Collections;
using System.Collections.Generic;
using DVLib.Graphics;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArrowsPuzzle.Editor
{
    public class EditorArrow
    {
        public int Id { get; private set; }

        public List<Vector2Int> Nodes { get; private set; } = new ();

        public Vector2Int Head=> Nodes [^1];

        public Vector2Int Tail => Nodes[0];

        public int Length => Nodes.Count;

        private List<Vector2> _points;

        private Vector2[] _headPolygon;

        private Color _normalColor = Color.cyan;

        private Color _highlightColor = Color.yellow;

        private Color _color;
        public EditorArrow(int id)
        {
            Id = id;
            _points = new List<Vector2>();
            _headPolygon = new Vector2[3];
            _color = _normalColor;
        }

        public void ChangeGridSize()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                var node = Nodes [i];
                _points[i] = EditorConfig.ConvertIndexToNodePosition(node.x, node.y);
            }

            CreateHeadPolygon();
        }

        public void AddHeadNode(Vector2Int node)
        {
            Nodes.Add(node);

            var point = EditorConfig.ConvertIndexToNodePosition(node.x, node.y);
            _points.Add(point);

            CreateHeadPolygon();
        }

        public void AddTailNode(Vector2Int node)
        {
            Nodes.Insert(0,node);

            var point = EditorConfig.ConvertIndexToNodePosition(node.x, node.y);
            _points.Insert(0,point);
        }

        public void RemoveHeadNode()
        {
            Nodes.RemoveAt(Nodes.Count - 1);
            _points.RemoveAt(_points.Count-1);

            CreateHeadPolygon();
        }

        public void RemoveTailNode()
        {
            Nodes.RemoveAt(0);
            _points.RemoveAt(0);
        }

        void CreateHeadPolygon()
        {
            if (_points.Count > 1)
            {
                var point = _points[^1];
                var point2 = _points[^2];
                float offset1 = EditorConfig.ArrowLineWidth * 1.3f;
                float offset2 = EditorConfig.ArrowLineWidth * 1.1f;
                if (point.y < point2.y)
                {
                    _headPolygon[0] = new Vector2(point.x, point.y - offset2);
                    _headPolygon[1] = new Vector2(point.x - offset1, point.y + offset2);
                    _headPolygon[2] = new Vector2(point.x + offset1, point.y + offset2);
                }
                else if (point.y > point2.y)
                {
                    _headPolygon[0] = new Vector2(point.x, point.y + offset2);
                    _headPolygon[1] = new Vector2(point.x - offset1, point.y - offset2);
                    _headPolygon[2] = new Vector2(point.x + offset1, point.y - offset2);
                }
                else
                {
                    if (point.x > point2.x)
                    {
                        _headPolygon[0] = new Vector2(point.x + offset2, point.y);
                        _headPolygon[1] = new Vector2(point.x - offset2, point.y - offset1);
                        _headPolygon[2] = new Vector2(point.x - offset2, point.y + offset1);
                    }
                    else
                    {
                        _headPolygon[0] = new Vector2(point.x - offset2, point.y);
                        _headPolygon[1] = new Vector2(point.x + offset2, point.y - offset1);
                        _headPolygon[2] = new Vector2(point.x + offset2, point.y + offset1);
                    }
                }
            }
        }

        public void Draw(MeshGenerationContext mc)
        {
            //Draw Arrow's head
            if (_points.Count > 1)
            {
                GraphicX.Polygon.Draw(mc, _points.ToArray(), EditorConfig.ArrowLineWidth, _color);
                GraphicX.Polygon.DrawFill(mc, _headPolygon, _color);
            }
            else
            {
                Rect rect = new Rect(Vector2.zero, new Vector2(EditorConfig.GridSize / 2.0f,
                    EditorConfig.GridSize / 2.0f));
                rect.center = _points[0];

                GraphicX.Rectangle.DrawFill(mc,rect,_color,true,1,_color,true);
            }
        }

        public void Normal()
        {
            _color = _normalColor;
        }

        public void Highlight()
        {
            _color = _highlightColor;
        }

        public void ChangeGridSize(int newSize, int oldSize)
        {
            float scale = 1.0f *newSize / oldSize;

            for (int i = 0; i < _points.Count; i++)
            {
                _points[i]*=scale;
            }

            CreateHeadPolygon();
        }
    }
}
