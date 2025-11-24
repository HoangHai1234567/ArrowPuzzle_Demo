using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using Shapes;
using UnityEngine;

namespace ArrowsPuzzle
{
    public class Arrow : MonoBehaviour
    {
        public enum Direction
        {
            Up,
            Right,
            Down,
            Left,
        }

        #region Serialize properties

        [SerializeField] private Polyline _body;

        [SerializeField] private LineRenderer _lineRenderer;

        [SerializeField] private SplineComputer _splineComputer;

        [SerializeField] private Transform _head;

        [SerializeField] private float _moveSpeed = 0.01f;

        [SerializeField] private Ease _moveEase;

        [SerializeField] private Color _normalColor = Color.white;

        [SerializeField] private float _normalColorFadeDuration = 0.4f;

        [SerializeField] private Color _errorColor = Color.red;

        [SerializeField] private Color _highlightColor = Color.blue;

        [SerializeField] private Color _hintColor = Color.green;

        [SerializeField] private float _errorColorFadeDuration = 0.5f;

        #endregion

        #region Private properties

        private double _percentHead = 0;
        private double _percentTail = 0;
        private double _length = 0;

        private float _segmentLength = 1;
        private int _segments = 1;
        private float _pathLength = 1;

        private readonly List<Vector3> _points = new();

        private SpriteRenderer _headRenderer;
        private Vector2Int[] _nodes;

        #endregion

        #region Public properties

        public int ID { get; private set; }

        public Vector2Int[] Nodes => _nodes;

        public Vector2Int Head => _nodes[^1];

        public Direction GoDirection { get; private set; }

        public bool IsMoving { get; private set; }

        public int Length => _nodes.Length;

        public int OutNodeLength = 20;

        #endregion

        #region Unity messages

        private void Start()
        {
            _headRenderer = _head.GetComponent<SpriteRenderer>();
            Normal();
        }

        #endregion

        public Vector2Int GetNode(int index)
        {
            return _nodes[index];
        }

        public void SetSpeed(float speed)
        {
            _moveSpeed = speed;
        }

        /// <summary>
        /// Bắn thẳng ra ngoài (bản gốc).
        /// </summary>
        public void SetData(int id, Vector2Int[] nodes, int outNodeLength)
        {
            ID = id;
            OutNodeLength = outNodeLength;
            _nodes = nodes;

            var head = Head;
            var firstBody = GetNode(Length - 2);

            var dir = head - firstBody;
            if (dir.y == 0)
            {
                GoDirection = dir.x > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                GoDirection = dir.y > 0 ? Direction.Up : Direction.Down;
            }

            Vector3 p1 = new Vector3(_nodes[^1].x, _nodes[^1].y);
            Vector3 p2 = new Vector3(_nodes[^2].x, _nodes[^2].y);

            var direction = (p1 - p2).normalized * outNodeLength;

            Vector2Int outNode = _nodes[^1] + new Vector2Int((int)direction.x, (int)direction.y);
            Vector3 outPoint = new Vector3(outNode.x, outNode.y, 0);

            StartCoroutine(_Create());
            IEnumerator _Create()
            {
                yield return null;

                List<SplinePoint> ps = new();

                for (int i = 0; i < nodes.Length - 1; i++)
                {
                    var node = nodes[i];
                    ps.Add(new SplinePoint(new Vector3(node.x, node.y)));
                }

                var fNode = nodes[^1];
                Vector3 fPoint = new Vector3(fNode.x, fNode.y);
                var fDir = outPoint - fPoint;
                var fLength = Mathf.FloorToInt(fDir.magnitude);
                fDir.Normalize();
                for (int j = 0; j < fLength; j++)
                {
                    ps.Add(new SplinePoint(fPoint + j * fDir));
                }

                ps.Add(new SplinePoint(outPoint));

                _segments = ps.Count - 1;
                _pathLength = _segments;
                _segmentLength = 1.0f / _segments;

                _splineComputer.SetPoints(ps.ToArray());

                yield return null;
                _percentTail = 0;
                _percentHead = _splineComputer.GetPointPercent(nodes.Length - 1);
                _length = _percentHead - _percentTail;
                UpdatePath(_percentHead, _percentTail);
            }
        }

