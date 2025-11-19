using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowsPuzzle
{
    public class MapManager : MonoBehaviour
    {
        #region Serialize properties

        [SerializeField] private Arrow _arrowPrefab;

        [SerializeField] private Dot _dotPrefab;

        [SerializeField] private float _arrowMoveSpeed = 1;

        [SerializeField] private Transform _levelParent;

        [SerializeField] private Vector2 _levelParentOffset = Vector2.zero;

        [SerializeField] LineRenderer _lineRendererHighlight;

        #endregion

        #region Private properties

        private Level _level;

        private int[,] _map;

        private Dot[,] _dots;

        private Vector2Int _midNode;

        private Camera _cam;

        #endregion

        #region Public properties

        public Dictionary<int, Arrow> Arrows { get; private set; }

        #endregion

        #region Unity messages

        private void Start()
        {
            _cam = Camera.main;

            EventManager.GamePlay.OnArrowHighlight += GamePlay_OnArrowHighlight;
            EventManager.GamePlay.OnArrowUnHighlight += GamePlay_OnArrowUnHighlight;

            _lineRendererHighlight.gameObject.SetActive(false);
        }

        #endregion

        #region Event handlers


        private void GamePlay_OnArrowUnHighlight(Arrow arrow)
        {
            HideHighlightLine(arrow);
        }

        private void GamePlay_OnArrowHighlight(Arrow arrow)
        {
            ShowHighlightLine(arrow);
        }


        private void ShowHighlightLine(Arrow arrow)
        {
            _lineRendererHighlight.gameObject.SetActive(true);

            var head = _levelParent.TransformPoint(new Vector3(arrow.Head.x,arrow.Head.y));

            Vector3 startPoint = Vector3.zero;
            Vector3 endPoint = Vector3.zero;


            switch (arrow.GoDirection)
            {
                case Arrow.Direction.Up:
                case Arrow.Direction.Down:
                    //vertical
                    startPoint.x = head.x;
                    endPoint.x = head.x;


                    var topPoint = _cam.ScreenToWorldPoint(new Vector3(0,Screen.height));
                    var bottomPoint = _cam.ScreenToWorldPoint(Vector3.zero);

                    startPoint.y = topPoint.y;
                    endPoint.y = bottomPoint.y;
                    break;
                case Arrow.Direction.Right:
                case Arrow.Direction.Left:
                    //horizontal
                    var leftPoint = _cam.ScreenToWorldPoint(Vector3.zero);
                    var rightPoint = _cam.ScreenToWorldPoint(new Vector3(Screen.width, 0));

                    startPoint.x = leftPoint.x;
                    endPoint.x = rightPoint.x;

                    startPoint.y = head.y;
                    endPoint.y = head.y;
                    break;
                default:
                    break;
            }



            _lineRendererHighlight.positionCount = 2;


            _lineRendererHighlight.SetPosition(0,startPoint);
            _lineRendererHighlight.SetPosition(1,endPoint);
        }

        private void HideHighlightLine(Arrow arrow)
        {
            _lineRendererHighlight.gameObject.SetActive(false);

        }

        #endregion

        public void Create(Level level,Action callback)
        {
            _level = level;
            _map = new int[_level.Data.Height, _level.Data.Width];
            _dots = new Dot[_level.Data.Height,_level.Data.Width];


            StartCoroutine(_Create());
            IEnumerator _Create()
            {
                yield return _CreateArrows();

                callback?.Invoke();

                yield return _CreateDots();
            }
        }

        private IEnumerator _CreateArrows()
        {
            yield return null;
            Arrows = new Dictionary<int,Arrow>();
            int outLength = Mathf.Max(_level.Data.Width, _level.Data.Height);

            int count = 0;
            foreach (var pair in _level.Data.Arrows)
            {
                int id = pair.Key;
                var nodes = pair.Value;
                if (nodes.Length > 1)
                {
                    var arrow = Instantiate(_arrowPrefab);
                    arrow.transform.SetParent(_levelParent);
                    arrow.SetSpeed(_arrowMoveSpeed);

                    for (int i = 0; i < nodes.Length; i++)
                    {
                        var node = nodes[i];
                        _map[node.y, node.x] = id;
                    }

                    arrow.SetData(id,nodes,outLength+5+ nodes.Length);
                    Arrows.Add(id,arrow);

                    count++;
                }

                if (count % 10 == 0)
                {
                    yield return null;
                }
            }

            //yield return null;
            float width = _level.Data.Width;
            float height = _level.Data.Height;
            _midNode = new Vector2Int(_level.Data.Width / 2, _level.Data.Height / 2);

            _levelParent.localPosition = new Vector2(-width / 2 + 0.5f + _levelParentOffset.x,
                -height / 2 + 0.5f + _levelParentOffset.y);
        }

        private IEnumerator _CreateDots()
        {
            yield return null;
            int count = 0;
            foreach (var pair in Arrows)
            {
                foreach (var node in pair.Value.Nodes)
                {
                    var dot = Instantiate(_dotPrefab, _levelParent);
                    dot.transform.localPosition = new Vector3(node.x, node.y);
                    dot.gameObject.SetActive(false);

                    _dots[node.y,node.x] = dot;

                    count++;
                }

                if (count % 20 == 0)
                {
                    yield return null;
                }
            }
        }

        public bool ArrowCanGo(Arrow arrow,out int hitId,out Vector2Int hitNode)
        {
            var head = arrow.Head;
            switch (arrow.GoDirection)
            {
                case ArrowsPuzzle.Arrow.Direction.Up:
                    for (int y = head.y+1; y < _level.Data.Height; y++)
                    {
                        if (_map[y, head.x] != 0)
                        {
                            hitId = _map[y, head.x];
                            hitNode = new Vector2Int(head.x, y);
                            return false;
                        }
                    }
                    break;
                case ArrowsPuzzle.Arrow.Direction.Right:
                    for (int x = head.x+1; x <_level.Data.Width; x++)
                    {
                        if (_map[head.y, x] != 0)
                        {
                            hitId = _map[head.y,x];
                            hitNode = new Vector2Int(x, head.y);
                            return false;
                        }
                    }
                    break;
                case ArrowsPuzzle.Arrow.Direction.Down:
                    for (int y = head.y-1; y >=0; y--)
                    {
                        if (_map[y, head.x] != 0)
                        {
                            hitId = _map[y, head.x];
                            hitNode = new Vector2Int(head.x, y);
                            return false;
                        }
                    }
                    break;
                case ArrowsPuzzle.Arrow.Direction.Left:
                    for (int x = head.x-1; x >= 0; x--)
                    {
                        if (_map[head.y, x] != 0)
                        {
                            hitId = _map[head.y,x];
                            hitNode = new Vector2Int(x, head.y);
                            return false;
                        }
                    }
                    break;
                default:
                    break;
            }

            hitId = 0;
            hitNode = new Vector2Int(-1, -1);
            return true;
        }

        public void RemoveArrow(Arrow arrow)
        {
            if (Arrows.ContainsKey(arrow.ID))
            {
                foreach (var node in arrow.Nodes)
                {
                    _map[node.y, node.x] = 0;
                }
                Arrows.Remove(arrow.ID);
            }

        }

        public bool CheckClickArrow(Vector3 position, out Arrow arrow)
        {
            //Debug.Log(position);
            var localPos = _levelParent.InverseTransformPoint(position);

            //Debug.Log(position);
            int x = Mathf.RoundToInt(localPos.x);
            int y = Mathf.RoundToInt(localPos.y);
            //Debug.Log($"X: {x}, Y: {y}");
            if (IsInsideNode(x,y))
            {
                int id = _map[y, x];
                if (id!=0 && Arrows.ContainsKey(id))
                {
                    var tmpArrow = Arrows[id];

                    if (!tmpArrow.IsMoving && tmpArrow.CheckMouseClick(localPos))
                    {
                        arrow = tmpArrow;
                        return true;
                    }

                }
            }
            arrow = null;
            return false;
        }

        public bool IsInsideNode(int x, int y)
        {
            if (x >= 0 &&
                x < _level.Data.Width &&
                y >= 0 &&
                y < _level.Data.Height)
            {
                return true;
            }

            return false;
        }

        public bool IsLevelCleared()
        {
            return Arrows.Count==0;
        }

        public void ShowArrowDots(Arrow arrow)
        {
            foreach (var node in arrow.Nodes)
            {
                _dots[node.y,node.x].Show();
            }
        }

        public void HideArrowDots(Arrow arrow)
        {
            foreach (var node in arrow.Nodes)
            {
                _dots[node.y, node.x].Hide();
            }
        }

        public void ShowWinDots(float delay, Action callback)
        {
            StartCoroutine(_Show());

            IEnumerator _Show()
            {
                yield return new WaitForSeconds(delay);

                float radius = 0;
                int value = Mathf.Max(_level.Data.Width, _level.Data.Height);
                var wait = new WaitForSeconds(0.05f);
                while (radius<value)
                {
                    for (int r = 0; r <= Mathf.FloorToInt(radius * Mathf.Sqrt(0.5f)); r++)
                    {
                        int d = Mathf.FloorToInt(Mathf.Sqrt(radius * radius - r * r));

                        ShowDot(_midNode.x - d, _midNode.y + r);
                        ShowDot(_midNode.x + d, _midNode.y + r);
                        ShowDot(_midNode.x - d, _midNode.y - r);
                        ShowDot(_midNode.x + d, _midNode.y - r);
                        ShowDot(_midNode.x + r, _midNode.y - d);
                        ShowDot(_midNode.x + r, _midNode.y + d);
                        ShowDot(_midNode.x - r, _midNode.y - d);
                        ShowDot(_midNode.x - r, _midNode.y + d);
                    }

                    radius += 0.5f;
                    yield return wait;
                }
                callback?.Invoke();
            }
        }

        void ShowDot(int x, int y)
        {
            if (IsInsideNode(x, y))
            {
                _dots[y,x]?.Win();
            }
        }

        public void FindHint(Action<Arrow> callback)
        {
            Arrow hintArrow = null;
            foreach (var pair in Arrows)
            {
                var arrow = pair.Value;
                var head = arrow.Head;
                bool isValid = true;
                switch (arrow.GoDirection)
                {
                    case Arrow.Direction.Up:
                        for (int y = head.y+1; y < _level.Data.Height; y++)
                        {
                            if (_map[y, head.x] != 0)
                            {
                                isValid = false;
                                break;
                            }
                        }
                        break;
                    case Arrow.Direction.Right:
                        for (int x = head.x + 1; x < _level.Data.Width; x++)
                        {
                            if (_map[head.y, x] != 0)
                            {
                                isValid = false;
                                break;
                            }
                        }
                        break;
                    case Arrow.Direction.Down:
                        for (int y = head.y -1; y > -1; y--)
                        {
                            if (_map[y, head.x] != 0)
                            {
                                isValid = false;
                                break;
                            }
                        }
                        break;
                    case Arrow.Direction.Left:
                        for (int x = head.x - 1; x > -1; x--)
                        {
                            if (_map[head.y, x] != 0)
                            {
                                isValid = false;
                                break;
                            }
                        }
                        break;
                    default:
                        break;
                }

                if (isValid)
                {
                    hintArrow = arrow;
                    break;
                }
            }
            callback?.Invoke(hintArrow);
        }
    }
}