        /// <summary>
        /// Overload: chỉ có bound 4 góc.
        /// </summary>
        public void SetDataWithBounds(
            int id,
            Vector2Int[] nodes,
            int outNodeLength,
            Vector3 boundTopLeftLocal,
            Vector3 boundTopRightLocal,
            Vector3 boundBottomRightLocal,
            Vector3 boundBottomLeftLocal)
        {
            SetDataWithBoundsInternal(
                id, nodes, outNodeLength,
                boundTopLeftLocal, boundTopRightLocal, boundBottomRightLocal, boundBottomLeftLocal,
                false, Vector3.zero, Vector3.zero
            );
        }

        /// <summary>
        /// Overload: có bound + đường đi lên tháp (towerBaseLocal, towerNearLocal).
        /// </summary>
        public void SetDataWithBounds(
            int id,
            Vector2Int[] nodes,
            int outNodeLength,
            Vector3 boundTopLeftLocal,
            Vector3 boundTopRightLocal,
            Vector3 boundBottomRightLocal,
            Vector3 boundBottomLeftLocal,
            Vector3 towerBaseLocal,
            Vector3 towerNearLocal)
        {
            SetDataWithBoundsInternal(
                id, nodes, outNodeLength,
                boundTopLeftLocal, boundTopRightLocal, boundBottomRightLocal, boundBottomLeftLocal,
                true, towerBaseLocal, towerNearLocal
            );
        }

        /// <summary>
        /// Core: tính P + path quanh bound, có thể dừng tại towerBaseLocal rồi đi lên towerNearLocal.
        /// </summary>
        private void SetDataWithBoundsInternal(
            int id,
            Vector2Int[] nodes,
            int outNodeLength,
            Vector3 boundTopLeftLocal,
            Vector3 boundTopRightLocal,
            Vector3 boundBottomRightLocal,
            Vector3 boundBottomLeftLocal,
            bool hasTowerPath,
            Vector3 towerBaseLocal,
            Vector3 towerNearLocal)
        {
            ID = id;
            OutNodeLength = outNodeLength;
            _nodes = nodes;

            var head = Head;
            var firstBody = GetNode(Length - 2);

            var dirInt = head - firstBody;
            if (dirInt.y == 0)
            {
                GoDirection = dirInt.x > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                GoDirection = dirInt.y > 0 ? Direction.Up : Direction.Down;
            }

            Vector3 headLocal = new Vector3(head.x, head.y, 0);
            Vector3 bodyLocal = new Vector3(firstBody.x, firstBody.y, 0);
            Vector3 rayDirLocal = (headLocal - bodyLocal).normalized;

            Debug.Log($"[Arrow] SetDataWithBounds LOCAL ID={ID} GoDir={GoDirection} headLocal={headLocal}, bodyLocal={bodyLocal}");

            Vector3 TL = boundTopLeftLocal;      // A
            Vector3 TR = boundTopRightLocal;     // B
            Vector3 BR = boundBottomRightLocal;  // D
            Vector3 BL = boundBottomLeftLocal;   // C

            Debug.Log($"[Arrow] Bounds LOCAL: TL={TL}, TR={TR}, BR={BR}, BL={BL}");

            Vector2 p = headLocal;
            Vector2 r = rayDirLocal;

            bool found = false;
            float bestT = float.MaxValue;
            Vector2 bestHit = Vector2.zero;
            string bestEdge = "";

            void TestEdge(Vector3 a, Vector3 b, string edgeName)
            {
                Vector2 inter;
                float t;
                if (RaySegmentIntersection(p, r, a, (b - a), out inter, out t))
                {
                    if (!found || t < bestT)
                    {
                        found = true;
                        bestT = t;
                        bestHit = inter;
                        bestEdge = edgeName;
                    }
                }
            }

            // P trên cạnh nào?
            TestEdge(TL, TR, "Top");     // AB
            TestEdge(TR, BR, "Right");   // BD
            TestEdge(BR, BL, "Bottom");  // DC
            TestEdge(BL, TL, "Left");    // CA

            if (!found)
            {
                Debug.LogWarning($"[Arrow] ID={ID} không tìm được giao bound, fallback SetData.");
                SetData(id, nodes, outNodeLength);
                return;
            }

            Vector3 boundPointLocal = bestHit;
            Debug.Log($"[Arrow] ID={ID} hit bound LOCAL tại {boundPointLocal}, edge={bestEdge}, t={bestT}");

            StartCoroutine(_Create());

            IEnumerator _Create()
            {
                yield return null;

                List<SplinePoint> ps = new();

                // 1) Path gốc (trừ head)
                for (int i = 0; i < nodes.Length - 1; i++)
                {
                    var node = nodes[i];
                    ps.Add(new SplinePoint(new Vector3(node.x, node.y, 0)));
                }

                // helper chia đoạn
                void AddSegmentPoints(Vector3 start, Vector3 end, bool includeStart)
                {
                    Vector3 seg = end - start;
                    float len = seg.magnitude;

                    if (len <= Mathf.Epsilon)
                    {
                        if (includeStart) ps.Add(new SplinePoint(start));
                        ps.Add(new SplinePoint(end));
                        return;
                    }

                    seg.Normalize();
                    int steps = Mathf.FloorToInt(len);
                    int jStart = includeStart ? 0 : 1;

                    for (int j = jStart; j < steps; j++)
                    {
                        ps.Add(new SplinePoint(start + j * seg));
                    }

                    ps.Add(new SplinePoint(end));
                }

                // 2) headLocal -> P
                AddSegmentPoints(headLocal, boundPointLocal, true);

                // 3) Build path quanh bound
                Vector3[] corners = new[] { TL, TR, BR, BL };
                int startEdgeIndex;
                switch (bestEdge)
                {
                    case "Top": startEdgeIndex = 0; break;    // TL->TR
                    case "Right": startEdgeIndex = 1; break;  // TR->BR
                    case "Bottom": startEdgeIndex = 2; break; // BR->BL
                    case "Left":
                    default: startEdgeIndex = 3; break;       // BL->TL
                }

                if (hasTowerPath)
                {
                    Debug.Log($"[Arrow] ID={ID} hasTowerPath base={towerBaseLocal}, near={towerNearLocal}");

                    Vector3 current = boundPointLocal;
                    int edgeIndex = startEdgeIndex;
                    bool reachedTower = false;
                    int safety = 0;

                    while (!reachedTower && safety++ < 10)
                    {
                        int sIdx = edgeIndex;
                        int eIdx = (edgeIndex + 1) % 4;

                        Vector3 a = corners[sIdx];
                        Vector3 b = corners[eIdx];

                        if (PointOnSegment(towerBaseLocal, a, b, 0.05f))
                        {
                            Debug.Log($"[Arrow] ID={ID} towerBase nằm trên cạnh {edgeIndex} từ {a} tới {b}");
                            // đi từ current -> towerBase
                            AddSegmentPoints(current, towerBaseLocal, false);
                            // rồi đi lên towerNear và dừng
                            AddSegmentPoints(towerBaseLocal, towerNearLocal, false);
                            reachedTower = true;
                            break;
                        }
                        else
                        {
                            // đi tới corner tiếp theo
                            AddSegmentPoints(current, b, false);
                            current = b;
                            edgeIndex = (edgeIndex + 1) % 4;
                        }
                    }

                    if (!reachedTower)
                    {
                        Debug.LogWarning($"[Arrow] ID={ID} không tìm được cạnh chứa towerBase, fallback đi hết 4 góc.");
                        // fallback: như case không có towerPath
                        BuildFullRectFromP(ps, corners, boundPointLocal, bestEdge, AddSegmentPoints);
                    }
                }
                else
                {
                    // Không có tower path: đi hết 4 góc như cũ
                    BuildFullRectFromP(ps, corners, boundPointLocal, bestEdge, AddSegmentPoints);
                }

                _segments = ps.Count - 1;
                _pathLength = _segments;
                _segmentLength = 1.0f / _segments;

                _splineComputer.SetPoints(ps.ToArray());

                yield return null;

                _percentTail = 0;
                _percentHead = _splineComputer.GetPointPercent(nodes.Length - 1);
                _length = _percentHead - _percentTail;
                UpdatePath(_percentHead, _percentTail);
            }
        }

        /// <summary>
        /// Helper: đi quanh cả 4 góc theo thứ tự clockwise từ điểm P (boundPointLocal).
        /// </summary>
        private void BuildFullRectFromP(
            List<SplinePoint> ps,
            Vector3[] corners,
            Vector3 boundPointLocal,
            string bestEdge,
            Action<Vector3, Vector3, bool> AddSegmentPoints)
        {
            int[] order;
            switch (bestEdge)
            {
                case "Top":
                    // P trên AB: P -> B -> D -> C -> A
                    order = new[] { 1, 2, 3, 0 };
                    break;
                case "Right":
                    // P trên BD: P -> D -> C -> A -> B
                    order = new[] { 2, 3, 0, 1 };
                    break;
                case "Bottom":
                    // P trên DC: P -> C -> A -> B -> D
                    order = new[] { 3, 0, 1, 2 };
                    break;
                case "Left":
                default:
                    // P trên CA: P -> A -> B -> D -> C
                    order = new[] { 0, 1, 2, 3 };
                    break;
            }

            Debug.Log($"[Arrow] BuildFullRectFromP edge={bestEdge}, order={order[0]}-{order[1]}-{order[2]}-{order[3]}");

            Vector3 current = boundPointLocal;
            foreach (int idx in order)
            {
                Vector3 corner = corners[idx];
                AddSegmentPoints(current, corner, false);
                current = corner;
            }
        }

        /// <summary>
        /// Kiểm tra 1 điểm có nằm trên đoạn AB hay không (theo 2D, dùng x,y).
        /// </summary>
        private bool PointOnSegment(Vector3 p, Vector3 a, Vector3 b, float tolerance = 0.01f)
        {
            Vector2 ap = new Vector2(p.x - a.x, p.y - a.y);
            Vector2 ab = new Vector2(b.x - a.x, b.y - a.y);

            // kiểm tra gần như thẳng hàng: cross ~ 0
            float cross = ap.x * ab.y - ap.y * ab.x;
            if (Mathf.Abs(cross) > tolerance)
            {
                return false;
            }

            float dot = Vector2.Dot(ap, ab);
            if (dot < -tolerance) return false;
            if (dot - ab.sqrMagnitude > tolerance) return false;

            return true;
        }

        private void UpdatePath(double headPercent, double tailPercent)
        {
            int headIndex = Mathf.FloorToInt((float)headPercent * _pathLength);
            int tailIndex = Mathf.CeilToInt((float)tailPercent * _pathLength);

            var hP = headIndex * _segmentLength;
            var tP = tailIndex * _segmentLength;

            _points.Clear();

            SampleCollection sampleCollection = new SampleCollection();
            _splineComputer.GetSamples(sampleCollection);

            if (tailPercent < tP)
            {
                var tail = _splineComputer.EvaluatePosition(tailPercent);
                _points.Add(tail);
            }

            for (int i = tailIndex; i <= headIndex; i++)
            {
                var p = _splineComputer.EvaluatePosition(i);
                _points.Add(p);
            }

            if (headPercent > hP)
            {
                var p = _splineComputer.EvaluatePosition(headPercent);
                if (Vector3.SqrMagnitude(p - _points[^1]) > 0.00001f)
                {
                    _points.Add(p);
                }
            }

            _head.localPosition = _points[^1];
            var direction = _points[^1] - _points[^2];
            _head.up = direction;

            _lineRenderer.positionCount = _points.Count;
            _lineRenderer.SetPositions(_points.ToArray());
        }

        private void MoveForward(double value)
        {
            _percentHead = value;
            _percentTail = _percentHead - _length;
            UpdatePath(_percentHead, _percentTail);
        }

        private void MoveBackward(double value)
        {
            _percentTail = value;
            _percentHead = _percentTail + _length;
            UpdatePath(_percentHead, _percentTail);
        }

        private void GoForward(double to, float speed, Action<Arrow> callback)
        {
            IsMoving = true;
            DOTween.To(
                    () => _percentHead,
                    MoveForward,
                    to,
                    speed
                )
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    IsMoving = false;
                    callback?.Invoke(this);
                })
                .SetSpeedBased(true);
        }

        private bool RaySegmentIntersection(
            Vector2 p, Vector2 r,
            Vector2 q, Vector2 s,
            out Vector2 intersection,
            out float tRay)
        {
            float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

            intersection = Vector2.zero;
            tRay = 0f;

            float rxs = Cross(r, s);
            Vector2 qmp = q - p;

            if (Mathf.Abs(rxs) < 1e-6f) return false;

            float t = Cross(qmp, s) / rxs;
            float u = Cross(qmp, r) / rxs;

            if (t >= 0f && u >= 0f && u <= 1f)
            {
                intersection = p + t * r;
                tRay = t;
                return true;
            }

            return false;
        }

        public void GoForward(Action<Arrow> callback)
        {
            GoForward(1, _moveSpeed, callback);
        }

        private void GoBackward(double to, Action<Arrow> callback)
        {
            IsMoving = true;
            DOTween.To(
                    () => _percentTail,
                    MoveBackward,
                    to,
                    _moveSpeed
                )
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    IsMoving = false;
                    callback?.Invoke(this);
                })
                .SetSpeedBased(true);
        }

        public void GoBackward(Action<Arrow> callback)
        {
            GoBackward(0, callback);
        }

        private Color _lastColor;

        public void Normal()
        {
            _lastColor = _normalColor;
            ChangeColor(_normalColor, _normalColorFadeDuration);
        }

        public void Error()
        {
            _lastColor = _errorColor;
            ChangeColor(_errorColor, _errorColorFadeDuration);
        }

        public void Highlight()
        {
            ChangeColor(_highlightColor, _normalColorFadeDuration);
        }

        public void Hint()
        {
            _lastColor = _hintColor;
            ChangeColor(_hintColor, _normalColorFadeDuration);
        }

        public void UnHighlight()
        {
            ChangeColor(_lastColor, _normalColorFadeDuration);
        }

        private void ChangeColor(Color toColor, float duration)
        {
            _headRenderer.DOColor(toColor, duration)
                .OnUpdate(() =>
                {
                    _lineRenderer.startColor = _headRenderer.color;
                    _lineRenderer.endColor = _headRenderer.color;
                });
        }

        public void GoForwardError(Vector2Int node, Arrow other, Action callback)
        {
            var hitPoint = new Vector3(node.x, node.y);
            SplineSample result = new SplineSample();
            _splineComputer.Project(hitPoint, ref result);
            double _tail = _percentTail;
            GoForward(result.percent, _moveSpeed * 0.75f, (arrow =>
            {
                GoBackward(_tail, (arrow1 =>
                {
                    callback?.Invoke();
                }));
            }));
        }

        public bool CheckMouseClick(Vector3 position)
        {
            SplineSample result = new SplineSample();
            _splineComputer.Project(position, ref result);
            var dis = Vector3.Distance(position, result.position);
            return dis < 0.5f;
        }
    }
}
